using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    /// <summary>
    /// Reusable minimalist progress form matching the forest-green app theme.
    /// Compatible with C# 7.3 / .NET Framework.
    /// Inherit and call SetTitle(), SetStepMarkers(), SetProgress() to drive the UI.
    /// </summary>
    public class ModernProgressForm : Form
    {
        // ── State ─────────────────────────────────────────────────────────
        private int _progressValue = 0;
        private string _statusMessage = "Initializing...";
        private string _stepLabel = "";
        private string _title = "Processing...";
        private int[] _stepMarkers = { 25, 50, 75, 100 };

        // ── Animation ─────────────────────────────────────────────────────
        private Timer _shimmerTimer;
        private float _shimmerOffset = 0f;

        // ── Colors — forest green theme ───────────────────────────────────
        private readonly Color _bgColor = Color.FromArgb(6, 64, 43);       // main dark green
        private readonly Color _cardColor = Color.FromArgb(8, 80, 54);       // slightly lighter card
        private readonly Color _trackColor = Color.FromArgb(4, 45, 30);       // dark sunken track
        private readonly Color _fillColor = Color.FromArgb(255, 255, 255);   // white fill
        private readonly Color _fillDim = Color.FromArgb(180, 220, 200);   // soft white-green start
        private readonly Color _textPrimary = Color.FromArgb(255, 255, 255);   // white
        private readonly Color _textSecondary = Color.FromArgb(160, 210, 180);   // muted green-white
        private readonly Color _successColor = Color.FromArgb(100, 255, 180);   // bright mint
        private readonly Color _errorColor = Color.FromArgb(255, 100, 100);   // soft red

        // ── Layout ────────────────────────────────────────────────────────
        private const int Padding = 36;
        private const int BarHeight = 8;
        private const int BarRadius = 4;
        private const int Radius = 16;
        private Rectangle _barTrackRect;

        // ── Constructor ───────────────────────────────────────────────────
        public void SetProgress(string percentageString, string message, string stepLabel = "")
        {
            if (!double.TryParse(percentageString.TrimEnd('%'), out double percent)) return;
            SetProgress((int)percent, message, stepLabel);
        }
        public ModernProgressForm()
        {
            this.Text = "";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(460, 220);
            this.BackColor = _bgColor;
            this.DoubleBuffered = true;

            ApplyRoundedCorners();
            ApplyFormRegion();
            RecalculateLayout();
            StartShimmer();
        }

        // ── Public API ────────────────────────────────────────────────────

        public void SetTitle(string title)
        {
            _title = title;
            SafeInvalidate();
        }

        public void SetProgress(int value, string message, string stepLabel = "")
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetProgress(value, message, stepLabel)));
                return;
            }
            _progressValue = value < 0 ? 0 : value > 100 ? 100 : value;
            _statusMessage = message;
            _stepLabel = stepLabel;
            this.Invalidate();
        }

        public void SetStepMarkers(params int[] markerPercentages)
        {
            _stepMarkers = markerPercentages;
            SafeInvalidate();
        }

        // ── Layout ────────────────────────────────────────────────────────

        private void RecalculateLayout()
        {
            // Bar sits in the lower half of the form
            int barY = this.Height - Padding - BarHeight - 38;
            _barTrackRect = new Rectangle(Padding, barY, this.Width - Padding * 2, BarHeight);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyFormRegion();
            RecalculateLayout();
        }

        // ── Rounded form region (clips the window shape) ──────────────────

        private void ApplyFormRegion()
        {
            using (GraphicsPath path = RoundedRectPath(new Rectangle(0, 0, this.Width, this.Height), Radius))
            {
                this.Region = new Region(path);
            }
        }

        // ── Animation ─────────────────────────────────────────────────────

        private void StartShimmer()
        {
            _shimmerTimer = new Timer();
            _shimmerTimer.Interval = 16;
            _shimmerTimer.Tick += (s, e) =>
            {
                _shimmerOffset = (_shimmerOffset + 2.5f) % (_barTrackRect.Width + 80);
                this.Invalidate();
            };
            _shimmerTimer.Start();
        }

        protected void StopAnimation()
        {
            if (_shimmerTimer != null)
            {
                _shimmerTimer.Stop();
                _shimmerTimer.Dispose();
            }
        }

        // ── Paint ─────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawBackground(g);
            DrawTitle(g);
            DrawStepLabel(g);
            DrawTrack(g);
            DrawFill(g);
            DrawStatusMessage(g);
        }

        private void DrawBackground(Graphics g)
        {
            // Solid rounded background — no card border, no glow, clean
            using (SolidBrush bg = new SolidBrush(_bgColor))
            {
                using (GraphicsPath path = RoundedRectPath(new Rectangle(0, 0, this.Width, this.Height), Radius))
                {
                    g.FillPath(bg, path);
                }
            }
        }

        private void DrawTitle(Graphics g)
        {
            // Title top-left
            using (Font titleFont = new Font("Segoe UI", 14f, FontStyle.Bold))
            using (SolidBrush titleBrush = new SolidBrush(_textPrimary))
            {
                g.DrawString(_title, titleFont, titleBrush, new PointF(Padding, Padding));
            }

            // Percentage — right-aligned on same row
            bool isError = IsError();
            Color pctColor = isError ? _errorColor
                           : _progressValue == 100 ? _successColor
                           : _textPrimary;

            string pctText = _progressValue + "%";
            using (Font pctFont = new Font("Segoe UI", 14f, FontStyle.Bold))
            using (SolidBrush pctBrush = new SolidBrush(pctColor))
            {
                SizeF pctSize = g.MeasureString(pctText, pctFont);
                g.DrawString(pctText, pctFont, pctBrush,
                    new PointF(this.Width - Padding - pctSize.Width, Padding));
            }
        }

        private void DrawStepLabel(Graphics g)
        {
            // Step label below the title, muted
            string label = string.IsNullOrEmpty(_stepLabel) ? "" : _stepLabel.ToUpperInvariant();
            using (Font stepFont = new Font("Segoe UI", 8f, FontStyle.Regular))
            using (SolidBrush stepBrush = new SolidBrush(_textSecondary))
            {
                g.DrawString(label, stepFont, stepBrush, new PointF(Padding, Padding + 28));
            }
        }

        private void DrawTrack(Graphics g)
        {
            using (SolidBrush trackBrush = new SolidBrush(_trackColor))
            {
                FillRoundRect(g, trackBrush, _barTrackRect, BarRadius);
            }
        }

        private void DrawFill(Graphics g)
        {
            if (_progressValue <= 0) return;

            int fillWidth = (int)(_barTrackRect.Width * (_progressValue / 100f));
            if (fillWidth < BarRadius * 2) fillWidth = BarRadius * 2;

            Rectangle fillRect = new Rectangle(_barTrackRect.Left, _barTrackRect.Top, fillWidth, BarHeight);

            bool isError = IsError();

            Color fillStart = isError ? Color.FromArgb(180, 60, 60)
                            : _progressValue == 100 ? Color.FromArgb(60, 200, 130)
                            : _fillDim;

            Color fillEnd = isError ? _errorColor
                            : _progressValue == 100 ? _successColor
                            : _fillColor;

            using (LinearGradientBrush fillBrush = new LinearGradientBrush(
                fillRect, fillStart, fillEnd, LinearGradientMode.Horizontal))
            {
                FillRoundRect(g, fillBrush, fillRect, BarRadius);
            }

            // Shimmer (only while running, not on error)
            if (_progressValue > 0 && _progressValue < 100 && !isError)
            {
                int shimX = _barTrackRect.Left + (int)_shimmerOffset - 40;
                g.SetClip(new Region(fillRect), CombineMode.Replace);

                using (LinearGradientBrush shimBrush = new LinearGradientBrush(
                    new Rectangle(shimX, _barTrackRect.Top, 60, BarHeight),
                    Color.Transparent, Color.FromArgb(50, 255, 255, 255),
                    LinearGradientMode.Horizontal))
                {
                    ColorBlend cb = new ColorBlend(3);
                    cb.Colors = new Color[] { Color.Transparent, Color.FromArgb(50, 255, 255, 255), Color.Transparent };
                    cb.Positions = new float[] { 0f, 0.5f, 1f };
                    shimBrush.InterpolationColors = cb;
                    g.FillRectangle(shimBrush, new Rectangle(shimX, _barTrackRect.Top - 1, 60, BarHeight + 2));
                }

                g.ResetClip();
            }
        }

        private void DrawStatusMessage(Graphics g)
        {
            bool isError = IsError();
            Color msgColor = isError ? _errorColor
                           : _progressValue == 100 ? _successColor
                           : _textSecondary;

            using (Font msgFont = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (SolidBrush msgBrush = new SolidBrush(msgColor))
            {
                g.DrawString(_statusMessage, msgFont, msgBrush,
                    new PointF(Padding, _barTrackRect.Bottom + 10));
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private bool IsError()
        {
            return _statusMessage.IndexOf("fail", StringComparison.OrdinalIgnoreCase) >= 0
                || _statusMessage.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void FillRoundRect(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = RoundedRectPath(rect, radius))
            {
                g.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void ApplyRoundedCorners()
        {
            try
            {
                const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
                int pref = 2;
                NativeMethods.DwmSetWindowAttribute(this.Handle,
                    DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(int));
            }
            catch { }
        }

        private void SafeInvalidate()
        {
            if (this.InvokeRequired) this.Invoke((Action)this.Invalidate);
            else this.Invalidate();
        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(
            IntPtr hwnd, int attr, ref int attrValue, int attrSize);
    }
}