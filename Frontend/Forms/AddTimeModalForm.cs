using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.Sidebar_Animation;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.UIResponsiveness;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class AddTimeModalForm : Form
    {
        private readonly int _sessionId;
        private readonly string _originalDuration;
        private readonly AdminService _adminService;
        private PanelHandler _panelHandler;
        private LabelSwapAnimator swapAnimator;
        private UIResponsiveness _uiResponsiveness;
        private UIResizer uiResizer;
        public SessionModalAction Action { get; private set; } = SessionModalAction.None;

        public TimeSpan? UpdatedRemaining { get; private set; }


        private readonly Dictionary<int, TimeSpan> _remainingBySessionId;
  
        private Timer _refreshTimer;

        private bool _isHourMode = true;
        public AddTimeModalForm(int sessionId, Dictionary<int, TimeSpan> remainingDict)
        {
            InitializeComponent();
            UIHandler();

            _adminService = new AdminService();

            _sessionId = sessionId;
            _remainingBySessionId = remainingDict;

            this.Text = $"Update Session {_sessionId}";

            InitializeRefreshTimer();

            AddTimePanel.SetDoubleBuffered(true);
            swapAnimator = new LabelSwapAnimator(HourLabel, MinutesLabel, this);

        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (_remainingBySessionId.TryGetValue(_sessionId, out var remaining))
            {
                DurationLabel.Text = "Time Remaining: " + ToMmSs(remaining);
            }
            else
            {
                DurationLabel.Text = "00:00";
            }
        }
        public enum SessionModalAction
        {
            None,
            Updated,
            Terminated
        }
        private void UIHandler()
        {
            _panelHandler = new PanelHandler();
            _panelHandler.AddPanels(AddTimePanel);
        }

        private static string ToMmSs(TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero)
                return "00:00";

            if (ts.TotalHours >= 1)
                return ts.ToString(@"hh\:mm\:ss");

            return ts.ToString(@"mm\:ss");
        }

        private void AddTimeBtn_Click(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(AddTimePanel);
        }

        private async void UpdateSessionBtn_Click(object sender, EventArgs e)
        {
            UpdateSessionBtn.Enabled = false;

            try
            {
                TimeSpan newDuration;
                string input = DurationTb.Text.Trim();

                if (!TryParseDuration(input, _isHourMode, out newDuration))
                {
                    ShowNotification("Information", "Invalid input. Use a number (e.g. 2) or H:MM format (e.g. 1:30).", NotificationType.Information);
                    return;
                }

                if (newDuration.TotalHours > 24)
                {
                    ShowNotification("Information", "Duration cannot exceed 24 hours.", NotificationType.Information);
                    return;
                }

                if (newDuration <= TimeSpan.Zero)
                {
                    ShowNotification("Information", "Duration must be greater than zero.", NotificationType.Information);
                    return;
                }

                string normalizedDuration = newDuration.ToString(@"hh\:mm\:ss");

                await _adminService.UpdateSessionDuration(_sessionId, normalizedDuration);

                UpdatedRemaining = newDuration;
                Action = SessionModalAction.Updated;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateSessionBtn.Enabled = true;
            }
        }

        private static bool TryParseDuration(string input, bool isHourMode, out TimeSpan result)
        {
            result = TimeSpan.Zero;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (input.Contains(":"))
            {
                // H:MM or M:SS style — split into two parts
                var parts = input.Split(':');
                if (parts.Length != 2) return false;

                if (!int.TryParse(parts[0], out int left) || !int.TryParse(parts[1], out int right))
                    return false;

                if (right < 0 || right > 59) return false;

                // In hour mode: left = hours, right = minutes
                // In minute mode: left = minutes, right = seconds
                result = isHourMode
                    ? new TimeSpan(left, right, 0)
                    : new TimeSpan(0, left, right);
            }
            else
            {
                // Plain number
                if (!int.TryParse(input, out int value)) return false;

                result = isHourMode
                    ? TimeSpan.FromHours(value)
                    : TimeSpan.FromMinutes(value);
            }

            return true;
        }


        private void InitializeRefreshTimer()
        {
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 500; // smoother refresh
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void AddTimeModalForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _refreshTimer?.Stop();
        }

        private void UpdateStudentTimeCloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void HourLabel_Click(object sender, EventArgs e)
        {
            _isHourMode = false;
            swapAnimator.Swap(new Point(290, 197), new Point(290, 139), label1IsActive: true);
        }

        private void MinutesLabel_Click(object sender, EventArgs e)
        {
            _isHourMode = true;
            swapAnimator.Swap(new Point(290, 139), new Point(290, 197), label1IsActive: false);
        }

        private void DurationTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ':' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == ':' && DurationTb.Text.Contains(":"))
            {
                e.Handled = true;
                return;
            }

            // Trigger update on Enter
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // suppress the ding sound
                UpdateSessionBtn_Click(sender, EventArgs.Empty);
            }
        }

        private void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            new NotificationModalForm(title, message, type).Show(this);
        }
    }
}
