// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerExtensions.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides extension methods for the <see cref="IController" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides extension methods for the <see cref="IController" />.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Binds the specified key to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="key">The key.</param>
    /// <param name="command">A plot controller command that takes key event arguments.</param>
    public static void BindKeyDown(this IController controller, PlotterKey key, IViewCommand<PlotterKeyEventArgs> command)
    {
        controller.Bind(new PlotterKeyGesture(key), command);
    }

    /// <summary>
    /// Binds the specified modifier+key to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The key modifiers.</param>
    /// <param name="command">A plot controller command that takes key event arguments.</param>
    public static void BindKeyDown(this IController controller, PlotterKey key, PlotterModifierKeys modifiers, IViewCommand<PlotterKeyEventArgs> command)
    {
        controller.Bind(new PlotterKeyGesture(key, modifiers), command);
    }

    /// <summary>
    /// Binds the specified mouse button to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="mouseButton">The mouse button.</param>
    /// <param name="command">A plot controller command that takes mouse event arguments.</param>
    public static void BindMouseDown(this IController controller, PlotterMouseButton mouseButton, IViewCommand<PlotterMouseDownEventArgs> command)
    {
        controller.Bind(new PlotterMouseDownGesture(mouseButton), command);
    }

    /// <summary>
    /// Binds the specified modifier+mouse button gesture to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="mouseButton">The mouse button.</param>
    /// <param name="modifiers">The modifiers.</param>
    /// <param name="command">A plot controller command that takes mouse event arguments.</param>
    public static void BindMouseDown(this IController controller, PlotterMouseButton mouseButton, PlotterModifierKeys modifiers, IViewCommand<PlotterMouseDownEventArgs> command)
    {
        controller.Bind(new PlotterMouseDownGesture(mouseButton, modifiers), command);
    }

    /// <summary>
    /// Binds the specified modifiers+mouse button+click count gesture to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="mouseButton">The mouse button.</param>
    /// <param name="modifiers">The modifiers.</param>
    /// <param name="clickCount">The click count.</param>
    /// <param name="command">A plot controller command that takes mouse event arguments.</param>
    public static void BindMouseDown(this IController controller, PlotterMouseButton mouseButton, PlotterModifierKeys modifiers, int clickCount, IViewCommand<PlotterMouseDownEventArgs> command)
    {
        controller.Bind(new PlotterMouseDownGesture(mouseButton, modifiers, clickCount), command);
    }

    /// <summary>
    /// Binds the touch down event to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="command">A plot controller command that takes touch event arguments.</param>
    public static void BindTouchDown(this IController controller, IViewCommand<PlotterTouchEventArgs> command)
    {
        controller.Bind(new PlotterTouchGesture(), command);
    }

    /// <summary>
    /// Binds the mouse enter event to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="command">A plot controller command that takes mouse event arguments.</param>
    public static void BindMouseEnter(this IController controller, IViewCommand<PlotterMouseEventArgs> command)
    {
        controller.Bind(new PlotterMouseEnterGesture(), command);
    }

    /// <summary>
    /// Binds the mouse wheel event to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="command">A plot controller command that takes mouse wheel event arguments.</param>
    public static void BindMouseWheel(this IController controller, IViewCommand<PlotterMouseWheelEventArgs> command)
    {
        controller.Bind(new PlotterMouseWheelGesture(), command);
    }

    /// <summary>
    /// Binds the modifier+mouse wheel event to the specified command.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    /// <param name="modifiers">The modifier key(s).</param>
    /// <param name="command">A plot controller command that takes mouse wheel event arguments.</param>
    public static void BindMouseWheel(this IController controller, PlotterModifierKeys modifiers, IViewCommand<PlotterMouseWheelEventArgs> command)
    {
        controller.Bind(new PlotterMouseWheelGesture(modifiers), command);
    }

    /// <summary>
    /// Unbinds the specified mouse down gesture.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="mouseButton">The mouse button.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <param name="clickCount">The click count.</param>
    public static void UnbindMouseDown(this IController controller, PlotterMouseButton mouseButton, PlotterModifierKeys modifiers = PlotterModifierKeys.None, int clickCount = 1)
    {
        controller.Unbind(new PlotterMouseDownGesture(mouseButton, modifiers, clickCount));
    }

    /// <summary>
    /// Unbinds the specified key down gesture.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    public static void UnbindKeyDown(this IController controller, PlotterKey key, PlotterModifierKeys modifiers = PlotterModifierKeys.None)
    {
        controller.Unbind(new PlotterKeyGesture(key, modifiers));
    }

    /// <summary>
    /// Unbinds the mouse enter gesture.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public static void UnbindMouseEnter(this IController controller)
    {
        controller.Unbind(new PlotterMouseEnterGesture());
    }

    /// <summary>
    /// Unbinds the touch down gesture.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public static void UnbindTouchDown(this IController controller)
    {
        controller.Unbind(new PlotterTouchGesture());
    }

    /// <summary>
    /// Unbinds the mouse wheel gesture.
    /// </summary>
    /// <param name="controller">The controller.</param>
    public static void UnbindMouseWheel(this IController controller)
    {
        controller.Unbind(new PlotterMouseWheelGesture());
    }
}
