using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class UpdateSettingProgressBarForm : ModernProgressForm
    {
        private readonly AdminService _adminService;
        private readonly Form _mainForm;
        private readonly SignalRService _signalRService;
        private bool _syncCompleted = false;

        public UpdateSettingProgressBarForm(Form mainForm, SignalRService signalRService)
        {
            _adminService = new AdminService();
            _mainForm = mainForm;
            _signalRService = signalRService;

            SetTitle("Database Sync");
            SetStepMarkers(10, 30, 75, 100);

            // Subscribe to live SignalR progress
            if (_signalRService != null)
                _signalRService.StudentSyncingProgress += OnSyncingProgress;

            this.Shown += async (s, e) => await StartSync();
        }

        // Fired by SignalR on every "SyncingProgress" push — percentage is "0" to "100"
        private void OnSyncingProgress(string processedPercentage)
        {
            if (_syncCompleted) return;

            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => OnSyncingProgress(processedPercentage)));
                return;
            }

            if (!double.TryParse(processedPercentage.TrimEnd('%'), out double percent)) return;

            SetProgress(processedPercentage, $"Syncing enrolled students... {(int)percent}%", "Step 3 of 4");
        }
        private void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            new NotificationModalForm(title, message, type).Show(this);
        }

        private async Task StartSync()
        {
            try
            {
                SetProgress(0, "Preparing sync...", "Step 1 of 4");
                await Task.Delay(300);

                SetProgress(10, "Connecting to database...", "Step 2 of 4");

                // Hand off completely to SignalR — backend drives 0–100%
                bool success = await _adminService.UpdateSetting(true);

                _syncCompleted = true;

                if (!success)
                {
                    SetProgress(0, "Sync failed.", "Error");
                    ShowNotification("Error", "Failed to sync with the database. Please try again.", NotificationType.Error);
                }
                else
                {
                    ShowNotification("Success", "Student enrollment synced successfully.", NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                _syncCompleted = true;
                SetProgress(0, "An error occurred.", "Error");
                MessageBox.Show($"System Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ReturnToMainForm();
            }
        }

        private void ReturnToMainForm()
        {
            if (this.InvokeRequired) { this.Invoke((Action)ReturnToMainForm); return; }

            // Always unsubscribe to prevent memory leaks or ghost callbacks
            if (_signalRService != null)
                _signalRService.StudentSyncingProgress -= OnSyncingProgress;

            StopAnimation();
            _mainForm?.Show();
            _mainForm?.BringToFront();
            this.Close();
        }
    }
}