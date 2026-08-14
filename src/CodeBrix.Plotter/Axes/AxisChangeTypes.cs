// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AxisChangeTypes.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Defines change types for the <see cref="Axis.AxisChanged" /> event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter.Axes; //was previously: OxyPlot.Axes;

/// <summary>
/// Defines the ways in which an <see cref="Axis" /> range can change.
/// </summary>
public enum AxisChangeTypes
{
    /// <summary>
    /// The axis was zoomed by the user.
    /// </summary>
    Zoom,

    /// <summary>
    /// The axis was panned by the user.
    /// </summary>
    Pan,

    /// <summary>
    /// The axis zoom/pan was reset by the user.
    /// </summary>
    Reset
}
