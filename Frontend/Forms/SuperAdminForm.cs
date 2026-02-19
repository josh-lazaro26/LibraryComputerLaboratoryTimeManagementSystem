using LibraryComputerLaboratoryTimeManagementSystem.FORMS;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.Sidebar_Animation;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class SuperAdminForm : Form
    {
        private AdminService _ApiClient;
        private ButtonHoverEffect _ButtonHoverEffect;


        private bool isFullScreen = false;
        private FormWindowState previousWindowState;
        private FormBorderStyle previousBorderStyle;
        private Rectangle previousBounds;


        public SuperAdminForm()
        {
            InitializeComponent();
            _ApiClient = new AdminService(); // or inject it
            SuperadminPasswordTb.UseSystemPasswordChar = true;
            ShowPasswordPb.Visible = true;   // eye-open
            HidePasswordPb.Visible = false;  // eye-closed

            ButtonHover();

            SuperAdminUsernameTb.KeyDown += SuperAdminUsernameTb_KeyDown;
            SuperadminPasswordTb.KeyDown += SuperadminPasswordTb_KeyDown;

            SuperAdminLoginBtn.FlatStyle = FlatStyle.Flat;
            SuperAdminLoginBtn.BackColor = Color.FromArgb(59, 130, 105);
            SuperAdminLoginBtn.ForeColor = SystemColors.ControlLightLight;
            SuperAdminLoginBtn.UseVisualStyleBackColor = false;
            this.KeyPreview = true;
            this.KeyDown += SuperAdminForm_KeyDown;
            this.FormBorderStyle = FormBorderStyle.None;


        }

        private void SuperAdminForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullScreen();
                e.SuppressKeyPress = true;
            }
        }
        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                // Save current state
                previousWindowState = this.WindowState;
                previousBorderStyle = this.FormBorderStyle;
                previousBounds = this.Bounds;

                // Enter fullscreen (REAL fullscreen)
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Normal; // Important reset
                this.Bounds = Screen.FromControl(this).Bounds;

                isFullScreen = true;
            }
            else
            {
                // Exit fullscreen and restore everything exactly
                this.FormBorderStyle = previousBorderStyle;
                this.WindowState = previousWindowState;
                this.Bounds = previousBounds;

                isFullScreen = false;
            }
        }

        private async void SuperAdminLoginBtn_Click(object sender, EventArgs e)
        {

            var username = SuperAdminUsernameTb.Text;
            var password = SuperadminPasswordTb.Text;

            SuperAdminLoginBtn.Text = "Logging in...";
            SuperAdminLoginBtn.Cursor = Cursors.WaitCursor;

            var authResponseJson = await _ApiClient.AuthenticateAsync(username, password);

            SuperAdminLoginBtn.Text = "Login";
            SuperAdminLoginBtn.Cursor = Cursors.Hand;


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
        private void ButtonHover()
        {

            _ButtonHoverEffect = new ButtonHoverEffect(
                normalBackColor: Color.FromArgb(0, 68, 52),
                hoverBackColor: Color.FromArgb(0, 102, 78),
                normalForeColor: Color.White,
                hoverForeColor: Color.White
            );
            _ButtonHoverEffect.Attach(CloseBtn);
            _ButtonHoverEffect.Attach(SuperAdminLoginBtn);
            _ButtonHoverEffect.Attach(MaximizeBtn);
            _ButtonHoverEffect.Attach(MinimizeBtn);
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

        private void SuperAdminUsernameTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SuperAdminLoginBtn.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void SuperadminPasswordTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SuperAdminLoginBtn.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MaximizeBtn_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
