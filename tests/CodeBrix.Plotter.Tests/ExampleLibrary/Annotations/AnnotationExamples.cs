// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Annotations;
using CodeBrix.Plotter.Axes;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("Annotations")]
[Tags("Annotations")]
public static class AnnotationExamples
{
    [Example("Tool tips")]
    public static PlotModel ToolTips()
    {
        var model = new PlotModel { Title = "Tool tips" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
        model.Annotations.Add(new LineAnnotation { Slope = 0.1, Intercept = 1, Text = "LineAnnotation", ToolTip = "This is a tool tip for the LineAnnotation" });
        model.Annotations.Add(new RectangleAnnotation { MinimumX = 20, MaximumX = 70, MinimumY = 10, MaximumY = 40, TextRotation = 10, Text = "RectangleAnnotation", ToolTip = "This is a tooltip for the RectangleAnnotation", Fill = PlotterColor.FromAColor(99, PlotterColors.Blue), Stroke = PlotterColors.Black, StrokeThickness = 2 });
        model.Annotations.Add(new EllipseAnnotation { X = 20, Y = 60, Width = 20, Height = 15, Text = "EllipseAnnotation", ToolTip = "This is a tool tip for the EllipseAnnotation", TextRotation = 10, Fill = PlotterColor.FromAColor(99, PlotterColors.Green), Stroke = PlotterColors.Black, StrokeThickness = 2 });
        model.Annotations.Add(new PointAnnotation { X = 50, Y = 50, Text = "P1", ToolTip = "This is a tool tip for the PointAnnotation" });
        model.Annotations.Add(new ArrowAnnotation { StartPoint = new DataPoint(8, 4), EndPoint = new DataPoint(0, 0), Color = PlotterColors.Green, Text = "ArrowAnnotation", ToolTip = "This is a tool tip for the ArrowAnnotation" });
        model.Annotations.Add(new TextAnnotation { TextPosition = new DataPoint(60, 60), Text = "TextAnnotation", ToolTip = "This is a tool tip for the TextAnnotation" });
        return model;
    }
}
