// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterMouseEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides data for the mouse events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides data for the mouse events.
/// </summary>
public class PlotterMouseEventArgs : PlotterInputEventArgs
{
    /// <summary>
    /// Gets or sets the position of the mouse cursor.
    /// </summary>
    public ScreenPoint Position { get; set; }
}
