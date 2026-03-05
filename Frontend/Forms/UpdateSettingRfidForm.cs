using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class UpdateSettingRfidForm : Form
    {
        private readonly AdminService _adminService;
        private readonly Form _mainForm;  
        public string ScannedRfid { get; private set; }

        public UpdateSettingRfidForm(Form mainForm)  // UPDATED CONSTRUCTOR
        {
            InitializeComponent();
            _adminService = new AdminService();
            _mainForm = mainForm; 
            this.Shown += (s, e) => RFIDTextBox.Focus();
        }


        private string GetRoleFromJwt(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3) return null;

                string payload = parts[1];

                payload = payload.Replace('-', '+').Replace('_', '/');

                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                var jsonString = Encoding.UTF8.GetString(jsonBytes);

                JObject jwtJson = JObject.Parse(jsonString);

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

                    string role = GetRoleFromJwt(accessToken);

                    bool isAuthorized =
                        string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(role, "SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(role, "SUPERADMIN", StringComparison.OrdinalIgnoreCase);

                    if (!isAuthorized)
                    {
                        MessageBox.Show("Access denied. Only Admin or Super Admin can access this setting.",
                            "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        RFIDTextBox.Clear();
                        RFIDTextBox.Focus();
                        return;
                    }

                    ScannedRfid = rfidValue;

                    var updateSettingForm = new UpdateSettingProgressBarForm(_mainForm);  // PASS mainForm
                    updateSettingForm.Show();
                    this.Close();
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
    }
}