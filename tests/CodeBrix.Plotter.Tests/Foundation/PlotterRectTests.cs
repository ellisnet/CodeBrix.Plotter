// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterRectTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="PlotterRect" /> class and it's extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using SilverAssertions;
using Xunit;
namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Unit tests for <see cref="PlotterRect" />.
/// </summary>
public class PlotterRectTests
{
    /// <summary>
    /// Tests the Equals method.
    /// </summary>
    [Fact]
    public void Equals_compares_by_value()
    {
        new PlotterRect(1, 2, 3, 4).Equals(new PlotterRect(1, 2, 3, 4)).Should().BeTrue();
        new PlotterRect(1, 2, 3, 4).Equals(new PlotterRect()).Should().BeFalse();
    }

    /// <summary>
    /// Tests the Inflate method.
    /// </summary>
    [Fact]
    public void Inflate()
    {
        new PlotterRect(1, 2, 3, 4).Inflate(0.1, 0.2).Should().Be(new PlotterRect(0.9, 1.8, 3.2, 4.4));
        new PlotterRect(10, 20, 30, 40).Inflate(new PlotterThickness(1, 2, 3, 4)).Should().Be(new PlotterRect(9, 18, 34, 46));
    }

    /// <summary>
    /// Tests the Deflate method.
    /// </summary>
    [Fact]
    public void Deflate()
    {
        new PlotterRect(10, 20, 30, 40).Deflate(new PlotterThickness(1, 2, 3, 4)).Should().Be(new PlotterRect(11, 22, 26, 34));
        new PlotterRect(10, 20, 30, 40).Deflate(new PlotterThickness(15)).Should().Be(new PlotterRect(25, 35, 0, 10));
    }

    /// <summary>
    /// Tests the Offset method.
    /// </summary>
    [Fact]
    public void Offset()
    {
        new PlotterRect(1, 2, 3, 4).Offset(0.1, 0.2).Should().Be(new PlotterRect(1.1, 2.2, 3, 4));
    }
}
