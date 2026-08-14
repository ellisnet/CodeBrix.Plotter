// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleBarSeriesExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Legends;
using CodeBrix.Plotter.Series;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("RectangleBarSeries")]
[Tags("Series")]
public static class RectangleBarSeriesExamples
{
    [Example("RectangleBarSeries")]
    [DocumentationExample("Series/RectangleBarSeries")]
    public static PlotModel RectangleBarSeries()
    {
        var model = new PlotModel { Title = "RectangleBarSeries" };
        var l = new Legend
        {
            LegendPlacement = LegendPlacement.Outside
        };

        model.Legends.Add(l);
        var s1 = new RectangleBarSeries { Title = "RectangleBarSeries 1" };
        s1.Items.Add(new RectangleBarItem { X0 = 2, X1 = 8, Y0 = 1, Y1 = 4 });
        s1.Items.Add(new RectangleBarItem { X0 = 6, X1 = 12, Y0 = 6, Y1 = 7 });
        model.Series.Add(s1);

        var s2 = new RectangleBarSeries { Title = "RectangleBarSeries 2" };
        s2.Items.Add(new RectangleBarItem { X0 = 2, X1 = 8, Y0 = -4, Y1 = -1 });
        s2.Items.Add(new RectangleBarItem { X0 = 6, X1 = 12, Y0 = -7, Y1 = -6 });
        model.Series.Add(s2);

        return model;
    }
}
