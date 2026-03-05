using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class UpdateSettingProgressBarForm : ModernProgressForm  // <-- changed from Form
    {
        private readonly AdminService _adminService;
        private readonly Form _mainForm;

        public UpdateSettingProgressBarForm(Form mainForm)
        {
            // No InitializeComponent() — ModernProgressForm handles all UI
            _adminService = new AdminService();
            _mainForm = mainForm;

            SetTitle("Database Sync");
            SetStepMarkers(10, 30, 75, 100);

            this.Shown += async (s, e) => await StartSync();
        }

        private async Task StartSync()
        {
            try
            {
                // Step 1
                SetProgress(10, "Preparing sync...", "Step 1 of 4");
                await Task.Delay(500);

                // Step 2
                SetProgress(30, "Connecting to database...", "Step 2 of 4");
                await Task.Delay(500);

                // Step 3
                SetProgress(50, "Syncing enrolled students...", "Step 3 of 4");
                bool success = await _adminService.UpdateSetting(true);

                // Step 4
                SetProgress(75, "Finalizing sync...", "Step 4 of 4");
                await Task.Delay(500);

                if (success)
                {
                    SetProgress(100, "Sync complete!", "Done");
                    await Task.Delay(800);
                    MessageBox.Show("Student enrollment synced successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SetProgress(0, "Sync failed.", "Error");
                    MessageBox.Show("Failed to sync with the database. Please try again.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
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
            StopAnimation();
            _mainForm?.Show();
            _mainForm?.BringToFront();
            this.Close();
        }
    }
}