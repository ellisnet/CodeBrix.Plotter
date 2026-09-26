using System.Collections.Generic;
using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Series;

namespace CodeBrix.Plotter.Tests;

/// <summary>
/// An <see cref="IPlotView" /> for input tests: it records the tracker calls and drives touch sequences through its
/// controller the way a host view does (the first touch starts the gesture, every move carries the before and after
/// positions of all touches, the last touch up completes it).
/// </summary>
public class RecordingPlotView : IPlotView
{
    /// <summary>
    /// The current touch positions, in the order the touches went down.
    /// </summary>
    private readonly List<ScreenPoint> touches = new List<ScreenPoint>();

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingPlotView" /> class, showing a laid-out model with a
    /// bottom and a left linear axis (both 0 to 100, fixed plot margins) and a line series along the diagonal.
    /// </summary>
    /// <param name="controller">The controller that receives the input.</param>
    public RecordingPlotView(IController controller)
    {
        // Fixed plot margins: with automatic margins the plot area moves when the tick labels change width after a
        // pan or zoom, which would shift the screen-to-data mapping independently of the manipulator under test.
        var model = new PlotModel { PlotMargins = new PlotterThickness(50, 10, 10, 40) };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 100 });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100 });
        var series = new LineSeries();
        for (var i = 0; i <= 100; i += 10)
        {
            series.Points.Add(new DataPoint(i, i));
        }

        model.Series.Add(series);

        this.ActualModel = model;
        this.ActualController = controller;
        this.ClientArea = new PlotterRect(0, 0, 400, 300);
        this.Layout(true);
    }

    /// <inheritdoc />
    public PlotModel ActualModel { get; set; }

    /// <remarks>
    /// <see cref="IPlotView" /> narrows this to <see cref="PlotModel" />, so the
    /// wider <see cref="IView" /> declaration is implemented explicitly.
    /// </remarks>
    Model IView.ActualModel => this.ActualModel;

    /// <inheritdoc />
    public IController ActualController { get; set; }

    /// <inheritdoc />
    public PlotterRect ClientArea { get; set; }

    /// <summary>
    /// Gets a value indicating whether the tracker is currently shown.
    /// </summary>
    public bool IsTrackerVisible { get; private set; }

    /// <summary>
    /// Gets how many times the tracker was shown.
    /// </summary>
    public int ShowTrackerCount { get; private set; }

    /// <summary>
    /// Gets the horizontal axis of the model.
    /// </summary>
    public Axis XAxis => this.ActualModel.Axes[0];

    /// <summary>
    /// Gets the vertical axis of the model.
    /// </summary>
    public Axis YAxis => this.ActualModel.Axes[1];

    /// <summary>
    /// Returns the screen position of a data point.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    /// <returns>The screen position.</returns>
    public ScreenPoint ScreenOf(double x, double y) => this.XAxis.Transform(x, y, this.YAxis);

    /// <summary>
    /// Returns the data point at a screen position.
    /// </summary>
    /// <param name="point">The screen position.</param>
    /// <returns>The data point.</returns>
    public DataPoint DataAt(ScreenPoint point) => this.XAxis.InverseTransform(point.X, point.Y, this.YAxis);

    /// <summary>
    /// Puts a touch down. The first touch starts the gesture.
    /// </summary>
    /// <param name="position">The touch position.</param>
    public void TouchDown(ScreenPoint position)
    {
        this.touches.Add(position);
        if (this.touches.Count == 1)
        {
            this.ActualController.HandleTouchStarted(this, new PlotterTouchEventArgs
            {
                Position = position,
                DeltaTranslation = new ScreenVector(0, 0),
                DeltaScale = new ScreenVector(1, 1),
            });
        }
        else
        {
            var current = this.touches.ToArray();
            var previous = current[..^1];
            this.ActualController.HandleTouchDelta(this, new PlotterTouchEventArgs(current, previous));
        }
    }

    /// <summary>
    /// Moves a touch.
    /// </summary>
    /// <param name="index">The touch, in the order the touches went down.</param>
    /// <param name="position">The new position.</param>
    public void TouchMove(int index, ScreenPoint position)
    {
        var previous = this.touches.ToArray();
        this.touches[index] = position;
        this.ActualController.HandleTouchDelta(this, new PlotterTouchEventArgs(this.touches.ToArray(), previous));
    }

    /// <summary>
    /// Lifts every touch, completing the gesture.
    /// </summary>
    public void TouchUpAll()
    {
        var position = this.touches[0];
        this.touches.Clear();
        this.ActualController.HandleTouchCompleted(this, new PlotterTouchEventArgs { Position = position });
    }

    /// <inheritdoc />
    public void HideTracker()
    {
        this.IsTrackerVisible = false;
    }

    /// <inheritdoc />
    public void HideZoomRectangle()
    {
    }

    /// <summary>
    /// Lays the model out again, as a host view does when it re-renders, so the axis transforms follow a pan or zoom.
    /// </summary>
    /// <param name="updateData">Whether the model data should be updated.</param>
    public void InvalidatePlot(bool updateData = true)
    {
        this.Layout(updateData);
    }

    /// <summary>
    /// Updates and renders the model into <see cref="ClientArea" />.
    /// </summary>
    /// <param name="updateData">Whether the model data should be updated.</param>
    private void Layout(bool updateData)
    {
        ((IPlotModel)this.ActualModel).Update(updateData);
        ((IPlotModel)this.ActualModel).Render(new NullRenderContext(), this.ClientArea);
    }

    /// <inheritdoc />
    public void SetClipboardText(string text)
    {
    }

    /// <inheritdoc />
    public void SetCursorType(CursorType cursorType)
    {
    }

    /// <inheritdoc />
    public void ShowTracker(TrackerHitResult trackerHitResult)
    {
        this.IsTrackerVisible = true;
        this.ShowTrackerCount++;
    }

    /// <inheritdoc />
    public void ShowZoomRectangle(PlotterRect rectangle)
    {
    }
}
