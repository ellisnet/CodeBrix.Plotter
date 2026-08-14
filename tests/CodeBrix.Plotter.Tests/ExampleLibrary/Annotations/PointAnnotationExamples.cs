// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointAnnotationExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Annotations;
using CodeBrix.Plotter.Axes;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("PointAnnotation")]
[Tags("Annotations")]
public static class PointAnnotationExamples
{
    [Example("PointAnnotation")]
    public static PlotModel PointAnnotation()
    {
        var model = new PlotModel { Title = "PointAnnotation" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

        model.Annotations.Add(new PointAnnotation { X = 50, Y = 50, Text = "P1" });
        return model;
    }

    [Example("PointAnnotation - shapes")]
    public static PlotModel PointAnnotationShapes()
    {
        var model = new PlotModel { Title = "PointAnnotation - shapes" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Maximum = 120 });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

        // filled
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 20,
                    Y = 60,
                    Text = "Circle",
                    Shape = MarkerType.Circle,
                    Fill = PlotterColors.LightGray,
                    Stroke = PlotterColors.DarkGray,
                    StrokeThickness = 1
                });
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 40,
                    Y = 60,
                    Text = "Square",
                    Shape = MarkerType.Square,
                    Fill = PlotterColors.LightBlue,
                    Stroke = PlotterColors.DarkBlue,
                    StrokeThickness = 1
                });
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 60,
                    Y = 60,
                    Text = "Triangle",
                    Shape = MarkerType.Triangle,
                    Fill = PlotterColors.IndianRed,
                    Stroke = PlotterColors.Black,
                    StrokeThickness = 1
                });
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 80,
                    Y = 60,
                    Text = "Diamond",
                    Shape = MarkerType.Diamond,
                    Fill = PlotterColors.ForestGreen,
                    Stroke = PlotterColors.Black,
                    StrokeThickness = 1
                });
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 100,
                    Y = 60,
                    Text = "Custom",
                    Shape = MarkerType.Custom,
                    CustomOutline =
                        new[]
                            {
                                new ScreenPoint(-1, -1), new ScreenPoint(1, 1), new ScreenPoint(-1, 1),
                                new ScreenPoint(1, -1)
                            },
                    Stroke = PlotterColors.Black,
                    Fill = PlotterColors.CadetBlue,
                    StrokeThickness = 1
                });

        // not filled
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 20,
                    Y = 40,
                    Text = "Cross",
                    Shape = MarkerType.Cross,
                    Stroke = PlotterColors.IndianRed,
                    StrokeThickness = 1
                });
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 40,
                    Y = 40,
                    Text = "Plus",
                    Shape = MarkerType.Plus,
                    Stroke = PlotterColors.Navy,
                    StrokeThickness = 1
                });
        model.Annotations.Add(
            new PointAnnotation
                {
                    X = 60,
                    Y = 40,
                    Text = "Star",
                    Shape = MarkerType.Star,
                    Stroke = PlotterColors.DarkOliveGreen,
                    StrokeThickness = 1
                });

        return model;
    }

    [Example("PointAnnotation - text alignments")]
    public static PlotModel PointAnnotationTextAlignment()
    {
        var model = new PlotModel { Title = "PointAnnotation - text alignments" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = -50, Maximum = 50 });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -50, Maximum = 50 });

        for (var ha = -1; ha <= 1; ha++)
        {
            var h = (HorizontalAlignment)ha;
            for (var va = -1; va <= 1; va++)
            {
                var v = (VerticalAlignment)va;
                model.Annotations.Add(
                    new PointAnnotation
                        {
                            X = ha * 20,
                            Y = va * 20,
                            Size = 10,
                            Text = h + "," + v,
                            TextHorizontalAlignment = h,
                            TextVerticalAlignment = v
                        });
            }
        }

        return model;
    }
}
