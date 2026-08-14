// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotCommands.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Defines common commands for the plots.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Defines common commands for the plots.
/// </summary>
public static class PlotCommands
{
    /// <summary>
    /// Initializes static members of the <see cref="PlotCommands" /> class.
    /// </summary>
    static PlotCommands()
    {
        // commands that can be triggered from key events
        Reset = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandleReset(view, args));
        CopyCode = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandleCopyCode(view, args));

        // commands that can be triggered from mouse down events
        ResetAt = new DelegatePlotCommand<PlotterMouseEventArgs>((view, controller, args) => HandleReset(view, args));
        PanAt = new DelegatePlotCommand<PlotterMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new PanManipulator(view), args));
        ZoomRectangle = new DelegatePlotCommand<PlotterMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new ZoomRectangleManipulator(view), args));
        Track = new DelegatePlotCommand<PlotterMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new TrackerManipulator(view) { Snap = false, PointsOnly = false }, args));
        SnapTrack = new DelegatePlotCommand<PlotterMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new TrackerManipulator(view) { Snap = true, PointsOnly = false }, args));
        PointsOnlyTrack = new DelegatePlotCommand<PlotterMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new TrackerManipulator(view) { Snap = false, PointsOnly = true }, args));
        ZoomWheel = new DelegatePlotCommand<PlotterMouseWheelEventArgs>((view, controller, args) => HandleZoomByWheel(view, args));
        ZoomWheelFine = new DelegatePlotCommand<PlotterMouseWheelEventArgs>((view, controller, args) => HandleZoomByWheel(view, args, 0.1));
        ZoomInAt = new DelegatePlotCommand<PlotterMouseEventArgs>((view, controller, args) => HandleZoomAt(view, args, 0.05));
        ZoomOutAt = new DelegatePlotCommand<PlotterMouseEventArgs>((view, controller, args) => HandleZoomAt(view, args, -0.05));

        // commands that can be triggered from mouse enter events
        HoverTrack = new DelegatePlotCommand<PlotterMouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new TrackerManipulator(view) { LockToInitialSeries = false, Snap = false, PointsOnly = false }, args));
        HoverSnapTrack = new DelegatePlotCommand<PlotterMouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new TrackerManipulator(view) { LockToInitialSeries = false, Snap = true, PointsOnly = false }, args));
        HoverPointsOnlyTrack = new DelegatePlotCommand<PlotterMouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new TrackerManipulator(view) { LockToInitialSeries = false, Snap = false, PointsOnly = true }, args));

        // Touch events
        SnapTrackTouch = new DelegatePlotCommand<PlotterTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new TouchTrackerManipulator(view) { Snap = true, PointsOnly = false }, args));
        PointsOnlyTrackTouch = new DelegatePlotCommand<PlotterTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new TouchTrackerManipulator(view) { Snap = true, PointsOnly = true }, args));
        PanZoomByTouch = new DelegatePlotCommand<PlotterTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new TouchManipulator(view), args));

        // commands that can be triggered from key events
        PanLeft = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, -0.1, 0));
        PanRight = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0.1, 0));
        PanUp = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, -0.1));
        PanDown = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, 0.1));
        PanLeftFine = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, -0.01, 0));
        PanRightFine = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0.01, 0));
        PanUpFine = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, -0.01));
        PanDownFine = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, 0.01));

        ZoomIn = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, 1));
        ZoomOut = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, -1));
        ZoomInFine = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, 0.1));
        ZoomOutFine = new DelegatePlotCommand<PlotterKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, -0.1));
    }

    /// <summary>
    /// Gets the reset axes command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> Reset { get; private set; }

    /// <summary>
    /// Gets the reset axes command (for mouse events).
    /// </summary>
    public static IViewCommand<PlotterMouseEventArgs> ResetAt { get; private set; }

    /// <summary>
    /// Gets the copy code command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> CopyCode { get; private set; }

    /// <summary>
    /// Gets the pan/zoom touch command.
    /// </summary>
    public static IViewCommand<PlotterTouchEventArgs> PanZoomByTouch { get; private set; }

    /// <summary>
    /// Gets the pan command.
    /// </summary>
    public static IViewCommand<PlotterMouseDownEventArgs> PanAt { get; private set; }

    /// <summary>
    /// Gets the zoom rectangle command.
    /// </summary>
    public static IViewCommand<PlotterMouseDownEventArgs> ZoomRectangle { get; private set; }

    /// <summary>
    /// Gets the zoom by mouse wheel command.
    /// </summary>
    public static IViewCommand<PlotterMouseWheelEventArgs> ZoomWheel { get; private set; }

    /// <summary>
    /// Gets the fine-control zoom by mouse wheel command.
    /// </summary>
    public static IViewCommand<PlotterMouseWheelEventArgs> ZoomWheelFine { get; private set; }

    /// <summary>
    /// Gets the tracker command.
    /// </summary>
    public static IViewCommand<PlotterMouseDownEventArgs> Track { get; private set; }

    /// <summary>
    /// Gets the snap tracker command.
    /// </summary>
    public static IViewCommand<PlotterMouseDownEventArgs> SnapTrack { get; private set; }

    /// <summary>
    /// Gets the snap tracker command.
    /// </summary>
    public static IViewCommand<PlotterTouchEventArgs> SnapTrackTouch { get; private set; }

    /// <summary>
    /// Gets the points only tracker command.
    /// </summary>
    public static IViewCommand<PlotterMouseDownEventArgs> PointsOnlyTrack { get; private set; }

    /// <summary>
    /// Gets the points only tracker command.
    /// </summary>
    public static IViewCommand<PlotterTouchEventArgs> PointsOnlyTrackTouch { get; private set; }

    /// <summary>
    /// Gets the mouse hover tracker.
    /// </summary>
    public static IViewCommand<PlotterMouseEventArgs> HoverTrack { get; private set; }

    /// <summary>
    /// Gets the mouse hover snap tracker.
    /// </summary>
    public static IViewCommand<PlotterMouseEventArgs> HoverSnapTrack { get; private set; }

    /// <summary>
    /// Gets the mouse hover points only tracker.
    /// </summary>
    public static IViewCommand<PlotterMouseEventArgs> HoverPointsOnlyTrack { get; private set; }

    /// <summary>
    /// Gets the pan left command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanLeft { get; private set; }

    /// <summary>
    /// Gets the pan right command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanRight { get; private set; }

    /// <summary>
    /// Gets the pan up command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanUp { get; private set; }

    /// <summary>
    /// Gets the pan down command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanDown { get; private set; }

    /// <summary>
    /// Gets the fine control pan left command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanLeftFine { get; private set; }

    /// <summary>
    /// Gets the fine control pan right command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanRightFine { get; private set; }

    /// <summary>
    /// Gets the fine control pan up command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanUpFine { get; private set; }

    /// <summary>
    /// Gets the fine control pan down command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> PanDownFine { get; private set; }

    /// <summary>
    /// Gets the zoom in command.
    /// </summary>
    public static IViewCommand<PlotterMouseEventArgs> ZoomInAt { get; private set; }

    /// <summary>
    /// Gets the zoom out command.
    /// </summary>
    public static IViewCommand<PlotterMouseEventArgs> ZoomOutAt { get; private set; }

    /// <summary>
    /// Gets the zoom in command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> ZoomIn { get; private set; }

    /// <summary>
    /// Gets the zoom out command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> ZoomOut { get; private set; }

    /// <summary>
    /// Gets the fine control zoom in command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> ZoomInFine { get; private set; }

    /// <summary>
    /// Gets the fine control zoom out command.
    /// </summary>
    public static IViewCommand<PlotterKeyEventArgs> ZoomOutFine { get; private set; }

    /// <summary>
    /// Handles the reset event.
    /// </summary>
    /// <param name="view">The view to reset.</param>
    /// <param name="args">The <see cref="PlotterInputEventArgs" /> instance containing the event data.</param>
    private static void HandleReset(IPlotView view, PlotterInputEventArgs args)
    {
        args.Handled = true;
        view.ActualModel.ResetAllAxes();
        view.InvalidatePlot(false);
    }

    /// <summary>
    /// Handles the copy code event.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="args">The <see cref="PlotterInputEventArgs"/> instance containing the event data.</param>
    private static void HandleCopyCode(IPlotView view, PlotterInputEventArgs args)
    {
        args.Handled = true;
        var code = view.ActualModel.ToCode();
        view.SetClipboardText(code);
    }

    /// <summary>
    /// Zooms the view by the specified factor at the position specified in the <see cref="PlotterMouseEventArgs" />.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="args">The <see cref="PlotterMouseWheelEventArgs" /> instance containing the event data.</param>
    /// <param name="delta">The zoom factor.</param>
    private static void HandleZoomAt(IPlotView view, PlotterMouseEventArgs args, double delta)
    {
        var m = new ZoomStepManipulator(view) { Step = delta, FineControl = args.IsControlDown };
        m.Started(args);
    }

    /// <summary>
    /// Zooms the view by the mouse wheel delta in the specified <see cref="PlotterKeyEventArgs" />.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="args">The <see cref="PlotterMouseWheelEventArgs" /> instance containing the event data.</param>
    /// <param name="factor">The zoom speed factor. Default value is 1.</param>
    private static void HandleZoomByWheel(IPlotView view, PlotterMouseWheelEventArgs args, double factor = 1)
    {
        var m = new ZoomStepManipulator(view) { Step = args.Delta * 0.001 * factor, FineControl = args.IsControlDown };
        m.Started(args);
    }

    /// <summary>
    /// Zooms the view by the key in the specified factor.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="args">The <see cref="PlotterInputEventArgs" /> instance containing the event data.</param>
    /// <param name="delta">The zoom factor (positive zoom in, negative zoom out).</param>
    private static void HandleZoomCenter(IPlotView view, PlotterInputEventArgs args, double delta)
    {
        args.Handled = true;
        view.ActualModel.ZoomAllAxes(1 + (delta * 0.12));
        view.InvalidatePlot(false);
    }

    /// <summary>
    /// Pans the view by the key in the specified vector.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="args">The <see cref="PlotterInputEventArgs" /> instance containing the event data.</param>
    /// <param name="dx">The horizontal delta (percentage of plot area width).</param>
    /// <param name="dy">The vertical delta (percentage of plot area height).</param>
    private static void HandlePan(IPlotView view, PlotterInputEventArgs args, double dx, double dy)
    {
        args.Handled = true;
        dx *= view.ActualModel.PlotArea.Width;
        dy *= view.ActualModel.PlotArea.Height;
        view.ActualModel.PanAllAxes(dx, dy);
        view.InvalidatePlot(false);
    }
}
