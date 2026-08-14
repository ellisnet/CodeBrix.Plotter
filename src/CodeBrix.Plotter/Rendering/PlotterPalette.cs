// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterPalette.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a palette of colors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Represents a palette of colors.
/// </summary>
public class PlotterPalette
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotterPalette" /> class.
    /// </summary>
    public PlotterPalette()
    {
        this.Colors = new List<PlotterColor>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotterPalette" /> class.
    /// </summary>
    /// <param name="colors">The colors.</param>
    public PlotterPalette(params PlotterColor[] colors)
    {
        this.Colors = new List<PlotterColor>(colors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotterPalette" /> class.
    /// </summary>
    /// <param name="colors">The colors.</param>
    public PlotterPalette(IEnumerable<PlotterColor> colors)
    {
        this.Colors = new List<PlotterColor>(colors);
    }

    /// <summary>
    /// Gets or sets the colors.
    /// </summary>
    /// <value>The colors.</value>
    public IList<PlotterColor> Colors { get; set; }

    /// <summary>
    /// Interpolates the specified colors to a palette of the specified size.
    /// </summary>
    /// <param name="paletteSize">The size of the palette.</param>
    /// <param name="colors">The colors.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette Interpolate(int paletteSize, params PlotterColor[] colors)
    {
        if (colors == null || colors.Length == 0 || paletteSize < 1)
        {
            // There is no color to interpolate or no color required.
            return new PlotterPalette(new PlotterColor[0]);
        }

        var palette = new PlotterColor[paletteSize];

        double incrementStepSize = (paletteSize == 1) ? 0 : (1.0d / (paletteSize - 1));

        for (int i = 0; i < paletteSize; i++)
        {
            double y = i * incrementStepSize;
            double x = y * (colors.Length - 1);
            int i0 = (int)x;
            int i1 = i0 + 1 < colors.Length ? i0 + 1 : i0;
            palette[i] = PlotterColor.Interpolate(colors[i0], colors[i1], x - i0);
        }

        return new PlotterPalette(palette);
    }

    /// <summary>
    /// Creates a palette with reversed color order.
    /// </summary>
    /// <returns>The reversed <see cref="PlotterPalette" />.</returns>
    public PlotterPalette Reverse()
    {
        return new PlotterPalette(this.Colors.Reverse());
    }
}
