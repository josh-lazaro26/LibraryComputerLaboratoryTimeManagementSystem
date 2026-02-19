using System;
using System.Windows.Forms;

public class SidebarAnimator
{
    private readonly Timer _timer;
    private readonly Panel _panel;

    private int _expandedWidth;
    private int _collapsedWidth;
    private int _step;

    private bool _isExpanded;
    private bool _isAnimating;

    public bool IsExpanded => _isExpanded;

    public SidebarAnimator(Panel panel, int expandedWidth = 300, int collapsedWidth = 55, int step = 8, int interval = 5)
    {
        _panel = panel;
        _expandedWidth = expandedWidth;
        _collapsedWidth = collapsedWidth;
        _step = step;

        _timer = new Timer();
        _timer.Interval = interval; // Lower = smoother
        _timer.Tick += Animate;

        // Enable smoother rendering (reduces flicker)
        typeof(Panel).InvokeMember("DoubleBuffered",
            System.Reflection.BindingFlags.SetProperty |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic,
            null, _panel, new object[] { true });

        _isExpanded = _panel.Width >= _expandedWidth;
    }

    public void Toggle()
    {
        if (_isAnimating) return; // Prevent spam clicks
        _isAnimating = true;
        _timer.Start();
    }

    private void Animate(object sender, EventArgs e)
    {
        if (_isExpanded)
        {
            // Collapse
            if (_panel.Width > _collapsedWidth)
            {
                _panel.Width -= _step;
            }
            else
            {
                _timer.Stop();
                _panel.Width = _collapsedWidth;
                _isExpanded = false;
                _isAnimating = false;
            }
        }
        else
        {
            // Expand
            if (_panel.Width < _expandedWidth)
            {
                _panel.Width += _step;
            }
            else
            {
                _timer.Stop();
                _panel.Width = _expandedWidth;
                _isExpanded = true;
                _isAnimating = false;
            }
        }
    }
}
