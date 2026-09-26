using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests;

/// <summary>
/// Tests the <see cref="PlotterTouchEventArgs" /> class.
/// </summary>
public class PlotterTouchEventArgsTests
{
    [Fact]
    public void constructor_keeps_the_touch_arrays()
    {
        //Arrange
        var current = new[] { new ScreenPoint(10, 10), new ScreenPoint(50, 10) };
        var previous = new[] { new ScreenPoint(10, 10), new ScreenPoint(30, 10) };

        //Act
        var e = new PlotterTouchEventArgs(current, previous);

        //Assert
        e.CurrentTouches.Should().BeSameAs(current);
        e.PreviousTouches.Should().BeSameAs(previous);
        e.Position.Should().Be(current[0]);
        e.DeltaScale.X.Should().BeApproximately(2, 1e-12);
    }

    [Fact]
    public void parameterless_constructor_leaves_the_touch_arrays_null()
    {
        //Act
        var e = new PlotterTouchEventArgs();

        //Assert
        e.CurrentTouches.Should().BeNull();
        e.PreviousTouches.Should().BeNull();
    }
}
