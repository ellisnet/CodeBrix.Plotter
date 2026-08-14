// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterSizeTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="PlotterSize" /> class and it's extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Unit tests for <see cref="PlotterSize" />.
/// </summary>
public class PlotterSizeTests
{
    /// <summary>
    /// Checks the rotating properties of GetBounds() method.
    /// </summary>
    [Fact]
    public void CheckBoundsCalculation()
    {
        AssertBoundsRect(new PlotterRect(-20, -25, 40, 50), 0, HorizontalAlignment.Center, VerticalAlignment.Middle);
        AssertBoundsRect(new PlotterRect(-20, -25, 40, 50), 180, HorizontalAlignment.Center, VerticalAlignment.Middle);
        AssertBoundsRect(new PlotterRect(-20, -25, 40, 50), 360, HorizontalAlignment.Center, VerticalAlignment.Middle);
        AssertBoundsRect(new PlotterRect(-25, -20, 50, 40), 90, HorizontalAlignment.Center, VerticalAlignment.Middle);
        AssertBoundsRect(new PlotterRect(-32, -32, 63, 63), 45, HorizontalAlignment.Center, VerticalAlignment.Middle);
        AssertBoundsRect(new PlotterRect(-32, -32, 63, 63), -45, HorizontalAlignment.Center, VerticalAlignment.Middle);

        AssertBoundsRect(new PlotterRect(0, 0, 40, 50), 0, HorizontalAlignment.Left, VerticalAlignment.Top);
        AssertBoundsRect(new PlotterRect(-40, -50, 40, 50), 180, HorizontalAlignment.Left, VerticalAlignment.Top);
        AssertBoundsRect(new PlotterRect(-50, 0, 50, 40), 90, HorizontalAlignment.Left, VerticalAlignment.Top);
        AssertBoundsRect(new PlotterRect(-35, 0, 63, 63), 45, HorizontalAlignment.Left, VerticalAlignment.Top);
        AssertBoundsRect(new PlotterRect(0, -28, 63, 63), -45, HorizontalAlignment.Left, VerticalAlignment.Top);

        AssertBoundsRect(new PlotterRect(-40, -50, 40, 50), 0, HorizontalAlignment.Right, VerticalAlignment.Bottom);
        AssertBoundsRect(new PlotterRect(-40, -50, 40, 50), 360, HorizontalAlignment.Right, VerticalAlignment.Bottom);
        AssertBoundsRect(new PlotterRect(0, 0, 40, 50), 180, HorizontalAlignment.Right, VerticalAlignment.Bottom);
        AssertBoundsRect(new PlotterRect(0, -40, 50, 40), 90, HorizontalAlignment.Right, VerticalAlignment.Bottom);
        AssertBoundsRect(new PlotterRect(-28, -63, 63, 63), 45, HorizontalAlignment.Right, VerticalAlignment.Bottom);
        AssertBoundsRect(new PlotterRect(-63, -35, 63, 63), -45, HorizontalAlignment.Right, VerticalAlignment.Bottom);

        AssertBoundsRect(new PlotterRect(0, -50, 40, 50), 0, HorizontalAlignment.Left, VerticalAlignment.Bottom);
        AssertBoundsRect(new PlotterRect(0, -35, 63, 63), 45, HorizontalAlignment.Left, VerticalAlignment.Bottom);

        AssertBoundsRect(new PlotterRect(0, -25, 40, 50), 0, HorizontalAlignment.Left, VerticalAlignment.Middle);
        AssertBoundsRect(new PlotterRect(-17, -17, 63, 63), 45, HorizontalAlignment.Left, VerticalAlignment.Middle);
    }

    /// <summary>
    /// Checks the calculation of the GetPolygon() method.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="angle">The angle.</param>
    /// <param name="ha">The horizontal alignment.</param>
    /// <param name="va">The vertical alignment.</param>
    /// <param name="expectedX0">The expected x0.</param>
    /// <param name="expectedY0">The expected y0.</param>
    /// <param name="expectedX2">The expected x2.</param>
    /// <param name="expectedY2">The expected y2.</param>
    [Theory]
    [InlineData(0, 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 0, 0, 40, 50)]
    [InlineData(0, 0, 0, HorizontalAlignment.Right, VerticalAlignment.Top, -40, 0, 0, 50)]
    [InlineData(0, 0, 0, HorizontalAlignment.Right, VerticalAlignment.Bottom, -40, -50, 0, 0)]
    [InlineData(0, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Middle, -20, -25, 20, 25)]
    [InlineData(0, 0, 90, HorizontalAlignment.Center, VerticalAlignment.Middle, 25, -20, -25, 20)]
    [InlineData(0, 0, 180, HorizontalAlignment.Center, VerticalAlignment.Middle, 20, 25, -20, -25)]
    public void CheckPolygonCalculation(double x, double y, double angle, HorizontalAlignment ha, VerticalAlignment va, double expectedX0, double expectedY0, double expectedX2, double expectedY2)
    {
        const double Delta = 1;
        var size = new PlotterSize(40, 50);
        var p = size.GetPolygon(new ScreenPoint(x, y), angle, ha, va).ToArray();
        p[0].X.Should().BeApproximately(expectedX0, Delta);
        p[0].Y.Should().BeApproximately(expectedY0, Delta);
        p[2].X.Should().BeApproximately(expectedX2, Delta);
        p[2].Y.Should().BeApproximately(expectedY2, Delta);
    }

    /// <summary>
    /// Asserts that the rectangle rotated by the specified angle and alignment equals the specified expected rectangle.
    /// </summary>
    /// <param name="expected">The expected.</param>
    /// <param name="angle">The angle.</param>
    /// <param name="horizontalAlignment">The horizontal alignment.</param>
    /// <param name="verticalAlignment">The vertical alignment.</param>
    private static void AssertBoundsRect(PlotterRect expected, double angle, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
    {
        var actual = new PlotterSize(40, 50).GetBounds(angle, horizontalAlignment, verticalAlignment);
        const double Delta = 1;
        var errorMessage = string.Format("{0} is not equal to {1} rotated by {2} angle when aligned {3} and {4}", expected, actual, angle, horizontalAlignment, verticalAlignment);
        actual.Left.Should().BeApproximately(expected.Left, Delta);
        actual.Top.Should().BeApproximately(expected.Top, Delta);
        actual.Width.Should().BeApproximately(expected.Width, Delta);
        actual.Height.Should().BeApproximately(expected.Height, Delta);
    }

    /// <summary>
    /// Tests the Equals method.
    /// </summary>
    [Fact]
    public void Equals_compares_by_value()
    {
        new PlotterSize(1, 2).Equals(new PlotterSize(1, 2)).Should().BeTrue();
        new PlotterSize(1, 2).Equals(new PlotterSize()).Should().BeFalse();
    }
}
