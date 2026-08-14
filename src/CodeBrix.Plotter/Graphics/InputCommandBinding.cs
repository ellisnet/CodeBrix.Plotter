// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputCommandBinding.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents an binding by an input gesture and a command binding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Represents an binding by an input gesture and a command binding.
/// </summary>
public class InputCommandBinding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputCommandBinding" /> class by a gesture.
    /// </summary>
    /// <param name="gesture">The gesture.</param>
    /// <param name="command">The command.</param>
    public InputCommandBinding(PlotterInputGesture gesture, IViewCommand command)
    {
        this.Gesture = gesture;
        this.Command = command;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputCommandBinding" /> class by a key gesture.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifiers.</param>
    /// <param name="command">The command.</param>
    public InputCommandBinding(PlotterKey key, PlotterModifierKeys modifiers, IViewCommand command)
        : this(new PlotterKeyGesture(key, modifiers), command)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputCommandBinding" /> class by a mouse gesture.
    /// </summary>
    /// <param name="mouseButton">The mouse button.</param>
    /// <param name="modifiers">The modifiers.</param>
    /// <param name="command">The command.</param>
    public InputCommandBinding(PlotterMouseButton mouseButton, PlotterModifierKeys modifiers, IViewCommand command)
        : this(new PlotterMouseDownGesture(mouseButton, modifiers), command)
    {
    }

    /// <summary>
    /// Gets the gesture.
    /// </summary>
    public PlotterInputGesture Gesture { get; private set; }

    /// <summary>
    /// Gets the command.
    /// </summary>
    public IViewCommand Command { get; private set; }
}
