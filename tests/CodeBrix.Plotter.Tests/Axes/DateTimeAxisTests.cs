// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeAxisTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using CodeBrix.Plotter.Axes;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class DateTimeAxisTests
{
    [Fact]
    public void ToDouble_ValidDate()
    {
        var date = new DateTime(2011, 3, 15);
        DateTimeAxis.ToDouble(date).Should().Be(date.ToOADate());
    }

    [Fact]
    public void ToDouble_NoDate()
    {
        DateTimeAxis.ToDouble(new DateTime()).Should().Be(-693593);
    }

    [Fact]
    public void ToDateTime_ValidDate()
    {
        var date = new DateTime(2011, 3, 15);
        DateTimeAxis.ToDateTime(date.ToOADate(), DateTimeAxis.DefaultPrecision).Should().Be(date);
    }

    [Fact]
    public void ToDateTime_NoDate()
    {
        DateTimeAxis.ToDateTime(-693593, DateTimeAxis.DefaultPrecision).Should().Be(new DateTime());
    }

    [Fact]
    public void ToDateTime_NaN()
    {
        DateTimeAxis.ToDateTime(double.NaN, DateTimeAxis.DefaultPrecision).Should().Be(new DateTime());
    }

    [Fact]
    public void ToDateTime_VeryBigValue()
    {
        DateTimeAxis.ToDateTime(double.MaxValue, DateTimeAxis.DefaultPrecision).Should().Be(new DateTime());
    }

    [Fact]
    public void ToDateTime_VerySmallValue()
    {
        DateTimeAxis.ToDateTime(double.MinValue, DateTimeAxis.DefaultPrecision).Should().Be(new DateTime());
    }
}
