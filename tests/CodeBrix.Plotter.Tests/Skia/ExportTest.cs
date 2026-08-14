// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportTest.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using SilverAssertions;

namespace CodeBrix.Plotter.Tests.Skia; //was previously: OxyPlot.SkiaSharp.Tests;

public static class ExportTest
{
    public static void ExportExamples_CheckThatAllFilesExist(IEnumerable<ExampleInfo> examples, IExporter exporter, string directory, string extension)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        foreach (var example in examples)
        {
            void ExportModelAndCheckFileExists(PlotModel model, string fileName)
            {
                if (model == null)
                {
                    return;
                }

                var path = Path.Combine(directory, FileNameUtilities.CreateValidFileName(fileName, extension));
                using (var s = File.Create(path))
                {
                    exporter.Export(model, s);
                }

                File.Exists(path).Should().BeTrue();
            }

            ExportModelAndCheckFileExists(example.PlotModel, $"{example.Category} - {example.Title}");
        }
    }
}
