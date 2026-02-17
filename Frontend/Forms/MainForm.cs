using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.Sidebar_Animation;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.HelperClasses;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Admin;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Session;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Student;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
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

        private SlidePanelController _sidePanelController;
        private ButtonHoverEffect _sidebarHoverEffect;
        private UIResponsiveness _uiResponsiveness;
        private AdminService _adminService;
        private StudentServices _studentServices;
        private UserServices _userService;
        private string _currentStudentRfid;
        private List<Button> _timeButtons;

        private List<Label> _timeLabels;
        private Dictionary<int, TimeSpan> _remainingBySessionId; // sessionId -> remaining
        private System.Windows.Forms.Timer _countdownTimer;

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
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            InitTimeButtons();
            WireTimeButtonClicks();

            InitTimeLabelsAndTimer();
            await LoadAdminsAsync();
            await LoadStudentsAsync();
            await LoadActiveSessionsToButtons();
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
            }
            else
            {
                AdminCreation.Visible = false;
            }
        }
        private void UIHandler()
        {
            _sidePanelController = new SlidePanelController(
                   SidePanelSlider,
                   SliderBtn,
                   Slider1Btn,
                   expandedWidth: SidePanelSlider.Width,
                   slideSpeed: 10
               );

            _sidebarHoverEffect = new ButtonHoverEffect(
                normalBackColor: Color.FromArgb(0, 68, 52),
                hoverBackColor: Color.FromArgb(0, 102, 78),
                normalForeColor: Color.White,
                hoverForeColor: Color.White
            );

            _sidebarHoverEffect.Attach(DashboardSidebarBtn);
            _sidebarHoverEffect.Attach(ListOfStudentsSidebarBtn);
            _sidebarHoverEffect.Attach(TimeManagementSidebarBtn);
            _sidebarHoverEffect.Attach(ReportsSidebarBtn);
            _sidebarHoverEffect.Attach(AdminCreation);

            _panelHandler = new PanelHandler();
            _panelHandler.AddPanels(DashboardPanel, StudentPanel, AdminReportsPanel, AdminCreationPanel, TimeManagementPanel, ListOfStudentsPanel);

            // Initialize UI responsiveness
            _uiResponsiveness = new UIResponsiveness(SidePanelSlider, stepSize: 10);
            _uiResponsiveness.RegisterContentPanels(DashboardPanel, StudentPanel, AdminReportsPanel, AdminCreationPanel, TimeManagementPanel, ListOfStudentsPanel);
            // Add ONLY the components you want centered
            _uiResponsiveness.AddControlsToCenter(
                FullNameTbPanel,
                StudentIDTbPanel,
                WelcomeAdminLabel,
                RegisterStudentBtn,
                StudentRegistrationLabel
            );
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
                    lbl.Text = "00:00";
                }
            }
        }

        private static TimeSpan ParseApiDuration(string durationStr)
        {
            if (!TimeSpan.TryParse(durationStr, out var ts))
                return TimeSpan.Zero;

            // your API often returns negative spans like "-00:00:35..."
            // use absolute value so you can count down visually
            return ts.Duration();
        }

        private static string ToMmSs(TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero) return "00:00";

            // If there is 1 hour or more, show hh:mm:ss
            if (ts.TotalHours >= 1)
            {
                return ts.ToString(@"hh\:mm\:ss");
            }

            // Otherwise just show mm:ss
            return ts.ToString(@"mm\:ss");
        }


        private void DashboardSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(DashboardPanel);
        }
        private void StudentManagementSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(StudentPanel);
        }
        private async void AdminCreation_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(AdminCreationPanel);
            await LoadAdminsAsync();
        }
        private void ReportsSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(AdminReportsPanel);
        }
        private void ListOfStudentsSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(ListOfStudentsPanel);
        }
        private async void TimeManagementSidebarBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(TimeManagementPanel);
            await LoadActiveSessionsToButtons();
        }

        private async Task LoadStudentsAsync(string query = null, int pageNumber = 1, int pageSize = 10)
        {
            var json = await _studentServices.GetStudents(query, pageNumber, pageSize);
            if (json == null)
            {
                MessageBox.Show("Failed to load students.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PagedUserResponse>(json);

            if (result == null || !result.IsSuccess || result.Value?.Items == null)
            {
                MessageBox.Show("Invalid response from server.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ListOfStudentDgv.AutoGenerateColumns = true;
            ListOfStudentDgv.DataSource = result.Value.Items;
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
                    btn.Enabled = false;

                    lbl.Text = "00:00";
                }
            }
        }

        private void SetAllLabelsZero()
        {
            if (_timeLabels == null) return;
            foreach (var lbl in _timeLabels)
                lbl.Text = "00:00";
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
                MessageBox.Show("Admin created successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to create admin.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                YearLevel = StudentYearLevelCb.SelectedItem?.ToString() 
            };

            RegisterStudentBtn.Enabled = false;

            var isSuccess = await _studentServices.CreateStudents(student); // assuming _studentService injected

            RegisterStudentBtn.Enabled = true;

            if (isSuccess)
            {
                MessageBox.Show("Student registered successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to register student.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            }
        }
        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            await LoadAdminsAsync();
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
                MessageBox.Show("Please select an admin row first.");
                return;
            }

            var confirm = MessageBox.Show("Delete this admin?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                await _adminService.DeleteAdmin(new AdminDeletionDAO { Id = _selectedAdminId.Value });
                await LoadAdminsAsync();
                _selectedAdminId = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete failed: {ex.Message}");
            }
        }

        private async void UpdateAdmin_Btn_Click(object sender, EventArgs e)
        {
            if (_selectedAdminId == null)
            {
                MessageBox.Show("Please select an admin row first.");
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
                MessageBox.Show("Admin updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}");
            }
            finally
            {
                UpdateAdmin_Btn.Enabled = true;
            }
        }
    }
}

