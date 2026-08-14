// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaletteExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("Palettes")]
public class PaletteExamples
{
    [Example("Default palette")]
    public static PlotModel DefaultPalette()
    {
        return HeatMapSeriesExamples.CreatePeaks(null, false);
    }

    [Example("Jet (200 colors) palette")]
    public static PlotModel Jet200()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Jet(200), false);
    }

    [Example("Jet (20 colors) palette")]
    public static PlotModel Jet20()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Jet(20), false);
    }

    [Example("Hue (400 colors) palette")]
    public static PlotModel Hue400()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Hue(400), false);
    }

    [Example("Hue distinct (200 colors) palette")]
    public static PlotModel HueDistinct200()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.HueDistinct(200), false);
    }

    [Example("Hot (200 colors) palette")]
    public static PlotModel Hot200()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Hot(200), false);
    }

    [Example("Hot (64 colors) palette")]
    public static PlotModel Hot64()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Hot64, false);
    }

    [Example("Hot (30 colors) palette")]
    public static PlotModel Hot30()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Hot(30), false);
    }

    [Example("Blue-white-red (200 colors) palette")]
    public static PlotModel BlueWhiteRed200()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.BlueWhiteRed(200), false);
    }

    [Example("Blue-white-red (40 colors) palette")]
    public static PlotModel BlueWhiteRed40()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.BlueWhiteRed(40), false);
    }

    [Example("Black-white-red (500 colors) palette")]
    public static PlotModel BlackWhiteRed500()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.BlackWhiteRed(500), false);
    }

    [Example("Black-white-red (3 colors) palette")]
    public static PlotModel BlackWhiteRed3()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.BlackWhiteRed(3), false);
    }

    [Example("Cool (200 colors) palette")]
    public static PlotModel Cool200()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Cool(200), false);
    }

    [Example("Rainbow (200 colors) palette")]
    public static PlotModel Rainbow200()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Rainbow(200), false);
    }

    [Example("Viridis palette")]
    public static PlotModel Viridis()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Viridis(), false);
    }

    [Example("Plasma palette")]
    public static PlotModel Plasma()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Plasma(), false);
    }

    [Example("Magma palette")]
    public static PlotModel Magma()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Magma(), false);
    }

    [Example("Inferno palette")]
    public static PlotModel Inferno()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Inferno(), false);
    }

    [Example("Cividis palette")]
    public static PlotModel Cividis()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Cividis(), false);
    }

    [Example("Viridis (10 colors) palette")]
    public static PlotModel Viridis10()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Viridis(10), false);
    }

    [Example("Rainbow (7 colors) palette")]
    public static PlotModel Rainbow7()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Rainbow(7), false);
    }

    [Example("Jet (6 colors) palette")]
    public static PlotModel Vertical_6()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Jet(6), false);
    }
}
