using LibraryComputerLaboratoryTimeManagementSystem.FORMS;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.UserServices;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem
{
    public partial class RFIDForm : Form
    {
        private readonly AdminService _adminService;

        public RFIDForm()
        {
            InitializeComponent();
            _adminService = new AdminService();
            this.Shown += (s, e) => RFIDTextBox.Focus();
        }

        private async void RFIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                string rfidValue = RFIDTextBox.Text.Trim();
                if (string.IsNullOrEmpty(rfidValue)) return;

                // 1. Backdoor for "superadmin" text
                if (string.Equals(rfidValue, "superadmin", StringComparison.OrdinalIgnoreCase))
                {
                    SuperAdminState.isSuperAdmin = true;
                    var superForm = new SuperAdminForm();
                    superForm.Show();
                    this.Hide();
                    return;
                }

                try
                {
                    RFIDTextBox.Enabled = false;

                    // FIX: AuthenticateRfid returns bool, not string
                    bool isSuccess = await _adminService.AuthenticateRfid(rfidValue);

                    RFIDTextBox.Enabled = true;

                    if (!isSuccess)
                    {
                        MessageBox.Show("Authentication failed. Invalid RFID or Server Error.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        RFIDTextBox.Clear();
                        RFIDTextBox.Focus();
                        return;
                    }

                    // FIX: The service already saved the token to ApiConfig.Token. Use that.
                    string accessToken = ApiConfig.Token;

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        MessageBox.Show("Login successful but no token was saved.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. Decode Role from the saved token
                    string role = GetRoleFromJwt(accessToken);

                    if (string.Equals(role, "SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(role, "SUPERADMIN", StringComparison.OrdinalIgnoreCase))
                    {
                        SuperAdminState.isSuperAdmin = true;
                        var superForm = new SuperAdminForm();
                        superForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        SuperAdminState.isSuperAdmin = false;
                        var main = new MainForm();
                        main.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    RFIDTextBox.Enabled = true;
                    MessageBox.Show($"System Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    RFIDTextBox.Clear();
                    RFIDTextBox.Focus();
                }
            }
        }

        private string GetRoleFromJwt(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3) return null;

                var payload = parts[1];

                // Fix Base64 padding
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                var jsonString = Encoding.UTF8.GetString(jsonBytes);
                var jwtJson = JObject.Parse(jsonString);

                var role = jwtJson["role"]?.ToString();
                if (string.IsNullOrEmpty(role))
                {
                    role = jwtJson["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]?.ToString();
                }

                return role;
            }
            catch
            {
                return null;
            }
        }
    }
}
