using System;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem
{
    public class SlidePanelController
    {
        private readonly Panel _panel;
        private readonly Button _btnOpen;   // SliderBtn: shown when expanded
        private readonly Button _btnClose;  // Slider1Btn: shown when collapsed
        private readonly Timer _timer;

        private bool _isExpanded;
        private readonly int _expandedWidth;
        private readonly int _slideSpeed;

        // For button mini-slide
        private int _btnExpandedX;   // button X when panel open
        private int _btnCollapsedX;  // button X when panel closed (only ~40px visible)
        private Button _activeButton; // which button is sliding right now

        public SlidePanelController(
            Panel panel,
            Button btnOpen,
            Button btnClose,
            int expandedWidth,
            int slideSpeed = 10)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));
            _btnOpen = btnOpen ?? throw new ArgumentNullException(nameof(btnOpen));
            _btnClose = btnClose ?? throw new ArgumentNullException(nameof(btnClose));
            _expandedWidth = expandedWidth;
            _slideSpeed = slideSpeed;

            _isExpanded = true;
            _panel.Width = _expandedWidth;

            // buttons start at same place
            _btnExpandedX = _btnOpen.Left;           // e.g. right edge of panel
            _btnCollapsedX = _btnExpandedX - (_expandedWidth - 10); // leaves ~40px

            _btnOpen.Visible = true;
            _btnClose.Visible = false;

            _btnOpen.Text = "<";
            _btnClose.Text = ">";

            _timer = new Timer();
            _timer.Interval = 10;
            _timer.Tick += Timer_Tick;

            _btnOpen.Click += ToggleButton_Click;
            _btnClose.Click += ToggleButton_Click;
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            if (_timer.Enabled) return; // ignore while animating

            // choose which button will slide
            _activeButton = (Button)sender;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isExpanded)
            {
                // collapse: shrink panel width, slide current button left
                _panel.Width -= _slideSpeed;
                _activeButton.Left -= _slideSpeed;

                if (_panel.Width <= 0)
                {
                    _panel.Width = 0;
                    _isExpanded = false;
                    _timer.Stop();

                    // after collapse finishes: hide open-btn, show close-btn at collapsed position
                    _btnOpen.Visible = false;
                    _btnClose.Visible = true;
                    _btnClose.Left = _btnCollapsedX;
                    _activeButton = null;
                }
            }
            else
            {
                // expand: grow panel width, slide current button right
                _panel.Width += _slideSpeed;
                _activeButton.Left += _slideSpeed;

                if (_panel.Width >= _expandedWidth)
                {
                    _panel.Width = _expandedWidth;
                    _isExpanded = true;
                    _timer.Stop();

                    // after expand finishes: hide close-btn, show open-btn at expanded position
                    _btnClose.Visible = false;
                    _btnOpen.Visible = true;
                    _btnOpen.Left = _btnExpandedX;
                    _activeButton = null;
                }
            }
        }
    }
}
