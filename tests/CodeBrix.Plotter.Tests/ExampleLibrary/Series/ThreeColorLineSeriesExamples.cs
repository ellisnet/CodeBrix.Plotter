// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreeColorLineSeriesExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides examples for the ThreeColorLineSeries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Legends;
using CodeBrix.Plotter.Series;
using CodeBrix.Plotter.Tests.ExampleLibrary.Utilities;

namespace CodeBrix.Plotter.Tests.ExampleLibrary.Series; //was previously: ExampleLibrary.Series;

/// <summary>
/// Provides examples for the <see cref="ThreeColorLineSeries" />.
/// </summary>
[Examples("ThreeColorLineSeries")]
[Tags("Series")]
public class ThreeColorLineSeriesExamples
{
    /// <summary>
    /// Creates an example showing temperatures.
    /// </summary>
    /// <returns>A <see cref="PlotModel" />.</returns>
    [Example("Temperatures")]
    [DocumentationExample("Series/ThreeColorLineSeries")]
    public static PlotModel ThreeColorLineSeries()
    {
        var model = new PlotModel { Title = "ThreeColorLineSeries" };
        var l = new Legend
        {
            LegendSymbolLength = 24
        };

        model.Legends.Add(l);

        var s1 = new ThreeColorLineSeries
        {
            Title = "Temperature at Eidesmoen, December 1986.",
            TrackerFormatString = "December {2:0}: {4:0.0} °C",
            MarkerSize = 4,
            MarkerStroke = PlotterColors.Black,
            MarkerStrokeThickness = 1.5,
            MarkerType = MarkerType.Circle,
            InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
            StrokeThickness = 3,
        };

        var temperatures = new[] { 5, 0, 7, 7, 4, 3, 5, 5, 11, 4, 2, 3, 2, 1, 0, 2, -1, 0, 0, -3, -6, -13, -10, -10, 0, -4, -5, -4, 3, 0, -5 };

        for (int i = 0; i < temperatures.Length; i++)
        {
            s1.Points.Add(new DataPoint(i + 1, temperatures[i]));
        }

        model.Series.Add(s1);
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Temperature", Unit = "°C", ExtraGridlines = new[] { 0.0 } });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Date" });

        return model;
    }

    [Example("Temperatures (Y Axis reversed)")]
    public static PlotModel TwoColorLineSeriesReversed()
    {
        return ThreeColorLineSeries().ReverseYAxis();
    }
}
