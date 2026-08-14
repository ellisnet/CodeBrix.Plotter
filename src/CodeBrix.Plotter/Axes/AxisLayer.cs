// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AxisLayer.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Specifies the layer of an <see cref="Axis" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter.Axes; //was previously: OxyPlot.Axes;

/// <summary>
/// Specifies the layer of an <see cref="Axis" />.
/// </summary>
public enum AxisLayer
{
    /// <summary>
    /// Below all series.
    /// </summary>
    BelowSeries,

    /// <summary>
    /// Above all series.
    /// </summary>
    AboveSeries
}
