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
            foreach (Control ctrl in controls)
            {
                Rectangle original = originalBounds[ctrl];

                // Resize position & size
                ctrl.Left = (int)(original.Left * widthRatio);
                ctrl.Top = (int)(original.Top * heightRatio);
                ctrl.Width = (int)(original.Width * widthRatio);
                ctrl.Height = (int)(original.Height * heightRatio);

                // Resize font
                float originalFontSize = originalFontSizes[ctrl];
                ctrl.Font = new Font(ctrl.Font.FontFamily, originalFontSize * Math.Min(widthRatio, heightRatio), ctrl.Font.Style);

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
