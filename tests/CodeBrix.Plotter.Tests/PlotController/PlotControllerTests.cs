// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotControllerTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Tests the <see cref="PlotController" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

/// <summary>
/// Tests the <see cref="PlotController" /> class.
/// </summary>
public class PlotControllerTests
{
    /// <summary>
    /// Tests the <see cref="PlotController.Unbind(PlotterInputGesture)" /> method.
    /// </summary>
    public class Unbind
    {
        /// <summary>
        /// When unbinding a gesture, the gesture should be removed from the InputCommandBindings.
        /// </summary>
        [Fact]
        public void UnbindLeftMouseButton()
        {
            var c = new PlotController();
            c.Unbind(new PlotterMouseDownGesture(PlotterMouseButton.Left));
            c.InputCommandBindings.Any(b => b.Gesture.Equals(new PlotterMouseDownGesture(PlotterMouseButton.Left))).Should().BeFalse();
        }

        /// <summary>
        /// When unbinding a command, the command should be removed from the InputCommandBindings.
        /// </summary>
        [Fact]
        public void UnbindPlotCommand()
        {
            var c = new PlotController();
            c.Unbind(PlotCommands.SnapTrack);
            c.InputCommandBindings.Any(b => b.Command == PlotCommands.SnapTrack).Should().BeFalse();
        }

        /// <summary>
        /// When unbinding all gestures, the InputCommandBindings collection should be empty.
        /// </summary>
        [Fact]
        public void UnbindAll()
        {
            var c = new PlotController();
            c.UnbindAll();
            c.InputCommandBindings.Count.Should().Be(0);
        }
    }
    /// <summary>
    /// Tests the touch binding of the default <see cref="PlotController" />, driving touch sequences the way a host
    /// view does.
    /// </summary>
    public class DefaultTouchBinding
    {
        /// <summary>
        /// The tolerance for comparing axis values.
        /// </summary>
        private const double Tolerance = 1e-6;

        [Fact]
        public void touch_gesture_is_bound_to_PanZoomTrackByTouch()
            => new PlotController().InputCommandBindings
                .Single(b => b.Gesture.Equals(new PlotterTouchGesture())).Command
                .Should().BeSameAs(PlotCommands.PanZoomTrackByTouch);

        [Fact]
        public void one_finger_drag_pans_the_axes_and_keeps_the_plot_under_the_finger()
        {
            //Arrange
            var view = new RecordingPlotView(new PlotController());
            var start = view.ScreenOf(50, 50);

            //Act
            view.TouchDown(start);
            for (var i = 1; i <= 4; i++)
            {
                view.TouchMove(0, new ScreenPoint(start.X + (i * 10), start.Y + (i * 5)));
            }

            var end = view.DataAt(new ScreenPoint(start.X + 40, start.Y + 20));
            view.TouchUpAll();

            //Assert
            view.XAxis.ActualMinimum.Should().BeLessThan(0);
            view.YAxis.ActualMinimum.Should().BeGreaterThan(0);
            (view.XAxis.ActualMaximum - view.XAxis.ActualMinimum).Should().BeApproximately(100, Tolerance);
            end.X.Should().BeApproximately(50, Tolerance);
            end.Y.Should().BeApproximately(50, Tolerance);
        }

        [Fact]
        public void two_finger_pinch_zooms_around_the_pinch_centre()
        {
            //Arrange
            var view = new RecordingPlotView(new PlotController());
            var centre = view.ScreenOf(50, 50);

            //Act
            view.TouchDown(new ScreenPoint(centre.X - 20, centre.Y));
            view.TouchDown(new ScreenPoint(centre.X + 20, centre.Y));
            view.TouchMove(1, new ScreenPoint(centre.X + 40, centre.Y));
            view.TouchMove(0, new ScreenPoint(centre.X - 40, centre.Y));
            view.TouchUpAll();

            //Assert
            view.XAxis.ActualMinimum.Should().BeApproximately(25, Tolerance);
            view.XAxis.ActualMaximum.Should().BeApproximately(75, Tolerance);
            view.YAxis.ActualMinimum.Should().BeApproximately(25, Tolerance);
            view.YAxis.ActualMaximum.Should().BeApproximately(75, Tolerance);
            view.IsTrackerVisible.Should().BeFalse();
        }

        [Fact]
        public void pinch_while_moving_keeps_the_data_under_the_moving_pinch_centre()
        {
            //Arrange
            var view = new RecordingPlotView(new PlotController());
            var centre = view.ScreenOf(50, 50);

            //Act
            view.TouchDown(new ScreenPoint(centre.X - 20, centre.Y));
            view.TouchDown(new ScreenPoint(centre.X + 20, centre.Y));
            view.TouchMove(1, new ScreenPoint(centre.X + 40, centre.Y + 30));
            view.TouchMove(0, new ScreenPoint(centre.X - 40, centre.Y + 30));
            var underCentre = view.DataAt(new ScreenPoint(centre.X, centre.Y + 30));
            view.TouchUpAll();

            //Assert
            underCentre.X.Should().BeApproximately(50, Tolerance);
            underCentre.Y.Should().BeApproximately(50, Tolerance);
            (view.XAxis.ActualMaximum - view.XAxis.ActualMinimum).Should().BeApproximately(50, Tolerance);
        }

        [Fact]
        public void stationary_touch_shows_the_tracker_and_release_hides_it()
        {
            //Arrange
            var view = new RecordingPlotView(new PlotController());
            var start = view.ScreenOf(50, 50);

            //Act
            view.TouchDown(start);
            var shownOnDown = view.IsTrackerVisible;
            view.TouchMove(0, new ScreenPoint(start.X + 3, start.Y - 2));
            var shownAfterJitter = view.IsTrackerVisible;
            view.TouchUpAll();

            //Assert
            shownOnDown.Should().BeTrue();
            shownAfterJitter.Should().BeTrue();
            view.IsTrackerVisible.Should().BeFalse();
            view.XAxis.ActualMinimum.Should().Be(0);
            view.XAxis.ActualMaximum.Should().Be(100);
            view.YAxis.ActualMinimum.Should().Be(0);
            view.YAxis.ActualMaximum.Should().Be(100);
        }

        [Fact]
        public void drag_past_the_slop_hides_the_tracker_and_does_not_show_it_again()
        {
            //Arrange
            var view = new RecordingPlotView(new PlotController());
            var start = view.ScreenOf(50, 50);

            //Act
            view.TouchDown(start);
            view.TouchMove(0, new ScreenPoint(start.X + 15, start.Y));
            var shownAfterSlop = view.IsTrackerVisible;
            view.TouchMove(0, new ScreenPoint(start.X + 30, start.Y));
            view.TouchMove(0, new ScreenPoint(start.X + 45, start.Y));
            var shownWhilePanning = view.IsTrackerVisible;
            view.TouchUpAll();

            //Assert
            shownAfterSlop.Should().BeFalse();
            shownWhilePanning.Should().BeFalse();
            view.IsTrackerVisible.Should().BeFalse();
            view.ShowTrackerCount.Should().Be(1);
            view.XAxis.ActualMinimum.Should().BeLessThan(0);
        }

        [Fact]
        public void mouse_and_key_bindings_are_unchanged()
        {
            //Arrange
            var c = new PlotController();

            //Act
            var left = c.InputCommandBindings.Single(b => b.Gesture.Equals(new PlotterMouseDownGesture(PlotterMouseButton.Left))).Command;
            var right = c.InputCommandBindings.Single(b => b.Gesture.Equals(new PlotterMouseDownGesture(PlotterMouseButton.Right))).Command;
            var wheel = c.InputCommandBindings.Single(b => b.Gesture.Equals(new PlotterMouseWheelGesture())).Command;
            var home = c.InputCommandBindings.Single(b => b.Gesture.Equals(new PlotterKeyGesture(PlotterKey.Home))).Command;

            //Assert
            left.Should().BeSameAs(PlotCommands.SnapTrack);
            right.Should().BeSameAs(PlotCommands.PanAt);
            wheel.Should().BeSameAs(PlotCommands.ZoomWheel);
            home.Should().BeSameAs(PlotCommands.Reset);
            c.InputCommandBindings.Count(b => b.Gesture is PlotterTouchGesture).Should().Be(1);
        }
    }

    /// <summary>
    /// Tests that the touch commands an application binds itself keep their behaviour.
    /// </summary>
    public class ExplicitTouchBindings
    {
        /// <summary>
        /// Creates a view whose controller has only the specified touch command bound.
        /// </summary>
        /// <param name="command">The touch command.</param>
        /// <returns>The view.</returns>
        private static RecordingPlotView CreateView(IViewCommand<PlotterTouchEventArgs> command)
        {
            var controller = new PlotController();
            controller.UnbindAll();
            controller.BindTouchDown(command);
            return new RecordingPlotView(controller);
        }

        [Fact]
        public void SnapTrackTouch_shows_the_tracker_hides_it_on_move_and_never_pans()
        {
            //Arrange
            var view = CreateView(PlotCommands.SnapTrackTouch);
            var start = view.ScreenOf(50, 50);

            //Act
            view.TouchDown(start);
            var shownOnDown = view.IsTrackerVisible;
            view.TouchMove(0, new ScreenPoint(start.X + 3, start.Y));
            var shownAfterMove = view.IsTrackerVisible;
            view.TouchMove(0, new ScreenPoint(start.X + 40, start.Y));
            view.TouchUpAll();

            //Assert
            shownOnDown.Should().BeTrue();
            shownAfterMove.Should().BeFalse();
            view.XAxis.ActualMinimum.Should().Be(0);
            view.XAxis.ActualMaximum.Should().Be(100);
        }

        [Fact]
        public void PointsOnlyTrackTouch_shows_the_tracker_until_release()
        {
            //Arrange
            var view = CreateView(PlotCommands.PointsOnlyTrackTouch);

            //Act
            view.TouchDown(view.ScreenOf(50, 50));
            var shownOnDown = view.IsTrackerVisible;
            view.TouchUpAll();

            //Assert
            shownOnDown.Should().BeTrue();
            view.IsTrackerVisible.Should().BeFalse();
        }

        [Fact]
        public void PanZoomByTouch_pans_from_the_first_move_and_shows_no_tracker()
        {
            //Arrange
            var view = CreateView(PlotCommands.PanZoomByTouch);
            var start = view.ScreenOf(50, 50);

            //Act
            view.TouchDown(start);
            view.TouchMove(0, new ScreenPoint(start.X + 4, start.Y));
            var underFinger = view.DataAt(new ScreenPoint(start.X + 4, start.Y));
            view.TouchUpAll();

            //Assert
            underFinger.X.Should().BeApproximately(50, 1e-6);
            view.XAxis.ActualMinimum.Should().BeLessThan(0);
            view.ShowTrackerCount.Should().Be(0);
        }
    }
}
