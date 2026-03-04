using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class UpdateSettingProgressBarForm : Form
    {
        private readonly AdminService _adminService;

        public UpdateSettingProgressBarForm()
        {
            InitializeComponent();
            _adminService = new AdminService();
            this.Shown += async (s, e) => await StartSync();
        }

        private async Task StartSync()
        {
            try
            {
                // Step 1
                SetProgress(10, "Preparing sync...");
                await Task.Delay(500);

                // Step 2
                SetProgress(30, "Connecting to database...");
                await Task.Delay(500);

                // Step 3
                SetProgress(50, "Syncing enrolled students...");
                bool success = await _adminService.UpdateSetting(true);

                // Step 4
                SetProgress(75, "Finalizing sync...");
                await Task.Delay(500);

                if (success)
                {
                    SetProgress(100, "Sync complete!");
                    await Task.Delay(800);

                    MessageBox.Show("Student enrollment synced successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SetProgress(0, "Sync failed.");
                    MessageBox.Show("Failed to sync with the database. Please try again.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                SetProgress(0, "An error occurred.");
                MessageBox.Show($"System Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void SetProgress(int value, string message)
        {
            UpdateSettingProgressBar.Value = value;
            ProgressBarLabel.Text = message;
        }
    }
}