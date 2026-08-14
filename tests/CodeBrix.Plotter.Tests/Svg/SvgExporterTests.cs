// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SvgExporterTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CodeBrix.Plotter.Series;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class SvgExporterTests
{
    const string DestinationDirectory = "SvgExporterTests_ExampleLibrary";

    public SvgExporterTests()
    {
        Directory.CreateDirectory(DestinationDirectory);
    }

    [Fact]
    public void ExportToString_TestPlot_ValidSvgDocument()
    {
        var plotModel = new PlotModel { Title = "Test plot" };
        plotModel.Series.Add(new FunctionSeries(Math.Sin, 0, Math.PI * 8, 200, "Math.Sin"));
        var svg = SvgExporter.ExportToString(plotModel, 800, 500, true);
        SvgAssert.IsValidDocument(svg);
    }

    [Fact]
    public void ExportToString_TestPlot_ValidSvgString()
    {
        var plotModel = new PlotModel { Title = "Test plot" };
        plotModel.Series.Add(new FunctionSeries(Math.Sin, 0, Math.PI * 8, 200, "Math.Sin"));
        var svg = SvgExporter.ExportToString(plotModel, 800, 500, false);
        SvgAssert.IsValidElement(svg);
    }

    [Fact]
    public void Export_TestPlot_ValidSvgString()
    {
        var plotModel = new PlotModel { Title = "Test plot" };
        const string FileName = "SvgExporterTests_Plot1.svg";
        plotModel.Series.Add(new FunctionSeries(Math.Sin, 0, Math.PI * 8, 200, "Math.Sin"));
        using (var s = File.Create(FileName))
        {
            SvgExporter.Export(plotModel, s, 800, 500, true);
        }

        SvgAssert.IsValidFile(FileName);
    }

    [Theory]
    [MemberData(nameof(ExampleTestData.AllForAutomatedTest), MemberType = typeof(ExampleTestData))]
    public void Export_AllExamplesInExampleLibrary_CheckThatAllFilesExist(string exampleCategory, string exampleTitle)
    {
        var example = ExampleTestData.Get(exampleCategory, exampleTitle);
        void ExportModelAndCheckFileExists(PlotModel model, string fileName)
        {
            if (model == null)
            {
                return;
            }

            var path = Path.Combine(DestinationDirectory, FileNameUtilities.CreateValidFileName(fileName, ".svg"));
            using (var s = File.Create(path))
            {
                SvgExporter.Export(model, s, 800, 500, true);
            }

            File.Exists(path).Should().BeTrue();
        }

        ExportModelAndCheckFileExists(example.PlotModel, $"{example.Category} - {example.Title}");

        if (example.IsTransposable)
        {
            ExportModelAndCheckFileExists(example.GetModel(ExampleFlags.Transpose), $"{example.Category} - {example.Title} - Transposed");
        }

        if (example.IsReversible)
        {
            ExportModelAndCheckFileExists(example.GetModel(ExampleFlags.Reverse), $"{example.Category} - {example.Title} - Reversed");
        }
    }
}
