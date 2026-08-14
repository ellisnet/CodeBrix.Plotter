// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StemSeriesExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides examples for the <see cref="StemSeries" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Legends;
using CodeBrix.Plotter.Series;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

/// <summary>
/// Provides examples for the <see cref="StemSeries" />.
/// </summary>
[Examples("StemSeries")]
[Tags("Series")]
public static class StemSeriesExamples
{
    [Example("StemSeries")]
    [DocumentationExample("Series/StemSeries")]
    public static PlotModel StemSeries()
    {
        return CreateExampleModel(new StemSeries
                     {
                         Color = PlotterColors.SkyBlue,
                         MarkerType = MarkerType.Circle,
                         MarkerSize = 6,
                         MarkerStroke = PlotterColors.White,
                         MarkerStrokeThickness = 1.5
                     });
    }

    /// <summary>
    /// Creates an example model and fills the specified series with points.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>A plot model.</returns>
    private static PlotModel CreateExampleModel(DataPointSeries series)
    {
        var model = new PlotModel { Title = "StemSeries" };
        var l = new Legend
        {
            LegendSymbolLength = 24
        };

        model.Legends.Add(l);
        series.Title = "sin(x)";
        for (double x = 0; x < Math.PI * 2; x += 0.1)
        {
            series.Points.Add(new DataPoint(x, Math.Sin(x)));
        }

        model.Series.Add(series);
        return model;
    }
}
