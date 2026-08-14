// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterMouseWheelEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides data for mouse wheel events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides data for mouse wheel events.
/// </summary>
public class PlotterMouseWheelEventArgs : PlotterMouseEventArgs
{
    /// <summary>
    /// Gets or sets the change.
    /// </summary>
    public int Delta { get; set; }
}
