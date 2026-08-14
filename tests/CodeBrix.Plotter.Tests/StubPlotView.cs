using System;
using CodeBrix.Plotter;

namespace CodeBrix.Plotter.Tests;

/// <summary>
/// A do-nothing <see cref="IPlotView" /> for tests that need a view instance
/// but not view behaviour.
/// </summary>
/// <remarks>
/// This is deliberately a plain class rather than a mocking-framework proxy:
/// <c>PlotModelTests.PlotControl_CollectedPlotControl_ReferenceShouldNotBeAlive</c>
/// asserts that nothing keeps the view alive once the test drops its
/// reference, and a dynamic proxy can be rooted by the mocking framework's own
/// caches.
/// </remarks>
public class StubPlotView : IPlotView
{
    /// <inheritdoc />
    public PlotModel ActualModel { get; set; }

    /// <remarks>
    /// <see cref="IPlotView" /> narrows this to <see cref="PlotModel" />, so the
    /// wider <see cref="IView" /> declaration is implemented explicitly.
    /// </remarks>
    Model IView.ActualModel => this.ActualModel;

    /// <inheritdoc />
    public IController ActualController { get; set; }

    /// <inheritdoc />
    public PlotterRect ClientArea { get; set; }

    /// <inheritdoc />
    public void HideTracker()
    {
    }

    /// <inheritdoc />
    public void HideZoomRectangle()
    {
    }

    /// <inheritdoc />
    public void InvalidatePlot(bool updateData = true)
    {
    }

    /// <inheritdoc />
    public void SetClipboardText(string text)
    {
    }

    /// <inheritdoc />
    public void SetCursorType(CursorType cursorType)
    {
    }

    /// <inheritdoc />
    public void ShowTracker(TrackerHitResult trackerHitResult)
    {
    }

    /// <inheritdoc />
    public void ShowZoomRectangle(PlotterRect rectangle)
    {
    }
}
