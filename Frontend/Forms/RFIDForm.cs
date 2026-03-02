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

                try
                {
                    RFIDTextBox.Enabled = false;

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

                    string accessToken = ApiConfig.Token;


                    if (string.IsNullOrEmpty(accessToken))
                    {
                        MessageBox.Show("Login successful but no token was saved.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Decode Role from the saved token
                    string role = GetRoleFromJwt(accessToken);

                    if (string.Equals(role, "SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(role, "SUPERADMIN", StringComparison.OrdinalIgnoreCase))
                    {
                        SuperAdminState.isSuperAdmin = true;
                        var superForm = new MainForm();
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

                string payload = parts[1];

                // Convert Base64Url to Base64
                payload = payload.Replace('-', '+').Replace('_', '/');

                // Fix padding
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                var jsonString = Encoding.UTF8.GetString(jsonBytes);

                JObject jwtJson = JObject.Parse(jsonString);

                // Try common role claim names
                string role =
                    jwtJson["roles"]?.ToString() ??
                    jwtJson["Roles"]?.ToString() ??
                    jwtJson["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]?.ToString();

                return role;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid token format: " + ex.Message);
                return null;
            }
        }
    }
}