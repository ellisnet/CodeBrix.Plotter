// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListBuilderTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CodeBrix.Plotter.Series;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

public class ListBuilderTests
{
    private IList<TestObject> src;

    public ListBuilderTests()
    {
        this.src = new List<TestObject> { new TestObject { A = 3.14 } };
    }

    [Fact]
    public void Fill()
    {
        var target = new List<ScatterPoint>();

        var filler = new ListBuilder<ScatterPoint>();
        filler.Add("A", 0d);
        filler.Fill(target, this.src, args => new ScatterPoint(Convert.ToDouble(args[0]), 0));

        target.Count.Should().Be(1);
        target[0].X.Should().Be(3.14);
    }

    [Fact]
    public void FillDataPoints()
    {
        var target = new List<DataPoint>();

        var filler = new ListBuilder<DataPoint>();
        filler.Add("A", 0d);
        filler.Fill(target, this.src, args => new DataPoint(Convert.ToDouble(args[0]), 0));

        target.Count.Should().Be(1);
        target[0].X.Should().Be(3.14);
    }

    [Fact]
    public void Fill_InvalidProperty_UsesDefaultValue()
    {
        var target = new List<ScatterPoint>();

        var filler = new ListBuilder<ScatterPoint>();
        filler.Add("B", 0d);
        filler.Fill(target, this.src, args => new ScatterPoint(Convert.ToDouble(args[0]), 0));
    }

    [Fact]
    public void Fill_NullProperty_DefaultValueIsExpected()
    {
        var target = new List<ScatterPoint>();

        var filler = new ListBuilder<ScatterPoint>();
        filler.Add(null, 42);
        filler.Fill(target, this.src, args => new ScatterPoint(Convert.ToDouble(args[0]), 0));

        target.Count.Should().Be(1);
        target[0].X.Should().Be(42);
    }

    private class TestObject
    {
        public double A { get; set; }
    }
}
