// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenPointHelperTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class ScreenPointHelperTests
{
    [Fact]
    public void IsPointInPolygon_NullPoints()
    {
        ScreenPointHelper.IsPointInPolygon(default(ScreenPoint), null).Should().BeFalse();
    }

    [Fact]
    public void ResamplePoints()
    {
        var points = CreatePointList();
        var result = ScreenPointHelper.ResamplePoints(points, 1);
        result.Count.Should().Be(4);
    }

    [Fact]
    public void GetCentroid()
    {
        var points = CreatePointList();
        var centroid = ScreenPointHelper.GetCentroid(points);
        centroid.X.Should().BeApproximately(0.041666, 1e-6);
        centroid.Y.Should().BeApproximately(0.708333, 1e-6);
    }

    private static IList<ScreenPoint> CreatePointList()
    {
        var points = new List<ScreenPoint>();
        points.Add(new ScreenPoint(-1, -1));
        points.Add(new ScreenPoint(1, -2));
        points.Add(new ScreenPoint(2, 2));
        points.Add(new ScreenPoint(-2, 3));
        return points;
    }
}
