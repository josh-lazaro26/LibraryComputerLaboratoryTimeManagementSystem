using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.Sidebar_Animation;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Admin;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Session;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Student;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.StudentServices;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.UserServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms.AddTimeModalForm;

namespace LibraryComputerLaboratoryTimeManagementSystem.FORMS
{
    public partial class MainForm : Form
    {
        private PanelHandler _panelHandler;
        private int? _selectedAdminId;
        private bool _isSidebarExpanded = false;
        public PanelPositionAnimator _panelAnimator;
        private ButtonHoverEffect _sidebarHoverEffect;
        private AdminService _adminService;
        private StudentServices _studentServices;
        private UserServices _userService;
        private string _currentStudentRfid;
        private List<Button> _timeButtons;
        public SidebarAnimator _sidebarAnimator;
        private List<Label> _timeLabels;
        private bool _isSidebarOpen = false;
        private Dictionary<int, TimeSpan> _remainingBySessionId; 
        private System.Windows.Forms.Timer _countdownTimer;
        private string _selectedStudentId;

        private readonly SignalRService _signalRService;
        public MainForm()
        {
            InitializeComponent();
            UIHandler();
            AdminCreationVisibility();
            _adminService = new AdminService();
            _studentServices = new StudentServices();
            _userService = new UserServices();
            AdminDgv.CellClick += AdminDgv_CellClick;
            AdminDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AdminDgv.MultiSelect = false;

            _sidebarAnimator = new SidebarAnimator(SidebarPanel, 300, 55, 8, 5);

            _signalRService = new SignalRService(() => Task.FromResult(ApiConfig.Token));

            SignalRInitialize();

            _signalRService.LoggedOutSession += (int sessionId) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => HandleSessionTerminated(sessionId)));
                    Console.WriteLine($"Session is terminated: {sessionId}");
                }
                else
                {
                    HandleSessionTerminated(sessionId);
                }
            };


            _signalRService.NewStudentOpenedSession += (Object data) =>
            {
                try
                {
                    // Ensure UI thread
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => HandleNewSession(data)));
                    }
                    else
                    {
                        HandleNewSession(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };

            StudentFirstNameTb.KeyPress += LettersOnly_KeyPress;
            StudentMiddleNameTb.KeyPress += LettersOnly_KeyPress;
            StudentLastNameTb.KeyPress += LettersOnly_KeyPress;
            AdminFirstNameTb.KeyPress += LettersOnly_KeyPress;
            AdminMiddleNameTb.KeyPress += LettersOnly_KeyPress;
            AdminLastNameTb.KeyPress += LettersOnly_KeyPress;
            ListOfStudentDgv.CellClick += ListOfStudentDgv_CellClick;
            ListOfStudentDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ListOfStudentDgv.MultiSelect = false;
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            InitTimeButtons();
            WireTimeButtonClicks();

            InitYearLevelComboBox();
            InitTimeLabelsAndTimer();

            StudentCourseCb.SelectedIndex = 0;
            ListOfStudentCourseCb.SelectedIndex = 0;
            _panelHandler.ShowOnly(DashboardPanel);
            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.Apply(this);
                _panelAnimator?.Toggle();
            }

            await LoadAdminsAsync();
            await LoadStudentsAsync();
            await LoadActiveSessionsToButtons();
        }

        private void HandleNewSession(object data)
        {
            // If backend sends JSON string
            SessionDto session;

            if (data is string json)
            {
                session = JsonConvert.DeserializeObject<SessionDto>(json);
            }
            else
            {
                // If SignalR already sends object
                session = JsonConvert.DeserializeObject<SessionDto>(data.ToString());
            }

            if (session == null) return;

            // Find first vacant button
            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (btn.Tag == null) // vacant slot
                {
                    btn.Text = session.Name;
                    btn.Tag = session;
                    btn.Enabled = true;
                    btn.ForeColor = SystemColors.ControlLight;

                    var remaining = ParseApiDuration(session.Duration);
                    _remainingBySessionId[session.Id] = remaining;
                    lbl.Text = ToMmSs(remaining);

                    break;
                }
            }
        }

        private void HandleSessionTerminated(int sessionId)
        {
            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (btn.Tag is SessionDto s && s.Id == sessionId)
                {
                    // Clear button (make it vacant again)
                    btn.Text = "Vacant";
                    btn.Tag = null;
                    btn.Enabled = true;
                    btn.ForeColor = SystemColors.ControlLight;

                    lbl.Text = "00:00:00";

                    // Remove from countdown dictionary
                    if (_remainingBySessionId.ContainsKey(sessionId))
                    {
                        _remainingBySessionId.Remove(sessionId);
                    }

                    break;
                }
            }
        }

        private void SignalRInitialize()
        {
            try
            {
                _ = _signalRService.InitializeAsync();
                Console.WriteLine("signalR initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void AdminDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = AdminDgv.Rows[e.RowIndex];
            var admin = row.DataBoundItem as AdminDto; 
                                                       
            _selectedAdminId = Convert.ToInt32(row.Cells[0].Value); // if Id is column 0

            if (admin == null) return;

            _selectedAdminId = admin.Id;

            IDPersonnelTb.Text = admin.PersonnelId;
            AdminFirstNameTb.Text = admin.FirstName;
            AdminMiddleNameTb.Text = admin.MiddleName;
            AdminLastNameTb.Text = admin.LastName;
        }

        public void AdminCreationVisibility()
        {
            if (SuperAdminState.isSuperAdmin)
            {
                AdminCreation.Visible = true;
                ReportBtn.Visible = true;
            }
        }

        private void SetupAdminCreationAnimation()
        {
            if (_panelAnimator == null)
                _panelAnimator = new PanelPositionAnimator(5, 8);

            _panelAnimator.Clear();

            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.UpdatePanelAnimations(this);
                // Snap panels to correct position based on sidebar state
                SnapPanelsToCurrentState();
                return;
            }

            _panelAnimator.AddPanel(AdminFields, new Point(358, 124), new Point(224, 124));
            _panelAnimator.AddPanel(AdminTablePanel, new Point(784, 124), new Point(673, 124));
            SnapPanelsToCurrentState();
        }

        private void SetupDashboardAnimation()
        {
            if (_panelAnimator == null)
                _panelAnimator = new PanelPositionAnimator(5, 8);

            _panelAnimator.Clear();

            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.UpdatePanelAnimations(this);
                SnapPanelsToCurrentState();
                return;
            }

            _panelAnimator.AddPanel(WelcomeAdminPanel, new Point(385, 229), new Point(261, 229));
            SnapPanelsToCurrentState();
        }

        private void SetupStudentRegistrationAnimation()
        {
            if (_panelAnimator == null)
                _panelAnimator = new PanelPositionAnimator(5, 8);

            _panelAnimator.Clear();

            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.UpdatePanelAnimations(this);
                SnapPanelsToCurrentState();
                return;
            }

            _panelAnimator.AddPanel(RegisterStudentFieldPanel, new Point(299, 112), new Point(167, 112));
            SnapPanelsToCurrentState();
        }
        private void SetupAdminReportsPanelAnimation()
        {
            if (_panelAnimator == null)
                _panelAnimator = new PanelPositionAnimator(5, 8);

            _panelAnimator.Clear();

            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.UpdatePanelAnimations(this);
                SnapPanelsToCurrentState();
                return;
            }

            _panelAnimator.AddPanel(ReportDgvPanel, new Point(311, 83), new Point(193, 83));
            SnapPanelsToCurrentState();
        }
        private void SetupStudentCreationAnimation()
        {
            if (_panelAnimator == null)
                _panelAnimator = new PanelPositionAnimator(5, 8);

            _panelAnimator.Clear();

            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.UpdatePanelAnimations(this);
                SnapPanelsToCurrentState();
                return;
            }

            _panelAnimator.AddPanel(StudentFieldsPanel, new Point(355, 127), new Point(258, 127));
            _panelAnimator.AddPanel(StudentsTablePanel, new Point(732, 127), new Point(644, 127));
            SnapPanelsToCurrentState();
        }

        private void SetupTimeManagementAnimation()
        {
            if (_panelAnimator == null)
                _panelAnimator = new PanelPositionAnimator(5, 8);

            _panelAnimator.Clear();

            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.UpdatePanelAnimations(this);
                SnapPanelsToCurrentState();
                return;
            }

            _panelAnimator.AddPanel(TimeManagementBtnPanel1, new Point(308, 123), new Point(162, 123));
            _panelAnimator.AddPanel(TimeManagementBtnPanel2, new Point(555, 123), new Point(442, 123));
            _panelAnimator.AddPanel(TimeManagementBtnPanel3, new Point(802, 123), new Point(716, 123));
            _panelAnimator.AddPanel(TimeManagementBtnPanel4, new Point(1050, 123), new Point(994, 123));
            SnapPanelsToCurrentState();
        }

        private void SnapPanelsToCurrentState()
        {
            _panelAnimator?.SnapToState(!_sidebarAnimator.IsExpanded);
        }

        private void UIHandler()
        {
            _sidebarHoverEffect = new ButtonHoverEffect(
                normalBackColor: Color.FromArgb(0, 68, 52),
                hoverBackColor: Color.FromArgb(0, 102, 78),
                normalForeColor: Color.White,
                hoverForeColor: Color.White
            );


            _sidebarHoverEffect.Attach(DashboardSidebarBtn);
            _sidebarHoverEffect.Attach(ListOfStudentsSidebarBtn);
            _sidebarHoverEffect.Attach(TimeManagementSidebarBtn);
            _sidebarHoverEffect.Attach(StudentManagementSidebarBtn);
            _sidebarHoverEffect.Attach(SidebarBtn);
            _sidebarHoverEffect.Attach(LogoutBtn);

            _sidebarHoverEffect.Attach(AdminCreation);
            _sidebarHoverEffect.Attach(ReportBtn);

            _panelHandler = new PanelHandler();
            _panelHandler.AddPanels(DashboardPanel, StudentPanel, AdminReportsPanel, AdminCreationPanel, TimeManagementPanel, ListOfStudentsPanel);
        }

        private void WireTimeButtonClicks()
        {
            foreach (var btn in _timeButtons)
            {
                btn.Click += TimeButton_Click;
            }
        }

        private async void TimeButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is SessionDto session)
            {
                if (!_remainingBySessionId.TryGetValue(session.Id, out var currentRemaining))
                {
                    currentRemaining = TimeSpan.Zero;
                }

                using (var modal = new AddTimeModalForm(session.Id, _remainingBySessionId))
                {
                    if (modal.ShowDialog(this) == DialogResult.OK)
                    {
                        switch (modal.Action)
                        {
                            case SessionModalAction.Updated:
                                if (modal.UpdatedRemaining.HasValue)
                                {
                                    _remainingBySessionId[session.Id] =
                                        modal.UpdatedRemaining.Value;
                                }
                                break;

                            case SessionModalAction.Terminated:
                                // Reload from backend
                                await LoadActiveSessionsToButtons();
                                break;
                        }
                    }
                }

            }
        }

        private void InitTimeLabelsAndTimer()
        {
            _timeLabels = new List<Label>
            {
                StudentBtnLabel1, StudentBtnLabel2, StudentBtnLabel3, StudentBtnLabel4,
                StudentBtnLabel5, StudentBtnLabel6, StudentBtnLabel7, StudentBtnLabel8,
                StudentBtnLabel9, StudentBtnLabel10, StudentBtnLabel11, StudentBtnLabel12,
                StudentBtnLabel13, StudentBtnLabel14, StudentBtnLabel15, StudentBtnLabel16
            };

            _remainingBySessionId = new Dictionary<int, TimeSpan>();

            _countdownTimer = new System.Windows.Forms.Timer();
            _countdownTimer.Interval = 1000; // 1 second
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start(); // start ticking for updates
        }
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (_timeLabels == null || _remainingBySessionId == null) return;


            // Decrement and update labels by button order (1..16)
            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (btn.Tag is SessionDto s && _remainingBySessionId.TryGetValue(s.Id, out var remaining))
                {
                    remaining = remaining - TimeSpan.FromSeconds(1);
                    if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

                    _remainingBySessionId[s.Id] = remaining;
                    lbl.Text = ToMmSs(remaining);
                }
                else
                {
                    lbl.Text = "00:00:00";
                }
            }
        }

        private static TimeSpan ParseApiDuration(string durationStr)
        {
            if (!TimeSpan.TryParse(durationStr, out var ts))
                return TimeSpan.Zero;

            // API often returns negative spans like "-00:00:35..."
            // use absolute value so you can count down visually
            return ts.Duration();
        }

        private static string ToMmSs(TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero) return "00:00:00";

            return ts.ToString(@"hh\:mm\:ss");
        }

        private void DashboardSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(DashboardPanel);
            HeaderPanelLabel.Text = "Dashboard";
            SetupDashboardAnimation();
        }
        private void StudentManagementSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(StudentPanel);
            HeaderPanelLabel.Text = "Student Registration";
            SetupStudentRegistrationAnimation();
        }
        private async void AdminCreation_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(AdminCreationPanel);
            HeaderPanelLabel.Text = "Admin Creation";
            SetupAdminCreationAnimation();
            await LoadAdminsAsync();
        }
        private void ReportsSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(AdminReportsPanel);
        }
        private void ListOfStudentsSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(ListOfStudentsPanel);
            HeaderPanelLabel.Text = "List of Students";
            SetupStudentCreationAnimation();
        }
        private async void TimeManagementSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(TimeManagementPanel);
            HeaderPanelLabel.Text = "Time Management";
            SetupTimeManagementAnimation();
            await LoadActiveSessionsToButtons();
        }

        private async Task LoadStudentsAsync(string query = null, int pageNumber = 1, int pageSize = 10)
        {
            var json = await _studentServices.GetStudents(query, pageNumber, pageSize);
            if (json == null)
            {
                ShowNotification("Error", "Failed to load students.", NotificationType.Error);
                return;
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PagedUserResponse>(json);

            if (result == null || !result.IsSuccess || result.Value?.Items == null)
            {
                ShowNotification("Error", "Invalid response from server.", NotificationType.Error);
                return;
            }

            ListOfStudentDgv.AutoGenerateColumns = true;
            ListOfStudentDgv.DataSource = result.Value.Items;

            if (ListOfStudentDgv.Columns.Contains("Id"))
                ListOfStudentDgv.Columns["Id"].Visible = false;
        }
        private async Task LoadAdminsAsync(string query = null, int pageNumber = 1, int pageSize = 10)
        {
            var json = await _adminService.GetAdmins(query, pageNumber, pageSize);
            if (json == null) return;

            var result = JsonConvert.DeserializeObject<PagedAdminResponse>(json);
            AdminDgv.AutoGenerateColumns = true;
            AdminDgv.DataSource = result.Value.Items;

            if (AdminDgv.Columns.Contains("Id"))
                AdminDgv.Columns["Id"].Visible = false; 
        }

        private async Task LoadActiveSessionsToButtons()
        {
            SetAllButtonsVacant();
            SetAllLabelsZero();

            var resp = await _adminService.GetActiveSessions(pageNumber: 1, pageSize: 16);
            if (resp == null || !resp.IsSuccess || resp.Value?.Items == null) return;


            var sessions = resp.Value.Items.Take(_timeButtons.Count).ToList();

            _remainingBySessionId.Clear();

            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (i < sessions.Count)
                {
                    var s = sessions[i];

                    btn.Text = s.Name;   // name on button
                    btn.Tag = s;
                    btn.Enabled = true;

                    var remaining = ParseApiDuration(s.Duration);
                    _remainingBySessionId[s.Id] = remaining;
                    lbl.Text = ToMmSs(remaining);
                }
                else
                {
                    btn.Text = "Vacant";
                    btn.Tag = null;
                    btn.Enabled = true;
                    btn.ForeColor = SystemColors.ControlLight;

                    lbl.Text = "00:00:00:00";
                }
            }
        }

        private void SetAllLabelsZero()
        {
            if (_timeLabels == null) return;
            foreach (var lbl in _timeLabels)
                lbl.Text = "00:00:00";
        }

        private async void RegisterAdminBtn_Click(object sender, EventArgs e)
        {
            AdminCreationDAO adminCreationDao = new AdminCreationDAO();
            adminCreationDao.PersonnelId = IDPersonnelTb.Text;
            adminCreationDao.FirstName = AdminFirstNameTb.Text;
            adminCreationDao.MiddleName = AdminMiddleNameTb.Text;
            adminCreationDao.LastName = AdminLastNameTb.Text;
            adminCreationDao.RFID = AdminRfidTb.Text;

            var isSuccess = await _adminService.CreateAdmin(adminCreationDao);

            if (isSuccess)
            {
                ShowNotification("Success", "Admin created successfully.", NotificationType.Success);
            }
            else
            {
                ShowNotification("Error", "Failed to create admin.", NotificationType.Error);
            }
        }

        private async void RegisterStudentBtn_Click(object sender, EventArgs e)
        {
            var student = new StudentCreationDAO
            {
                FirstName = StudentFirstNameTb.Text,
                MiddleName = StudentMiddleNameTb.Text,
                LastName = StudentLastNameTb.Text,
                RFID = _currentStudentRfid,
                StudentId = StudentIDTb.Text,
                Course = StudentCourseCb.SelectedItem?.ToString(),
                YearLevel = StudentYearLevelCb.SelectedValue?.ToString()
            };
            using (var dialog = new DialogForm("Register Student", "Are you sure you want to register this student?"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.IsConfirmed)
                {
                    RegisterStudentBtn.Enabled = false;

                    var isSuccess = await _studentServices.CreateStudents(student);

                    RegisterStudentBtn.Enabled = true;

                    if (isSuccess)
                    {
                        ShowNotification("Success", "Student registered successfully.", NotificationType.Success);
                    }
                    else
                    {
                        ShowNotification("Error", "Failed to register student.", NotificationType.Error);
                    }
                }
            }
        }

        private void RegisterRfidBtn_Click(object sender, EventArgs e)
        {
            using (var rfidForm = new StudentRFIDForm())
            {
                if (rfidForm.ShowDialog(this) == DialogResult.OK)
                {
                    _currentStudentRfid = rfidForm.ScannedRfid;
                }
            }
        }

        private void InitTimeButtons()
        {
            _timeButtons = new List<Button>
        {
            StudentTimeBtn1, StudentTimeBtn2, StudentTimeBtn3, StudentTimeBtn4,
            StudentTimeBtn5, StudentTimeBtn6, StudentTimeBtn7, StudentTimeBtn8,
            StudentTimeBtn9, StudentTimeBtn10, StudentTimeBtn11, StudentTimeBtn12,
            StudentTimeBtn13, StudentTimeBtn14, StudentTimeBtn15, StudentTimeBtn16
        };

            // Optional: consistent look
            foreach (var btn in _timeButtons)
            {
                btn.Text = "Vacant";

                btn.ForeColor = SystemColors.ControlLight;

            }
        }

        private void SetAllButtonsVacant()
        {
            if (_timeButtons == null) return;

            foreach (var btn in _timeButtons)
            {
                btn.Text = "Vacant";
                btn.Tag = null;
                btn.Enabled = false; // optional

                btn.ForeColor = SystemColors.ControlLight;
            }
        }

        private void AdminDgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (AdminDgv.Columns.Contains("Id"))
                AdminDgv.Columns["Id"].Visible = false;
        }

        private async void DeleteAdmin_Btn_Click(object sender, EventArgs e)
        {
            if (_selectedAdminId == null)
            {
                ShowNotification("Information", "Please select an admin row first.", NotificationType.Information);
                return;
            }

            using (var dialog = new DialogForm("Delete Admin", "Are you sure you want to delete this admin?"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.IsConfirmed)
                {
                    try
                    {
                        await _adminService.DeleteAdmin(new AdminDeletionDAO { Id = _selectedAdminId.Value });
                        await LoadAdminsAsync();
                        _selectedAdminId = null;
                    }
                    catch (Exception ex)
                    {
                        ShowNotification("Error", $"Delete failed: {ex.Message}", NotificationType.Error);
                    }
                }
            }
        }

        private async void UpdateAdmin_Btn_Click(object sender, EventArgs e)
        {
            if (_selectedAdminId == null)
            {
                ShowNotification("Information", "Please select an admin row first.", NotificationType.Information);
                return;
            }

            var adminUpdate = new AdminUpdateDAO
            {
                Id = _selectedAdminId.Value,
                PersonnelId = IDPersonnelTb.Text?.Trim(),
                FirstName = AdminFirstNameTb.Text?.Trim(),
                MiddleName = AdminMiddleNameTb.Text?.Trim(),
                LastName = AdminLastNameTb.Text?.Trim(),
                RFID = AdminRfidTb.Text?.Trim(), // include only if your API expects it
            };

            try
            {
                UpdateAdmin_Btn.Enabled = false;

                await _adminService.UpdateAdmin(adminUpdate);

                await LoadAdminsAsync();      // refresh grid
                _selectedAdminId = null;      // optional: clear selection
                ShowNotification("Success", "Admin updated successfully.", NotificationType.Success);
            }
            catch (Exception ex)
            {
                ShowNotification("Error", $"Update failed: {ex.Message}", NotificationType.Error);
            }
            finally
            {
                UpdateAdmin_Btn.Enabled = true;
            }
        }

        private void InitYearLevelComboBox()
        {
            // Registration form - uses your own format
            StudentYearLevelCb.DataSource = new List<YearItem>
            {
                new YearItem { Value = 0, Text = "First Year" },
                new YearItem { Value = 1, Text = "Second Year" },
                new YearItem { Value = 2, Text = "Third Year" },
                new YearItem { Value = 3, Text = "Fourth Year" }
            };
                    StudentYearLevelCb.DisplayMember = "Text";
                    StudentYearLevelCb.ValueMember = "Value";

                    // List of students form - must match API response exactly
            ListOfStudentYearLevelCb.DataSource = new List<YearItem>
            {
                new YearItem { Value = 0, Text = "First Year" },
                new YearItem { Value = 1, Text = "Second Year" },
                new YearItem { Value = 2, Text = "Third Year" },
                new YearItem { Value = 3, Text = "Fourth Year" }
            };
            ListOfStudentYearLevelCb.DisplayMember = "Text";
            ListOfStudentYearLevelCb.ValueMember = "Value";
        }

        private void SidebarBtn_Click(object sender, EventArgs e)
        {
            _sidebarAnimator.Toggle();

            // If maximized, always use scaled positions
            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.UpdatePanelAnimations(this);
                _panelAnimator?.Toggle();
                return;
            }

            // Normal window - use original hardcoded positions
            if (AdminCreationPanel.Visible)
            {
                SetupAdminCreationAnimation();
                _panelAnimator?.Toggle();
            }
            else if (TimeManagementPanel.Visible)
            {
                SetupTimeManagementAnimation();
                _panelAnimator?.Toggle();
            }
            else if (ListOfStudentsPanel.Visible)
            {
                SetupStudentCreationAnimation();
                _panelAnimator?.Toggle();
            }
            else if (StudentPanel.Visible)
            {
                SetupStudentRegistrationAnimation();
                _panelAnimator?.Toggle();
            }
            else if (DashboardPanel.Visible)
            {
                SetupDashboardAnimation();
                _panelAnimator?.Toggle();
            }
            else if (AdminReportsPanel.Visible)
            {
                SetupAdminReportsPanelAnimation();
                _panelAnimator?.Toggle();
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new DialogForm("Close", "Are you sure you want to close this application?"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.IsConfirmed)
                {
                    Application.Exit();
                }
            }
        }

        private void MaximizeBtn_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                MainFormResponsiveLayout.RestoreNormalLayout(this); // Full restore
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                MainFormResponsiveLayout.Apply(this);
            }
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void LettersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (Backspace, Delete, etc.)
            if (char.IsControl(e.KeyChar))
                return;

            // Allow letters and space only
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new DialogForm("Logout", "Are you sure you want to logout?"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.IsConfirmed)
                {
                    var RfidForm = new RFIDForm();
                    RfidForm.Show();
                    this.Close();
                }
            }
        }

        private async void UpdateStudentBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedStudentId))
            {
                ShowNotification("Warning", "Please select a student row first.", NotificationType.Warning);
                return;
            }

            var studentUpdate = new StudentUpdateDAO
            {
                Id = _selectedStudentId,
                FirstName = ListOfStudentFirstNameTb.Text?.Trim(),
                MiddleName = ListOfStudentMiddleNameTb.Text?.Trim(),
                LastName = ListOfStudentLastNameTb.Text?.Trim(),
                StudentId = ListOfStudentStudentIdTb.Text?.Trim(),
                Course = ListOfStudentCourseCb.SelectedItem?.ToString(),
                YearLevel = ListOfStudentYearLevelCb.SelectedValue?.ToString()
            };

            using (var dialog = new DialogForm("Update Student", "Are you sure you want to update this student?"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.IsConfirmed)
                {
                    try
                    {
                        UpdateStudentBtn.Enabled = false;

                        await _adminService.UpdateStudent(studentUpdate);

                        await LoadStudentsAsync();
                        _selectedStudentId = null;
                        ClearStudentFields();
                        ShowNotification("Success", "Student updated successfully.", NotificationType.Success);

                    }
                    catch (Exception ex)
                    {
                        ShowNotification("Error", $"Update failed: {ex.Message}", NotificationType.Error);
                    }
                    finally
                    {
                        UpdateStudentBtn.Enabled = true;
                    }
                }
            }
        }
        private void ClearStudentFields()
        {
            ListOfStudentFirstNameTb.Text = string.Empty;
            ListOfStudentMiddleNameTb.Text = string.Empty;
            ListOfStudentLastNameTb.Text = string.Empty;
            ListOfStudentStudentIdTb.Text = string.Empty;
            ListOfStudentCourseCb.SelectedIndex = 0;
            ListOfStudentYearLevelCb.SelectedIndex = 0;
            _selectedStudentId = null;
        }
        private async void DeleteStudentBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedStudentId))
            {
                ShowNotification("Information", "Please select a student row first.", NotificationType.Information);
                return;
            }

            using (var dialog = new DialogForm("Delete Student", "Are you sure you want to delete this student?"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.IsConfirmed)
                {
                    try
                    {
                        await _adminService.DeleteStudent(new StudentDeletionDAO { Id = _selectedStudentId });
                        await LoadStudentsAsync();
                        _selectedStudentId = null;

                        ShowNotification("Success", "Student deleted successfully.", NotificationType.Success);

                    }
                    catch (Exception ex)
                    {
                        ShowNotification("Error", $"Delete failed: {ex.Message}", NotificationType.Error);
                    }
                }
            }
        }   
        private void ListOfStudentDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = ListOfStudentDgv.Rows[e.RowIndex];
            var student = row.DataBoundItem as UserDto;

            if (student == null) return;

            _selectedStudentId = student.Id.ToString();

            ListOfStudentFirstNameTb.Text = row.Cells["FirstName"].Value?.ToString();
            ListOfStudentMiddleNameTb.Text = row.Cells["MiddleName"].Value?.ToString();
            ListOfStudentLastNameTb.Text = row.Cells["LastName"].Value?.ToString();
            ListOfStudentStudentIdTb.Text = row.Cells["StudentId"].Value?.ToString();
            ListOfStudentCourseCb.SelectedItem = row.Cells["Course"].Value?.ToString();

            var yearLevelText = row.Cells["YearLevel"].Value?.ToString();
            foreach (YearItem item in ListOfStudentYearLevelCb.Items)
            {
                if (item.Text == yearLevelText)
                {
                    ListOfStudentYearLevelCb.SelectedItem = item;
                    break;
                }
            }
        }

        private void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            new NotificationModalForm(title, message, type).Show(this);
        }

        private void ReportBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(AdminReportsPanel);
            HeaderPanelLabel.Text = "Admin Reports";
            SetupAdminReportsPanelAnimation();
        }
    }
}

