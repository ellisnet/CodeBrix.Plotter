using System;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Skia;
using SkiaSharp;

// ReSharper disable once CheckNamespace
namespace PicoScope.Rendering;

/// <summary>
/// Draws a CodeBrix.Plotter <see cref="PlotModel"/> onto a SkiaSharp surface.
/// </summary>
/// <remarks>
/// <para>
/// CodeBrix.Plotter renders but does not host -- there is no plot view control
/// for any UI framework -- so each head supplies its own canvas and calls this
/// from its paint handler. That is the whole integration: a couple of dozen
/// lines shared by WPF and WinUI alike.
/// </para>
/// <para>
/// The <see cref="SkiaRenderContext"/> caches typefaces, fonts and shapers per
/// font descriptor, so one instance is kept alive across frames rather than
/// being rebuilt each paint. It does not own the canvas it is handed and will
/// not dispose it.
/// </para>
/// </remarks>
public sealed class ScopeChartRenderer : IDisposable
{
    private readonly SkiaRenderContext _context;
    private bool _disposed;

    /// <summary>
    /// Creates a renderer.
    /// </summary>
    public ScopeChartRenderer()
    {
        _context = new SkiaRenderContext
        {
            RenderTarget = RenderTarget.Screen,
            UseTextShaping = true,
            MiterLimit = 10
        };
    }

    /// <summary>
    /// The colour painted behind the plot.
    /// </summary>
    public SKColor BackgroundColor { get; set; } = new SKColor(24, 24, 30);

    /// <summary>
    /// Draws the model to fill the given surface.
    /// </summary>
    /// <param name="model">The plot to draw.</param>
    /// <param name="surface">The surface to draw onto.</param>
    /// <param name="info">The surface's pixel dimensions.</param>
    /// <param name="dpiScale">The logical-to-device scale factor.</param>
    /// <remarks>
    /// Must be called on the thread that owns the model. A
    /// <see cref="PlotModel"/> is not thread-safe, and must not be mutated while
    /// it is being rendered.
    /// </remarks>
    public void Render(PlotModel model, SKSurface surface, SKImageInfo info, float dpiScale = 1.0f)
    {
        if (surface == null) { throw new ArgumentNullException(nameof(surface)); }

        SKCanvas canvas = surface.Canvas;
        canvas.Clear(BackgroundColor);

        if (model == null || info.Width <= 0 || info.Height <= 0) { return; }

        _context.SkCanvas = canvas;
        _context.DpiScale = dpiScale;

        //Update() resolves axis ranges from the data. Without it a freshly built
        //  model renders empty. Pass false because only the layout is changing
        //  here -- the data was already applied when it was set.
        ((IPlotModel)model).Update(false);
        ((IPlotModel)model).Render(_context, new PlotterRect(0, 0, info.Width, info.Height));
    }

    /// <summary>
    /// Releases the cached font resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) { return; }
        _disposed = true;
        _context.Dispose();
    }
}
