// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterColorTests.cs" company="OxyPlot">
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
public class PlotterColorTests
{
    [Fact]
    public void Parse()
    {
        PlotterColor.Parse("#FF0000").Should().Be(PlotterColors.Red);
        PlotterColor.Parse("#FFFF0000").Should().Be(PlotterColors.Red);
        PlotterColor.Parse("255,0,0").Should().Be(PlotterColors.Red);
        PlotterColor.Parse("255,255,0,0").Should().Be(PlotterColors.Red);
        PlotterColor.Parse(null).Should().Be(PlotterColors.Undefined);
        PlotterColor.Parse("None").Should().Be(PlotterColors.Undefined);
        PlotterColor.Parse("Auto").Should().Be(PlotterColors.Automatic);
        PlotterColor.Parse("#00000000").Should().Be(PlotterColors.Undefined);
        PlotterColor.Parse("#00000001").Should().Be(PlotterColors.Automatic);
        PlotterColor.Parse("#FFF").Should().Be(PlotterColors.White);
    }

    [Fact]
    public void ColorDifference()
    {
        PlotterColor.ColorDifference(PlotterColors.Red, PlotterColors.Green).Should().BeApproximately(1.1189122525867927d, 1e-6);
    }

    [Fact]
    public void FromAColor()
    {
        PlotterColor.FromAColor(0x80, PlotterColors.Red).Should().Be(PlotterColor.FromArgb(0x80, 0xff, 0, 0));
    }

    [Fact]
    public void FromHsv()
    {
        PlotterColor.FromHsv(0, 1, 1).Should().Be(PlotterColors.Red);
    }

    [Fact]
    public void FromRgb()
    {
        PlotterColor.FromRgb(255, 0, 0).Should().Be(PlotterColors.Red);
    }

    [Fact]
    public void FromArgb()
    {
        PlotterColor.FromArgb(255, 255, 0, 0).Should().Be(PlotterColors.Red);
    }

    [Fact]
    public void FromUInt32()
    {
        PlotterColor.FromUInt32(0xFFFF0000).Should().Be(PlotterColors.Red);
    }

    [Fact]
    public void GetColorName()
    {
        PlotterColors.Red.GetColorName().Should().Be("Red");
    }

    [Fact]
    public void ChangeIntensity()
    {
        PlotterColors.Red.ChangeIntensity(0.5).Should().Be(PlotterColor.FromArgb(255, 127, 0, 0));
    }

    [Fact]
    public void ChangeSaturation()
    {
        PlotterColors.Red.ChangeSaturation(0.5).Should().Be(PlotterColor.FromArgb(255, 255, 127, 127));
    }

    [Fact]
    public void ChangeSaturation_OverSaturate()
    {
        PlotterColors.Red.ChangeSaturation(2).Should().Be(PlotterColor.FromArgb(255, 255, 0, 0));
    }

    [Fact]
    public void Complementary()
    {
        PlotterColors.Red.Complementary().Should().Be(PlotterColors.Cyan);
    }

    [Fact]
    public void ToByteString()
    {
        PlotterColors.Red.ToByteString().Should().Be("255,255,0,0");
    }

    [Fact]
    public void ToCode()
    {
        PlotterColors.Red.ToCode().Should().Be("PlotterColors.Red");
        PlotterColor.FromArgb(0x01, 0x02, 0x03, 0x04).ToCode().Should().Be("PlotterColor.FromArgb(1, 2, 3, 4)");
    }

    [Fact]
    public void ToHsv()
    {
        PlotterColors.Red.ToHsv().Should().Equal(new double[] { 0, 1, 1 });
    }

    [Fact]
    public void ToString_formats_the_color()
    {
        PlotterColors.Red.ToString().Should().Be("#ffff0000");
        PlotterColors.Automatic.ToString().Should().Be("#00000001");
        PlotterColors.Undefined.ToString().Should().Be("#00000000");
    }

    [Fact]
    public void ToUint()
    {
        PlotterColors.Red.ToUint().Should().Be(0xFFFF0000);
    }

    [Fact]
    public void HueDifference()
    {
        PlotterColor.HueDifference(PlotterColors.Red, PlotterColors.Blue).Should().BeApproximately(1.0 / 3, 1e-6);
    }
}
