using System;
using System.Drawing;
using System.Windows.Forms;

namespace PaginationDemo
{
    /// <summary>
    /// Demo form — drop this into a WinForms project alongside
    /// PaginationControl.cs / PaginationControl.Designer.cs.
    /// Targets .NET Framework 4.x, C# 7.3.
    /// </summary>
    public class DemoForm : Form
    {
        private PaginationControl _pagination;
        private Label _statusLabel;

        public DemoForm()
        {
            Text = "Pagination Demo";
            Size = new Size(700, 200);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(74, 182, 172);   // teal background like the screenshot
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── Status label ──────────────────────────────────────────────
            _statusLabel = new Label
            {
                Text = "Current page: 10 of 20",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            Controls.Add(_statusLabel);

            // ── Pagination control ────────────────────────────────────────
            _pagination = new PaginationControl
            {
                TotalPages = 20,
                CurrentPage = 10,
                Width = 460,
                Height = 52
            };

            // Centre horizontally, vertically
            _pagination.Left = (ClientSize.Width - _pagination.Width) / 2;
            _pagination.Top = (ClientSize.Height - _pagination.Height) / 2;
            _pagination.Anchor = AnchorStyles.None;

            _pagination.PageChanged += OnPageChanged;
            Controls.Add(_pagination);
        }

        private void OnPageChanged(object sender, PageChangedEventArgs e)
        {
            _statusLabel.Text = $"Current page: {e.NewPage} of {_pagination.TotalPages}";
        }
    }
}