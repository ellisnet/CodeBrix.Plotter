// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdfExporterTests.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests.Skia; //was previously: OxyPlot.SkiaSharp.Tests;

using global::SkiaSharp;

using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Skia;

public class PdfExporterTests
{
    private const string PDF_FOLDER = "PDF";
    private string outputDirectory;

    public PdfExporterTests()
    {
        this.outputDirectory = Path.Combine(AppContext.BaseDirectory, PDF_FOLDER);
        Directory.CreateDirectory(this.outputDirectory);
    }

    [Fact]
    public void Export_SomeExamplesInExampleLibrary_CheckThatAllFilesExist()
    {
        var exporter = new PdfExporter { Width = 297 / 25.4f * 72, Height = 210 / 25.4f * 72 };
        var directory = Path.Combine(this.outputDirectory, "CodeBrix.Plotter.Tests.ExampleLibrary");
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetFirstExampleOfEachCategoryForAutomatedTest(), exporter, directory, ".pdf");
        ExportTest.ExportExamples_CheckThatAllFilesExist(Examples.GetRenderingCapabilitiesForAutomatedTest(), exporter, directory, ".pdf");
    }

    [Fact]
    public void Pdf_Export_Unicode()
    {
        var model = new PlotModel { Title = "Unicode support ☺", TitleFont = "Arial" };
        model.Axes.Add(new LinearAxis { Title = "λ", Position = AxisPosition.Bottom });
        model.Axes.Add(new LinearAxis { Title = "Ж", Position = AxisPosition.Left });
        var exporter = new PdfExporter { Width = 400, Height = 400 };
        var fileName = Path.Combine(this.outputDirectory, "unicode.pdf");
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(model, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }

    [Fact]
    public void Pdf_Export_CJK()
    {
        var fontManager = SKFontManager.Default;

        var chineseChar = 0x9019; // 這
        var chineseTypeface = fontManager.MatchCharacter(chineseChar);

        var koreanChar = 0xC790; // 자
        var koreanTypeface = fontManager.MatchCharacter(koreanChar);

        var japaneseChar = 0x3053; // こ
        var japaneseTypeface = fontManager.MatchCharacter(japaneseChar);

        var model = new PlotModel { Title = "Chinese, Japanese and Korean characters" };
        model.Axes.Add(new LinearAxis { Title = $"Korean (used Font: {koreanTypeface.FamilyName}): 자동 번역 된 텍스트입니다.", Position = AxisPosition.Bottom, TitleFont = koreanTypeface.FamilyName });
        model.Axes.Add(new LinearAxis { Title = $"Japanese (used Font: {japaneseTypeface.FamilyName}): これは自動翻訳されたテキストです", Position = AxisPosition.Left, TitleFont = japaneseTypeface.FamilyName });
        model.Axes.Add(new LinearAxis { Title = $"Chinese (used Font: {chineseTypeface.FamilyName}): 這是一些自動翻譯的文字", Position = AxisPosition.Right, TitleFont = chineseTypeface.FamilyName });
        var exporter = new PdfExporter { Width = 400, Height = 400 };
        var fileName = Path.Combine(this.outputDirectory, "cjk.pdf");
        using (var stream = File.OpenWrite(fileName))
        {
            exporter.Export(model, stream);
        }

        File.Exists(fileName).Should().BeTrue();
    }
}
