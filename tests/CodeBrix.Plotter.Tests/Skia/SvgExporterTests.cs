// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SvgExporterTests.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using CodeBrix.Plotter.Skia;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using Xunit;

namespace CodeBrix.Plotter.Tests.Skia; //was previously: OxyPlot.SkiaSharp.Tests;

public class SvgExporterTests
{
    private const string SVG_FOLDER = "SVG";
    private string outputDirectory;

    [Fact]
    public void Export_SomeExamplesInExampleLibrary_CheckThatAllFilesExist()
    {
        var exporter = new SvgExporter { Width = 1000, Height = 750 };
        var directory = Path.Combine(this.outputDirectory, "CodeBrix.Plotter.Tests.ExampleLibrary");
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetFirstExampleOfEachCategoryForAutomatedTest(), exporter, directory, ".svg");
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetRenderingCapabilitiesForAutomatedTest(), exporter, directory, ".svg");
    }

    [Fact]
    public void TestMultilineAlignment()
    {
        var exporter = new SvgExporter { Width = 1000, Height = 750 };
        var model = RenderingCapabilities.DrawMultilineTextAlignmentRotation();
        using var stream = File.Create(Path.Combine(this.outputDirectory, "Multiline-Alignment.svg"));
        exporter.Export(model, stream);
    }

    public SvgExporterTests()
    {
        this.outputDirectory = Path.Combine(AppContext.BaseDirectory, SVG_FOLDER);
        Directory.CreateDirectory(this.outputDirectory);
    }

    [Fact]
    public void BackgroundColor()
    {
        var model = ShowCases.CreateNormalDistributionModel();
        model.Background = PlotterColors.AliceBlue;
        var exporter = new SvgExporter { Width = 1000, Height = 750 };
        using var stream = File.Create(Path.Combine(this.outputDirectory, "Background.svg"));
        exporter.Export(model, stream);
    }

}
