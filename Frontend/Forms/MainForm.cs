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
using Newtonsoft.Json.Linq;
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
        private Dictionary<string, TimeSpan> _remainingBySessionId;
        private System.Windows.Forms.Timer _countdownTimer;
        private string _selectedStudentId;
        private string _selectedEvaluationId = null;

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

            _signalRService.NewStudentOpenedSession += (Guid userId, string schoolId, TimeSpan availableDuration) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => HandleNewSession(userId, schoolId, availableDuration)));
                    //Console.WriteLine($"Student kwan {userId} Logged in his skol id is {schoolId} his doration is {availableDuration}");
                }
                else
                    HandleNewSession(userId, schoolId, availableDuration);
            };

            _signalRService.LoggedOutSession += (Guid userId) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => HandleSessionTerminated(userId)));
                    //Console.WriteLine($"User ID kol:{userId} gi logged out");
                }
                else
                {
                    HandleSessionTerminated(userId);
                }
            };

            _signalRService.UpdatedSession += (TimeSpan duration) =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => HandleSessionUpdated(duration)));
                else
                    HandleSessionUpdated(duration);
            };

            _signalRService.Terminate += () =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => HandleTerminate()));
                else
                    HandleTerminate();
            };

            ListOfStudentDgv.CellClick += ListOfStudentDgv_CellClick;
            ListOfStudentDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ListOfStudentDgv.MultiSelect = false;
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            InitTimeButtons();
            WireTimeButtonClicks();

            InitTimeLabelsAndTimer();

            _panelHandler.ShowOnly(DashboardPanel);
            if (this.WindowState == FormWindowState.Maximized)
            {
                MainFormResponsiveLayout.Apply(this);
                _panelAnimator?.Toggle();
            }
            Console.WriteLine("tro o kon fols:"+SuperAdminState.isSuperAdmin);
            await LoadAdminsAsync();
            await LoadActiveSessionsToButtons();
            await LoadEvaluations();
        }

        private void HandleNewSession(Guid userId, string schoolId, TimeSpan availableDuration)
        {
            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (btn.Tag == null) // vacant slot
                {
                    // No SessionDto from SignalR anymore — store what we have
                    btn.Text = schoolId;
                    var sessionId = userId.ToString();
                    btn.Tag = new SessionDto {Name = schoolId };
                    _remainingBySessionId[sessionId] = availableDuration;
                    btn.Enabled = true;
                    btn.ForeColor = SystemColors.ControlLight;

                    _remainingBySessionId[userId.ToString()] = availableDuration;
                    lbl.Text = ToMmSs(availableDuration);
                    break;
                }
            }
        }

        private void HandleSessionTerminated(Guid userId)
        {
            var userIdStr = userId.ToString();

            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (btn.Tag is SessionDto s &&
                    (s.UserId == userIdStr || s.Id == userIdStr))
                {
                    if (s.Id != null)
                        _remainingBySessionId.Remove(s.Id);

                    btn.Text = "Vacant";
                    btn.Tag = null;
                    btn.Enabled = true;
                    btn.ForeColor = SystemColors.ControlLight;
                    lbl.Text = "00:00:00";

                    Console.WriteLine($"Cleared slot {i + 1} for user {userIdStr}");
                    break;
                }
            }
            _ = LoadActiveSessionsToButtons();
        }

        private void HandleSessionUpdated(TimeSpan duration)
        {
            // Update the first active session found — or match by context if you track current userId
            Console.WriteLine($"Session updated with new duration: {duration}");
            _ = LoadActiveSessionsToButtons(); // safest: reload all
        }

        private void HandleTerminate()
        {
            Console.WriteLine("Session terminated by server.");
            _ = LoadActiveSessionsToButtons();
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
            _panelAnimator.AddPanel(StudentsTablePanel, new Point(314, 111), new Point(177, 111));
            SnapPanelsToCurrentState();
        }
        private void SetupEvaluationAnimation()
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
            _panelAnimator.AddPanel(EvaluationTbPanel, new Point(317, 138), new Point(178, 138));
            _panelAnimator.AddPanel(EvaluationListPanel, new Point(818, 138), new Point(679, 138));
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
            _sidebarHoverEffect.Attach(SidebarBtn);
            _sidebarHoverEffect.Attach(EvaluationSidebarBtn);
            _sidebarHoverEffect.Attach(LogoutBtn);

            _sidebarHoverEffect.Attach(AdminCreation);
            _sidebarHoverEffect.Attach(ReportBtn);

            _panelHandler = new PanelHandler();
            _panelHandler.AddPanels(DashboardPanel, DatabaseSyncPanel, AdminReportsPanel, AdminCreationPanel, TimeManagementPanel, ListOfStudentsPanel,EvaluationPanel);
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
                    currentRemaining = TimeSpan.Zero;

                using (var modal = new AddTimeModalForm(session.Id, session.UserId, _remainingBySessionId))
                {
                    if (modal.ShowDialog(this) == DialogResult.OK)
                    {
                        switch (modal.Action)
                        {
                            case SessionModalAction.Updated:
                                if (modal.UpdatedRemaining.HasValue)
                                {
                                    var newDuration = modal.UpdatedRemaining.Value.ToString(@"hh\:mm\:ss");

                                    // Use UserId (GUID string), not session.Id
                                    await _adminService.UpdateSession(session.UserId, newDuration);

                                    _remainingBySessionId[session.Id] = modal.UpdatedRemaining.Value;
                                }
                                break;

                            case SessionModalAction.Terminated:
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

            _remainingBySessionId = new Dictionary<string, TimeSpan>();

            _countdownTimer = new System.Windows.Forms.Timer();
            _countdownTimer.Interval = 1000; // 1 second
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start(); // start ticking for updates
        }
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (_timeLabels == null || _remainingBySessionId == null) return;

            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (btn.Tag is SessionDto s && s.Id != null && _remainingBySessionId.TryGetValue(s.Id, out var remaining))
                {
                    // Freeze the timer if inactive — keep button text as-is
                    if (!s.Active)
                    {
                        lbl.Text = ToMmSs(remaining); // just display, don't decrement
                        continue;
                    }

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
            HeaderPanelLabel.Text = "Reports";
        }
        private void EvaluationSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(EvaluationPanel);
            HeaderPanelLabel.Text = "Evaluation";
            SetupEvaluationAnimation();
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
            if (resp == null || !resp.IsSuccess || resp.Items == null) return;

            var sessions = resp.Items.Take(_timeButtons.Count).ToList();
            _remainingBySessionId.Clear();

            for (int i = 0; i < _timeButtons.Count; i++)
            {
                var btn = _timeButtons[i];
                var lbl = _timeLabels[i];

                if (i < sessions.Count)
                {
                    var s = sessions[i];
                    btn.Tag = s;
                    btn.Enabled = true;
                    btn.ForeColor = Color.White;
                    btn.Text = !string.IsNullOrEmpty(s.SchoolId) ? s.SchoolId : "Unknown";

                    var remaining = ParseApiDuration(s.Duration);
                    _remainingBySessionId[s.Id] = remaining;

                    // If inactive, show frozen time but don't let it tick
                    lbl.Text = ToMmSs(remaining);
                }
                else
                {
                    btn.Text = "Vacant";
                    btn.Tag = null;
                    btn.Enabled = true;
                    btn.ForeColor = SystemColors.ControlLight;
                    lbl.Text = "00:00:00";
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
            else if (EvaluationPanel.Visible)
            {
                SetupEvaluationAnimation();
                _panelAnimator?.Toggle();
            }
            else if (ListOfStudentsPanel.Visible)
            {
                SetupStudentCreationAnimation();
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

        private async void LogoutBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new DialogForm("Logout", "Are you sure you want to logout?"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.IsConfirmed)
                {
                    var adminService = new AdminService();
                    bool success = await adminService.Logout();

                    if (success)
                    {
                        var RfidForm = new RFIDForm();
                        RfidForm.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Logout failed. Please try again.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            ListOfStudentStudentIdTb.Text = row.Cells["StudentId"].Value?.ToString();
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

        private async void AddEvaluationBtn_Click(object sender, EventArgs e)
        {
            // Replace "EvaluationQuestionTextBox" with your actual TextBox name
            string question = EvaluationTb.Text.Trim();

            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("Please enter a question.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = await _adminService.CreateEvaluation(question);

            if (success)
            {
                MessageBox.Show("Evaluation created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadEvaluations(); // refresh DGV
            }
            else
            {
                MessageBox.Show("Failed to create evaluation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task LoadEvaluations()
        {
            var body = await _adminService.GetLatestEvaluation();

            if (string.IsNullOrWhiteSpace(body))
            {
                EvaluationDgv.DataSource = null;
                return;
            }

            var item = JsonConvert.DeserializeObject<EvaluationModel>(body);

            EvaluationDgv.DataSource = item != null
                ? new List<EvaluationModel> { item }
                : new List<EvaluationModel>();

            if (EvaluationDgv.Columns["Id"] != null)
                EvaluationDgv.Columns["Id"].Visible = false;
        }
        private async void UpdateEvaluationBtn_Click(object sender, EventArgs e)
        {
            string id = _selectedEvaluationId;
            string question = EvaluationTb.Text.Trim();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("Please select an evaluation from the list and enter a question.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = await _adminService.UpdateEvaluation(id, question);

            if (success)
            {
                MessageBox.Show("Evaluation updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _selectedEvaluationId = null;
                await LoadEvaluations(); // refresh DGV
            }
            else
            {
                MessageBox.Show("Failed to update evaluation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EvaluationDgv_SelectionChanged(object sender, EventArgs e)
        {
            if (EvaluationDgv.SelectedRows.Count > 0)
            {
                _selectedEvaluationId = EvaluationDgv.SelectedRows[0].Cells["Id"].Value?.ToString();
                EvaluationTb.Text = EvaluationDgv.SelectedRows[0].Cells["Question"].Value?.ToString();
            }
        }

        private void SyncDatabaseBtn_Click(object sender, EventArgs e)
        {
            var updateSettingRfidForm = new UpdateSettingRfidForm(this);
            this.Hide();
            updateSettingRfidForm.Show();
        }
    }
}

