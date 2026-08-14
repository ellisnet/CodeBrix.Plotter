// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotController.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides an <see cref="IPlotController" /> with a default set of plot bindings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides an <see cref="IPlotController" /> with a default set of plot bindings.
/// </summary>
public class PlotController : ControllerBase, IPlotController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotController" /> class.
    /// </summary>
    public PlotController()
    {
        // Zoom rectangle bindings: MMB / control RMB / control+alt LMB
        this.BindMouseDown(PlotterMouseButton.Middle, PlotCommands.ZoomRectangle);
        this.BindMouseDown(PlotterMouseButton.Right, PlotterModifierKeys.Control, PlotCommands.ZoomRectangle);
        this.BindMouseDown(PlotterMouseButton.Left, PlotterModifierKeys.Control | PlotterModifierKeys.Alt, PlotCommands.ZoomRectangle);

        // Reset bindings: Same as zoom rectangle, but double click / A key
        this.BindMouseDown(PlotterMouseButton.Middle, PlotterModifierKeys.None, 2, PlotCommands.ResetAt);
        this.BindMouseDown(PlotterMouseButton.Right, PlotterModifierKeys.Control, 2, PlotCommands.ResetAt);
        this.BindMouseDown(PlotterMouseButton.Left, PlotterModifierKeys.Control | PlotterModifierKeys.Alt, 2, PlotCommands.ResetAt);
        this.BindKeyDown(PlotterKey.A, PlotCommands.Reset);
        this.BindKeyDown(PlotterKey.C, PlotterModifierKeys.Control | PlotterModifierKeys.Alt, PlotCommands.CopyCode);
        this.BindKeyDown(PlotterKey.Home, PlotCommands.Reset);
        this.BindCore(new PlotterShakeGesture(), PlotCommands.Reset);

        // Pan bindings: RMB / alt LMB / Up/down/left/right keys (panning direction on axis is opposite of key as it is more intuitive)
        this.BindMouseDown(PlotterMouseButton.Right, PlotCommands.PanAt);
        this.BindMouseDown(PlotterMouseButton.Left, PlotterModifierKeys.Alt, PlotCommands.PanAt);
        this.BindKeyDown(PlotterKey.Left, PlotCommands.PanLeft);
        this.BindKeyDown(PlotterKey.Right, PlotCommands.PanRight);
        this.BindKeyDown(PlotterKey.Up, PlotCommands.PanUp);
        this.BindKeyDown(PlotterKey.Down, PlotCommands.PanDown);
        this.BindKeyDown(PlotterKey.Left, PlotterModifierKeys.Control, PlotCommands.PanLeftFine);
        this.BindKeyDown(PlotterKey.Right, PlotterModifierKeys.Control, PlotCommands.PanRightFine);
        this.BindKeyDown(PlotterKey.Up, PlotterModifierKeys.Control, PlotCommands.PanUpFine);
        this.BindKeyDown(PlotterKey.Down, PlotterModifierKeys.Control, PlotCommands.PanDownFine);

        this.BindTouchDown(PlotCommands.PanZoomByTouch);

        // Tracker bindings: LMB
        this.BindMouseDown(PlotterMouseButton.Left, PlotCommands.SnapTrack);
        this.BindMouseDown(PlotterMouseButton.Left, PlotterModifierKeys.Control, PlotCommands.Track);
        this.BindMouseDown(PlotterMouseButton.Left, PlotterModifierKeys.Shift, PlotCommands.PointsOnlyTrack);

        // Tracker bindings: Touch
        this.BindTouchDown(PlotCommands.SnapTrackTouch);

        // Zoom in/out binding: XB1 / XB2 / mouse wheels / +/- keys
        this.BindMouseDown(PlotterMouseButton.XButton1, PlotCommands.ZoomInAt);
        this.BindMouseDown(PlotterMouseButton.XButton2, PlotCommands.ZoomOutAt);
        this.BindMouseWheel(PlotCommands.ZoomWheel);
        this.BindMouseWheel(PlotterModifierKeys.Control, PlotCommands.ZoomWheelFine);
        this.BindKeyDown(PlotterKey.Add, PlotCommands.ZoomIn);
        this.BindKeyDown(PlotterKey.Subtract, PlotCommands.ZoomOut);
        this.BindKeyDown(PlotterKey.PageUp, PlotCommands.ZoomIn);
        this.BindKeyDown(PlotterKey.PageDown, PlotCommands.ZoomOut);
        this.BindKeyDown(PlotterKey.Add, PlotterModifierKeys.Control, PlotCommands.ZoomInFine);
        this.BindKeyDown(PlotterKey.Subtract, PlotterModifierKeys.Control, PlotCommands.ZoomOutFine);
        this.BindKeyDown(PlotterKey.PageUp, PlotterModifierKeys.Control, PlotCommands.ZoomInFine);
        this.BindKeyDown(PlotterKey.PageDown, PlotterModifierKeys.Control, PlotCommands.ZoomOutFine);
    }
}
