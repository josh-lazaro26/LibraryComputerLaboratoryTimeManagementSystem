using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PaginationDemo
{
    /// <summary>
    /// A teal-themed pagination control matching the pill/rounded design.
    /// Supports prev/next buttons, ellipsis, and a highlighted current page.
    /// Compatible with C# 7.3 / .NET Framework 4.x
    /// </summary>
    public partial class PaginationControl : UserControl
    {
        // ── Colours (match the screenshot) ─────────────────────────────────
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color TealAccent = Color.FromArgb(74, 182, 172);  // active page bg
        private static readonly Color TealText = Color.FromArgb(74, 182, 172);  // inactive page fg
        private static readonly Color ActiveText = Color.White;
        private static readonly Color EllipsisColor = Color.FromArgb(74, 182, 172);
        private static readonly Color HoverBg = Color.FromArgb(220, 245, 243);

        // ── State ───────────────────────────────────────────────────────────
        private int _totalPages = 20;
        private int _currentPage = 10;

        /// <summary>Raised whenever the user clicks a page button.</summary>
        public event EventHandler<PageChangedEventArgs> PageChanged;

        // ── Public properties ───────────────────────────────────────────────
        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = Math.Max(1, value); Rebuild(); }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = Clamp(value, 1, _totalPages); Rebuild(); }
        }

        // ── Constructor ─────────────────────────────────────────────────────
        public PaginationControl()
        {
            InitializeComponent();

            // Pill-shaped outer container
            this.BackColor = BackgroundColor;
            this.Height = 52;
            this.AutoSize = false;

            flowPanel.FlowDirection = FlowDirection.LeftToRight;
            flowPanel.WrapContents = false;
            flowPanel.Padding = new Padding(12, 6, 12, 6);

            // Draw the rounded-rectangle border on the control itself
            this.Paint += OnControlPaint;
            this.Resize += (s, e) => Invalidate();

            Rebuild();
        }

        // ── Paint the outer pill border ─────────────────────────────────────
        private void OnControlPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = RoundedRect(new Rectangle(1, 1, Width - 2, Height - 2), Height / 2))
            using (var brush = new SolidBrush(BackgroundColor))
            using (var pen = new Pen(Color.FromArgb(210, 210, 210), 1f))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        // ── Rebuild all child buttons ───────────────────────────────────────
        private void Rebuild()
        {
            flowPanel.SuspendLayout();
            flowPanel.Controls.Clear();

            // Prev
            flowPanel.Controls.Add(MakeNavButton("‹ Prev", _currentPage > 1,
                () => NavigateTo(_currentPage - 1)));

            // Always show page 1
            AddPageItems();

            // Next
            flowPanel.Controls.Add(MakeNavButton("Next ›", _currentPage < _totalPages,
                () => NavigateTo(_currentPage + 1)));

            flowPanel.ResumeLayout(true);
            Invalidate();
        }

        /// <summary>
        /// Emits: 1  …  (cur-1)  [cur]  (cur+1)  …  totalPages
        /// with ellipsis suppressed when adjacent pages touch.
        /// </summary>
        private void AddPageItems()
        {
            int cur = _currentPage;
            int total = _totalPages;

            // Page 1
            flowPanel.Controls.Add(MakePageButton(1));

            // Left ellipsis?
            if (cur - 1 > 2)
                flowPanel.Controls.Add(MakeEllipsis());
            else if (cur - 1 == 2)
                flowPanel.Controls.Add(MakePageButton(2));

            // Pages around current  (prev / current / next)
            if (cur - 1 > 1)
                flowPanel.Controls.Add(MakePageButton(cur - 1));

            if (cur != 1 && cur != total)
                flowPanel.Controls.Add(MakePageButton(cur));   // highlighted

            if (cur + 1 < total)
                flowPanel.Controls.Add(MakePageButton(cur + 1));

            // Right ellipsis?
            if (total - cur > 2)
                flowPanel.Controls.Add(MakeEllipsis());
            else if (total - cur == 2)
                flowPanel.Controls.Add(MakePageButton(total - 1));

            // Last page
            if (total > 1)
                flowPanel.Controls.Add(MakePageButton(total));
        }

        // ── Factory helpers ─────────────────────────────────────────────────
        private Control MakePageButton(int page)
        {
            bool active = (page == _currentPage);
            var btn = new PageButton(page.ToString(), active, false)
            {
                Tag = page
            };
            btn.Click += (s, e) => NavigateTo((int)((Control)s).Tag);
            return btn;
        }

        private Control MakeEllipsis()
        {
            var lbl = new Label
            {
                Text = "…",
                ForeColor = EllipsisColor,
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                AutoSize = false,
                Width = 24,
                Height = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 2, 0, 2)
            };
            return lbl;
        }

        private Control MakeNavButton(string label, bool enabled, Action onClick)
        {
            var btn = new PageButton(label, false, true)
            {
                Enabled = enabled
            };
            if (enabled)
                btn.Click += (s, e) => onClick();
            return btn;
        }

        // ── Navigation ──────────────────────────────────────────────────────
        private void NavigateTo(int page)
        {
            page = Clamp(page, 1, _totalPages);
            if (page == _currentPage) return;
            _currentPage = page;
            Rebuild();
            PageChanged?.Invoke(this, new PageChangedEventArgs(page));
        }

        // ── Utility ─────────────────────────────────────────────────────────
        private static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v;

        internal static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // ── Custom owner-drawn button ────────────────────────────────────────────
    internal sealed class PageButton : Control
    {
        private static readonly Color TealAccent = Color.FromArgb(74, 182, 172);
        private static readonly Color TealText = Color.FromArgb(74, 182, 172);
        private static readonly Color ActiveText = Color.White;
        private static readonly Color HoverBg = Color.FromArgb(220, 245, 243);
        private static readonly Color DisabledFg = Color.FromArgb(180, 200, 200);

        private readonly bool _isActive;
        private readonly bool _isNav;
        private bool _hovered;

        public PageButton(string text, bool isActive, bool isNav)
        {
            _isActive = isActive;
            _isNav = isNav;

            Text = text;
            Font = new Font("Segoe UI", isNav ? 10f : 11f, FontStyle.Regular);
            Cursor = Cursors.Hand;
            Margin = new Padding(2, 2, 2, 2);
            DoubleBuffered = true;

            // Size
            if (isNav)
            {
                Width = TextRenderer.MeasureText(text, Font).Width + 16;
                Height = 36;
            }
            else
            {
                Width = 36;
                Height = 36;
            }

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Height / 2;

            if (_isActive)
            {
                // Filled teal circle
                using (var path = PaginationControl.RoundedRect(rect, r))
                using (var brush = new SolidBrush(TealAccent))
                    g.FillPath(brush, path);

                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(ActiveText))
                    g.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), sf);
            }
            else if (_hovered && Enabled)
            {
                using (var path = PaginationControl.RoundedRect(rect, r))
                using (var brush = new SolidBrush(HoverBg))
                    g.FillPath(brush, path);

                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(TealText))
                    g.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), sf);
            }
            else
            {
                Color fg = Enabled ? TealText : DisabledFg;
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(fg))
                    g.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), sf);
            }
        }
    }

    // ── Event args ───────────────────────────────────────────────────────────
    public class PageChangedEventArgs : EventArgs
    {
        public int NewPage { get; }
        public PageChangedEventArgs(int page) { NewPage = page; }
    }
}