// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotModelTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Tests the <see cref="PlotModel.Render" /> method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Series;
using CodeBrix.Plotter.Tests.ExampleLibrary;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class PlotModelTests
{
    [Theory]
    [MemberData(nameof(ExampleTestData.AllForAutomatedTest), MemberType = typeof(ExampleTestData))]
    public void Update_AllExamples_ThrowsNoExceptions(string exampleCategory, string exampleTitle)
    {
        var example = ExampleTestData.Get(exampleCategory, exampleTitle);
        ((IPlotModel)example.PlotModel)?.Update(true);

        var first = 1; // skip the 'none', since we do that above for clarity
        var all = (int)(ExampleFlags.Transpose | ExampleFlags.Reverse);

        for (int flags = first; flags < all; flags++)
        {
            ((IPlotModel)example.GetModel((ExampleFlags)flags))?.Update(true);
        }
    }

    [Fact]
    public void B11_Backgrounds()
    {
        var plot = new PlotModel { Title = "Backgrounds" };
        plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X-axis" });
        var yaxis1 = new LinearAxis { Position = AxisPosition.Left, Title = "Y1", Key = "Y1", StartPosition = 0, EndPosition = 0.5 };
        var yaxis2 = new LinearAxis { Position = AxisPosition.Left, Title = "Y2", Key = "Y2", StartPosition = 0.5, EndPosition = 1 };
        plot.Axes.Add(yaxis1);
        plot.Axes.Add(yaxis2);

        Action<LineSeries> addExamplePoints = ls =>
            {
                ls.Points.Add(new DataPoint(3, 13));
                ls.Points.Add(new DataPoint(10, 47));
                ls.Points.Add(new DataPoint(30, 23));
                ls.Points.Add(new DataPoint(40, 65));
                ls.Points.Add(new DataPoint(80, 10));
            };

        var ls1 = new LineSeries { Background = PlotterColors.LightSeaGreen, YAxisKey = "Y1" };
        addExamplePoints(ls1);
        plot.Series.Add(ls1);

        var ls2 = new LineSeries { Background = PlotterColors.LightSkyBlue, YAxisKey = "Y2" };
        addExamplePoints(ls2);
        plot.Series.Add(ls2);

        // PlotterAssert.AreEqual(plot, "B11");
    }

    [Fact]
    public void PlotControl_CollectedPlotControl_ReferenceShouldNotBeAlive()
    {
        var pm = new PlotModel();
        var plot = new StubPlotView();
        ((IPlotModel)pm).AttachPlotView(plot);
        pm.PlotView.Should().NotBeNull();

        // ReSharper disable once RedundantAssignment
        plot = null;
        GC.Collect();

        // Verify that the reference is lost
        // In debug builds a reference may be kept around for the debugger.
#if !DEBUG
        pm.PlotView.Should().BeNull();
#endif
    }

    /// <summary>
    /// Tests rendering on a collapsed output surface.
    /// </summary>
    [Fact]
    public void Collapsed()
    {
        var model = new PlotModel();
        var rc = new NullRenderContext();
        ((IPlotModel)model).Render(rc, new PlotterRect(0, 0, 0, 0));
    }

    /// <summary>
    /// Tests rendering on a small output surface.
    /// </summary>
    [Fact]
    public void NoPadding()
    {
        var model = new PlotModel { Padding = new PlotterThickness(0) };
        var rc = new NullRenderContext();
        ((IPlotModel)model).Render(rc, new PlotterRect(0, 0, double.Epsilon, double.Epsilon));
    }

    /// <summary>
    /// The same axis cannot be added more than once.
    /// </summary>
    [Fact]
    public void AddAxisTwice()
    {
        var model = new PlotModel();
        var axis = new LinearAxis();
        model.Axes.Add(axis);
        Assert.Throws<InvalidOperationException>(() => model.Axes.Add(axis));
    }

    /// <summary>
    /// The same axis cannot be added to different PlotModels.
    /// </summary>
    [Fact]
    public void AddAxisToDifferentModels()
    {
        var model1 = new PlotModel();
        var model2 = new PlotModel();
        var axis = new LinearAxis();
        model1.Axes.Add(axis);
        Assert.Throws<InvalidOperationException>(() => model2.Axes.Add(axis));
    }

    /// <summary>
    /// An exception should be thrown when the axis key is invalid.
    /// </summary>
    [Fact]
    public void InvalidAxisKey()
    {
        var model = new PlotModel();
        model.Axes.Add(new LinearAxis());
        model.Series.Add(new LineSeries { XAxisKey = "invalidKey" });
        ((IPlotModel)model).Update(true);
        (model.GetLastPlotException() as InvalidOperationException).Should().NotBeNull();
    }

    /// <summary>
    /// When PlotMargins is not set, the ActualPlotMargins should use the desired size of the axes.
    /// </summary>
    [Fact]
    public void AutoPlotMargins()
    {
        var plot = new PlotModel { Title = "Auto PlotMargins" };
        var verticalAxis = new LinearAxis { Position = AxisPosition.Left };
        var horizontalAxis = new LinearAxis { Position = AxisPosition.Bottom };
        plot.Axes.Add(verticalAxis);
        plot.Axes.Add(horizontalAxis);
        plot.UpdateAndRenderToNull(800, 600);
        plot.ActualPlotMargins.Left.Should().BeApproximately(26, 1);
        plot.ActualPlotMargins.Top.Should().BeApproximately(5, 1);
        plot.ActualPlotMargins.Right.Should().BeApproximately(7.5, 1);
        plot.ActualPlotMargins.Bottom.Should().BeApproximately(21, 1);
    }

    /// <summary>
    /// When PlotMargins is not set, the ActualPlotMargins should have the same value.
    /// </summary>
    [Fact]
    public void FixedPlotMargins()
    {
        var plot = new PlotModel { PlotMargins = new PlotterThickness(23, 19, 17, 31) };
        plot.UpdateAndRenderToNull(800, 600);
        plot.ActualPlotMargins.Left.Should().Be(plot.PlotMargins.Left);
        plot.ActualPlotMargins.Top.Should().Be(plot.PlotMargins.Top);
        plot.ActualPlotMargins.Right.Should().Be(plot.PlotMargins.Right);
        plot.ActualPlotMargins.Bottom.Should().Be(plot.PlotMargins.Bottom);
    }

    /// <summary>
    /// When GetAxis is called with an unknown key, an InvalidOperationException should be thrown.
    /// </summary>
    [Fact]
    public void GetAxis()
    {
        var plot = new PlotModel { Title = "Get Axis Or Default" };
        var verticalAxis = new LinearAxis { Key = "YAxis", Position = AxisPosition.Left };
        var horizontalAxis = new LinearAxis { Key = "XAxis", Position = AxisPosition.Bottom };

        plot.Axes.Add(verticalAxis);
        plot.Axes.Add(horizontalAxis);

        Assert.Throws<InvalidOperationException>(() => plot.GetAxis("ThisIsAnInvalidKey"));
    }

    /// <summary>
    /// When GetAxisOrDefault is called with an unknown key, the provided default value should
    /// be returned.
    /// </summary>
    [Fact]
    public void GetAxisOrDefault()
    {
        var plot = new PlotModel { Title = "Get Axis Or Default" };
        var verticalAxis = new LinearAxis { Key = "YAxis", Position = AxisPosition.Left };
        var horizontalAxis = new LinearAxis { Key = "XAxis", Position = AxisPosition.Bottom };

        plot.Axes.Add(verticalAxis);
        plot.Axes.Add(horizontalAxis);

        var defaultValue = new LinearAxis { Key = "DefaultAxis" };

        plot.GetAxisOrDefault("ThisIsAnInvalidKey", defaultValue).Should().Be(defaultValue);
    }
}
