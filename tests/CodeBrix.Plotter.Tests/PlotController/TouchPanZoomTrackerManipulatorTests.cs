using System;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests;

/// <summary>
/// Tests the <see cref="TouchPanZoomTrackerManipulator" /> class.
/// </summary>
public class TouchPanZoomTrackerManipulatorTests
{
    /// <summary>
    /// Creates a view whose controller binds the touch gesture to a manipulator made by the specified factory.
    /// </summary>
    /// <param name="configure">Configures each new manipulator.</param>
    /// <returns>The view.</returns>
    private static RecordingPlotView CreateView(Action<TouchPanZoomTrackerManipulator> configure)
    {
        var controller = new PlotController();
        controller.BindTouchDown(new DelegatePlotCommand<PlotterTouchEventArgs>((view, c, args) =>
        {
            var manipulator = new TouchPanZoomTrackerManipulator(view);
            configure(manipulator);
            c.AddTouchManipulator(view, manipulator, args);
        }));
        return new RecordingPlotView(controller);
    }

    [Fact]
    public void constructor_sets_the_defaults()
    {
        //Act
        var manipulator = new TouchPanZoomTrackerManipulator(new StubPlotView());

        //Assert
        manipulator.TouchSlop.Should().Be(10);
        manipulator.Snap.Should().BeTrue();
        manipulator.PointsOnly.Should().BeFalse();
    }

    [Fact]
    public void TouchSlop_keeps_the_tracker_for_moves_within_it()
    {
        //Arrange
        var view = CreateView(m => m.TouchSlop = 50);
        var start = view.ScreenOf(50, 50);

        //Act
        view.TouchDown(start);
        view.TouchMove(0, new ScreenPoint(start.X + 30, start.Y));
        var shownWithinSlop = view.IsTrackerVisible;
        view.TouchUpAll();

        //Assert
        shownWithinSlop.Should().BeTrue();
        view.IsTrackerVisible.Should().BeFalse();
        view.XAxis.ActualMinimum.Should().Be(0);
    }

    [Fact]
    public void Delta_does_not_pan_an_axis_with_panning_disabled()
    {
        //Arrange
        var view = CreateView(m => { });
        view.XAxis.IsPanEnabled = false;
        var start = view.ScreenOf(50, 50);

        //Act
        view.TouchDown(start);
        view.TouchMove(0, new ScreenPoint(start.X + 40, start.Y + 40));
        view.TouchUpAll();

        //Assert
        view.XAxis.ActualMinimum.Should().Be(0);
        view.YAxis.ActualMinimum.Should().BeGreaterThan(0);
        view.IsTrackerVisible.Should().BeFalse();
    }
}
