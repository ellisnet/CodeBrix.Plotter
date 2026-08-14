// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimatorTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="Decimator" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests.Rendering.Utilities; //was previously: OxyPlot.Tests.Rendering.Utilities;

/// <summary>
/// Provides unit tests for the <see cref="Decimator" /> class.
/// </summary>
public class DecimatorTests
{
    /// <summary>
    /// Tests decimation of a list of points.
    /// </summary>
    [Fact]
    public void Decimate()
    {
        // TODO: write some better tests
        var input = new List<ScreenPoint>();
        var output = new List<ScreenPoint>();
        int n = 1000;
        for (int i = 0; i < n; i++)
        {
            input.Add(new ScreenPoint(Math.Round((double)i / n), Math.Sin(i)));
        }

        Decimator.Decimate(input, output);
        output.Count.Should().Be(6);
    }
}
