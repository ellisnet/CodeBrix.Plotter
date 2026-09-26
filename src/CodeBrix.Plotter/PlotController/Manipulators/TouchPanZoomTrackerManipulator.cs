namespace CodeBrix.Plotter;

/// <summary>
/// Provides the default touch manipulator: a one-finger drag pans, a two-finger pinch zooms around the centre of
/// the pinch, and a touch that has not moved further than <see cref="TouchSlop" /> shows the tracker until it is
/// released.
/// </summary>
/// <remarks>
/// The tracker part is a composed <see cref="TouchTrackerManipulator" />: it is started when the touch goes down and
/// completed (which hides the tracker) either when the touch moves beyond <see cref="TouchSlop" />, when a second
/// finger joins, or when the touch is released. Movement within the slop neither pans nor hides the tracker, so a
/// touch-and-hold keeps showing it. When a single finger first leaves the slop, the axes pan from the position where
/// the touch went down, so the plot stays under the finger.
/// </remarks>
public class TouchPanZoomTrackerManipulator : PlotManipulator<PlotterTouchEventArgs>
{
    /// <summary>
    /// The composed tracker manipulator.
    /// </summary>
    private readonly TouchTrackerManipulator tracker;

    /// <summary>
    /// The position where the touch went down.
    /// </summary>
    private ScreenPoint startPosition;

    /// <summary>
    /// Whether the composed tracker manipulator has been started and not yet completed.
    /// </summary>
    private bool isTracking;

    /// <summary>
    /// Whether the touch has become a pan or pinch.
    /// </summary>
    private bool isManipulating;

    /// <summary>
    /// Whether the next single-finger delta pans from <see cref="startPosition" />.
    /// </summary>
    private bool panFromStart;

    /// <summary>
    /// Whether one of the assigned axes can pan.
    /// </summary>
    private bool isPanEnabled;

    /// <summary>
    /// Whether one of the assigned axes can zoom.
    /// </summary>
    private bool isZoomEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="TouchPanZoomTrackerManipulator" /> class.
    /// </summary>
    /// <param name="plotView">The plot view.</param>
    public TouchPanZoomTrackerManipulator(IPlotView plotView)
        : base(plotView)
    {
        this.tracker = new TouchTrackerManipulator(plotView) { Snap = true, PointsOnly = false };
        this.TouchSlop = 10;
    }

    /// <summary>
    /// Gets or sets the distance, in device-independent units, a single touch may move from where it went down before
    /// it becomes a pan. The default is 10.
    /// </summary>
    public double TouchSlop { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tracker snaps to the nearest point. The default is <c>true</c>.
    /// </summary>
    public bool Snap
    {
        get => this.tracker.Snap;
        set => this.tracker.Snap = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the tracker is shown on points only (not interpolating).
    /// The default is <c>false</c>.
    /// </summary>
    public bool PointsOnly
    {
        get => this.tracker.PointsOnly;
        set => this.tracker.PointsOnly = value;
    }

    /// <summary>
    /// Occurs when an input device begins a manipulation on the plot.
    /// </summary>
    /// <param name="e">The <see cref="PlotterTouchEventArgs" /> instance containing the event data.</param>
    public override void Started(PlotterTouchEventArgs e)
    {
        this.AssignAxes(e.Position);
        base.Started(e);

        this.startPosition = e.Position;
        this.isManipulating = false;
        this.panFromStart = true;

        this.isPanEnabled = (this.XAxis != null && this.XAxis.IsPanEnabled)
                            || (this.YAxis != null && this.YAxis.IsPanEnabled);

        this.isZoomEnabled = (this.XAxis != null && this.XAxis.IsZoomEnabled)
                             || (this.YAxis != null && this.YAxis.IsZoomEnabled);

        this.tracker.Started(e);
        this.isTracking = true;

        e.Handled |= this.isPanEnabled || this.isZoomEnabled;
    }

    /// <summary>
    /// Occurs when a touch delta event is handled.
    /// </summary>
    /// <param name="e">The <see cref="PlotterTouchEventArgs" /> instance containing the event data.</param>
    public override void Delta(PlotterTouchEventArgs e)
    {
        base.Delta(e);

        var isMultiTouch = e.CurrentTouches != null && e.CurrentTouches.Length > 1;

        if (!this.isManipulating)
        {
            if (!isMultiTouch && (e.Position - this.startPosition).Length <= this.TouchSlop)
            {
                // Still a tap or a touch-and-hold: keep the tracker, do not pan.
                return;
            }

            this.isManipulating = true;
            this.StopTracking(e);
        }

        if (!this.isPanEnabled && !this.isZoomEnabled)
        {
            return;
        }

        ScreenPoint previousPosition;
        ScreenPoint newPosition;
        var scale = 1.0;

        if (isMultiTouch)
        {
            if (e.PreviousTouches == null || e.PreviousTouches.Length != e.CurrentTouches.Length)
            {
                // A finger went down or up: no translation or scale in this event.
                this.panFromStart = false;
                return;
            }

            previousPosition = Midpoint(e.PreviousTouches[0], e.PreviousTouches[1]);
            newPosition = Midpoint(e.CurrentTouches[0], e.CurrentTouches[1]);
            scale = e.DeltaScale.X;
        }
        else
        {
            newPosition = e.Position;
            previousPosition = this.panFromStart ? this.startPosition : newPosition - e.DeltaTranslation;
        }

        this.panFromStart = false;

        // The data point under the previous pinch centre; after the pan it is under the new pinch centre, and the zoom
        // keeps it there. (Taken before panning: the axis transform is only refreshed when the plot is laid out again.)
        var anchor = this.InverseTransform(previousPosition.X, previousPosition.Y);

        if (this.XAxis != null)
        {
            this.XAxis.Pan(previousPosition, newPosition);
        }

        if (this.YAxis != null)
        {
            this.YAxis.Pan(previousPosition, newPosition);
        }

        if (isMultiTouch && scale > 0 && scale != 1)
        {
            if (this.XAxis != null)
            {
                this.XAxis.ZoomAt(scale, anchor.X);
            }

            if (this.YAxis != null)
            {
                this.YAxis.ZoomAt(scale, anchor.Y);
            }
        }

        this.PlotView.InvalidatePlot(false);
        e.Handled = true;
    }

    /// <summary>
    /// Occurs when a manipulation is complete.
    /// </summary>
    /// <param name="e">The <see cref="PlotterTouchEventArgs" /> instance containing the event data.</param>
    public override void Completed(PlotterTouchEventArgs e)
    {
        base.Completed(e);
        this.StopTracking(e);

        e.Handled |= this.isPanEnabled || this.isZoomEnabled;
    }

    /// <summary>
    /// Returns the point halfway between two points.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>The midpoint.</returns>
    private static ScreenPoint Midpoint(ScreenPoint a, ScreenPoint b)
    {
        return new ScreenPoint((a.X + b.X) / 2, (a.Y + b.Y) / 2);
    }

    /// <summary>
    /// Completes the composed tracker manipulator (hiding the tracker) if it is still running.
    /// </summary>
    /// <param name="e">The <see cref="PlotterTouchEventArgs" /> instance containing the event data.</param>
    private void StopTracking(PlotterTouchEventArgs e)
    {
        if (!this.isTracking)
        {
            return;
        }

        this.isTracking = false;
        this.tracker.Completed(e);
    }
}
