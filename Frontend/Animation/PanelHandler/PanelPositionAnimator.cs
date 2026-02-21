using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class PanelPositionAnimator
{
    private readonly Timer _timer;
    private readonly int _speed;

    private bool _isCollapsed;
    private bool _isAnimating;

    private class PanelTransition
    {
        public Control Panel;
        public Point ExpandedPosition;
        public Point CollapsedPosition;
    }

    private readonly List<PanelTransition> _panels = new List<PanelTransition>();

    public bool IsCollapsed => _isCollapsed;

    public PanelPositionAnimator(int interval = 5, int speed = 8)
    {
        _speed = speed;
        _timer = new Timer();
        _timer.Interval = interval;
        _timer.Tick += Animate;
    }

    public void AddPanel(Control panel, Point expandedPosition, Point collapsedPosition)
    {
        _panels.Add(new PanelTransition
        {
            Panel = panel,
            ExpandedPosition = expandedPosition,
            CollapsedPosition = collapsedPosition
        });
    }

    public void Toggle()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        _timer.Start();
    }

    private void Animate(object sender, EventArgs e)
    {
        bool allReached = true;

        foreach (var item in _panels)
        {
            Point target = _isCollapsed ? item.ExpandedPosition : item.CollapsedPosition;
            Point current = item.Panel.Location;

            int newX = MoveTowards(current.X, target.X, _speed);
            int newY = MoveTowards(current.Y, target.Y, _speed);

            item.Panel.Location = new Point(newX, newY);

            if (newX != target.X || newY != target.Y)
                allReached = false;
        }

        if (allReached)
        {
            _timer.Stop();
            _isCollapsed = !_isCollapsed;
            _isAnimating = false;
        }
    }
    public void Clear()
    {
        _panels.Clear();
    }

    private int MoveTowards(int current, int target, int step)
    {
        if (current < target)
            return Math.Min(current + step, target);
        if (current > target)
            return Math.Max(current - step, target);
        return current;
    }
    public void SnapToState(bool collapsed)
    {
        foreach (var item in _panels)
        {
            item.Panel.Location = collapsed
                ? item.CollapsedPosition
                : item.ExpandedPosition;
        }
        _isCollapsed = collapsed;
    }
}
