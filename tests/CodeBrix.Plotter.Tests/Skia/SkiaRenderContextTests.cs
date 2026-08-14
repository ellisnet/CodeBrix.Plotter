// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkiaRenderContextTests.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Series;
using CodeBrix.Plotter.Skia;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests.Skia; //was previously: OxyPlot.SkiaSharp.Tests;

using global::SkiaSharp;
using System.IO;

public class SkiaRenderContextTests
{
    private string outputDirectory;

    public SkiaRenderContextTests()
    {
        this.outputDirectory = Path.Combine(AppContext.BaseDirectory, "SkiaRenderContextTests");
        Directory.CreateDirectory(this.outputDirectory);
    }

    [Fact]
    public void TestDisposal()
    {
        var plotModel = (IPlotModel)ShowCases.CreateNormalDistributionModel();
        ((PlotModel)plotModel).DefaultFont = "Arial";
        plotModel.Update(true);

        string ExportToBitmap(string fileName, SkiaRenderContext context)
        {
            using var bitmap = new SKBitmap(800, 600);
            using var canvas = new SKCanvas(bitmap);

            context.SkCanvas = canvas;
            canvas.Clear(SKColors.White);
            plotModel.Render(context, new PlotterRect(0, 0, 800, 600));

            var path = Path.Combine(this.outputDirectory, fileName + ".png");
            using var fileStream = File.OpenWrite(path);
            using var skStream = new SKManagedWStream(fileStream);
            bitmap.Encode(skStream, SKEncodedImageFormat.Png, 0);

            return path;
        }

        using var renderContext = new SkiaRenderContext() { RenderTarget = RenderTarget.PixelGraphic };
        var path1 = ExportToBitmap("export1", renderContext);

        string path2;
        using (var renderContext2 = new SkiaRenderContext() { RenderTarget = RenderTarget.PixelGraphic })
        {
            path2 = ExportToBitmap("export2", renderContext2);
        }

        var path3 = ExportToBitmap("export3", renderContext);

        PngAssert.AreEqual(path1, path2, null, null);
        PngAssert.AreEqual(path1, path3, null, null);
    }

    /// <summary>
    /// An assigned <see cref="SkiaRenderContext.TypefaceResolver"/> is consulted for the font
    /// family and weight instead of the system font lookup.
    /// </summary>
    [Fact]
    public void TypefaceResolver_is_consulted_instead_of_the_system_lookup()
    {
        //Arrange
        var calls = new List<(string FontFamily, double FontWeight)>();
        using var typeface = LoadPackagedTypeface("Roboto-Regular.ttf");
        using var context = new SkiaRenderContext();
        context.TypefaceResolver = (fontFamily, fontWeight) =>
        {
            calls.Add((fontFamily, fontWeight));
            return typeface;
        };

        //Act
        var size = context.MeasureText("Hello plot", "No Such Family", 12, FontWeights.Normal);

        //Assert
        calls.Count.Should().Be(1);
        calls[0].FontFamily.Should().Be("No Such Family");
        calls[0].FontWeight.Should().Be(FontWeights.Normal);
        (size.Width > 0).Should().BeTrue();
        (size.Height > 0).Should().BeTrue();
    }

    /// <summary>
    /// Resolved typefaces are cached per font family and weight, so the resolver is consulted
    /// once per distinct pair.
    /// </summary>
    [Fact]
    public void TypefaceResolver_result_is_cached_per_family_and_weight()
    {
        //Arrange
        var callCount = 0;
        using var typeface = LoadPackagedTypeface("Roboto-Regular.ttf");
        using var context = new SkiaRenderContext();
        context.TypefaceResolver = (fontFamily, fontWeight) =>
        {
            callCount++;
            return typeface;
        };

        //Act
        context.MeasureText("Once", "Roboto", 12, FontWeights.Normal);
        context.MeasureText("Twice, from the cache", "Roboto", 16, FontWeights.Normal);
        context.MeasureText("A second weight resolves again", "Roboto", 12, FontWeights.Bold);

        //Assert
        callCount.Should().Be(2);
    }

    /// <summary>
    /// A resolver returning <c>null</c> falls back to the default typeface - never to the
    /// system font lookup - and text still measures.
    /// </summary>
    [Fact]
    public void TypefaceResolver_null_result_falls_back_to_the_default_typeface()
    {
        //Arrange
        var callCount = 0;
        using var context = new SkiaRenderContext();
        context.TypefaceResolver = (fontFamily, fontWeight) =>
        {
            callCount++;
            return null;
        };

        //Act
        var size = context.MeasureText("Unresolvable", "No Such Family", 12, FontWeights.Normal);

        //Assert
        callCount.Should().Be(1);
        (size.Width > 0).Should().BeTrue();
    }

    /// <summary>
    /// Assigning a different resolver clears the typeface cache, so the new resolver is
    /// consulted for families the old one already resolved.
    /// </summary>
    [Fact]
    public void TypefaceResolver_change_clears_the_typeface_cache()
    {
        //Arrange
        var firstCalls = 0;
        var secondCalls = 0;
        using var typeface = LoadPackagedTypeface("Roboto-Regular.ttf");
        using var context = new SkiaRenderContext();
        context.TypefaceResolver = (fontFamily, fontWeight) =>
        {
            firstCalls++;
            return typeface;
        };
        context.MeasureText("Resolved by the first resolver", "Roboto", 12, FontWeights.Normal);

        //Act
        context.TypefaceResolver = (fontFamily, fontWeight) =>
        {
            secondCalls++;
            return typeface;
        };
        context.MeasureText("Resolved again by the second", "Roboto", 12, FontWeights.Normal);

        //Assert
        firstCalls.Should().Be(1);
        secondCalls.Should().Be(1);
    }

    /// <summary>
    /// Re-assigning the resolver already in place keeps the typeface cache.
    /// </summary>
    [Fact]
    public void TypefaceResolver_reassigning_the_same_resolver_keeps_the_cache()
    {
        //Arrange
        var callCount = 0;
        using var typeface = LoadPackagedTypeface("Roboto-Regular.ttf");
        using var context = new SkiaRenderContext();
        TypefaceResolver resolver = (fontFamily, fontWeight) =>
        {
            callCount++;
            return typeface;
        };
        context.TypefaceResolver = resolver;
        context.MeasureText("Fill the cache", "Roboto", 12, FontWeights.Normal);

        //Act
        context.TypefaceResolver = resolver;
        context.MeasureText("Still cached", "Roboto", 12, FontWeights.Normal);

        //Assert
        callCount.Should().Be(1);
    }

    /// <summary>
    /// Disposing the render context leaves resolver-supplied typefaces alive - they stay owned
    /// by the resolver, unlike system-resolved typefaces which the context disposes.
    /// </summary>
    [Fact]
    public void Dispose_leaves_resolver_supplied_typefaces_undisposed()
    {
        //Arrange
        using var typeface = LoadPackagedTypeface("Roboto-Regular.ttf");
        var context = new SkiaRenderContext();
        context.TypefaceResolver = (fontFamily, fontWeight) => typeface;
        context.MeasureText("Cache the typeface", "Roboto", 12, FontWeights.Normal);

        //Act
        context.Dispose();

        //Assert
        typeface.Handle.Should().NotBe(IntPtr.Zero);
        typeface.FamilyName.Should().Be("Roboto");
    }

    /// <summary>
    /// A full plot model - title, axes, series - renders through a resolver, including the
    /// bold weight the default title font asks for.
    /// </summary>
    [Fact]
    public void PlotModel_renders_through_a_typeface_resolver()
    {
        //Arrange
        using var regular = LoadPackagedTypeface("Roboto-Regular.ttf");
        using var bold = LoadPackagedTypeface("Roboto-Bold.ttf");
        var resolvedWeights = new List<double>();

        var model = new PlotModel { Title = "Resolved fonts" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });
        var series = new LineSeries();
        series.Points.Add(new DataPoint(0, 0));
        series.Points.Add(new DataPoint(1, 1));
        model.Series.Add(series);

        using var context = new SkiaRenderContext { RenderTarget = RenderTarget.PixelGraphic };
        context.TypefaceResolver = (fontFamily, fontWeight) =>
        {
            resolvedWeights.Add(fontWeight);
            return fontWeight >= FontWeights.Bold ? bold : regular;
        };

        using var bitmap = new SKBitmap(400, 300);
        using var canvas = new SKCanvas(bitmap);
        context.SkCanvas = canvas;
        canvas.Clear(SKColors.White);

        //Act
        var plotModel = (IPlotModel)model;
        plotModel.Update(true);
        plotModel.Render(context, new PlotterRect(0, 0, 400, 300));

        //Assert
        (resolvedWeights.Count > 0).Should().BeTrue();
        resolvedWeights.Contains(FontWeights.Normal).Should().BeTrue();
        resolvedWeights.Contains(FontWeights.Bold).Should().BeTrue();
    }

    /// <summary>
    /// Loads a typeface from the font files the test project copies out of the
    /// CodeBrix.Platform.Fonts.Roboto package.
    /// </summary>
    /// <param name="fileName">The font file name.</param>
    /// <returns>The typeface.</returns>
    private static SKTypeface LoadPackagedTypeface(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fonts", fileName);
        File.Exists(path).Should().BeTrue();
        return SKTypeface.FromFile(path);
    }
}
