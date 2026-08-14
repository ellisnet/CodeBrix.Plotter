================================================================================
AGENT-README: CodeBrix.Plotter
A Comprehensive Guide for AI Coding Agents
================================================================================

OVERVIEW
--------
CodeBrix.Plotter is a plotting library for .NET 10.0+, built on SkiaSharp. It
describes a chart as an in-memory object graph -- a PlotModel holding axes,
series, annotations and legends -- and renders that model onto a SkiaSharp
canvas, or exports it to PNG, JPEG, PDF or SVG.

CodeBrix.Plotter is a port of the OxyPlot project (upstream `develop` branch,
commit 6b49a4ee: the v2.2.0 release plus the net10.0 / SkiaSharp upgrade
commits). It consolidates the upstream OxyPlot.Core and OxyPlot.SkiaSharp
packages into a single library and a single NuGet package.

All namespaces use "CodeBrix.Plotter" instead of "OxyPlot", and all public
types that upstream prefixed "Oxy" use the prefix "Plotter" instead --
PlotterColor, PlotterRect, PlotterSize, PlotterThickness, PlotterPen,
PlotterImage, PlotterPalette, PlotterMouseEventArgs, and so on. Do NOT use
OxyPlot namespaces or Oxy* type names; they do not exist in this library.


INSTALLATION
------------
NuGet Package: CodeBrix.Plotter.MitLicenseForever
Dependencies:
  - SkiaSharp
  - SkiaSharp.HarfBuzz

    dotnet add package CodeBrix.Plotter.MitLicenseForever

IMPORTANT: The NuGet package name is CodeBrix.Plotter.MitLicenseForever
(NOT CodeBrix.Plotter). The primary namespace is CodeBrix.Plotter.

Requirements: .NET 10.0 or higher
License: MIT License


SKIASHARP DEPENDENCIES
----------------------
CodeBrix.Plotter targets SkiaSharp 4.151.0 and SkiaSharp.HarfBuzz 4.151.0.
That exact version is shared across the CodeBrix family -- CodeBrix.Platform
depends on it -- so a CodeBrix.Platform application can consume this library
without a version conflict. The exact versions are recorded in the
`<PackageReference ... />` lines in
`src/CodeBrix.Plotter/CodeBrix.Plotter.csproj`.

The library takes a managed-only dependency on SkiaSharp. A consuming
application supplies the native asset package for its own platform
(SkiaSharp.NativeAssets.Linux, .Win32, .macOS, ...). The test project
references SkiaSharp.NativeAssets.Linux for that reason, because it renders
for real.

When bumping the SkiaSharp version, move SkiaSharp and SkiaSharp.HarfBuzz
together -- they ship as a matched set -- and keep the library and test
projects in lock-step.


KEY NAMESPACES
--------------
    using CodeBrix.Plotter;                 // PlotModel, PlotterColor, IRenderContext,
                                            //   the input event args and gestures, ...
    using CodeBrix.Plotter.Axes;            // LinearAxis, DateTimeAxis, CategoryAxis, ...
    using CodeBrix.Plotter.Series;          // LineSeries, BarSeries, PieSeries, ...
    using CodeBrix.Plotter.Annotations;     // TextAnnotation, ArrowAnnotation, ...
    using CodeBrix.Plotter.Legends;         // Legend, LegendBase
    using CodeBrix.Plotter.Utilities;       // Decimator, HistogramHelpers, ...
    using CodeBrix.Plotter.Skia;            // SkiaRenderContext and the exporters

There are only those seven namespaces. In particular there is no
CodeBrix.Plotter.Input, CodeBrix.Plotter.Rendering or CodeBrix.Plotter.Pdf --
the input model, the rendering abstractions and the PDF/SVG writers all live in
the root CodeBrix.Plotter namespace, even though their source files sit in
Input/, Rendering/, Pdf/ and Svg/ folders.


================================================================================
CORE API REFERENCE
================================================================================

PlotModel -- the root object
----------------------------
Everything starts with a PlotModel. It owns four collections and the plot-wide
appearance settings:

    var model = new PlotModel { Title = "My plot", Subtitle = "optional" };
    model.Axes.Add(...);          // ElementCollection<Axis>
    model.Series.Add(...);        // ElementCollection<Series>
    model.Annotations.Add(...);   // ElementCollection<Annotation>
    model.Legends.Add(...);       // ElementCollection<LegendBase>

A PlotModel must be updated before it can be rendered. Update() resolves axis
ranges from the data, assigns default axes to series that did not name one, and
recalculates everything derived. The update and render entry points are on the
IPlotModel interface, which PlotModel implements explicitly -- so you cast:

    ((IPlotModel)model).Update(updateData: true);
    ((IPlotModel)model).Render(renderContext, new PlotterRect(0, 0, w, h));

Pass updateData: true when the underlying data changed; false when only the
layout changed (a resize, for example).

A PlotModel is not thread-safe. Mutate it from one thread at a time, and call
model.InvalidatePlot(updateData) to ask an attached view to redraw.


Axes -- CodeBrix.Plotter.Axes
-----------------------------
    LinearAxis            evenly spaced numeric axis
    LogarithmicAxis       base-N logarithmic axis
    DateTimeAxis          axis over DateTime values; use
                          DateTimeAxis.ToDouble / ToDateTime to convert
    TimeSpanAxis          axis over TimeSpan values
    CategoryAxis          discrete labelled categories, for bar/column series
    AngleAxis             angular axis of a polar plot
    MagnitudeAxis         radial axis of a polar plot
    ColorAxis / LinearColorAxis / RangeColorAxis
                          maps values to colours for heat maps and contours

Common members: Position (AxisPosition.Left/Right/Top/Bottom), Title,
Minimum/Maximum (NaN means auto), MajorStep/MinorStep, StringFormat,
MajorGridlineStyle/MinorGridlineStyle, Key (referenced by a series'
XAxisKey/YAxisKey), IsZoomEnabled/IsPanEnabled, AbsoluteMinimum/AbsoluteMaximum.

Positioning two axes on the same side is done with StartPosition/EndPosition
(fractions of the plot area, 0 to 1), which is how stacked sub-plots are built.


Series -- CodeBrix.Plotter.Series
---------------------------------
Over forty series types, all derived from Series. The most used:

    LineSeries            points joined by a line; MarkerType for point markers
    AreaSeries            line series with the area to a baseline filled
    ScatterSeries         markers only, optionally sized/coloured per point
    BarSeries             horizontal bars against a CategoryAxis
    ColumnSeries          vertical columns against a CategoryAxis
    PieSeries             pie chart; PieSlice items rather than data points
    HeatMapSeries         2D value grid coloured through a ColorAxis
    ContourSeries         iso-lines over a 2D value grid
    HistogramSeries       binned frequency distribution
    CandleStickSeries     OHLC financial series
    BoxPlotSeries         box-and-whisker series
    ErrorBarSeries        bars with error indicators
    StairStepSeries       step-shaped line series
    StemSeries            vertical stems from a baseline
    FunctionSeries        LineSeries built by sampling a Func<double, double>
    TwoColorLineSeries / TwoColorAreaSeries
                          series coloured differently above/below a limit

Feeding data. Every XY series accepts explicit points:

    var line = new LineSeries();
    line.Points.Add(new DataPoint(x, y));

or binds to your own collection:

    line.ItemsSource = myItems;
    line.DataFieldX = nameof(MyItem.Time);
    line.DataFieldY = nameof(MyItem.Value);

DataPoint.Undefined (both coordinates NaN) breaks a line into segments.

Series that are not given an XAxisKey/YAxisKey bind to the first axis found in
the matching position.


Annotations -- CodeBrix.Plotter.Annotations
-------------------------------------------
    TextAnnotation, ArrowAnnotation, LineAnnotation, FunctionAnnotation,
    RectangleAnnotation, EllipseAnnotation, PointAnnotation, PolygonAnnotation,
    PolylineAnnotation, ImageAnnotation, TileMapAnnotation

Layer (AnnotationLayer.BelowAxes / BelowSeries / AboveSeries) decides where in
the drawing order an annotation appears.


Legends -- CodeBrix.Plotter.Legends
-----------------------------------
Add a Legend to model.Legends. Key members: LegendPosition, LegendPlacement
(Inside / Outside), LegendOrientation, LegendItemOrder, LegendMaxWidth /
LegendMaxHeight, and IsLegendVisible on the model itself. Series appear in the
legend when their Title is set and RenderInLegend is true.


Geometry and colour value types -- CodeBrix.Plotter
---------------------------------------------------
    DataPoint          a point in data space (X, Y)
    ScreenPoint        a point in device/screen space
    DataVector /
    ScreenVector       displacements in the corresponding space
    PlotterRect        a rectangle in screen space (Left, Top, Width, Height)
    PlotterSize        a width/height pair
    PlotterThickness   left/top/right/bottom margins
    PlotterColor       an immutable ARGB colour; PlotterColor.FromRgb,
                       .FromArgb, .FromAColor, .FromHsv; PlotterColor.Undefined
                       and .Automatic are sentinels, so test with IsUndefined()
                       / IsAutomatic() rather than == null
    PlotterColors      the named colour palette (PlotterColors.SkyBlue, ...)
    PlotterPalette     an ordered list of colours;
    PlotterPalettes    built-in palettes -- Jet, Rainbow, Hot, Gray, BlueWhiteRed,
                       Viridis, Plasma, Inferno, Magma, Cividis
    PlotterPen         stroke colour + thickness + dash pattern + line join
    PlotterImage       an encoded image (PNG/BMP/JPEG bytes) usable by
                       ImageAnnotation and IRenderContext.DrawImage

These are structs and are immutable; "modify" by constructing a new value.


Rendering -- CodeBrix.Plotter and CodeBrix.Plotter.Skia
-------------------------------------------------------
IRenderContext is the drawing abstraction the whole library targets: it draws
lines, polygons, ellipses, text and images, and measures text. Anything that
can draw can host a plot by implementing it; RenderContextBase supplies the
boilerplate.

SkiaRenderContext (CodeBrix.Plotter.Skia) is the SkiaSharp implementation.

    using var context = new SkiaRenderContext
    {
        RenderTarget = RenderTarget.Screen,   // Screen: hinted, subpixel text.
                                              // PixelGraphic / VectorGraphic:
                                              // unhinted, better for export.
        SkCanvas = canvas,                    // the SKCanvas to draw on
        DpiScale = 1.0f,                      // logical-to-device scale
        UseTextShaping = true,                // shape through HarfBuzz
        MiterLimit = 10,
    };

RendersToScreen is read-only and derived from RenderTarget; set RenderTarget.

    ((IPlotModel)model).Update(true);
    ((IPlotModel)model).Render(context, new PlotterRect(0, 0, width, height));

SkiaRenderContext caches SKTypeface, SKFont and SKShaper instances per font
descriptor, so keep one alive across frames when rendering repeatedly, and
dispose it when done. It does not own the SKCanvas and will not dispose it.

Text is shaped through HarfBuzz when the string needs it, and measured with
SKFont; RendersToScreen selects full hinting versus none.


Font resolution and the TypefaceResolver -- CodeBrix.Plotter.Skia
-----------------------------------------------------------------
By default SkiaRenderContext resolves a font family name through the system
font lookup (SKTypeface.FromFamilyName). The TypefaceResolver property (its
delegate type is also named TypefaceResolver, in the same namespace) replaces
that lookup:

    context.TypefaceResolver = (fontFamily, fontWeight) =>
        myFonts.TryGetValue(fontFamily, out SKTypeface found)
            ? found
            : myDefaultTypeface;

The contract, precisely:

  * While a resolver is assigned it owns resolution COMPLETELY. Every font
    family a draw or measure call names goes to the resolver; the system font
    lookup is never consulted, not even when the resolver cannot resolve the
    family. A host that must never render system fonts (the CodeBrix.Platform
    PlotterView add-in, for example) enforces that rule by assigning a
    resolver.
  * A resolver that cannot resolve a family should return its own default or
    fallback typeface rather than null. A null return is treated as
    SKTypeface.Default - still no system family lookup.
  * fontWeight arrives as the numeric weight (400 normal, 700 bold - the
    FontWeights constants).
  * Results are cached per (family, weight) pair, exactly like
    system-resolved typefaces, so the resolver runs once per distinct pair,
    not once per draw.
  * Assigning a different resolver (or null) clears the typeface and shaper
    caches. Re-assigning the delegate already in place is a no-op.
  * OWNERSHIP: typefaces returned by a resolver stay owned by the resolver -
    the render context never disposes them, neither on resolver change nor in
    Dispose(). (Typefaces from the system lookup are owned and disposed by
    the context, as always.)
  * TypefaceResolver is null by default, which is exactly the pre-existing
    behavior.


Exporters
---------
CodeBrix.Plotter.Skia -- SkiaSharp-backed, and the ones to prefer:

    PngExporter       .Export(model, stream) / .ExportToBitmap(model)
    JpegExporter      same, with Quality
    PdfExporter       vector PDF via SKDocument
    SvgExporter       vector SVG via SKSvgCanvas

    var exporter = new PngExporter { Width = 800, Height = 480, Dpi = 96 };
    using var stream = File.Create("plot.png");
    exporter.Export(model, stream);

CodeBrix.Plotter (root namespace) -- the dependency-free writer inherited from
OxyPlot core, which does not need SkiaSharp at all:

    SvgExporter       standalone SVG writer. Note this is the same type NAME as
                      the Skia one in a different namespace, so with both
                      `using` directives in scope you must qualify it.

Upstream also had a core `PdfExporter` alongside it. It was marked [Obsolete]
upstream in favour of the SkiaSharp one and is not part of CodeBrix.Plotter;
use CodeBrix.Plotter.Skia.PdfExporter. The PortableDocument machinery it was
built on is still here, because the core SvgExporter uses PdfRenderContext to
measure text.

The Skia exporters generally produce better text output because they shape and
measure with the real font; the core SVG writer is there when you want zero
native dependencies.


Input and controllers -- CodeBrix.Plotter and CodeBrix.Plotter.Input
--------------------------------------------------------------------
The library ships a UI-framework-independent input model so a host view can
map its own events onto plot commands:

    PlotterMouseEventArgs, PlotterMouseDownEventArgs, PlotterMouseWheelEventArgs,
    PlotterTouchEventArgs, PlotterKeyEventArgs      -- event arguments
    PlotterMouseDownGesture, PlotterMouseEnterGesture, PlotterMouseWheelGesture,
    PlotterTouchGesture, PlotterKeyGesture, PlotterShakeGesture  -- gestures
    PlotterMouseButton, PlotterKey, PlotterModifierKeys          -- enumerations

PlotController binds gestures to commands (pan, zoom, zoom-rectangle, tracker,
reset). IPlotView is the interface a host view implements; PlotModel attaches
to one through IPlotModel.AttachPlotView.

This library does NOT provide a view. Rendering a plot in an application means
either drawing onto an SKCanvas yourself, or consuming a companion add-in that
supplies the view.


================================================================================
ARCHITECTURE
================================================================================

    src/CodeBrix.Plotter/
        Annotations/      annotation types (CodeBrix.Plotter.Annotations)
        Axes/             axis types (CodeBrix.Plotter.Axes)
          Rendering/      axis renderers (CodeBrix.Plotter.Axes.Rendering)
        Foundation/       DataPoint, DataVector, PlotLength, MarkerType, and
                          the code-generation helpers
        Graphics/         Element, Model, selection and hit-testing plumbing
        Imaging/          PNG and BMP decoders, PNG encoder, Deflate
        Input/            event arguments, gestures, key/button enumerations
        Legends/          Legend and LegendBase
        Pdf/              the dependency-free PortableDocument PDF writer
        PlotController/   controllers, commands and manipulators
        PlotModel/        PlotModel and its partial classes
        PlotView/         IPlotView
        Rendering/        IRenderContext, RenderContextBase, PlotterColor,
                          PlotterRect, PlotterPen, palettes, text measurement
        Series/           series types (CodeBrix.Plotter.Series)
          FinancialSeries/  candlestick and volume series
        Skia/             SkiaRenderContext and the SkiaSharp exporters
                          (CodeBrix.Plotter.Skia)
        Svg/              the dependency-free SVG writer
        Utilities/        decimation, binning, histogram and math helpers

Folders mostly follow the upstream OxyPlot layout, which already groups by
concern. Note that folder does not always equal namespace: Foundation/,
Graphics/, Imaging/, Input/, Pdf/, PlotController/, PlotModel/, PlotView/,
Rendering/ and Svg/ all live in the root CodeBrix.Plotter namespace, while
Annotations/, Axes/, Legends/, Series/ and Skia/ carry sub-namespaces.

Every ported file records where it came from: its namespace line carries a
`//was previously: <upstream-namespace>;` comment, and its file header keeps
the upstream OxyPlot copyright notice verbatim.


================================================================================
CODING CONVENTIONS (CodeBrix family)
================================================================================

These apply to any change made to this repository.

  * Target framework is net10.0 only. No multi-targeting, ever.
  * Nullable reference types are OFF. Never write `?` on a reference type
    (`string?`, `MyClass?`, `object?`) and never use the null-forgiveness `!`
    operator. Value-type nullables (`int?`, `bool?`, `DateTime?`) are fine.
  * No `#nullable` directives anywhere.
  * No `global using` and no ImplicitUsings. Every file declares its own
    `using` directives, at the top, in one contiguous block, System.* first.
  * File-scoped namespaces only (`namespace X;`). Never block-scoped.
  * XML doc comments are required on every public and protected member --
    `<GenerateDocumentationFile>` is on, so CS1591 fires otherwise. Fix it by
    writing the comment, never by suppressing the warning.
  * No warning suppression at project level: no `<NoWarn>`, no
    `<WarningLevel>0</WarningLevel>`, no `<TreatWarningsAsErrors>false</>`.
    A clean build is 0 warnings and 0 errors.
  * Do not fabricate top-of-file banners on new files. Preserve the upstream
    OxyPlot header on ported files.
  * Tests use xUnit.v3 with SilverAssertions. Test classes are named
    `<ClassUnderTest>Tests`; test methods are `<Member>_<snake_case>` or plain
    snake_case; multi-statement bodies carry `//Arrange` `//Act` `//Assert`
    comments; single-statement tests are expression-bodied.
  * Any call inside a test that accepts a CancellationToken must be passed
    `TestContext.Current.CancellationToken` (xUnit1051).


================================================================================
TESTING
================================================================================

    dotnet test CodeBrix.Plotter.slnx

The test project is `tests/CodeBrix.Plotter.Tests`. It is the upstream OxyPlot
test suite -- the tests for OxyPlot.Core and OxyPlot.SkiaSharp only -- ported
to xUnit.v3 + SilverAssertions.

    tests/CodeBrix.Plotter.Tests/
        <mirrors the library folders>   core tests
        Skia/                           SkiaRenderContext and exporter tests
        ExampleLibrary/                 the upstream ExampleLibrary, ported as
                                        TEST-SUPPORT CODE ONLY. Several tests
                                        enumerate every example model and
                                        assert that updating, rendering and
                                        exporting it throws nothing. This code
                                        is NOT part of the shipped library and
                                        NOT in the NuGet package.

The library grants the test project access to its internals through
`src/CodeBrix.Plotter/InternalsVisibleTo.cs`.

Tests that render or export write their output under the test run's working
directory (bin/Debug/net10.0/), in per-test folders.

Expect the full suite to take a long time -- tens of minutes. Several tests
enumerate EVERY example model, in its normal, transposed and reversed forms,
and export each one to SVG, PDF, PNG or JPEG; that is thousands of real
renders. This is inherited from the upstream OxyPlot suite and is the point of
those tests: they are a broad smoke test that no series, axis or annotation
type throws while being laid out and drawn. To iterate quickly on one area,
filter, e.g.:

    dotnet test CodeBrix.Plotter.slnx --filter "FullyQualifiedName~AxisTests"

`PlotterAssert.AreEqual(plot, name)` compares a rendered SVG against a file in
a `baseline/` folder. On the first run for a given name there is no baseline,
so it writes one and passes; later runs compare against it. It therefore
catches CHANGES between runs on the same machine, not correctness in the
absolute -- a clean checkout always passes.


================================================================================
COMMON PITFALLS TO AVOID
================================================================================

  * Forgetting to Update() the model. A freshly built PlotModel has no
    resolved axis ranges; rendering it without `((IPlotModel)model).Update(true)`
    produces an empty or wrong plot.
  * Reaching for Update()/Render() directly on PlotModel. They are explicit
    IPlotModel implementations -- you must cast.
  * Comparing PlotterColor against null. It is a struct with sentinel values;
    use IsUndefined(), IsAutomatic(), IsInvisible() instead.
  * Mutating a PlotModel from several threads, or while it is rendering.
  * Ambiguity between `CodeBrix.Plotter.SvgExporter` (the dependency-free core
    writer) and `CodeBrix.Plotter.Skia.SvgExporter` (the SkiaSharp one). With
    both namespaces imported you must qualify the type name.
  * Looking for the legacy input EVENTS. `PlotModel.MouseDown`,
    `Element.MouseMove`, `Axis.AxisChanged`, `PlotModel.TrackerChanged`,
    `ElementCollection<T>.CollectionChanged` and the rest of that family were
    all marked [Obsolete] upstream and are NOT part of this library. Handle
    input through PlotController and the gesture-to-command bindings, which is
    the model upstream is migrating to.
  * Expecting a view. This library renders; it does not host. There is no
    PlotView control here.
  * Reusing a SkiaRenderContext across canvases without resetting SkCanvas, or
    disposing the SKCanvas it was handed -- it does not own the canvas.
  * Expecting a TypefaceResolver miss to fall back to the system font lookup.
    It never does -- return a default typeface from the resolver instead of
    null. And do not dispose a resolver-supplied typeface while the render
    context might still draw with it; the context caches it but the resolver
    owns it.
  * Using OxyPlot names. There is no OxyPlot namespace and no Oxy* type in this
    library; they are CodeBrix.Plotter and Plotter*.


================================================================================
WHAT THIS LIBRARY DOES NOT DO
================================================================================

  * It provides no UI control for any framework. The upstream OxyPlot.Wpf,
    OxyPlot.WindowsForms and similar view packages are not part of this port.
  * It does not render through System.Drawing, ImageSharp or PdfSharp. The
    upstream OxyPlot.Core.Drawing, OxyPlot.ImageSharp and OxyPlot.Pdf packages
    are not part of this port.
  * It does not ship native SkiaSharp binaries. A consuming application adds
    the SkiaSharp.NativeAssets.* package for its own platform.


================================================================================
END OF AGENT-README
================================================================================
