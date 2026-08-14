// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterPalettes.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides predefined palettes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides predefined palettes.
/// </summary>
public static partial class PlotterPalettes
{
    /// <summary>
    /// Initializes static members of the <see cref="PlotterPalettes" /> class.
    /// </summary>
    static PlotterPalettes()
    {
        BlueWhiteRed31 = BlueWhiteRed(31);
        Hot64 = Hot(64);
        Hue64 = Hue(64);
    }

    /// <summary>
    /// Gets the blue-white-red palette with 31 colors.
    /// </summary>
    public static PlotterPalette BlueWhiteRed31 { get; private set; }

    /// <summary>
    /// Gets the hot palette with 64 colors.
    /// </summary>
    public static PlotterPalette Hot64 { get; private set; }

    /// <summary>
    /// Gets the hue palette with 64 colors.
    /// </summary>
    public static PlotterPalette Hue64 { get; private set; }

    /// <summary>
    /// Creates a black/white/red palette with the specified number of colors.
    /// </summary>
    /// <param name="numberOfColors">The number of colors to create for the palette.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette BlackWhiteRed(int numberOfColors)
    {
        return PlotterPalette.Interpolate(numberOfColors, PlotterColors.Black, PlotterColors.White, PlotterColors.Red);
    }

    /// <summary>
    /// Creates a blue/white/red palette with the specified number of colors.
    /// </summary>
    /// <param name="numberOfColors">The number of colors to create for the palette.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette BlueWhiteRed(int numberOfColors)
    {
        return PlotterPalette.Interpolate(numberOfColors, PlotterColors.Blue, PlotterColors.White, PlotterColors.Red);
    }

    /// <summary>
    /// Creates a 'cool' palette with the specified number of colors.
    /// </summary>
    /// <param name="numberOfColors">The number of colors to create for the palette.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette Cool(int numberOfColors)
    {
        return PlotterPalette.Interpolate(numberOfColors, PlotterColors.Cyan, PlotterColors.Magenta);
    }

    /// <summary>
    /// Creates a gray-scale palette with the specified number of colors.
    /// </summary>
    /// <param name="numberOfColors">The number of colors to create for the palette.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette Gray(int numberOfColors)
    {
        return PlotterPalette.Interpolate(numberOfColors, PlotterColors.Black, PlotterColors.White);
    }

    /// <summary>
    /// Creates a 'hot' palette with the specified number of colors.
    /// </summary>
    /// <param name="numberOfColors">The number of colors to create for the palette.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette Hot(int numberOfColors)
    {
        return PlotterPalette.Interpolate(
            numberOfColors,
            PlotterColors.Black,
            PlotterColor.FromRgb(127, 0, 0),
            PlotterColor.FromRgb(255, 127, 0),
            PlotterColor.FromRgb(255, 255, 127),
            PlotterColors.White);
    }

    /// <summary>
    /// Creates a palette from the hue component of the HSV color model.
    /// </summary>
    /// <param name="numberOfColors">The number of colors.</param>
    /// <returns>The palette.</returns>
    /// <remarks>This palette is particularly appropriate for displaying periodic functions.</remarks>
    public static PlotterPalette Hue(int numberOfColors)
    {
        return PlotterPalette.Interpolate(
            numberOfColors,
            PlotterColors.Red,
            PlotterColors.Yellow,
            PlotterColors.Green,
            PlotterColors.Cyan,
            PlotterColors.Blue,
            PlotterColors.Magenta,
            PlotterColors.Red);
    }

    /// <summary>
    /// Creates a hue-based palette from magenta to red.
    /// </summary>
    /// <param name="numberOfColors">The number of colors.</param>
    /// <returns>The palette.</returns>
    /// <remarks>This palette contains only distinct colors and with the cool colors (blues) first.</remarks>
    public static PlotterPalette HueDistinct(int numberOfColors)
    {
        return PlotterPalette.Interpolate(
            numberOfColors,
            PlotterColors.Magenta,
            PlotterColors.Blue,
            PlotterColors.Cyan,
            PlotterColors.Green,
            PlotterColors.Yellow,
            PlotterColors.Red);
    }

    /// <summary>
    /// Creates a 'jet' palette with the specified number of colors.
    /// </summary>
    /// <param name="numberOfColors">The number of colors to create for the palette.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette Jet(int numberOfColors)
    {
        return PlotterPalette.Interpolate(
            numberOfColors,
            PlotterColors.DarkBlue,
            PlotterColors.Cyan,
            PlotterColors.Yellow,
            PlotterColors.Orange,
            PlotterColors.DarkRed);
    }

    /// <summary>
    /// Creates a rainbow palette with the specified number of colors.
    /// </summary>
    /// <param name="numberOfColors">The number of colors to create for the palette.</param>
    /// <returns>A palette.</returns>
    public static PlotterPalette Rainbow(int numberOfColors)
    {
        return PlotterPalette.Interpolate(
            numberOfColors,
            PlotterColors.Violet,
            PlotterColors.Indigo,
            PlotterColors.Blue,
            PlotterColors.Green,
            PlotterColors.Yellow,
            PlotterColors.Orange,
            PlotterColors.Red);
    }
}
