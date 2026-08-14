// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IController.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Specifies functionality to interact with a graphics view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Specifies functionality to interact with a graphics view.
/// </summary>
public interface IController
{
    /// <summary>
    /// Handles mouse down events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleMouseDown(IView view, PlotterMouseDownEventArgs args);

    /// <summary>
    /// Handles mouse move events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleMouseMove(IView view, PlotterMouseEventArgs args);

    /// <summary>
    /// Handles mouse up events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleMouseUp(IView view, PlotterMouseEventArgs args);

    /// <summary>
    /// Handles mouse enter events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleMouseEnter(IView view, PlotterMouseEventArgs args);

    /// <summary>
    /// Handles mouse leave events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleMouseLeave(IView view, PlotterMouseEventArgs args);

    /// <summary>
    /// Handles mouse wheel events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterMouseWheelEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleMouseWheel(IView view, PlotterMouseWheelEventArgs args);

    /// <summary>
    /// Handles touch started events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterTouchEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleTouchStarted(IView view, PlotterTouchEventArgs args);

    /// <summary>
    /// Handles touch delta events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterTouchEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleTouchDelta(IView view, PlotterTouchEventArgs args);

    /// <summary>
    /// Handles touch completed events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterTouchEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleTouchCompleted(IView view, PlotterTouchEventArgs args);

    /// <summary>
    /// Handles key down events.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="args">The <see cref="PlotterKeyEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleKeyDown(IView view, PlotterKeyEventArgs args);

    /// <summary>
    /// Handles the specified gesture.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="gesture">The gesture.</param>
    /// <param name="args">The <see cref="PlotterInputEventArgs" /> instance containing the event data.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    bool HandleGesture(IView view, PlotterInputGesture gesture, PlotterInputEventArgs args);

    /// <summary>
    /// Adds the specified mouse manipulator and invokes the <see cref="MouseManipulator.Started" /> method with the specified mouse event arguments.
    /// </summary>
    /// <param name="view">The plot view.</param>
    /// <param name="manipulator">The manipulator to add.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    void AddMouseManipulator(IView view, ManipulatorBase<PlotterMouseEventArgs> manipulator, PlotterMouseDownEventArgs args);

    /// <summary>
    /// Adds the specified mouse hover manipulator and invokes the <see cref="MouseManipulator.Started" /> method with the specified mouse event arguments.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="manipulator">The manipulator.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    void AddHoverManipulator(IView view, ManipulatorBase<PlotterMouseEventArgs> manipulator, PlotterMouseEventArgs args);

    /// <summary>
    /// Adds the specified touch manipulator and invokes the <see cref="MouseManipulator.Started" /> method with the specified mouse event arguments.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="manipulator">The manipulator.</param>
    /// <param name="args">The <see cref="PlotterMouseEventArgs" /> instance containing the event data.</param>
    void AddTouchManipulator(IView view, ManipulatorBase<PlotterTouchEventArgs> manipulator, PlotterTouchEventArgs args);

    /// <summary>
    /// Binds the specified command to the specified mouse down gesture. Removes old bindings to the gesture.
    /// </summary>
    /// <param name="gesture">The mouse down gesture.</param>
    /// <param name="command">The command. If <c>null</c>, the binding will be removed.</param>
    void Bind(PlotterMouseDownGesture gesture, IViewCommand<PlotterMouseDownEventArgs> command);

    /// <summary>
    /// Binds the specified command to the specified mouse enter gesture. Removes old bindings to the gesture.
    /// </summary>
    /// <param name="gesture">The mouse enter gesture.</param>
    /// <param name="command">The command. If <c>null</c>, the binding will be removed.</param>
    void Bind(PlotterMouseEnterGesture gesture, IViewCommand<PlotterMouseEventArgs> command);

    /// <summary>
    /// Binds the specified command to the specified mouse wheel gesture. Removes old bindings to the gesture.
    /// </summary>
    /// <param name="gesture">The mouse wheel gesture.</param>
    /// <param name="command">The command. If <c>null</c>, the binding will be removed.</param>
    void Bind(PlotterMouseWheelGesture gesture, IViewCommand<PlotterMouseWheelEventArgs> command);

    /// <summary>
    /// Binds the specified command to the specified touch gesture. Removes old bindings to the gesture.
    /// </summary>
    /// <param name="gesture">The touch gesture.</param>
    /// <param name="command">The command. If <c>null</c>, the binding will be removed.</param>
    void Bind(PlotterTouchGesture gesture, IViewCommand<PlotterTouchEventArgs> command);

    /// <summary>
    /// Binds the specified command to the specified key gesture. Removes old bindings to the gesture.
    /// </summary>
    /// <param name="gesture">The key gesture.</param>
    /// <param name="command">The command. If <c>null</c>, the binding will be removed.</param>
    void Bind(PlotterKeyGesture gesture, IViewCommand<PlotterKeyEventArgs> command);

    /// <summary>
    /// Unbinds the specified gesture.
    /// </summary>
    /// <param name="gesture">The gesture to unbind.</param>
    void Unbind(PlotterInputGesture gesture);

    /// <summary>
    /// Unbinds the specified command from all gestures.
    /// </summary>
    /// <param name="command">The command to unbind.</param>
    void Unbind(IViewCommand command);

    /// <summary>
    /// Unbinds all commands.
    /// </summary>
    void UnbindAll();
}
