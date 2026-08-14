// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PngExporterTests.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using CodeBrix.Plotter.Annotations;
using CodeBrix.Plotter.Series;
using CodeBrix.Plotter.Skia;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests.Skia; //was previously: OxyPlot.SkiaSharp.Tests;

public class PngExporterTests
{
    private const string PNG_FOLDER = "PNG";
    private string outputDirectory;

    public PngExporterTests()
    {
        this.outputDirectory = Path.Combine(AppContext.BaseDirectory, PNG_FOLDER);
        Directory.CreateDirectory(this.outputDirectory);
    }

    [Fact]
    public void Export_SomeExamplesInExampleLibrary_CheckThatAllFilesExist()
    {
        var exporter = new PngExporter { Width = 400, Height = 300 };
        var directory = Path.Combine(this.outputDirectory, "CodeBrix.Plotter.Tests.ExampleLibrary");
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetFirstExampleOfEachCategoryForAutomatedTest(), exporter, directory, ".png");
        exporter.Width = 800;
        exporter.Height = 600;
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetRenderingCapabilitiesForAutomatedTest(), exporter, directory, ".png");
    }

    [Fact]
    public void ExportToStream()
    {
        var plotModel = CreateTestModel1();
        var exporter = new PngExporter { Width = 400, Height = 300 };
        var stream = new MemoryStream();
        exporter.Export(plotModel, stream);

        (stream.Length > 0).Should().BeTrue();
    }

    [Fact]
    public void ExportToFile()
    {
        var plotModel = CreateTestModel1();
        var fileName = Path.Combine(this.outputDirectory, "Plot1.png");
        PngExporter.Export(plotModel, fileName, 400, 300);

        File.Exists(fileName).Should().BeTrue();
    }

    [Fact]
    public void ExportWithDifferentBackground()
    {
        var plotModel = CreateTestModel1();
        plotModel.Background = PlotterColors.Yellow;
        var fileName = Path.Combine(this.outputDirectory, "Background_Yellow.png");
        var exporter = new PngExporter { Width = 400, Height = 300 };
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(plotModel, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    [Theory]
    [InlineData(0.75)]
    [InlineData(1.2)]
    [InlineData(2)]
    [InlineData(3.1415)]
    public void ExportWithResolution(double factor)
    {
        var resolution = (int)(96 * factor);
        var plotModel = CreateTestModel1();
        var directory = Path.Combine(this.outputDirectory, "Resolution");
        Directory.CreateDirectory(directory);

        var fileName = Path.Combine(directory, $"Resolution{resolution}.png");
        var exporter = new PngExporter { Width = (int)(400 * factor), Height = (int)(300 * factor), Dpi = resolution };

        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(plotModel, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExportUseTextShapingAlignment(bool useTextShaping)
    {
        var model = RenderingCapabilities.DrawTextAlignment();
        model.Background = PlotterColors.White;
        var fileName = Path.Combine(this.outputDirectory, $"Alignment, UseTextShaping={useTextShaping}.png");
        var exporter = new PngExporter { Width = 450, Height = 200, UseTextShaping = useTextShaping };
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(model, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExportUseTextShapingMeasurements(bool useTextShaping)
    {
        var model = RenderingCapabilities.DrawTextWithMetrics("TeffVAll", "Arial", 60, double.NaN, double.NaN, 105, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, "");

        model.Background = PlotterColors.White;
        var fileName = Path.Combine(this.outputDirectory, $"Measurements, UseTextShaping={useTextShaping}.png");
        var exporter = new PngExporter { Width = 450, Height = 150, UseTextShaping = useTextShaping };
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(model, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PlotBackgroundImageTest(bool interpolate)
    {
        // this is a test of the DrawImage function; don't add pointless backgrounds to your plots

        var plotModel = CreateTestModel1();

        var pixelData = new PlotterColor[5, 5];
        for (int i = 0; i < pixelData.GetLength(0); i++)
        {
            for (int j = 0; j < pixelData.GetLength(1); j++)
            {
                pixelData[i, j] = PlotterColor.FromArgb(255, 128, (byte)((i * 255) / pixelData.GetLength(0)), (byte)((j * 255) / pixelData.GetLength(1)));
            }
        }

        var oxyImage = PlotterImage.Create(pixelData, ImageFormat.Png);
        var imageAnnotation = new ImageAnnotation()
        {
            ImageSource = oxyImage,
            X = new PlotLength(-0.0, PlotLengthUnit.RelativeToPlotArea),
            Y = new PlotLength(-0.0, PlotLengthUnit.RelativeToPlotArea),
            Width = new PlotLength(1.0, PlotLengthUnit.RelativeToPlotArea),
            Height = new PlotLength(1.0, PlotLengthUnit.RelativeToPlotArea),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Interpolate = interpolate
        };
        plotModel.Annotations.Add(imageAnnotation);

        var fileName = Path.Combine(this.outputDirectory, $"PlotBackground{(interpolate ? "Interpolated" : "Pixelated")}.png");
        var exporter = new PngExporter { Width = 400, Height = 300 };
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(plotModel, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LargeImageTest(bool interpolate)
    {
        // this is a test of the DrawImage function; don't add pointless backgrounds to your plots

        var plotModel = CreateTestModel1();

        var pixelData = new PlotterColor[5, 5];
        for (int i = 0; i < pixelData.GetLength(0); i++)
        {
            for (int j = 0; j < pixelData.GetLength(1); j++)
            {
                pixelData[i, j] = PlotterColor.FromArgb(255, 128, (byte)((i * 255) / pixelData.GetLength(0)), (byte)((j * 255) / pixelData.GetLength(1)));
            }
        }

        var oxyImage = PlotterImage.Create(pixelData, ImageFormat.Png);
        var imageAnnotation = new ImageAnnotation()
        {
            ImageSource = oxyImage,
            X = new PlotLength(-1, PlotLengthUnit.RelativeToViewport),
            Y = new PlotLength(-1, PlotLengthUnit.RelativeToViewport),
            Width = new PlotLength(3, PlotLengthUnit.RelativeToViewport),
            Height = new PlotLength(3, PlotLengthUnit.RelativeToViewport),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Interpolate = interpolate
        };
        plotModel.Annotations.Add(imageAnnotation);

        var fileName = Path.Combine(this.outputDirectory, $"LargeImage{(interpolate ? "Interpolated" : "Pixelated")}.png");
        var exporter = new PngExporter { Width = 400, Height = 300 };
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(plotModel, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    private static PlotModel CreateTestModel1()
    {
        var model = new PlotModel { Title = "Test 1" };
        model.Series.Add(new FunctionSeries(Math.Sin, 0, Math.PI * 8, 200, "sin(x)"));
        return model;
    }
}
