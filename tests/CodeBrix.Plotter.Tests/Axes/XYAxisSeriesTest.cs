// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XYAxisSeriesTest.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="XYAxisSeries" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter.Series;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Provides unit tests for the <see cref="XYAxisSeries" /> class.
/// </summary>
public class XYAxisSeriesTests
{
    /// <summary>
    /// Test class just to allow methods in XYAxisSeries to be tested.
    /// </summary>
    private class TestAxisSeries : XYAxisSeries
    {
        public override void Render(IRenderContext rc)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Generate lots of random data, then checking the FindWindowStartIndex method
    /// against this data.
    /// </summary>
    [Fact]
    public void RunFuzzTest()
    {
        for (int i = 0; i < 10000; ++i)
        {
            FuzzIteration(i);
        }
    }

    /// <summary>
    /// Generate lots of random data, then checking the FindWindowStartIndex method
    /// against this data. This time with data containing NaN-values.
    /// </summary>
    [Fact]
    public void RunFuzzWithNanTest()
    {
        for (int i = 0; i < 10000; ++i)
        {
            FuzzIterationWithNan(i);
        }
    }

    private static void FuzzIteration(int seed)
    {
        var xyAxiseries = new TestAxisSeries();

        var N = 15;
        var random = new Random(seed);
        var testData = new System.Collections.Generic.List<double>();
        var currentValue = random.NextDouble() - 0.5;
        for (int i = 0; i < N; ++i)
        {
            testData.Add(currentValue);
            currentValue += random.NextDouble();
        }

        var targetX = random.NextDouble() * (N / 2.0) - 1.0;
        var guess = random.Next(0, testData.Count);

        int foundIndex = xyAxiseries.FindWindowStartIndex(testData, x => x, targetX, guess);

        if (foundIndex > 0)
            testData[foundIndex].Should().BeLessThanOrEqualTo(targetX);
        if (foundIndex < testData.Count - 1)
            testData[foundIndex + 1].Should().BeGreaterThanOrEqualTo(targetX);
    }

    private static void FuzzIterationWithNan(int seed)
    {
        var xyAxiseries = new TestAxisSeries();

        var N = 15;
        var random = new Random(seed);
        var testData = new System.Collections.Generic.List<double>();
        var currentValue = random.NextDouble() - 0.5;
        for (int i = 0; i < N; ++i)
        {
            if (random.Next(4) == 0)
                testData.Add(double.NaN);
            testData.Add(currentValue);
            currentValue += random.NextDouble();
        }

        double? PrevNonNan(int index)
        {
            while (index >= 0)
            {
                if (double.IsNaN(testData[index]) == false)
                    return testData[index];
                index -= 1;
            }

            return null;
        }

        double? NextNonNan(int index)
        {
            while (index >= 0)
            {
                if (double.IsNaN(testData[index]) == false)
                    return testData[index];
                index += 1;
            }

            return null;
        }

        var targetX = random.NextDouble() * (N / 2.0) - 1.0;
        var guess = random.Next(0, testData.Count);

        int foundIndex = xyAxiseries.FindWindowStartIndex(testData, x => x, targetX, guess);

        if (foundIndex > 0)
        {
            if (double.IsNaN(testData[foundIndex]))
            {
                var prevNonNaN = PrevNonNan(foundIndex-1);
                if (prevNonNaN.HasValue)
                    prevNonNaN.Should().BeLessThanOrEqualTo(targetX);
            }
            else
            {
                testData[foundIndex].Should().BeLessThanOrEqualTo(targetX);
            }
        }

        if (foundIndex < testData.Count - 1)
        {
            if (double.IsNaN(testData[foundIndex + 1]))
            {
                var nextNonNaN = NextNonNan(foundIndex+1);
                if (nextNonNaN.HasValue)
                    nextNonNaN.Should().BeGreaterThanOrEqualTo(targetX);
            }
            else
            {
                testData[foundIndex + 1].Should().BeGreaterThanOrEqualTo(targetX);
            }
        }
    }
}
