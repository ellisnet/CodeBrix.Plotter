// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeGeneratorTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the CodeGenerator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using SilverAssertions;
using Xunit;
namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Provides unit tests for the <see cref="CodeGenerator" />.
/// </summary>
public class CodeGeneratorTests
{
    /// <summary>
    /// Test that code can be generated for all examples in the example library.
    /// </summary>
    [Theory]
    [MemberData(nameof(ExampleTestData.AllForAutomatedTest), MemberType = typeof(ExampleTestData))]
    public void GenerateCodeForAllExamplesInExampleLibrary(string exampleCategory, string exampleTitle)
    {
        var ei = ExampleTestData.Get(exampleCategory, exampleTitle);
        void CheckCode(PlotModel model)
        {
            if (model == null)
            {
                return;
            }

            var code = model.ToCode();
            code.Should().NotBeNull();
        }

        CheckCode(ei.PlotModel);
    }

    /// <summary>
    /// Test that code generation properly handles escapes sequences in strings
    /// </summary>
    [Fact]
    public void TestCodeGeneratorStringExtensions()
    {
        var plot = new PlotModel();

        // a custom tracker format string that shows x axis values in seconds as "m:ss.ff"
        var series = new Series.LineSeries()
        {
            TrackerFormatString = "{1}: {2:m\\:ss\\.ff}\n{3}: {4:0.##}",
        };
        plot.Series.Add(series);

        plot.ToCode().Should().Contain(@"{1}: {2:m\\:ss\\.ff}\n{3}: {4:0.##}");
    }
}
