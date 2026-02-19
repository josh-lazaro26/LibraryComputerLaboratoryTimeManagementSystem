using System;
using System.Drawing;
using System.Windows.Forms;

public class LabelSwapAnimator
{
    private readonly Control _label1;
    private readonly Control _label2;
    private readonly Timer _timer;
    private readonly Control _drawingSurface; // the parent panel/form to draw on

    private Bitmap _snap1, _snap2;
    private Point _label1Start, _label2Start;
    private Point _label1Target, _label2Target;
    private int _step;
    private int _totalSteps;
    private bool _label1IsActive;
    private PaintEventHandler _paintHandler;

    private Point _current1, _current2;

    public int DurationSteps { get => _totalSteps; set => _totalSteps = Math.Max(1, value); }
    public int Interval { get => _timer.Interval; set => _timer.Interval = value; }

    // label1 and label2 must share a common parent for drawing to work cleanly
    // Pass that common parent as drawingSurface
    public LabelSwapAnimator(Control label1, Control label2, Control drawingSurface, int steps = 20, int interval = 15)
    {
        _label1 = label1;
        _label2 = label2;
        _drawingSurface = drawingSurface;
        _totalSteps = steps;
        _timer = new Timer { Interval = interval };
        _timer.Tick += OnTick;
    }

    public void Swap(Point label1NewLocation, Point label2NewLocation, bool label1IsActive)
    {
        if (_timer.Enabled) return;

        _label1IsActive = label1IsActive;
        _label1Start = _current1 = _label1.Location;
        _label2Start = _current2 = _label2.Location;
        _label1Target = label1NewLocation;
        _label2Target = label2NewLocation;

        // Take snapshots of each label as they currently look
        _snap1 = SnapshotControl(_label1);
        _snap2 = SnapshotControl(_label2);

        // Hide the real controls during animation
        _label1.Visible = false;
        _label2.Visible = false;

        _step = 0;

        // Hook into the drawing surface paint event
        _paintHandler = (s, e) => DrawFrame(e.Graphics);
        _drawingSurface.Paint += _paintHandler;
        _drawingSurface.Invalidate();

        _timer.Start();
    }

    private void OnTick(object sender, EventArgs e)
    {
        _step++;
        float t = EaseInOut((float)_step / _totalSteps);

        _current1 = Lerp(_label1Start, _label1Target, t);
        _current2 = Lerp(_label2Start, _label2Target, t);

        _drawingSurface.Invalidate();

        if (_step >= _totalSteps)
        {
            _timer.Stop();
            Finish();
        }
    }

    private void DrawFrame(Graphics g)
    {
        if (_snap1 == null || _snap2 == null) return;
        g.DrawImage(_snap2, _current2.X, _current2.Y);
        g.DrawImage(_snap1, _current1.X, _current1.Y);
    }

    private void Finish()
    {
        _drawingSurface.Paint -= _paintHandler;
        _paintHandler = null;

        _label1.Location = _label1Target;
        _label2.Location = _label2Target;

        _label1.BackColor = _label1IsActive ? SystemColors.Control : SystemColors.Window;
        _label2.BackColor = _label1IsActive ? SystemColors.Window : SystemColors.Control;

        _label1.Visible = true;
        _label2.Visible = true;

        _snap1?.Dispose(); _snap1 = null;
        _snap2?.Dispose(); _snap2 = null;

        _drawingSurface.Invalidate();
    }

    private Bitmap SnapshotControl(Control c)
    {
        var bmp = new Bitmap(c.Width, c.Height);
        c.DrawToBitmap(bmp, new Rectangle(0, 0, c.Width, c.Height));
        return bmp;
    }

    private static Point Lerp(Point a, Point b, float t)
        => new Point((int)(a.X + (b.X - a.X) * t), (int)(a.Y + (b.Y - a.Y) * t));

    private static float EaseInOut(float t)
        => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
}
