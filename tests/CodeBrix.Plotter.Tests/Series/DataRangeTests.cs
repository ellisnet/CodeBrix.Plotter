// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRangeTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Tests the <see cref="DataRange" /> struct.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter.Series;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Contains the unit tests for the <see cref="DataRange"/> struct.
/// </summary>
public class DataRangeTests
{
    /// <summary>
    /// Tests that the properties are initialized correctly.
    /// </summary>
    [Fact]
    public void Initialize()
    {
        var range = new DataRange(3, 5);

        range.Minimum.Should().Be(3);
        range.Maximum.Should().Be(5);
    }

    /// <summary>
    /// Tests that an exception is thrown if one of the constructor parameters is NaN.
    /// </summary>
    [Fact]
    public void Initialize_ThrowsArgumentException_NaN()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new DataRange(double.NaN, 3));
        ex.Message.Should().Be("NaN values are not permitted");
    }

    /// <summary>
    /// Tests that an exception is thrown if provided minimum in constructor is larger than maximum.
    /// </summary>
    [Fact]
    public void Initialize_ThrowsArgumentException_MinGreaterMax()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new DataRange(5, 3));
        ex.Message.Should().Be("max must be larger or equal min");
    }

    /// <summary>
    /// Tests the range property.
    /// </summary>
    [Fact]
    public void RangeProperty()
    {
        var range = new DataRange(3, 5);

        range.Range.Should().Be(2);
    }

    /// <summary>
    /// Tests whether the <see cref="DataRange.IsDefined"/> method works correctly.
    /// </summary>
    [Fact]
    public void IsDefined()
    {
        new DataRange(3, 5).IsDefined().Should().BeTrue();
        new DataRange(double.NegativeInfinity, double.PositiveInfinity).IsDefined().Should().BeTrue();
        new DataRange(double.MinValue, double.MaxValue).IsDefined().Should().BeTrue();

#pragma warning disable SA1129 // Do not use default value type constructor
        new DataRange().IsDefined().Should().BeFalse();
#pragma warning restore SA1129 // Do not use default value type constructor

        DataRange.Undefined.IsDefined().Should().BeFalse();
    }

    /// <summary>
    /// Tests whether the default instance is undefined.
    /// </summary>
    [Fact]
    public void DefaultIsUndefined()
    {
        default(DataRange).IsDefined().Should().BeFalse();
    }

    /// <summary>
    /// Tests the method to determine whether a given value
    /// is inside the data range or not.
    /// </summary>
    [Fact]
    public void Contains()
    {
        var range = new DataRange(3, 5);

        range.Contains(3).Should().BeTrue();
        range.Contains(4).Should().BeTrue();
        range.Contains(5).Should().BeTrue();

        range.Contains(2).Should().BeFalse();
        range.Contains(6).Should().BeFalse();
    }

    /// <summary>
    /// Tests the method to determine whether a data range instance overlaps
    /// with a second one or not.
    /// </summary>
    [Fact]
    public void IntersectsWith()
    {
        var range = new DataRange(3, 5);

        range.IntersectsWith(new DataRange(1, 4)).Should().BeTrue();
        range.IntersectsWith(new DataRange(4, 6)).Should().BeTrue();
        range.IntersectsWith(new DataRange(4, 4)).Should().BeTrue();
        range.IntersectsWith(new DataRange(1, 6)).Should().BeTrue();

        range.IntersectsWith(new DataRange(1, 3)).Should().BeTrue();
        range.IntersectsWith(new DataRange(5, 6)).Should().BeTrue();

        range.IntersectsWith(new DataRange(-5, -3)).Should().BeFalse();
        range.IntersectsWith(new DataRange(13, 15)).Should().BeFalse();

        range.IntersectsWith(DataRange.Undefined).Should().BeFalse();
    }

    /// <summary>
    /// Tests whether the <see cref="DataRange.ToCode"/> method returns
    /// the expected string.
    /// </summary>
    [Fact]
    public void ToCode()
    {
        new DataRange(3, 5).ToCode().Should().Be("new DataRange(3,5)");
    }

    /// <summary>
    /// Tests whether the <see cref="DataRange.ToString"/> method returns
    /// the expected string.
    /// </summary>
    [Fact]
    public void TestToString()
    {
        new DataRange(3, 5).ToString().Should().Be("[3, 5]");
    }
}
