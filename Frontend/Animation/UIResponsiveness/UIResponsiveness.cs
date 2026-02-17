using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Animation.Sidebar_Animation
{
    public class UIResponsiveness
    {
        private readonly Panel _sidebarPanel;
        private readonly List<Panel> _contentPanels = new List<Panel>();
        private readonly List<Control> _controlsToCenter = new List<Control>();
        private readonly Timer _responsiveTimer;
        private int _lastSidebarWidth;
        private readonly int _stepSize;
        private readonly Dictionary<Control, int> _controlWidths = new Dictionary<Control, int>();

        public UIResponsiveness(Panel sidebarPanel, int stepSize = 10)
        {
            _sidebarPanel = sidebarPanel ?? throw new ArgumentNullException(nameof(sidebarPanel));
            _lastSidebarWidth = _sidebarPanel.Width;
            _stepSize = stepSize;

            _responsiveTimer = new Timer();
            _responsiveTimer.Interval = 50;
            _responsiveTimer.Tick += ResponsiveTimer_Tick;
            _responsiveTimer.Start();
        }

        public void RegisterContentPanel(Panel contentPanel)
        {
            if (!_contentPanels.Contains(contentPanel))
                _contentPanels.Add(contentPanel);
        }

        public void RegisterContentPanels(params Panel[] panels)
        {
            foreach (var panel in panels)
                RegisterContentPanel(panel);
        }

        // Add a single control to be centered
        public void AddControlToCenter(Control control)
        {
            if (!_controlsToCenter.Contains(control))
            {
                _controlsToCenter.Add(control);
                _controlWidths[control] = control.Width;
            }
        }

        // Add multiple controls to be centered
        public void AddControlsToCenter(params Control[] controls)
        {
            foreach (var control in controls)
                AddControlToCenter(control);
        }

        // Remove a control from centering
        public void RemoveControlFromCenter(Control control)
        {
            _controlsToCenter.Remove(control);
            _controlWidths.Remove(control);
        }

        private void ResponsiveTimer_Tick(object sender, EventArgs e)
        {
            if (_sidebarPanel.Width != _lastSidebarWidth)
            {
                _lastSidebarWidth = _sidebarPanel.Width;
                AdjustComponentPosition();
            }
        }

        private void AdjustComponentPosition()
        {
            int sidebarWidth = _sidebarPanel.Width;
            int mainFormWidth = _sidebarPanel.Parent.Width;
            int availableWidth = mainFormWidth - sidebarWidth;

            foreach (var control in _controlsToCenter)
            {
                if (control != null && _controlWidths.ContainsKey(control))
                {
                    // Calculate centered position based on available width
                    int centeredX = sidebarWidth + (availableWidth - _controlWidths[control]) / 2;
                    int targetPos = centeredX;

                    // Smooth animation toward target position
                    if (control.Left < targetPos)
                    {
                        control.Left += _stepSize;
                        if (control.Left > targetPos)
                            control.Left = targetPos;
                    }
                    else if (control.Left > targetPos)
                    {
                        control.Left -= _stepSize;
                        if (control.Left < targetPos)
                            control.Left = targetPos;
                    }
                }
            }
        }

        public void Stop()
        {
            _responsiveTimer.Stop();
        }
    }
}
