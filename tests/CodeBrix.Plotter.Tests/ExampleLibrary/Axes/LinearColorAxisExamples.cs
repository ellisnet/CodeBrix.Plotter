// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearColorAxisExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Axes;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("LinearColorAxis")]
[Tags("Axes")]
public class LinearColorAxisExamples
{
    private static PlotModel EnableRenderAsImage(PlotModel plotModel)
    {
        var axis = plotModel.Axes.OfType<LinearColorAxis>().Single();
        axis.RenderAsImage = true;
        plotModel.Title += " - RenderAsImage";
        return plotModel;
    }

    [Example("Peaks")]
    public static PlotModel Peaks()
    {
        return HeatMapSeriesExamples.CreatePeaks(null, false);
    }

    [Example("Peaks - RenderAsImage")]
    public static PlotModel PeaksRenderAsImage()
    {
        return EnableRenderAsImage(Peaks());
    }

    [Example("6 Colors")]
    public static PlotModel Horizontal6()
    {
        return HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Jet(6), false);
    }

    [Example("6 Colors - RenderAsImage")]
    public static PlotModel Horizontal6RenderAsImage()
    {
        return EnableRenderAsImage(Horizontal6());
    }

    [Example("Short")]
    public static PlotModel Short()
    {
        var model = HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Jet(600), false);
        var colorAxis = (LinearColorAxis)model.Axes[0];
        colorAxis.StartPosition = 0.02;
        colorAxis.EndPosition = 0.5;
        return model;
    }

    [Example("Short - RenderAsImage")]
    public static PlotModel ShortRenderAsImage()
    {
        return EnableRenderAsImage(Short());
    }

    [Example("Position None")]
    public static PlotModel Position_None()
    {
        var model = HeatMapSeriesExamples.CreatePeaks(PlotterPalettes.Jet(600), false);
        var colorAxis = (LinearColorAxis)model.Axes[0];
        colorAxis.Position = AxisPosition.None;
        return model;
    }
}
