// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearAxisTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="LinearAxis" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using CodeBrix.Plotter.Axes;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Provides unit tests for the <see cref="LinearAxis" /> class.
/// </summary>
public class LinearAxisTests
{
    /// <summary>
    /// Tests the <see cref="LinearAxis.GetHashCode" /> method.
    /// </summary>
    public new class GetHashCode
    {
        /// <summary>
        /// Given two axes with identical content, verify that the hash codes are different.
        /// </summary>
        [Fact]
        public void TwoEqualAxes()
        {
            var axis1 = new LinearAxis();
            var axis2 = new LinearAxis();
            (axis1.GetHashCode() != axis2.GetHashCode()).Should().BeTrue();
        }
    }

    /// <summary>
    /// Tests the <see cref="LinearAxis.FormatAsFractions" /> property.
    /// </summary>
    public class FormatAsFractions
    {
        /// <summary>
        /// Given 0, the FormatValue method should return 0.
        /// </summary>
        [Fact]
        public void FormatAsFractionsZero()
        {
            var axis = new LinearAxis
            {
                FormatAsFractions = true,
                FractionUnit = Math.PI,
                FractionUnitSymbol = "π"
            };
            axis.FormatValue(0).Should().Be("0");
        }

        /// <summary>
        /// Given PI/2, the FormatValue method should return π/2.
        /// </summary>
        [Fact]
        public void FormatAsFractionsPiHalf()
        {
            var axis = new LinearAxis
            {
                FormatAsFractions = true,
                FractionUnit = Math.PI,
                FractionUnitSymbol = "π"
            };
            axis.FormatValue(0.5 * Math.PI).Should().Be("π/2");
        }

        /// <summary>
        /// Given 2*PI, the FormatValue method should return 2π.
        /// </summary>
        [Fact]
        public void FormatAsFractionsTwoPi()
        {
            var axis = new LinearAxis
            {
                FormatAsFractions = true,
                FractionUnit = Math.PI,
                FractionUnitSymbol = "π"
            };
            axis.FormatValue(2 * Math.PI).Should().Be("2π");
        }

        /// <summary>
        /// Given 3/2*PI, the FormatValue method should return 3π/2.
        /// </summary>
        [Fact]
        public void FormatAsFractionsThreeHalfPi()
        {
            var axis = new LinearAxis
            {
                FormatAsFractions = true,
                FractionUnit = Math.PI,
                FractionUnitSymbol = "π"
            };
            axis.FormatValue(3d / 2 * Math.PI).Should().Be("3π/2");
        }

        /// <summary>
        /// Given 4 and a format string, the FormatValue method should return 1.273π.
        /// </summary>
        [Fact]
        public void FormatAsFractionsWithStringFormat()
        {
            var model = new PlotModel { Culture = CultureInfo.InvariantCulture };
            var axis = new LinearAxis
            {
                FormatAsFractions = true,
                FractionUnit = Math.PI,
                FractionUnitSymbol = "π",
                StringFormat = "0.###"
            };
            model.Axes.Add(axis);
            ((IPlotModel)model).Update(true);
            axis.FormatValue(4).Should().Be("1.273π");
        }
    }
}
