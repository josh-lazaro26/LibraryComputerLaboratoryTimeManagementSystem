using LibraryComputerLaboratoryTimeManagementSystem.FORMS;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class SuperAdminForm : Form
    {
        private AdminService _ApiClient;
        Color stripColor = Color.FromArgb(6, 64, 43);   // your dark green
        Color hoverColor = Color.FromArgb(0, 110, 80);  // lighter green
        Color closeHoverColor = Color.FromArgb(200, 60, 60); // red for close
        public SuperAdminForm()
        {
            InitializeComponent();
            _ApiClient = new AdminService(); // or inject it
            ControlStrip_HoverEvents();
            SuperadminPasswordTb.UseSystemPasswordChar = true;
            ShowPasswordPb.Visible = true;   // eye-open
            HidePasswordPb.Visible = false;  // eye-closed
        }

        private async void SuperAdminLoginBtn_Click(object sender, EventArgs e)
        {

            var username = SuperAdminUsernameTb.Text;
            var password = SuperadminPasswordTb.Text;

            SuperAdminLoginBtn.Enabled = false;
            var authResponseJson = await _ApiClient.AuthenticateAsync(username, password);
            SuperAdminLoginBtn.Enabled = true;

            if (authResponseJson == null)
            {
                MessageBox.Show("Invalid username or password.", "Login failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var obj = JObject.Parse(authResponseJson);
            var accessToken = (string)obj["value"]?["accessToken"];
            var refreshToken = (string)obj["value"]?["refresherToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                ApiConfig.Token = accessToken;

                MessageBox.Show("Login successful.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK; // signal success
                MainForm main = new MainForm();
                main.Show();
                this.Close();                        // closes dialog, Program.cs continues
            }
        }
        private void ControlStrip_HoverEvents()
        {
            // call this in constructor after InitializeComponent();
            ClosePb.MouseEnter += (s, e) => ClosePb.BackColor = closeHoverColor;
            ClosePb.MouseLeave += (s, e) => ClosePb.BackColor = stripColor;

            MaximizePb.MouseEnter += (s, e) => MaximizePb.BackColor = hoverColor;
            MaximizePb.MouseLeave += (s, e) => MaximizePb.BackColor = stripColor;

            MinimizePb.MouseEnter += (s, e) => MinimizePb.BackColor = hoverColor;
            MinimizePb.MouseLeave += (s, e) => MinimizePb.BackColor = stripColor;
        }
        private void ClosePb_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MaximizePb_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;   // toggle to max
            else
                this.WindowState = FormWindowState.Normal;      // toggle back
        }

        private void MinimizePb_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ShowPasswordPb_Click(object sender, EventArgs e)
        {
            SuperadminPasswordTb.UseSystemPasswordChar = false;   // show text
            ShowPasswordPb.Visible = false;
            HidePasswordPb.Visible = true;
        }

        private void HidePasswordPb_Click(object sender, EventArgs e)
        {
            SuperadminPasswordTb.UseSystemPasswordChar = true;    // hide text
            HidePasswordPb.Visible = false;
            ShowPasswordPb.Visible = true;
        }
    }
}
