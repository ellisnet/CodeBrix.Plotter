// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterKeyGesture.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a keyboard input gesture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Represents a keyboard input gesture.
/// </summary>
/// <remarks>The input gesture can be bound to a command in a <see cref="PlotController" />.</remarks>
public class PlotterKeyGesture : PlotterInputGesture
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotterKeyGesture" /> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    public PlotterKeyGesture(PlotterKey key, PlotterModifierKeys modifiers = PlotterModifierKeys.None)
    {
        this.Key = key;
        this.Modifiers = modifiers;
    }

    /// <summary>
    /// Gets or sets the modifier keys.
    /// </summary>
    public PlotterModifierKeys Modifiers { get; set; }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public PlotterKey Key { get; set; }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.</returns>
    public override bool Equals(PlotterInputGesture other)
    {
        var kg = other as PlotterKeyGesture;
        return kg != null && kg.Modifiers == this.Modifiers && kg.Key == this.Key;
    }
}
