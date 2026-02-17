using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.Sidebar_Animation
{
    public class ButtonHoverEffect
    {
        private readonly Color _normalBackColor;
        private readonly Color _hoverBackColor;
        private readonly Color _normalForeColor;
        private readonly Color _hoverForeColor;
        // Constructor
        public ButtonHoverEffect(
            Color normalBackColor,
            Color hoverBackColor,
            Color normalForeColor,
            Color hoverForeColor)
        {
            _normalBackColor = normalBackColor;
            _hoverBackColor = hoverBackColor;
            _normalForeColor = normalForeColor;
            _hoverForeColor = hoverForeColor;
        }

        // Call this for each button you want to have the hover effect
        public void Attach(Button button)
        {
            // Store original colors in Tag so each button can remember its own if needed
            button.Tag = new ButtonOriginalColors
            {
                BackColor = button.BackColor,
                ForeColor = button.ForeColor
            };

            button.MouseEnter += OnMouseEnter;
            button.MouseLeave += OnMouseLeave;
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = _hoverBackColor;
                btn.ForeColor = _hoverForeColor;
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                // Restore from Tag if you want per-button original colors
                if (btn.Tag is ButtonOriginalColors original)
                {
                    btn.BackColor = original.BackColor;
                    btn.ForeColor = original.ForeColor;
                }
                else
                {
                    btn.BackColor = _normalBackColor;
                    btn.ForeColor = _normalForeColor;
                }
            }
        }

        private class ButtonOriginalColors
        {
            public Color BackColor { get; set; }
            public Color ForeColor { get; set; }
        }
    }
}