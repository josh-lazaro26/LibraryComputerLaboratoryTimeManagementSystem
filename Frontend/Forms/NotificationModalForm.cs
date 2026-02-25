using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public enum NotificationType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public partial class NotificationModalForm : Form
    {
        private System.Windows.Forms.Timer _autoCloseTimer;

        public NotificationModalForm(string title, string message, NotificationType type = NotificationType.Information)
        {
            InitializeComponent();
            NotificationTitleLabel.Text = title;
            NotificationMessageLabel.Text = message;
            ApplyStyle(type);
            InitAutoCloseTimer();
        }

        private void InitAutoCloseTimer()
        {
            _autoCloseTimer = new System.Windows.Forms.Timer();
            _autoCloseTimer.Interval = 2000; // 2 seconds
            _autoCloseTimer.Tick += (s, e) =>
            {
                _autoCloseTimer.Stop();
                this.Close();
            };
            _autoCloseTimer.Start();
        }

        private void ApplyStyle(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Success:
                    this.BackColor = Color.FromArgb(0, 51, 25);
                    NotificationTitleLabel.ForeColor = Color.White;
                    NotificationTitlePanel.BackColor = Color.FromArgb(0, 153, 76);
                    break;

                case NotificationType.Warning:
                    this.BackColor = Color.FromArgb(80, 53, 0);
                    NotificationTitleLabel.ForeColor = Color.White;
                    NotificationTitlePanel.BackColor = Color.FromArgb(255, 165, 0);
                    break;

                case NotificationType.Error:
                    this.BackColor = Color.FromArgb(80, 0, 0);
                    NotificationTitleLabel.ForeColor = Color.White;
                    NotificationTitlePanel.BackColor = Color.FromArgb(204, 0, 0);
                    break;

                case NotificationType.Information:
                default:
                    this.BackColor = Color.FromArgb(0, 31, 80);
                    NotificationTitleLabel.ForeColor = Color.White;
                    NotificationTitlePanel.BackColor = Color.FromArgb(0, 102, 204);
                    break;
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            _autoCloseTimer.Stop();
            this.Close();
        }
    }
}