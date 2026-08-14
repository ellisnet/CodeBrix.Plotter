using System;
using CodeBrix.Plotter.Axes;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Provides unit tests for the <see cref="TimeSpanAxis" /> class.
/// </summary>
public class TimeSpanAxisTests
{
    [Theory]
    [InlineData("mm:ss:f", "01:02:3")]
    [InlineData("mm:ss:ff", "01:02:34")]
    [InlineData("mm:ss:fff", "01:02:345")]
    public void SupportMillisecondsInFormatStrings(string format, string expected)
    {
        var axis = new TimeSpanAxis { StringFormat = format };
        var formattedValue = axis.FormatValue(TimeSpanAxis.ToDouble(new System.TimeSpan(1, 1, 1, 2, 345)));
        formattedValue.Should().Be(expected);
    }
}
