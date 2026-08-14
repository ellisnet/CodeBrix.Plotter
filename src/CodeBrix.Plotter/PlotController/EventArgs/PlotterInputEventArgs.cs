// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterInputEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides an abstract base class for classes that contain event data for input events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides an abstract base class for classes that contain event data for input events.
/// </summary>
public abstract class PlotterInputEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the event was handled.
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    /// Gets or sets the modifier keys.
    /// </summary>
    public PlotterModifierKeys ModifierKeys { get; set; }

    /// <summary>
    /// Gets a value indicating whether the alt key was pressed when the event was raised.
    /// </summary>
    public bool IsAltDown
    {
        get
        {
            return (this.ModifierKeys & PlotterModifierKeys.Alt) == PlotterModifierKeys.Alt;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the control key was pressed when the event was raised.
    /// </summary>
    public bool IsControlDown
    {
        get
        {
            return (this.ModifierKeys & PlotterModifierKeys.Control) == PlotterModifierKeys.Control;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the shift key was pressed when the event was raised.
    /// </summary>
    public bool IsShiftDown
    {
        get
        {
            return (this.ModifierKeys & PlotterModifierKeys.Shift) == PlotterModifierKeys.Shift;
        }
    }
}
