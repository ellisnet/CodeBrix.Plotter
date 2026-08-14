// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayBuilderTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class ArrayBuilderTests
{
    [Fact]
    public void CreateVector_ByDelta_ReturnsExpectedArray()
    {
        var v = ArrayBuilder.CreateVector(0, 1, 0.1);
        v.Length.Should().Be(11);
        v[0].Should().Be(0);
        v[3].Should().Be(0.3);
        v[6].Should().Be(0.6);
        v[7].Should().Be(0.7);
        v[10].Should().Be(1);
    }

    [Fact]
    public void CreateVector_ByNumberOfSteps_ReturnsExpectedArray()
    {
        var v = ArrayBuilder.CreateVector(0, 1, 11);
        v.Length.Should().Be(11);
        v[0].Should().Be(0);
        v[3].Should().Be(0.3);
        v[6].Should().Be(0.6);
        v[7].Should().Be(0.7);
        v[10].Should().Be(1);
    }

    [Fact]
    public void Evaluate()
    {
        var xvector = ArrayBuilder.CreateVector(0, 1, 0.1);
        var yvector = ArrayBuilder.CreateVector(0, 1, 0.1);
        var dvector = ArrayBuilder.Evaluate((x, y) => x * y, xvector, yvector);

        dvector.GetUpperBound(0).Should().Be(10);
        dvector.GetUpperBound(1).Should().Be(10);
        dvector[0, 0].Should().Be(0);
        dvector[10, 10].Should().Be(1);
        dvector[3, 4].Should().Be(0.3 * 0.4);
    }

    [Fact]
    public void Min2D()
    {
        var array1 = new double[,] { { 4, 2 } };
        array1.Min2D().Should().Be(2);
        var array2 = new[,] { { 4, double.NaN } };
        array2.Min2D().Should().Be(double.NaN);
        array2.Min2D(true).Should().Be(4);
        var array3 = new[] { 4, double.NaN };
        array3.Min().Should().Be(double.NaN);
    }

    [Fact]
    public void Max2D()
    {
        var array1 = new double[,] { { 4, 2 } };
        array1.Max2D().Should().Be(4);
        var array2 = new[,] { { 4, double.NaN } };
        array2.Max2D().Should().Be(4);
        var array3 = new[] { 4, double.NaN };
        array3.Max().Should().Be(4);
    }
}
