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
            if (!double.TryParse(processedPercentage.TrimEnd('%'), out double percent)) return;

            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => OnSyncingProgress(processedPercentage)));
                return;
            }

            // Server's 0–100% maps into the Step 3 band: progress bar 50–75
            int mapped = 50 + (int)(percent / 100.0 * 25);
            SetProgress(mapped, $"Syncing enrolled students... {(int)percent}%", "Step 3 of 4");
        }

        private void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            new NotificationModalForm(title, message, type).Show(this);
        }

        private async Task StartSync()
        {
            try
            {
                SetProgress(10, "Preparing sync...", "Step 1 of 4");
                await Task.Delay(1000);

                SetProgress(30, "Connecting to database...", "Step 2 of 4");
                await Task.Delay(1000);

                // Step 3: trigger the sync — live progress arrives via OnSyncingProgress
                SetProgress(50, "Syncing enrolled students...", "Step 3 of 4");
                bool success = await _adminService.UpdateSetting(true);

                // Step 4: wait a moment so SignalR pushes can finish arriving
                SetProgress(75, "Finalizing sync...", "Step 4 of 4");
                await Task.Delay(2000);

                _syncCompleted = true;

                if (success)
                {
                    SetProgress(100, "Sync complete!", "Done");
                    await Task.Delay(1000);
                    ShowNotification("Success", "Student enrollment synced successfully.", NotificationType.Success);
                }
                else
                {
                    SetProgress(0, "Sync failed.", "Error");
                    ShowNotification("Error", "Failed to sync with the database. Please try again.", NotificationType.Error);
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