// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAxisExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Axes;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("Custom axes")]
[Tags("Axes")]
public static class CustomAxisExamples
{
    public class ArrowAxis : LinearAxis
    {
        public override void Render(IRenderContext rc, int pass)
        {
            base.Render(rc, pass);
            var points = new List<ScreenPoint>();
            if (this.IsHorizontal())
            {
                var xmax = this.Transform(this.ActualMaximum);
                points.Add(new ScreenPoint(xmax + 4, this.PlotModel.PlotArea.Bottom - 4));
                points.Add(new ScreenPoint(xmax + 18, this.PlotModel.PlotArea.Bottom));
                points.Add(new ScreenPoint(xmax + 4, this.PlotModel.PlotArea.Bottom + 4));
                //// etc.
            }
            else
            {
                var ymax = this.Transform(this.ActualMaximum);
                points.Add(new ScreenPoint(this.PlotModel.PlotArea.Left - 4, ymax - 4));
                points.Add(new ScreenPoint(this.PlotModel.PlotArea.Left, ymax - 18));
                points.Add(new ScreenPoint(this.PlotModel.PlotArea.Left + 4, ymax - 4));
                //// etc.
            }

            rc.DrawPolygon(points, PlotterColors.Black, PlotterColors.Undefined, 0, this.EdgeRenderingMode);
        }
    }

    [Example("ArrowAxis")]
    public static PlotModel CustomArrowAxis()
    {
        var model = new PlotModel { PlotAreaBorderThickness = new PlotterThickness(0), PlotMargins = new PlotterThickness(60, 60, 60, 60) };
        model.Axes.Add(new ArrowAxis { Position = AxisPosition.Bottom, AxislineStyle = LineStyle.Solid });
        model.Axes.Add(new ArrowAxis { Position = AxisPosition.Left, AxislineStyle = LineStyle.Solid });
        return model;
    }
}
