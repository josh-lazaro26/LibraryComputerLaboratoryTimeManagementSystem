using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.UIResponsiveness
{
    public class UIResizer
    {
        private Form targetForm;
        private float baseFormWidth;
        private float baseFormHeight;

        private Dictionary<Control, Rectangle> originalBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalFontSizes = new Dictionary<Control, float>();

        // Keep track of labels you DON'T want centered
        private HashSet<Control> excludeCentering = new HashSet<Control>();
        public UIResizer(Form form)
        {
            targetForm = form;
            baseFormWidth = form.Width;
            baseFormHeight = form.Height;

            SaveControlSizes(form.Controls);

            targetForm.Resize += TargetForm_Resize;
        }
        public void ExcludeFromCentering(params Control[] ctrls)
        {
            foreach (var ctrl in ctrls)
            {
                if (ctrl != null)
                    excludeCentering.Add(ctrl);
            }
        }

        private void SaveControlSizes(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                originalBounds[ctrl] = ctrl.Bounds;
                originalFontSizes[ctrl] = ctrl.Font.Size;

                if (ctrl.HasChildren)
                {
                    SaveControlSizes(ctrl.Controls);
                }
            }
        }

        private void TargetForm_Resize(object sender, EventArgs e)
        {
            float widthRatio = targetForm.Width / baseFormWidth;
            float heightRatio = targetForm.Height / baseFormHeight;

            ResizeControls(targetForm.Controls, widthRatio, heightRatio);
            CenterLabels(targetForm.Controls); // Recenter labels after resizing
        }

        private void ResizeControls(Control.ControlCollection controls, float widthRatio, float heightRatio)
        {
            float scale = Math.Min(widthRatio, heightRatio);

            foreach (Control ctrl in controls)
            {
                Rectangle original = originalBounds[ctrl];

                // ===== SAFE POSITION (NO SCATTERING) =====
                ctrl.Left = (int)(original.Left * widthRatio);
                ctrl.Top = (int)(original.Top * heightRatio);

                int newWidth = (int)(original.Width * widthRatio);
                int newHeight = (int)(original.Height * heightRatio);

                // ===== PREVENT OVERFLOW INSIDE PANELS =====
                if (ctrl.Parent != null)
                {
                    int maxWidth = ctrl.Parent.ClientSize.Width - ctrl.Left - 5; // margin safety
                    if (newWidth > maxWidth)
                        newWidth = maxWidth;
                }

                // ===== CONTROL-SPECIFIC RESIZING =====
                if (ctrl is TextBox)
                {
                    // Keep textboxes clean and inside panel
                    ctrl.Width = newWidth;
                    ctrl.Height = original.Height; // Don't stretch vertically (prevents ugly UI)
                }
                else if (ctrl is Label)
                {
                    ctrl.Width = newWidth;
                    ctrl.Height = original.Height; // Labels should not stretch vertically
                }
                else if (ctrl is Button)
                {
                    ctrl.Width = newWidth;
                    ctrl.Height = (int)(original.Height * heightRatio * 1.05f);
                }
                else if (ctrl is Panel)
                {
                    // Panels scale normally (they are containers)
                    ctrl.Width = newWidth;
                    ctrl.Height = newHeight;
                }
                else
                {
                    ctrl.Width = newWidth;
                    ctrl.Height = newHeight;
                }

                // ===== SAFE FONT SCALING (CONSISTENT LOGIN LOOK) =====
                float originalFontSize = originalFontSizes[ctrl];

                if (ctrl is Label)
                {
                    ctrl.Font = new Font(ctrl.Font.FontFamily, originalFontSize * scale * 1.15f, ctrl.Font.Style);
                }
                else if (ctrl is Button)
                {
                    ctrl.Font = new Font(ctrl.Font.FontFamily, originalFontSize * scale * 1.1f, ctrl.Font.Style);
                }
                else if (ctrl is TextBox)
                {
                    ctrl.Font = new Font(ctrl.Font.FontFamily, originalFontSize * scale * 1.05f, ctrl.Font.Style);
                }
                else
                {
                    ctrl.Font = new Font(ctrl.Font.FontFamily, originalFontSize * scale, ctrl.Font.Style);
                }

                // ===== RECURSIVE =====
                if (ctrl.HasChildren)
                {
                    ResizeControls(ctrl.Controls, widthRatio, heightRatio);
                }
            }
        }

        private void CenterLabels(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is Label && !excludeCentering.Contains(ctrl))
                {
                    if (ctrl.Parent != null)
                    {
                        ctrl.Left = (ctrl.Parent.ClientSize.Width - ctrl.Width) / 2;
                    }
                }

                if (ctrl.HasChildren)
                {
                    CenterLabels(ctrl.Controls);
                }
            }
        }

        public void ForceResize()
        {
            float widthRatio = targetForm.Width / baseFormWidth;
            float heightRatio = targetForm.Height / baseFormHeight;

            ResizeControls(targetForm.Controls, widthRatio, heightRatio);
            CenterLabels(targetForm.Controls);
        }

    }
}
