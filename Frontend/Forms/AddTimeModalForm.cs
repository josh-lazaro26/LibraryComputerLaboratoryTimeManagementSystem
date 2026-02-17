using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.Sidebar_Animation;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.UIResponsiveness;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class AddTimeModalForm : Form
    {
        private readonly int _sessionId;
        private readonly string _originalDuration;
        private readonly AdminService _adminService;
        private PanelHandler _panelHandler;

        private UIResponsiveness _uiResponsiveness;
        private UIResizer uiResizer;
        public SessionModalAction Action { get; private set; } = SessionModalAction.None;

        public TimeSpan? UpdatedRemaining { get; private set; }


        private readonly Dictionary<int, TimeSpan> _remainingBySessionId;
  
        private Timer _refreshTimer;

        public AddTimeModalForm(int sessionId, Dictionary<int, TimeSpan> remainingDict)
        {
            InitializeComponent();
            UIHandler();

            _adminService = new AdminService();

            _sessionId = sessionId;
            _remainingBySessionId = remainingDict;

            this.Text = $"Update Session {_sessionId}";

            InitializeRefreshTimer();

        }
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (_remainingBySessionId.TryGetValue(_sessionId, out var remaining))
            {
                DurationLabel.Text = "Time Remaining:" + ToMmSs(remaining);
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
            _panelHandler.AddPanels(AddTimePanel, MainAddModalPanel);
        }

        private static string ToMmSs(TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero)
                return "00:00";

            if (ts.TotalHours >= 1)
                return ts.ToString(@"hh\:mm\:ss");

            return ts.ToString(@"mm\:ss");
        }

        private void CancelSessionBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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
                // 🔴 GET VALUE FROM USER INPUT (not dictionary)
                if (!int.TryParse(DurationTb.Text, out int minutes))
                {
                    MessageBox.Show("Invalid minutes input.");
                    return;
                }

                TimeSpan newDuration = TimeSpan.FromMinutes(minutes);
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

        private void InitializeRefreshTimer()
        {
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 500; // smoother refresh
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void CancelSessionBtn_Click_2(object sender, EventArgs e)
        {
            _panelHandler.ShowOnly(MainAddModalPanel);

        }

        private void AddTimeModalForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _refreshTimer?.Stop();
        }
    }
}
