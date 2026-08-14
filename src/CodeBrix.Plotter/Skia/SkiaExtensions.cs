// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkiaExtensions.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter.Skia; //was previously: OxyPlot.SkiaSharp;

using global::SkiaSharp;

/// <summary>
/// Provides extension methods for conversion between SkiaSharp and oxyplot objects.
/// </summary>
public static class SkiaExtensions
{
    /// <summary>
    /// Converts a <see cref="PlotterColor"/> to a <see cref="SKColor"/>;
    /// </summary>
    /// <param name="color">The <see cref="PlotterColor"/>.</param>
    /// <returns>The <see cref="SKColor"/>.</returns>
    public static PlotterColor ToPlotterColor(this SKColor color)
    {
        return PlotterColor.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
    }

    /// <summary>
    /// Converts a <see cref="SKColor"/> to a <see cref="PlotterColor"/>;
    /// </summary>
    /// <param name="color">The <see cref="SKColor"/>.</param>
    /// <returns>The <see cref="PlotterColor"/>.</returns>
    public static SKColor ToSKColor(this PlotterColor color)
    {
        return new SKColor(color.R, color.G, color.B, color.A);
    }
}
