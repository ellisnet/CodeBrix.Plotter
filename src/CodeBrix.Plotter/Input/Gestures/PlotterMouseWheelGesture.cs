// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterMouseWheelGesture.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a mouse wheel gesture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Represents a mouse wheel gesture.
/// </summary>
/// <remarks>The input gesture can be bound to a command in a <see cref="PlotController" />.</remarks>
public class PlotterMouseWheelGesture : PlotterInputGesture
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotterMouseWheelGesture" /> class.
    /// </summary>
    /// <param name="modifiers">The modifiers.</param>
    public PlotterMouseWheelGesture(PlotterModifierKeys modifiers = PlotterModifierKeys.None)
    {
        this.Modifiers = modifiers;
    }

    /// <summary>
    /// Gets the modifier keys.
    /// </summary>
    public PlotterModifierKeys Modifiers { get; private set; }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.</returns>
    public override bool Equals(PlotterInputGesture other)
    {
        var mwg = other as PlotterMouseWheelGesture;
        return mwg != null && mwg.Modifiers == this.Modifiers;
    }
}
