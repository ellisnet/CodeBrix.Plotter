// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JpegExporterTests.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using CodeBrix.Plotter.Series;
using CodeBrix.Plotter.Skia;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests.Skia; //was previously: OxyPlot.SkiaSharp.Tests;

public class JpegExporterTests
{
    private const string JPEG_FOLDER = "JPEG";
    private string outputDirectory;

    public JpegExporterTests()
    {
        this.outputDirectory = Path.Combine(AppContext.BaseDirectory, JPEG_FOLDER);
        Directory.CreateDirectory(this.outputDirectory);
    }

    [Fact]
    public void Export_SomeExamplesInExampleLibrary_CheckThatAllFilesExist()
    {
        var exporter = new JpegExporter { Width = 400, Height = 300 };
        var directory = Path.Combine(this.outputDirectory, "CodeBrix.Plotter.Tests.ExampleLibrary");
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetFirstExampleOfEachCategoryForAutomatedTest(), exporter, directory, ".jpg");
        exporter.Width = 800;
        exporter.Height = 600;
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetRenderingCapabilitiesForAutomatedTest(), exporter, directory, ".jpg");
    }

    [Fact]
    public void ExportWithDifferentBackground()
    {
        var plotModel = CreateTestModel1();
        plotModel.Background = PlotterColors.Yellow;
        var exporter = new PngExporter { Width = 400, Height = 300 };
        var fileName = Path.Combine(this.outputDirectory, "BackgroundYellow.jpg");
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(plotModel, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(90)]
    [InlineData(100)]
    public void ExportWithQuality(int quality)
    {
        var plotModel = CreateTestModel1();
        var directory = Path.Combine(this.outputDirectory, "Quality");
        Directory.CreateDirectory(directory);
        var exporter = new JpegExporter { Width = 400, Height = 300, Quality = quality };

        var fileName = Path.Combine(directory, $"Quality_{quality}.jpg");
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
