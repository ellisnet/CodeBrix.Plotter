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
}
