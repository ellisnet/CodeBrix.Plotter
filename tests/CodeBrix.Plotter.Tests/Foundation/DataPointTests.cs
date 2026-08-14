// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataPointTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="DataPoint" /> type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using SilverAssertions;
using Xunit;
namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Provides unit tests for the <see cref="DataPoint" /> type.
/// </summary>
public class DataPointTests
{
    /// <summary>
    /// Given valid points, <see cref="DataPoint.IsDefined" /> should return <c>true</c>.
    /// </summary>
    [Fact]
    public void ValidPoints()
    {
        new DataPoint(1, 2).IsDefined().Should().BeTrue();
        new DataPoint(double.MaxValue, double.MaxValue).IsDefined().Should().BeTrue();
        new DataPoint(double.MinValue, double.MinValue).IsDefined().Should().BeTrue();
    }

    /// <summary>
    /// Given invalid points, <see cref="DataPoint.IsDefined" /> should return <c>false</c>.
    /// </summary>
    [Fact]
    public void InvalidPoints()
    {
        new DataPoint(double.NaN, double.NaN).IsDefined().Should().BeFalse();
        new DataPoint(double.NaN, 2).IsDefined().Should().BeFalse();
        new DataPoint(2, double.NaN).IsDefined().Should().BeFalse();
        var p = DataPoint.Undefined;
        p.IsDefined().Should().BeFalse();
    }

    /// <summary>
    /// Tests the <see cref="DataPoint.Equals" /> method.
    /// </summary>
    [Fact]
    public void Equals_compares_by_value()
    {
        new DataPoint(1, 2).Equals(new DataPoint(1, 2)).Should().BeTrue();
        new DataPoint(1, 2).Equals(new DataPoint()).Should().BeFalse();
        DataPoint.Undefined.Equals(DataPoint.Undefined).Should().BeTrue();
    }
}
