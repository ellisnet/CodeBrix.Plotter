// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterPaletteTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class PlotterPaletteTests
{
    [Fact]
    public void Interpolate()
    {
        var palette = PlotterPalette.Interpolate(5, PlotterColors.Blue, PlotterColors.White, PlotterColors.Red);
        palette.Colors.Count.Should().Be(5);
        palette.Colors[0].Should().Be(PlotterColors.Blue);
        palette.Colors[1].Should().Be(PlotterColor.FromRgb(127, 127, 255));
        palette.Colors[2].Should().Be(PlotterColors.White);
        palette.Colors[3].Should().Be(PlotterColor.FromRgb(255, 127, 127));
        palette.Colors[4].Should().Be(PlotterColors.Red);

        // Try with some invalid values.
        palette = null;
        palette = PlotterPalette.Interpolate(-9, PlotterColors.Blue, PlotterColors.White, PlotterColors.Red);
        palette.Colors.Count.Should().Be(0);

        palette = null;
        palette = PlotterPalette.Interpolate(5, null);
        palette.Colors.Count.Should().Be(0);

        palette = null;
        palette = PlotterPalette.Interpolate(0, null);
        palette.Colors.Count.Should().Be(0);

        // Try corner cases.
        palette = null;
        palette = PlotterPalette.Interpolate(1, PlotterColors.Blue, PlotterColors.White, PlotterColors.Red);
        palette.Colors.Count.Should().Be(1);
        palette.Colors[0].Should().Be(PlotterColors.Blue);

        palette = null;
        palette = PlotterPalette.Interpolate(2, PlotterColors.Blue, PlotterColors.White, PlotterColors.Red);
        palette.Colors.Count.Should().Be(2);
        palette.Colors[0].Should().Be(PlotterColors.Blue);
        palette.Colors[1].Should().Be(PlotterColors.Red);

        palette = null;
        palette = PlotterPalette.Interpolate(4, PlotterColors.Blue);
        palette.Colors.Count.Should().Be(4);
        palette.Colors[0].Should().Be(PlotterColors.Blue);
        palette.Colors[1].Should().Be(PlotterColors.Blue);
        palette.Colors[2].Should().Be(PlotterColors.Blue);
        palette.Colors[3].Should().Be(PlotterColors.Blue);
    }

    [Fact]
    public void Constructor()
    {
        var palette = new PlotterPalette(PlotterColors.Blue, PlotterColors.White, PlotterColors.Red);
        palette.Colors.Count.Should().Be(3);
    }
}
