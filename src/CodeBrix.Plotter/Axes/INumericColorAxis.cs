// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INumericColorAxis.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter.Axes; //was previously: OxyPlot.Axes;

/// <summary>
/// Specifies functionality for numeric color axes.
/// </summary>
public interface INumericColorAxis : IColorAxis
{
    /// <summary>
    /// Gets or sets the palette.
    /// </summary>
    /// <value>The palette.</value>
    PlotterPalette Palette { get; set; }

    /// <summary>
    /// Gets or sets the color of values above the maximum value.
    /// </summary>
    /// <value>The color of the high values.</value>
    PlotterColor HighColor { get; set; }

    /// <summary>
    /// Gets or sets the color of values below the minimum value.
    /// </summary>
    /// <value>The color of the low values.</value>
    PlotterColor LowColor { get; set; }

    /// <summary>
    /// Gets or sets the color used to represent NaN values.
    /// </summary>
    /// <value>A <see cref="PlotterColor" /> that defines the color. The default value is <c>PlotterColors.Gray</c>.</value>
    PlotterColor InvalidNumberColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to render the colors as an image.
    /// </summary>
    /// <value><c>true</c> if the rendering should use an image; otherwise, <c>false</c>.</value>
    bool RenderAsImage { get; set; }
}
