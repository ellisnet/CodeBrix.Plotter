// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterKeyEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides data for key events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides data for key events.
/// </summary>
public class PlotterKeyEventArgs : PlotterInputEventArgs
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public PlotterKey Key { get; set; }
}
