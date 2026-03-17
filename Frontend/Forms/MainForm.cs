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
using SignalRDemo;
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
        private List<Button> _timeButtons;
        public SidebarAnimator _sidebarAnimator;
        private List<Label> _timeLabels;
        private bool _isSidebarOpen = false;
        private Dictionary<string, TimeSpan> _remainingBySessionId;
        private System.Windows.Forms.Timer _countdownTimer;
        private string _selectedStudentId;
        private string _selectedEvaluationId = null;

        private readonly SignalRService _signalRService;
        private readonly UnauthenticatedSignalRService _unauthSignalR;

        private int _studentsPage = 1;
        private const int _studentsPageSize = 20;

        private int _reportsPage = 1;
        private const int _reportsPageSize = 20;

        // Add to your fields at the top of MainForm
        private int _clientDevicesPage = 1;
        private const int _clientDevicesPageSize = 20;

        private System.Windows.Forms.Timer _searchDebounceTimer;
        private System.Windows.Forms.Timer _reportSearchDebounceTimer;

        private string _selectedDeviceName = null;
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

            _unauthSignalR = new UnauthenticatedSignalRService();
            _ = _unauthSignalR.InitializeAsync();

            SignalRInitialize();

            PcListDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            PcListDgv.MultiSelect = false;
            PcListDgv.SelectionChanged += PcListDgv_SelectionChanged;

            SearchQuery.TextChanged += SearchQuery_TextChanged;
            StudentListNextPageBtn.Click += StudentListNextPageBtn_Click;
            StudentListPrevtPageBtn.Click += StudentListPrevtPageBtn_Click;

            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 400;
            _searchDebounceTimer.Tick += async (s, e) =>
            {
                _searchDebounceTimer.Stop();
                var currentQuery = SearchQuery.Text.Trim();
                Console.WriteLine($"Debounce fired — query: '{currentQuery}'");
                await LoadStudentsAsync(1, currentQuery);
            };

            ReportSearchQuery.TextChanged += ReportSearchQuery_TextChanged;

            _reportSearchDebounceTimer = new System.Windows.Forms.Timer();
            _reportSearchDebounceTimer.Interval = 400;
            _reportSearchDebounceTimer.Tick += async (s, e) =>
            {
                _reportSearchDebounceTimer.Stop();
                var currentQuery = ReportSearchQuery.Text.Trim();
                Console.WriteLine($"Report debounce fired — query: '{currentQuery}'");
                await LoadSessionHistoriesAsync(1);
            };

            _signalRService.NewSession += (Guid userId, string schoolId, TimeSpan availableDuration) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => HandleNewSession(userId, schoolId, availableDuration)));
                }
                else
                    HandleNewSession(userId, schoolId, availableDuration);
            };

            _signalRService.LoggedOutSession += (Guid userId) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => HandleSessionTerminated(userId)));
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

                if (btn.Tag == null)
                {
                    var sessionId = userId.ToString();

                    btn.Text = schoolId;
                    btn.Tag = new SessionDto
                    {
                        Name = schoolId,
                        Id = sessionId,      
                        UserId = sessionId,  
                        Active = true        
                    };

                    _remainingBySessionId[sessionId] = availableDuration;

                    btn.Enabled = true;
                    btn.ForeColor = SystemColors.ControlLight;
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
                    break;
                }
            }
            // Keep this reload — it's fine now that active filtering is in place
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

            _panelAnimator.AddPanel(EvaluationTbPanel, new Point(317, 138), new Point(193, 138));
            _panelAnimator.AddPanel(EvaluationListPanel, new Point(740, 138), new Point(616, 138));
            SnapPanelsToCurrentState();
        }
        private void SetupDevicesAnimation()
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

            _panelAnimator.AddPanel(ListOfDevicesPanel, new Point(390, 98), new Point(265, 98)); // ← fix
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
            _sidebarHoverEffect.Attach(ListOfDevicesSidebarBtn);
            _sidebarHoverEffect.Attach(LogoutBtn);

            _sidebarHoverEffect.Attach(AdminCreation);
            _sidebarHoverEffect.Attach(ReportBtn);

            _panelHandler = new PanelHandler();
            _panelHandler.AddPanels(DashboardPanel, PcToRestartPanel, AdminReportsPanel, AdminCreationPanel, TimeManagementPanel, ListOfStudentsPanel,EvaluationPanel);
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

                if (btn.Tag is SessionDto s && s.Id != null &&
                    _remainingBySessionId.TryGetValue(s.Id, out var remaining))
                {
                    // ← REMOVE the !s.Active freeze block entirely, or change to:
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
        private async void EvaluationSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(EvaluationPanel);
            HeaderPanelLabel.Text = "Evaluation";
            SetupEvaluationAnimation();
            await LoadEvaluations();
        }
        private async void ListOfDevicesSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(PcToRestartPanel);
            HeaderPanelLabel.Text = "PC Management";
            SetupDevicesAnimation();
            await LoadClientDevicesAsync(1);
        }
        private async void ListOfStudentsSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(ListOfStudentsPanel);
            HeaderPanelLabel.Text = "List of Students";
            SetupStudentCreationAnimation();
            await LoadStudentsAsync(1);
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

            StyleDataGridView(AdminDgv, MainFormResponsiveLayout.GetFontScale());
        }

        private async Task LoadActiveSessionsToButtons()
        {
            SetAllButtonsVacant();
            SetAllLabelsZero();

            var resp = await _adminService.GetActiveSessions(pageNumber: 1, pageSize: 16);
            if (resp == null || !resp.IsSuccess || resp.Items == null) return;

            var sessions = resp.Items
                .Where(s => s.Active)        
                .Take(_timeButtons.Count)
                .ToList();
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
            else if (PcToRestartPanel.Visible)
            {
                SetupDevicesAnimation();
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
                        ShowNotification("Error", "Logout failed. Please try again.", NotificationType.Information);
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
        }

        private void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            new NotificationModalForm(title, message, type).Show(this);
        }

        private async void ReportBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(AdminReportsPanel);
            HeaderPanelLabel.Text = "Admin Reports";
            SetupAdminReportsPanelAnimation();
            await LoadSessionHistoriesAsync(1); 
        }

        private async void AddEvaluationBtn_Click(object sender, EventArgs e)
        {
            // Replace "EvaluationQuestionTextBox" with your actual TextBox name
            string question = EvaluationTb.Text.Trim();

            if (string.IsNullOrWhiteSpace(question))
            {
                ShowNotification("Warning", "Please enter a question.", NotificationType.Warning);
                return;
            }

            bool success = await _adminService.CreateEvaluation(question);

            if (success)
            {
                ShowNotification("Success", "Evaluation created successfully.", NotificationType.Success);

                await LoadEvaluations(); // refresh DGV
            }
            else
            {
                ShowNotification("Error", "Failed to create evaluation.", NotificationType.Error);
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

            if (EvaluationDgv.Columns["CreatedAt"] != null)       // ← ADD
                EvaluationDgv.Columns["CreatedAt"].Visible = false;

            if (EvaluationDgv.Columns["LastModifiedAt"] != null)    // ← ADD
                EvaluationDgv.Columns["LastModifiedAt"].Visible = false;

            // ← ADD THIS BLOCK
            if (EvaluationDgv.Columns["LikedPercentage"] != null)
                EvaluationDgv.Columns["LikedPercentage"].HeaderText = "Like %";

            if (EvaluationDgv.Columns["DislikedPercentage"] != null)
                EvaluationDgv.Columns["DislikedPercentage"].HeaderText = "Dislike %";

            if (EvaluationDgv.Columns["TotalAnswers"] != null)
                EvaluationDgv.Columns["TotalAnswers"].HeaderText = "Total Answers";
            // ← END BLOCK

            StyleDataGridView(EvaluationDgv, MainFormResponsiveLayout.GetFontScale());
        }
        private async void UpdateEvaluationBtn_Click(object sender, EventArgs e)
        {
            string id = _selectedEvaluationId;
            string question = EvaluationTb.Text.Trim();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(question))
            {
                ShowNotification("Warning", "Please select an evaluation from the list and enter a question.", NotificationType.Warning);
                return;
            }

            bool success = await _adminService.UpdateEvaluation(id, question);

            if (success)
            {
                ShowNotification("Success", "Evaluation updated successfully.", NotificationType.Success);

                _selectedEvaluationId = null;
                await LoadEvaluations(); // refresh DGV
            }
            else
            {
                ShowNotification("Error", "Failed to update evaluation.", NotificationType.Error);

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
            var updateSettingRfidForm = new UpdateSettingRfidForm(this, _signalRService);
            this.Hide();
            updateSettingRfidForm.Show();
        }

        private async void RestartPcBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedDeviceName))
            {
                ShowNotification("Information", "Please select a PC from the list first.", NotificationType.Information);
                return;
            }

            using (var dialog = new DialogForm("Restart PC", $"Are you sure you want to restart \"{_selectedDeviceName}\"?"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || !dialog.IsConfirmed) return;

                RestartPcBtn.Enabled = false;
                try
                {
                    bool success = await _adminService.RestartClientDevice(_selectedDeviceName);

                    if (success)
                        ShowNotification("Success", $"Restart command sent to {_selectedDeviceName}.", NotificationType.Success);
                    else
                        ShowNotification("Error", "Failed to send restart command.", NotificationType.Error);
                }
                catch (Exception ex)
                {
                    ShowNotification("Error", $"Error: {ex.Message}", NotificationType.Error);
                }
                finally
                {
                    RestartPcBtn.Enabled = true;
                }
            }
        }
        private void PcListDgv_SelectionChanged(object sender, EventArgs e)
        {
            if (PcListDgv.SelectedRows.Count == 0) return;

            var row = PcListDgv.SelectedRows[0];
            var device = row.DataBoundItem as ClientDeviceDto;

            if (device != null)
            {
                _selectedDeviceName = device.DeviceName;
                Console.WriteLine($"Selected device: {_selectedDeviceName}");
            }
        }

        private async Task LoadClientDevicesAsync(int pageNumber = 1)
        {
            var json = await _adminService.GetClientDevices(pageNumber, _clientDevicesPageSize);
            if (string.IsNullOrWhiteSpace(json)) return;

            var jObj = JObject.Parse(json);

            var itemsToken = jObj["items"];
            if (itemsToken == null) return;

            var devices = itemsToken.ToObject<List<ClientDeviceDto>>();

            PcListDgv.AutoGenerateColumns = true;
            PcListDgv.DataSource = devices;

            if (PcListDgv.Columns.Contains("Id"))
                PcListDgv.Columns["Id"].Visible = false;


            if (PcListDgv.Columns.Contains("DeviceName"))
                PcListDgv.Columns["DeviceName"].HeaderText = "Device Name";

            if (PcListDgv.Columns.Contains("ConnectedAt"))
                PcListDgv.Columns["ConnectedAt"].HeaderText = "Connected At";

            StyleDataGridView(PcListDgv, MainFormResponsiveLayout.GetFontScale());

            var totalCount = jObj["total_count"]?.Value<int>() ?? 0;
            int totalPages = jObj["total_pages"]?.Value<int>() ?? 1;

            PcPageLabel.Text = $"Page {pageNumber} of {Math.Max(1, totalPages)}";
            PcPrevPageBtn.Enabled = jObj["has_previous_page"]?.Value<bool>() ?? false;
            PcNextPageBtn.Enabled = jObj["has_next_page"]?.Value<bool>() ?? false;

            _clientDevicesPage = pageNumber;
        }
        private async Task LoadStudentsAsync(int pageNumber = 1, string query = null)
        {
            Console.WriteLine($"LoadStudentsAsync — page: {pageNumber}, query: '{query}'");

            var json = await _adminService.GetStudents(query, pageNumber, _studentsPageSize);
            if (string.IsNullOrWhiteSpace(json)) return;

            var jObj = JObject.Parse(json);

            var students = jObj["items"].ToObject<List<UserDto>>();

            ListOfStudentDgv.AutoGenerateColumns = true;
            ListOfStudentDgv.DataSource = students;

            if (ListOfStudentDgv.Columns.Contains("Id"))
                ListOfStudentDgv.Columns["Id"].Visible = false;

            StyleDataGridView(ListOfStudentDgv, MainFormResponsiveLayout.GetFontScale());

            if (ListOfStudentDgv.Columns.Contains("Name"))
                ListOfStudentDgv.Columns["Name"].HeaderText = "Name";

            if (ListOfStudentDgv.Columns.Contains("SchoolId"))
                ListOfStudentDgv.Columns["SchoolId"].HeaderText = "School ID";

            if (ListOfStudentDgv.Columns.Contains("CourseCode"))
                ListOfStudentDgv.Columns["CourseCode"].HeaderText = "Course Code";

            if (ListOfStudentDgv.Columns.Contains("SchoolYear"))
                ListOfStudentDgv.Columns["SchoolYear"].HeaderText = "School Year";

            if (ListOfStudentDgv.Columns.Contains("EnrollmentStatus"))
                ListOfStudentDgv.Columns["EnrollmentStatus"].HeaderText = "Enrollment Status";

            int totalPages = jObj["total_pages"]?.Value<int>() ?? 1;
            ListOfStudentPageLabel.Text = $"Page {pageNumber} of {Math.Max(1, totalPages)}";

            StudentListPrevtPageBtn.Enabled = jObj["has_previous_page"]?.Value<bool>() ?? false;
            StudentListNextPageBtn.Enabled = jObj["has_next_page"]?.Value<bool>() ?? false;

            _studentsPage = pageNumber;
        }
        private async Task LoadSessionHistoriesAsync(int pageNumber = 1)
        {
            var json = await _adminService.GetSessionHistories(pageNumber, _reportsPageSize);
            if (string.IsNullOrWhiteSpace(json)) return;

            var jObj = JObject.Parse(json);
            var itemsToken = jObj["items"];
            if (itemsToken == null) return;

            var histories = itemsToken.ToObject<List<SessionHistoryDto>>();

            ReportsDataGridView.AutoGenerateColumns = true;
            ReportsDataGridView.DataSource = histories;

            if (ReportsDataGridView.Columns.Contains("ConsumedTime"))
                ReportsDataGridView.Columns["ConsumedTime"].Visible = false;

            if (ReportsDataGridView.Columns.Contains("FormattedConsumedTime"))
                ReportsDataGridView.Columns["FormattedConsumedTime"].HeaderText = "Consumed Time";

            if (ReportsDataGridView.Columns.Contains("SchoolId"))
                ReportsDataGridView.Columns["SchoolId"].HeaderText = "School ID";

            if (ReportsDataGridView.Columns.Contains("FullName"))
                ReportsDataGridView.Columns["FullName"].HeaderText = "Full Name";

            StyleDataGridView(ReportsDataGridView, MainFormResponsiveLayout.GetFontScale());

            int totalPages = jObj["total_pages"]?.Value<int>() ?? 1;
            ReportPageLabel.Text = $"Page {pageNumber} of {Math.Max(1, totalPages)}";

            ReportsPrevPageBtn.Enabled = jObj["has_previous_page"]?.Value<bool>() ?? false;
            ReportNextPageBtn.Enabled = jObj["has_next_page"]?.Value<bool>() ?? false;

            _reportsPage = pageNumber;
        }
        private void StyleDataGridView(DataGridView dgv, float fontScale = 1f)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;

            dgv.RowTemplate.Height = (int)(46 * fontScale);
            dgv.ColumnHeadersHeight = (int)(50 * fontScale);
            float baseSize = 14f * fontScale;

            dgv.Font = new System.Drawing.Font("Roboto", baseSize);
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Roboto", baseSize, System.Drawing.FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new System.Drawing.Font("Roboto", baseSize);

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(6, 64, 43);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(6, 64, 43);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(25, 25, 25);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(6, 80, 60);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 245, 238);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(25, 25, 25);
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(6, 80, 60);
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.BackgroundColor = Color.White;
            dgv.GridColor = Color.FromArgb(180, 215, 198);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns.Contains("Question"))
                dgv.Columns["Question"].FillWeight = 150;  
            if (dgv.Columns.Contains("LikedPercentage"))
                dgv.Columns["LikedPercentage"].FillWeight = 80;
            if (dgv.Columns.Contains("DislikedPercentage"))
                dgv.Columns["DislikedPercentage"].FillWeight = 80;
            if (dgv.Columns.Contains("TotalAnswers"))
                dgv.Columns["TotalAnswers"].FillWeight = 100;
            if (dgv.Columns.Contains("DeviceName"))
                dgv.Columns["DeviceName"].FillWeight = 250;
            if (dgv.Columns.Contains("ConnectedAt"))
                dgv.Columns["ConnectedAt"].FillWeight = 150;
            if (dgv.Columns.Contains("PersonnelId"))
                dgv.Columns["PersonnelId"].FillWeight = 200;
            if (dgv.Columns.Contains("RFID"))
                dgv.Columns["RFID"].FillWeight = 200;
        }

        private async void PcNextPageBtn_Click(object sender, EventArgs e)
        {
            await LoadClientDevicesAsync(_clientDevicesPage + 1);
        }

        private async void PcPrevPageBtn_Click(object sender, EventArgs e)
        {
            if (_clientDevicesPage > 1)
                await LoadClientDevicesAsync(_clientDevicesPage - 1);
        }

        private void SearchQuery_TextChanged(object sender, EventArgs e)
        {
            Console.WriteLine($"TextChanged fired — text: '{SearchQuery.Text}'"); // ← ADD
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }
        private async void StudentListPrevtPageBtn_Click(object sender, EventArgs e)
        {
            StudentListNextPageBtn.Enabled = false;
            StudentListPrevtPageBtn.Enabled = false;
            if (_studentsPage > 1)
                await LoadStudentsAsync(_studentsPage - 1, SearchQuery.Text.Trim());
        }

        private async void StudentListNextPageBtn_Click(object sender, EventArgs e)
        {
            StudentListNextPageBtn.Enabled = false;
            StudentListPrevtPageBtn.Enabled = false;
            await LoadStudentsAsync(_studentsPage + 1, SearchQuery.Text.Trim());
        }

        private async void ReportsPrevPageBtn_Click(object sender, EventArgs e)
        {
            ReportsPrevPageBtn.Enabled = false;
            ReportNextPageBtn.Enabled = false;
            if (_reportsPage > 1)
                await LoadSessionHistoriesAsync(_reportsPage - 1);
        }

        private async void ReportNextPageBtn_Click(object sender, EventArgs e)
        {
            ReportsPrevPageBtn.Enabled = false;
            ReportNextPageBtn.Enabled = false;
            await LoadSessionHistoriesAsync(_reportsPage + 1);
        }

        private void ReportSearchQuery_TextChanged(object sender, EventArgs e)
        {
            _reportSearchDebounceTimer.Stop();
            _reportSearchDebounceTimer.Start();
        }
    }
}

