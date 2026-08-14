// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SvgWriterTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class SvgWriterTests
{
    [Fact]
    public void WriteEllipse()
    {
        const string FileName = "SvgWriterTests_WriteEllipse.svg";
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 200, 200))
            {
                w.WriteEllipse(
                    10,
                    10,
                    100,
                    60,
                    w.CreateStyle(PlotterColors.Blue, PlotterColors.Black, 2, LineStyle.Solid.GetDashArray()),
                    EdgeRenderingMode.Adaptive);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WriteLine()
    {
        const string FileName = "SvgWriterTests_WriteLine.svg";
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 200, 200))
            {
                w.WriteLine(
                    new ScreenPoint(10, 10),
                    new ScreenPoint(150, 80),
                    w.CreateStyle(PlotterColors.Undefined, PlotterColors.Black, 2, LineStyle.Solid.GetDashArray()),
                    EdgeRenderingMode.Adaptive);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WritePolygon()
    {
        const string FileName = "SvgWriterTests_WritePolygon.svg";
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 200, 200))
            {
                w.WritePolygon(
                    CreatePointList(),
                    w.CreateStyle(PlotterColors.Blue, PlotterColors.Black, 2, LineStyleHelper.GetDashArray(LineStyle.Solid)),
                    EdgeRenderingMode.Adaptive);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WritePolyline()
    {
        const string FileName = "SvgWriterTests_WritePolyLine.svg";
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 200, 200))
            {
                w.WritePolyline(
                    CreatePointList(),
                    w.CreateStyle(PlotterColors.Blue, PlotterColors.Black, 2, LineStyleHelper.GetDashArray(LineStyle.Solid)),
                    EdgeRenderingMode.Adaptive);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WriteRectangle()
    {
        const string FileName = "SvgWriterTests_WriteRectangle.svg";
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 200, 200))
            {
                w.WriteRectangle(
                    10,
                    20,
                    150,
                    80,
                    w.CreateStyle(PlotterColors.Green, PlotterColors.Black, 2, LineStyle.Solid.GetDashArray()),
                    EdgeRenderingMode.Adaptive);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WriteText()
    {
        const string FileName = "SvgWriterTests_WriteText.svg";
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 200, 200))
            {
                w.WriteText(new ScreenPoint(10, 10), "Hello world!", PlotterColors.Black, "Arial", 20, FontWeights.Bold);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WriteClippedEllipse()
    {
        const string FileName = "SvgWriterTests_WriteClippedEllipse.svg";
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 200, 200))
            {
                w.WriteRectangle(5, 5, 95, 45, w.CreateStyle(PlotterColors.LightGreen, PlotterColors.Undefined, 0), EdgeRenderingMode.Adaptive);
                w.BeginClip(5, 5, 95, 45);
                w.WriteEllipse(10, 10, 100, 60, w.CreateStyle(PlotterColors.Blue, PlotterColors.Black, 2), EdgeRenderingMode.Adaptive);
                w.EndClip();
                w.Flush();
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WriteImage()
    {
        const string FileName = "SvgWriterTests_WriteImage.svg";
        var image = CreateImage();
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 500, 300))
            {
                for (int y = 0; y <= 200; y += 20)
                {
                    w.WriteLine(new ScreenPoint(0, y), new ScreenPoint(400, y), w.CreateStyle(PlotterColors.Undefined, PlotterColors.Black, 1), EdgeRenderingMode.Adaptive);
                }

                w.WriteImage(0, 0, 400, 200, image);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Fact]
    public void WriteClippedImage()
    {
        const string FileName = "SvgWriterTests_WriteClippedImage.svg";
        var image = CreateImage();
        using (var s = File.Create(FileName))
        {
            using (var w = new SvgWriter(s, 400, 400))
            {
                w.WriteImage(0, 0, 400, 200, image);
                w.WriteRectangle(100, 50, 200, 100, w.CreateStyle(PlotterColors.Undefined, PlotterColors.Black, 1), EdgeRenderingMode.Adaptive);
                w.WriteImage(1, 0.5, 2, 1, 100, 250, 200, 100, image);
            }
        }

        SvgAssert.IsValidFile(FileName);
    }

    private static PlotterImage CreateImage()
    {
        var data = new PlotterColor[4, 2];
        data[0, 0] = PlotterColors.Blue;
        data[1, 0] = PlotterColors.Green;
        data[2, 0] = PlotterColors.Red;
        data[3, 0] = PlotterColors.White;
        data[0, 1] = PlotterColors.Transparent;
        data[3, 1] = PlotterColor.FromAColor(127, PlotterColors.Yellow);
        data[1, 1] = PlotterColor.FromAColor(127, PlotterColors.Orange);
        data[2, 1] = PlotterColor.FromAColor(127, PlotterColors.Pink);
        return PlotterImage.Create(data, ImageFormat.Png);
    }

    private static IEnumerable<ScreenPoint> CreatePointList()
    {
        return new List<ScreenPoint>
            {
                new ScreenPoint(10, 20),
                new ScreenPoint(80, 30),
                new ScreenPoint(140, 120),
                new ScreenPoint(30, 140)
            };
    }
}
