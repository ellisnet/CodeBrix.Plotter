================================================================================
AGENT-README: CodeBrix.Plotter
A Guide for AI Coding Agents — CONSUMING the CodeBrix.Plotter.MitLicenseForever
NuGet package
================================================================================

OVERVIEW
========

CodeBrix.Plotter is a plotting library for .NET 10 or later, built on SkiaSharp.
It describes a chart as an in-memory object graph -- a PlotModel holding axes,
series, annotations and legends -- and renders that graph onto a SkiaSharp
canvas, or exports it to PNG, JPEG, PDF or SVG.

Provenance. CodeBrix.Plotter is a port of the OxyPlot project (upstream
`develop` branch, commit 6b49a4ee: the v2.2.0 release plus the net10.0 /
SkiaSharp upgrade commits). It consolidates the upstream OxyPlot.Core and
OxyPlot.SkiaSharp packages into one library and one NuGet package. All
namespaces use "CodeBrix.Plotter" instead of "OxyPlot", and every public type
that upstream prefixed "Oxy" uses the prefix "Plotter" instead --
PlotterColor, PlotterColors, PlotterRect, PlotterSize, PlotterThickness,
PlotterPen, PlotterPalette, PlotterPalettes, PlotterImage,
PlotterMouseEventArgs, PlotterKey, and so on. Do NOT use OxyPlot namespaces or
Oxy* type names; they do not exist in this library. Names that upstream did not
prefix (PlotModel, LineSeries, LinearAxis, DataPoint, ScreenPoint, ...) are
unchanged.


INSTALLATION
============

NuGet PackageId: CodeBrix.Plotter.MitLicenseForever

    dotnet add package CodeBrix.Plotter.MitLicenseForever

IMPORTANT: the package id is CodeBrix.Plotter.MitLicenseForever, NOT
"CodeBrix.Plotter". The assembly and the root namespace are CodeBrix.Plotter.

NuGet dependencies (by id, no versions -- take what the package resolves):

    SkiaSharp
    SkiaSharp.HarfBuzz

License: MIT.

Requirements and OS limits:

  * .NET 10 or later. The library is net10.0-only; it does not multi-target and
    there is no netstandard build.
  * No native binaries ship in this package. The dependency on SkiaSharp is
    managed-only, so a consuming APPLICATION must add the SkiaSharp native
    asset package for its own platform:

        SkiaSharp.NativeAssets.Linux        (Linux)
        SkiaSharp.NativeAssets.Win32        (Windows)
        SkiaSharp.NativeAssets.macOS        (macOS)

    On Linux, text shaping additionally needs HarfBuzzSharp.NativeAssets.Linux;
    HarfBuzzSharp brings the Windows and macOS natives in transitively but not
    the Linux one, so shaping any text on Linux fails to load libHarfBuzzSharp
    without it.
  * The SkiaSharp and SkiaSharp.HarfBuzz versions are pinned in lock-step with
    the rest of the CodeBrix family, so a CodeBrix.Platform application can
    consume this library without a version conflict. If you pin SkiaSharp
    yourself, move SkiaSharp and SkiaSharp.HarfBuzz together -- they ship as a
    matched set.
  * There is no UI control in this package. See WHAT THIS PACKAGE DOES NOT DO.


KEY NAMESPACES / USINGS
=======================

    using CodeBrix.Plotter;                 // PlotModel, IPlotModel, IRenderContext,
                                            //   PlotterColor(s), PlotterRect,
                                            //   DataPoint,
                                            //   ScreenPoint, MarkerType, LineStyle,
                                            //   the input model, the controller, the
                                            //   image codecs, the core SVG writer
    using CodeBrix.Plotter.Axes;            // Axis, LinearAxis, DateTimeAxis,
                                            //   CategoryAxis, the colour axes,
                                            //   AxisPosition, TickStyle, AxisLayer
    using CodeBrix.Plotter.Series;          // Series, LineSeries, BarSeries,
                                            //   PieSeries, the item types,
                                            //   LabelPlacement, VolumeStyle
    using CodeBrix.Plotter.Annotations;     // Annotation, TextAnnotation,
                                            //   ArrowAnnotation, AnnotationLayer, ...
    using CodeBrix.Plotter.Legends;         // Legend, LegendBase, LegendPosition,
                                            //   LegendPlacement, LegendOrientation, ...
    using CodeBrix.Plotter.Axes.Rendering;  // axis renderers (only needed to write a
                                            //   custom axis renderer)
    using CodeBrix.Plotter.Utilities;       // Helpers (Swap, ArgMin,
                                            //   LinearInterpolation) -- ONLY that
    using CodeBrix.Plotter.Skia;            // SkiaRenderContext, RenderTarget,
                                            //   TypefaceResolver, SkiaExtensions,
                                            //   PngExporter, JpegExporter,
                                            //   PdfExporter, SvgExporter

Those seven namespaces are the whole public surface. FOLDER IS NOT NAMESPACE in
this library, and it is the single most common source of a "type or namespace
not found" error:

  * There is NO CodeBrix.Plotter.Input, CodeBrix.Plotter.Rendering,
    CodeBrix.Plotter.Graphics, CodeBrix.Plotter.PlotModel,
    CodeBrix.Plotter.PlotController, CodeBrix.Plotter.PlotView,
    CodeBrix.Plotter.Imaging, CodeBrix.Plotter.Pdf, CodeBrix.Plotter.Svg or
    CodeBrix.Plotter.Foundation namespace, even though the source has folders
    with those names. Everything in them is in the ROOT CodeBrix.Plotter
    namespace.
  * CodeBrix.Plotter.Utilities contains exactly ONE public type: the static
    class Helpers. Decimator, HistogramHelpers, BinningOptions, ArrayBuilder,
    ArrayExtensions, Conrec, FractionHelper, ListBuilder<T>, ReflectionPath,
    StringHelper, ComparerHelper, HashCodeBuilder, XmlWriterBase,
    CanonicalSpline, CatmullRomSpline and InterpolationAlgorithms are all in the
    ROOT namespace despite living in Utilities/ folders.


================================================================================
CORE API REFERENCE
================================================================================

THE PLOT MODEL
==============

PlotModel -- the root object
----------------------------
Everything starts with a PlotModel. It owns four element collections plus the
plot-wide appearance settings:

    var model = new PlotModel { Title = "My plot", Subtitle = "optional" };
    model.Axes.Add(...);          // ElementCollection<Axis>
    model.Series.Add(...);        // ElementCollection<Series>
    model.Annotations.Add(...);   // ElementCollection<Annotation>
    model.Legends.Add(...);       // ElementCollection<LegendBase>

Appearance and layout members (all on PlotModel):

    string Title, Subtitle, TitleToolTip
    string TitleFont, SubtitleFont, DefaultFont
    double TitleFontSize, TitleFontWeight, SubtitleFontSize, SubtitleFontWeight,
           DefaultFontSize, TitlePadding, TitleClippingLength
    bool   ClipTitle
    PlotterColor TitleColor, SubtitleColor, TextColor, Background,
           PlotAreaBackground, PlotAreaBorderColor
    TitleHorizontalAlignment TitleHorizontalAlignment   // CenteredWithinPlotArea |
                                                        //   CenteredWithinView
    PlotterThickness Padding, PlotMargins, PlotAreaBorderThickness
    PlotType PlotType                                   // XY | Cartesian | Polar
    EdgeRenderingMode EdgeRenderingMode
    IList<PlotterColor> DefaultColors                   // series auto-colour cycle
    bool   AssignColorsToInvisibleSeries, IsLegendVisible
    double AxisTierDistance
    CultureInfo Culture; CultureInfo ActualCulture      // read-only
    Func<IRenderContext, IRenderContext> RenderingDecorator

Read-only geometry, valid after Update():

    PlotterRect PlotBounds, PlotArea, PlotAndAxisArea, TitleArea
    PlotterThickness ActualPlotMargins
    double Width, Height                                // == PlotBounds.Width/Height
    Axis DefaultXAxis, DefaultYAxis
    AngleAxis DefaultAngleAxis; MagnitudeAxis DefaultMagnitudeAxis
    IColorAxis DefaultColorAxis
    IPlotView PlotView

Methods:

    void  InvalidatePlot(bool updateData)
    void  GetAxesFromPoint(ScreenPoint pt, out Axis xaxis, out Axis yaxis)
    Axis  GetAxis(string key)                       // throws if not found
    Axis  GetAxisOrDefault(string key, Axis defaultAxis)
    LegendBase GetLegend(string key)
    Series GetSeriesFromPoint(ScreenPoint point, double limit = 100)
    PlotterColor GetDefaultColor()
    LineStyle    GetDefaultLineStyle()
    void  ResetAllAxes()
    void  PanAllAxes(double dx, double dy)
    void  ZoomAllAxes(double factor)
    Exception GetLastPlotException()
    string ToCode()                                 // C# source that rebuilds the model


IPlotModel -- update and render
-------------------------------
PlotModel implements IPlotModel EXPLICITLY, so Update() and Render() are not
visible on the PlotModel variable. You must cast:

    public interface IPlotModel
    {
        PlotterColor Background { get; }
        void Update(bool updateData);
        void Render(IRenderContext rc, PlotterRect rect);
        void AttachPlotView(IPlotView plotView);
    }

    ((IPlotModel)model).Update(updateData: true);
    ((IPlotModel)model).Render(renderContext, new PlotterRect(0, 0, w, h));

Pass updateData: true when the underlying data changed; false when only the
layout changed (a resize, for example). A freshly built PlotModel has no
resolved axis ranges: rendering it without an Update(true) first produces an
empty or wrong plot.

Note that the EXPORTERS take IPlotModel and call Update()/Render() themselves,
so no cast is needed when you go through PngExporter and friends -- passing a
PlotModel where IPlotModel is expected is an ordinary implicit conversion.

Errors raised while a plot renders are captured, not thrown out of Render();
call model.GetLastPlotException() to retrieve one.

A PlotModel is not thread-safe. Mutate it from one thread at a time, and never
while it is rendering. Model.SyncRoot is available as a lock object.


Element, PlotElement and ElementCollection<T>
---------------------------------------------
Every axis, series, annotation and legend derives from PlotElement, which
derives from Element. The members you will actually set:

    // PlotElement
    string Font; double FontSize; double FontWeight;   // FontWeights.Normal/.Bold
    PlotterColor TextColor
    EdgeRenderingMode EdgeRenderingMode
    object Tag
    string ToolTip
    PlotModel PlotModel                                 // read-only back-pointer

    // Element
    Model Parent                                        // read-only
    HitTestResult HitTest(HitTestArguments args)

ElementCollection<T> is an IList<T>/IReadOnlyList<T> that re-parents what you
add and clears the parent of what you remove; it is the type of
model.Axes/Series/Annotations/Legends. It exposes the usual list surface (Add,
Insert, Remove, RemoveAt, Clear, Contains, IndexOf, CopyTo, indexer, Count).


AXES
====

All axis types live in CodeBrix.Plotter.Axes and derive from Axis:

    Axis                        abstract base
      LinearAxis                evenly spaced numeric axis
        CategoryAxis            discrete labelled categories (bar/column charts)
          CategoryColorAxis     CategoryAxis that is also a colour axis
        DateTimeAxis            axis over DateTime values
        TimeSpanAxis            axis over TimeSpan values
        AngleAxis               angular axis of a polar plot
          AngleAxisFullPlotArea       angle axis spanning the full plot area
        MagnitudeAxis           radial axis of a polar plot
          MagnitudeAxisFullPlotArea   magnitude axis spanning the full plot area
        LinearColorAxis         value -> colour through a PlotterPalette
        RangeColorAxis          value -> colour through explicit ranges
      LogarithmicAxis           base-N logarithmic axis
        LogarithmicColorAxis    logarithmic value -> colour through a palette

Axis -- the members you set most
--------------------------------
    AxisPosition Position          // None | Left | Right | Top | Bottom | All
    string Title, Unit, Key, StringFormat, TitleFormatString
    double Minimum, Maximum                 // NaN = auto from the data
    double AbsoluteMinimum, AbsoluteMaximum // hard pan/zoom limits
    double MinimumRange, MaximumRange
    double MajorStep, MinorStep, MinimumMajorStep, MinimumMinorStep
    double MinimumMajorIntervalCount, MaximumMajorIntervalCount, IntervalLength
    double MinimumPadding, MaximumPadding   // fraction of the range, both ends
    double MinimumMargin, MaximumMargin, MinimumDataMargin, MaximumDataMargin
    LineStyle MajorGridlineStyle, MinorGridlineStyle, AxislineStyle,
              ExtraGridlineStyle
    PlotterColor MajorGridlineColor, MinorGridlineColor, AxislineColor,
              TicklineColor, MinorTicklineColor, ExtraGridlineColor, TitleColor
    double MajorGridlineThickness, MinorGridlineThickness, AxislineThickness,
           ExtraGridlineThickness, MajorTickSize, MinorTickSize
    double[] ExtraGridlines
    TickStyle TickStyle            // Crossing | Inside | Outside | None
    AxisLayer Layer                // BelowSeries | AboveSeries
    bool IsAxisVisible, IsPanEnabled, IsZoomEnabled, IsReversed,
         PositionAtZeroCrossing, CropGridlines, ClipTitle,
         UseSuperExponentialFormat
    double StartPosition, EndPosition   // 0..1 fractions of the plot area
    int    PositionTier                 // stacking several axes on one side
    double Angle, AxisDistance, AxisTickToLabelDistance, AxisTitleDistance,
           TitlePosition, TitleClippingLength, TitleFontSize, TitleFontWeight
    string TitleFont
    Func<double, string> LabelFormatter
    Func<double, bool>   FilterFunction
    double FilterMinValue, FilterMaxValue

Read-only after Update(): ActualMinimum, ActualMaximum, ActualMajorStep,
ActualMinorStep, ActualStringFormat, ActualTitle, ClipMinimum, ClipMaximum,
DataMinimum, DataMaximum, ScreenMin, ScreenMax, DesiredMargin, Scale, Offset.

Methods:

    bool   IsHorizontal(); bool IsVertical(); bool IsXyAxis(); bool IsLogarithmic()
    bool   IsValidValue(double value)
    string FormatValue(double x)
    double Transform(double x); double InverseTransform(double sx)
    ScreenPoint Transform(double x, double y, Axis yaxis)
    DataPoint   InverseTransform(double x, double y, Axis yaxis)
    static DataPoint InverseTransform(ScreenPoint p, Axis xaxis, Axis yaxis)
    static double    ToDouble(object value)
    void   Pan(double delta); void Pan(ScreenPoint ppt, ScreenPoint cpt)
    void   Zoom(double newScale); void Zoom(double x0, double x1)
    void   ZoomAt(double factor, double x); void ZoomAtCenter(double factor)
    void   Reset(); void Include(double value)

Two axes on the same side are laid out with StartPosition/EndPosition (fractions
of the plot area, 0 to 1) or PositionTier; that is how stacked sub-plots are
built.

LinearAxis
----------
    bool   FormatAsFractions
    double FractionUnit
    string FractionUnitSymbol

LogarithmicAxis
---------------
    double Base                  // default 10
    bool   PowerPadding
    static readonly double LowestValidRoundtripValue

CategoryAxis (the axis a BarSeries needs)
-----------------------------------------
    List<string>    Labels           // add one label per category, in order
    IList<string>   ActualLabels     // read-only, after Update()
    IEnumerable     ItemsSource      // bind categories instead of adding Labels
    string          LabelField       // property on the ItemsSource items
    double          GapWidth         // gap between category groups, default 1
    bool            IsTickCentered

DateTimeAxis
------------
    DateTimeIntervalType IntervalType, MinorIntervalType
        // Auto | Manual | Milliseconds | Seconds | Minutes | Hours | Days |
        //   Weeks | Months | Years
    CalendarWeekRule CalendarWeekRule
    DayOfWeek        FirstDayOfWeek
    TimeSpan         DateTimePrecision
    TimeZoneInfo     TimeZone
    static readonly TimeSpan DefaultPrecision

    static double   ToDouble(DateTime value)
    static DateTime ToDateTime(double value, TimeSpan precision)
    DateTime        ConvertToDateTime(double value)
    static DataPoint CreateDataPoint(DateTime x, double y)
    static DataPoint CreateDataPoint(DateTime x, DateTime y)
    static DataPoint CreateDataPoint(double x, DateTime y)

Plot DateTime data by converting it: `new DataPoint(DateTimeAxis.ToDouble(t), v)`
or `DateTimeAxis.CreateDataPoint(t, v)`.

TimeSpanAxis
------------
    static double   ToDouble(TimeSpan s)
    static TimeSpan ToTimeSpan(double value)

AngleAxis / MagnitudeAxis (polar plots; set model.PlotType = PlotType.Polar)
---------------------------------------------------------------------------
    AngleAxis: double StartAngle, EndAngle
    AngleAxisFullPlotArea and MagnitudeAxisFullPlotArea are the variants that
    use the whole plot area rather than a centred square.

Colour axes
-----------
A colour axis maps a value to a colour; HeatMapSeries, ContourSeries,
RectangleSeries, VectorSeries and ScatterSeries can all take one, found by
ColorAxisKey or, failing that, by being the model's only colour axis.

    interface IColorAxis : IPlotElement
    {
        PlotterColor GetColor(int paletteIndex);
        int          GetPaletteIndex(double value);
    }
    interface INumericColorAxis : IColorAxis { }

    LinearColorAxis      : LinearAxis,      INumericColorAxis
    LogarithmicColorAxis : LogarithmicAxis, INumericColorAxis
    CategoryColorAxis    : CategoryAxis,    IColorAxis
    RangeColorAxis       : LinearAxis,      IColorAxis

    // LinearColorAxis / LogarithmicColorAxis
    PlotterPalette Palette          // defaults to PlotterPalettes.Viridis()
    PlotterColor   HighColor, LowColor, InvalidNumberColor
    bool           RenderAsImage
    int            GetPaletteIndex(double value)

    // CategoryColorAxis
    PlotterPalette Palette
    PlotterColor   InvalidCategoryColor
    PlotterColor   GetColor(int paletteIndex)

    // RangeColorAxis
    void AddRange(double lowerBound, double upperBound, PlotterColor color)
    void ClearRanges()
    PlotterColor HighColor, LowColor, InvalidNumberColor

ColorAxisExtensions adds:

    PlotterColor GetColor(this IColorAxis axis, double value)
    PlotterColor GetColor<T>(this T axis, int paletteIndex)
    double GetHighValue<T>(this T axis, int paletteIndex)
    double GetLowValue<T>(this T axis, int paletteIndex)
        (the three generic overloads are constrained to
         where T : Axis, INumericColorAxis)

AxisUtilities (static, CodeBrix.Plotter.Axes) is available for custom axes:
CalculateMinorInterval, CalculateMinorInterval2, CreateTickValues(from, to,
step, maxTicks = 1000), FilterRedundantMinorTicks.

Axis renderers live in CodeBrix.Plotter.Axes.Rendering and are only needed if
you write a custom axis: AxisRendererBase<T>, HorizontalAndVerticalAxisRenderer,
HorizontalAndVerticalAxisRenderer<T>, ColorAxisRenderer<T>,
NumericColorAxisRenderer<T>, CategoryColorAxisRenderer, RangeColorAxisRenderer,
AngleAxisRenderer, AngleAxisFullPlotAreaRenderer, MagnitudeAxisRenderer,
MagnitudeAxisFullPlotAreaRenderer.


SERIES
======

All series live in CodeBrix.Plotter.Series. THE MOST COMMON MISTAKE IS GUESSING
THE ITEM TYPE, so the table below pairs every concrete series with the exact
collection you fill and the exact item type it holds.

    SERIES                  FILL THIS               WITH THIS ITEM TYPE
    ---------------------   ---------------------   ------------------------
    LineSeries              .Points                 DataPoint
    AreaSeries              .Points and .Points2    DataPoint
    TwoColorLineSeries      .Points                 DataPoint
    TwoColorAreaSeries      .Points and .Points2    DataPoint
    ThreeColorLineSeries    .Points                 DataPoint
    ExtrapolationLineSeries .Points (+ .Intervals)  DataPoint (+ DataRange)
    StairStepSeries         .Points                 DataPoint
    StemSeries              .Points                 DataPoint
    LinearBarSeries         .Points                 DataPoint
    FunctionSeries          (constructor)           built from a Func
    ScatterSeries           .Points                 ScatterPoint
    ScatterErrorSeries      .Points                 ScatterErrorPoint
    BarSeries               .Items                  BarItem
    ErrorBarSeries          .Items                  ErrorBarItem
    IntervalBarSeries       .Items                  IntervalBarItem
    TornadoBarSeries        .Items                  TornadoBarItem
    RectangleBarSeries      .Items                  RectangleBarItem
    BoxPlotSeries           .Items                  BoxPlotItem
    HistogramSeries         .Items                  HistogramItem
    RectangleSeries         .Items                  RectangleItem
    VectorSeries            .Items                  VectorItem
    HighLowSeries           .Items                  HighLowItem
    CandleStickSeries       .Items                  HighLowItem
    VolumeSeries            .Items                  OhlcvItem
    PieSeries               .Slices                 PieSlice
    HeatMapSeries           .Data                   double[,]
    ContourSeries           .Data + .ColumnCoordinates + .RowCoordinates

The abstract hierarchy, for when you subclass or type a variable:

    Series (abstract)
      ItemsSeries (abstract)
        PieSeries
        XYAxisSeries (abstract, ITransposablePlotElement)
          DataPointSeries (abstract)
            LineSeries
              AreaSeries -> TwoColorAreaSeries
              TwoColorLineSeries
              ThreeColorLineSeries
              ExtrapolationLineSeries
              StairStepSeries
              StemSeries
              FunctionSeries
            LinearBarSeries
          ScatterSeries<T> (abstract) -> ScatterSeries, ScatterErrorSeries
          BarSeriesBase<T> (abstract, IBarSeries)
            BarSeries (IStackableSeries) -> ErrorBarSeries
            IntervalBarSeries (IStackableSeries)
            TornadoBarSeries
          RectangleBarSeries
          BoxPlotSeries
          HistogramSeries
          HeatMapSeries
          ContourSeries
          RectangleSeries
          VectorSeries
          HighLowSeries -> CandleStickSeries
          VolumeSeries


Series -- members on every series
---------------------------------
    string Title                       // shown in the legend
    bool   IsVisible, RenderInLegend
    string LegendKey                   // which Legend this series belongs to
    string SeriesGroupName
    string TrackerKey, TrackerFormatString
    PlotterColor Background
    TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
    abstract void Render(IRenderContext rc)
    abstract void RenderLegend(IRenderContext rc, PlotterRect legendBox)

ItemsSeries adds:

    IEnumerable ItemsSource            // bind your own collection

XYAxisSeries adds:

    string XAxisKey, YAxisKey          // pick axes by Axis.Key
    Axis   XAxis, YAxis                // read-only, resolved by Update()
    double MinX, MaxX, MinY, MaxY      // read-only data extents
    ScreenPoint Transform(DataPoint p); DataPoint InverseTransform(ScreenPoint p)
    PlotterRect GetScreenRectangle()
    int FindWindowStartIndex<T>(IList<T> items, Func<T, double> xgetter,
                                double targetX, int initialGuess)
    const string DefaultTrackerFormatString

A series that is given no XAxisKey/YAxisKey binds to the model's default axis in
the matching position.

XYAxisSeries implements ITransposablePlotElement. TRANSPOSITION IS DRIVEN BY THE
AXES, NOT BY A PROPERTY: element.IsTransposed() is defined as
element.XAxis.IsVertical(). Put the series' X axis on Left/Right and the plot is
transposed; put it on Bottom/Top and it is not. This is how a horizontal bar
chart becomes a vertical column chart -- see BarSeries below. The extension
methods live on PlotElementExtensions (root namespace): IsTransposed(),
Orientate(ScreenPoint), Orientate(ScreenVector),
Orientate(ref HorizontalAlignment, ref VerticalAlignment), Transform(x, y),
InverseTransform(x, y). (The interface chain is IPlotElement ->
IXyAxisPlotElement -> ITransposablePlotElement.) PlotElementUtilities offers
the static equivalents
(GetClippingRect, Transform, TransformOrientated, InverseTransform,
InverseTransformOrientated).


DataPointSeries and LineSeries
------------------------------
DataPointSeries is the base of everything that plots DataPoints:

    List<DataPoint> Points             // fill this directly
    IEnumerable ItemsSource            // ...or bind
    string DataFieldX, DataFieldY      // property names on the bound items
    Func<object, DataPoint> Mapping    // ...or map explicitly
    bool CanTrackerInterpolatePoints

    var line = new LineSeries();
    line.Points.Add(new DataPoint(x, y));

    // or bind:
    line.ItemsSource = myItems;
    line.DataFieldX = nameof(MyItem.Time);
    line.DataFieldY = nameof(MyItem.Value);

    // or map:
    line.ItemsSource = myItems;
    line.Mapping = o => new DataPoint(((MyItem)o).Time, ((MyItem)o).Value);

Items that implement IDataPointProvider (DataPoint GetDataPoint()) are consumed
directly, with no DataField or Mapping needed.

DataPoint.Undefined (both coordinates NaN) breaks a line into segments.

LineSeries members:

    PlotterColor Color, MarkerFill, MarkerStroke, BrokenLineColor
    PlotterColor ActualColor, ActualMarkerFill        // read-only, resolved
    double StrokeThickness, BrokenLineThickness, MarkerSize, MarkerStrokeThickness
    double[] Dashes
    LineStyle LineStyle, BrokenLineStyle
    LineJoin  LineJoin
    MarkerType MarkerType             // None|Circle|Square|Diamond|Triangle|
                                      //   Cross|Plus|Star|Custom
    ScreenPoint[] MarkerOutline                       // used when MarkerType.Custom
    int    MarkerResolution
    double MinimumSegmentLength
    IInterpolationAlgorithm InterpolationAlgorithm    // smooth curves; see below
    string LabelFormatString; double LabelMargin
    LineLegendPosition LineLegendPosition             // None | Start | End
    Action<List<ScreenPoint>, List<ScreenPoint>> Decimator

Smoothing: assign one of the InterpolationAlgorithms (root namespace) --
CanonicalSpline, CatmullRomSpline, UniformCatmullRomSpline,
ChordalCatmullRomSpline -- or construct your own CanonicalSpline(tension) /
CatmullRomSpline(alpha). The interface is:

    interface IInterpolationAlgorithm
    {
        List<DataPoint>   CreateSpline(List<DataPoint> points,
                                      bool isClosed, double tolerance);
        List<ScreenPoint> CreateSpline(IList<ScreenPoint> points,
                                      bool isClosed, double tolerance);
    }

AreaSeries : LineSeries -- fills between two lines:

    List<DataPoint> Points2            // the second boundary
    double ConstantY2                  // ...or a constant baseline
    string DataFieldX2, DataFieldY2
    PlotterColor Color2, Fill, ActualColor2, ActualFill
    bool   Reverse2

TwoColorLineSeries : LineSeries -- one colour above Limit, another below:

    double Limit; PlotterColor Color2, ActualColor2
    LineStyle LineStyle2, ActualLineStyle2; double[] Dashes2

TwoColorAreaSeries : AreaSeries -- adds Fill2, ActualFill2, MarkerFill2,
MarkerStroke2, Limit, LineStyle2, Dashes2, ActualDashArray2.

ThreeColorLineSeries : LineSeries -- a low band, a normal band and a high band:

    double LimitLo, LimitHi
    PlotterColor ColorLo, ColorHi, ActualColorLo, ActualColorHi
    LineStyle LineStyleLo, LineStyleHi, ActualLineStyleLo, ActualLineStyleHi
    double[] DashesLo, DashesHi

ExtrapolationLineSeries : LineSeries -- draws named X intervals differently:

    IList<DataRange> Intervals         // struct DataRange(double min, double max)
    PlotterColor ExtrapolationColor, ActualExtrapolationColor
    LineStyle ExtrapolationLineStyle, ActualExtrapolationLineStyle
    double[] ExtrapolationDashes
    bool     IgnoreExtraplotationForScaling   // spelling is as shown

    series.Intervals.Add(new DataRange(0, 5));

DataRange is a struct: DataRange(double min, double max), Minimum, Maximum,
IsDefined(), Contains(double), IntersectsWith(DataRange),
DataRange.Undefined.

StairStepSeries : LineSeries -- VerticalStrokeThickness, VerticalLineStyle.

StemSeries : LineSeries -- double Base (the baseline the stems rise from).

FunctionSeries : LineSeries -- samples a function into Points at construction:

    FunctionSeries()
    FunctionSeries(Func<double,double> f, double x0, double x1, double dx,
                   string title = null)
    FunctionSeries(Func<double,double> f, double x0, double x1, int n,
                   string title = null)
    FunctionSeries(Func<double,double> fx, Func<double,double> fy,
                   double t0, double t1, double dt, string title = null)
    FunctionSeries(Func<double,double> fx, Func<double,double> fy,
                   double t0, double t1, int n, string title = null)

    model.Series.Add(new FunctionSeries(Math.Sin, -10, 10, 0.05, "sin(x)"));

LinearBarSeries : DataPointSeries -- bars on a NUMERIC x axis (no CategoryAxis):

    double BarWidth                    // default 5
    double BaseValue, BaseLine, ActualBaseLine
    PlotterColor FillColor, StrokeColor, NegativeFillColor, NegativeStrokeColor,
                 ActualColor
    double StrokeThickness


Scatter series
--------------
ScatterSeries<T> where T : ScatterPoint is the base; ScatterSeries is
ScatterSeries<ScatterPoint> and ScatterErrorSeries is
ScatterSeries<ScatterErrorPoint>.

    class ScatterPoint : ICodeGenerating
    {
        ScatterPoint(double x, double y, double size = double.NaN,
                     double value = double.NaN, object tag = null)
        double X { get; }  double Y { get; }      // set through the constructor
        double Size { get; set; }                 // NaN = use MarkerSize
        double Value { get; set; }                // NaN = use MarkerFill;
                                                  //   otherwise mapped by ColorAxis
        object Tag { get; set; }
    }

    class ScatterErrorPoint : ScatterPoint
    {
        ScatterErrorPoint(double x, double y, double errorX, double errorY,
                          double size = double.NaN, double value = double.NaN,
                          object tag = null)
        double ErrorX { get; }  double ErrorY { get; }
    }

ScatterSeries<T> members:

    List<T> Points; ReadOnlyCollection<T> ActualPoints
    IEnumerable ItemsSource; Func<object, T> Mapping
    string DataFieldX, DataFieldY, DataFieldSize, DataFieldValue, DataFieldTag
    MarkerType MarkerType; ScreenPoint[] MarkerOutline
    double MarkerSize, MarkerStrokeThickness
    PlotterColor MarkerFill, MarkerStroke, ActualMarkerFillColor
    IColorAxis ColorAxis; string ColorAxisKey     // colours points by .Value
    int    BinSize                                // bins points for speed
    double MinValue, MaxValue                     // read-only
    string LabelFormatString; double LabelMargin

Items implementing IScatterPointProvider (ScatterPoint GetScatterPoint()) are
consumed directly.

ScatterErrorSeries adds DataFieldErrorX, DataFieldErrorY, ErrorBarColor,
ErrorBarStrokeThickness, ErrorBarStopWidth, MinimumErrorSize, and
SelectAll(Func<ScatterErrorPoint, bool>).


Bar and column series
=====================
THERE IS NO ColumnSeries IN THIS LIBRARY. Upstream folded it into a
TRANSPOSABLE BarSeries, and this port follows that. One series type draws both
horizontal bars and vertical columns; the AXIS POSITIONS decide which.

    // Horizontal bars: CategoryAxis on the Left, value axis on the Bottom.
    model.Axes.Add(new CategoryAxis { Position = AxisPosition.Left });
    model.Axes.Add(new LinearAxis   { Position = AxisPosition.Bottom });
    model.Series.Add(new BarSeries { ... });        // no keys needed

    // Vertical columns: CategoryAxis on the Bottom, value axis on the Left,
    // and the series names them explicitly, because "X" must be the value axis.
    model.Axes.Add(new CategoryAxis
        { Position = AxisPosition.Bottom, Key = "categories" });
    model.Axes.Add(new LinearAxis   { Position = AxisPosition.Left,   Key = "values" });
    model.Series.Add(new BarSeries { XAxisKey = "values", YAxisKey = "categories" });

BarSeriesBase<T> where T : BarItemBase -- shared by BarSeries, ErrorBarSeries,
IntervalBarSeries and TornadoBarSeries:

    List<T> Items                      // fill this
    List<T> ActualItems                // Items, or the ItemsSource projection
    double  BarWidth                   // default 1
    PlotterColor StrokeColor, LabelColor
    double  StrokeThickness, LabelMargin, LabelAngle
    LabelPlacement LabelPlacement      // Outside | Inside | Middle | Base

    abstract class BarItemBase { int CategoryIndex { get; set; } }   // -1 = next slot

BarSeries : BarSeriesBase<BarItem>, IStackableSeries

    class BarItem : BarItemBase, ICodeGenerating
    {
        BarItem()
        BarItem(double value, int categoryIndex = -1)
        double Value { get; set; }
        PlotterColor Color { get; set; }      // Automatic = use the series fill
    }

    // BarSeries
    PlotterColor FillColor, NegativeFillColor, ActualFillColor
    double BaseValue, BaseLine
    bool   IsStacked
    string StackGroup                  // series sharing a group stack together
    string ColorField, ValueField      // when bound through ItemsSource
    string LabelFormatString

    var bars = new BarSeries { Title = "2026", LabelFormatString = "{0}" };
    bars.Items.Add(new BarItem(42));
    bars.Items.Add(new BarItem(17));

ErrorBarSeries : BarSeries

    class ErrorBarItem : BarItem
    {
        ErrorBarItem()
        ErrorBarItem(double value, double error, int categoryIndex = -1)
        double Error { get; set; }
    }

    // ErrorBarSeries
    double ErrorStrokeThickness, ErrorWidth

IntervalBarSeries : BarSeriesBase<IntervalBarItem>, IStackableSeries -- each bar
spans from Start to End rather than from a baseline:

    class IntervalBarItem : BarItemBase, ICodeGenerating
    {
        IntervalBarItem()
        IntervalBarItem(double start, double end, string title = null)
        double Start { get; set; }  double End { get; set; }
        string Title { get; set; }  PlotterColor Color { get; set; }
    }

    // IntervalBarSeries
    PlotterColor FillColor, ActualFillColor
    string ColorField, StartField, EndField, LabelFormatString
    bool   IsStacked => true;  bool OverlapsStack => true;  string StackGroup => ""

TornadoBarSeries : BarSeriesBase<TornadoBarItem> -- a minimum bar and a maximum
bar spreading from a base value:

    class TornadoBarItem : BarItemBase, ICodeGenerating
    {
        double BaseValue { get; set; }
        double Minimum { get; set; }   PlotterColor MinimumColor { get; set; }
        double Maximum { get; set; }   PlotterColor MaximumColor { get; set; }
    }

    // TornadoBarSeries
    double BaseValue
    PlotterColor MinimumFillColor, MaximumFillColor,
                 ActualMinimumFillColor, ActualMaximumFillColor
    string MinimumColorField, MaximumColorField

RectangleBarSeries : XYAxisSeries -- arbitrary rectangles in data space, NOT
category-based:

    class RectangleBarItem : ICodeGenerating
    {
        RectangleBarItem()
        RectangleBarItem(double x0, double y0, double x1, double y1)
        double X0, X1, Y0, Y1 { get; set; }
        PlotterColor Color { get; set; }   string Title { get; set; }
    }

    // RectangleBarSeries
    IList<RectangleBarItem> Items
    PlotterColor FillColor, ActualFillColor
    string LabelFormatString

BarSeriesManager coordinates stacking and slot widths across all the bar series
sharing one CategoryAxis. You do not create one; Update() does. Its interface
contract is IBarSeries (with IStackableSeries adding StackGroup/OverlapsStack).


Statistical and distribution series
-----------------------------------
BoxPlotSeries : XYAxisSeries

    class BoxPlotItem
    {
        BoxPlotItem(double x, double lowerWhisker, double boxBottom,
                    double median, double boxTop, double upperWhisker)
        double X, LowerWhisker, BoxBottom, Median, BoxTop, UpperWhisker { get; set; }
        double Mean { get; set; }                 // NaN unless you set it
        IList<double> Outliers { get; set; }
        IList<double> Values { get; }             // the five box values
        object Tag { get; set; }
    }

    // BoxPlotSeries
    IList<BoxPlotItem> Items
    double BoxWidth, WhiskerWidth, StrokeThickness
    double MedianPointSize, MedianThickness, MeanPointSize, MeanThickness,
           OutlierSize
    bool   ShowBox, ShowMedianAsDot, ShowMeanAsDot
    PlotterColor Fill, Stroke
    LineStyle LineStyle
    MarkerType OutlierType; ScreenPoint[] OutlierOutline
    string OutlierTrackerFormatString
    bool IsValidPoint(BoxPlotItem item, Axis xaxis, Axis yaxis)

HistogramSeries : XYAxisSeries

    class HistogramItem : ICodeGenerating
    {
        HistogramItem(double rangeStart, double rangeEnd, double area, int count)
        HistogramItem(double rangeStart, double rangeEnd, double area, int count,
                      PlotterColor color)
        double RangeStart, RangeEnd, Area { get; set; }
        double Width { get; }        // RangeEnd - RangeStart
        double Height { get; }       // Area / Width
        double Value { get; }        // == Height
        int    Count { get; set; }
        PlotterColor Color { get; set; }
        bool Contains(DataPoint p)
    }

    // HistogramSeries
    List<HistogramItem> Items
    Func<object, HistogramItem> Mapping
    Func<HistogramItem, PlotterColor> ColorMapping    // colour each bin by value
    double BaseValue, BaseLine, ActualBaseLine, StrokeThickness
    PlotterColor FillColor, ActualFillColor, StrokeColor,
                 NegativeFillColor, NegativeStrokeColor
    double MinValue, MaxValue                        // read-only
    string LabelFormatString; double LabelMargin
    LabelPlacement LabelPlacement

Bins are built for you by HistogramHelpers (root namespace):

    static List<double> CreateUniformBins(double start, double end, int binCount)
    static IList<HistogramItem> Collect(IEnumerable<double> samples,
                                        IEnumerable<double> binBreaks,
                                        BinningOptions binningOptions)

    class BinningOptions
    {
        BinningOptions(BinningOutlierMode outlierMode,
                       BinningIntervalType intervalType,
                       BinningExtremeValueMode extremeValuesMode)
        BinningOutlierMode      OutlierMode { get; }
        BinningIntervalType     IntervalType { get; }
        BinningExtremeValueMode ExtremeValuesMode { get; }
    }
    enum BinningOutlierMode      { RejectOutliers, IgnoreOutliers, CountOutliers }
    enum BinningIntervalType     { InclusiveLowerBound, InclusiveUpperBound }
    enum BinningExtremeValueMode { ExcludeExtremeValues, IncludeExtremeValues }


Grid, field and area series
---------------------------
HeatMapSeries : XYAxisSeries -- a 2D value grid coloured through a colour axis:

    double[,] Data                       // Data[x, y]
    double X0, X1, Y0, Y1                // data-space bounds of the grid
    HeatMapCoordinateDefinition CoordinateDefinition   // Center | Edge
    HeatMapRenderMethod RenderMethod                   // Bitmap | Rectangles
    bool   Interpolate
    IColorAxis ColorAxis; string ColorAxisKey
    double MinValue, MaxValue            // read-only
    string LabelFormatString; double LabelFontSize
    void   Invalidate()                  // call after mutating Data in place

ContourSeries : XYAxisSeries -- iso-lines over a 2D value grid:

    double[,] Data
    double[] ColumnCoordinates, RowCoordinates
    double[] ContourLevels; double ContourLevelStep
    PlotterColor[] ContourColors; PlotterColor Color, ActualColor
    LineStyle LineStyle; double StrokeThickness, MinimumSegmentLength
    string LabelFormatString; PlotterColor LabelBackground
    double LabelSpacing; int LabelStep; bool MultiLabel
    void CalculateContours()

    // The contour tracer itself is available directly:
    Conrec.Contour(double[,] d, double[] x, double[] y, double[] z,
                   Conrec.RendererDelegate renderer)

RectangleSeries : XYAxisSeries -- value-coloured rectangles in data space:

    class RectangleItem : ICodeGenerating, IEquatable<RectangleItem>
    {
        RectangleItem(double x1, double x2, double y1, double y2, double value)
        RectangleItem(DataPoint a, DataPoint b, double value)
        DataPoint A { get; }  DataPoint B { get; }  double Value { get; }
        bool Contains(DataPoint p);  bool IsDefined()
        static readonly RectangleItem Undefined
    }

    // RectangleSeries
    List<RectangleItem> Items; Func<object, RectangleItem> Mapping
    IColorAxis ColorAxis; string ColorAxisKey
    double MinValue, MaxValue; bool CanTrackerInterpolatePoints
    string LabelFormatString; double LabelFontSize

VectorSeries : XYAxisSeries -- an arrow field, each arrow coloured by value:

    class VectorItem : ICodeGenerating, IEquatable<VectorItem>
    {
        VectorItem(DataPoint origin, DataVector direction, double value)
        DataPoint  Origin { get; }  DataVector Direction { get; }
        double Value { get; }  bool IsDefined()
        static readonly VectorItem Undefined
    }

    // VectorSeries
    IList<VectorItem> Items; Func<object, VectorItem> Mapping
    IColorAxis ColorAxis; string ColorAxisKey
    PlotterColor Color; double StrokeThickness, MinimumSegmentLength
    LineStyle LineStyle; LineJoin LineJoin
    double ArrowHeadLength, ArrowHeadWidth, ArrowHeadPosition, ArrowVeeness,
           ArrowStartPosition, ArrowLabelPosition
    DataPoint StartPoint
    double MinValue, MaxValue; bool CanTrackerInterpolatePoints
    string LabelFormatString; double LabelFontSize


Financial series
----------------
HighLowSeries : XYAxisSeries and CandleStickSeries : HighLowSeries share one
item type:

    class HighLowItem : ICodeGenerating
    {
        HighLowItem()
        HighLowItem(double x, double high, double low,
                    double open = double.NaN, double close = double.NaN)
        double X, High, Low, Open, Close { get; set; }
        static readonly HighLowItem Undefined
    }

    // HighLowSeries
    List<HighLowItem> Items; Func<object, HighLowItem> Mapping
    string DataFieldX, DataFieldHigh, DataFieldLow, DataFieldOpen, DataFieldClose
    PlotterColor Color, ActualColor
    LineStyle LineStyle; LineJoin LineJoin; double[] Dashes
    double StrokeThickness, TickLength
    bool IsValidItem(HighLowItem pt, Axis xaxis, Axis yaxis)

    // CandleStickSeries adds
    PlotterColor IncreasingColor, DecreasingColor
    double CandleWidth
    int    FindByX(double x, int startIndex = -1)

VolumeSeries : XYAxisSeries -- volume bars beneath a candlestick chart:

    class OhlcvItem
    {
        OhlcvItem()
        OhlcvItem(double x, double open, double high, double low, double close,
                  double buyvolume = 0, double sellvolume = 0)
        double X, Open, High, Low, Close, BuyVolume, SellVolume { get; set; }
        bool IsValid()
        static int FindIndex(List<OhlcvItem> items, double targetX, int guessIdx)
        static readonly OhlcvItem Undefined
    }

    // VolumeSeries
    List<OhlcvItem> Items
    VolumeStyle VolumeStyle            // None | Combined | Stacked | PositiveNegative
    PlotterColor PositiveColor, NegativeColor, InterceptColor
    bool   PositiveHollow, NegativeHollow
    double BarWidth, StrokeThickness, StrokeIntensity, InterceptStrokeThickness
    LineStyle InterceptLineStyle
    double MinimumVolume, MaximumVolume, AverageVolume    // read-only
    void   Append(OhlcvItem bar)
    int    FindByX(double x, int startingIndex = -1)


Pie chart
---------
PieSeries : ItemsSeries -- NOT an XY series, so it needs no axes at all:

    class PieSlice : ICodeGenerating
    {
        PieSlice(string label, double value)
        string Label { get; }  double Value { get; }
        PlotterColor Fill { get; set; }  PlotterColor ActualFillColor { get; }
        bool IsExploded { get; set; }
    }

    // PieSeries
    IList<PieSlice> Slices
    string LabelField, ValueField, ColorField, IsExplodedField   // for ItemsSource
    double StartAngle, AngleSpan, AngleIncrement
    double Diameter, InnerDiameter, ExplodedDistance
    PlotterColor Stroke, InsideLabelColor; double StrokeThickness
    string InsideLabelFormat, OutsideLabelFormat, LegendFormat
    double InsideLabelPosition; bool AreInsideLabelsAngled
    double TickDistance, TickRadialLength, TickHorizontalLength, TickLabelDistance


ANNOTATIONS
===========

All annotations live in CodeBrix.Plotter.Annotations. Add them to
model.Annotations.

    Annotation (abstract)                      // Layer, XAxisKey/YAxisKey, clipping
      TransposableAnnotation (abstract)
        ImageAnnotation
        TextualAnnotation (abstract)           // Text, TextPosition, alignment,
                                               //   TextRotation
          TextAnnotation
          ArrowAnnotation
          ShapeAnnotation (abstract)           // Fill, Stroke, StrokeThickness
            RectangleAnnotation
            EllipseAnnotation
            PointAnnotation
            PolygonAnnotation
          PathAnnotation (abstract)            // Color, LineStyle, LineJoin,
                                               //   Minimum/MaximumX/Y, text border
            LineAnnotation
            FunctionAnnotation
            PolylineAnnotation

Annotation (every annotation has these):

    AnnotationLayer Layer      // BelowAxes | BelowSeries | AboveSeries
    string XAxisKey, YAxisKey; Axis XAxis, YAxis
    bool   ClipByXAxis, ClipByYAxis
    ScreenPoint Transform(DataPoint p); DataPoint InverseTransform(ScreenPoint p)
    void   EnsureAxes()

TextualAnnotation: string Text; DataPoint TextPosition;
HorizontalAlignment TextHorizontalAlignment; VerticalAlignment
TextVerticalAlignment; double TextRotation.

TextAnnotation: PlotterColor Background, Stroke; double StrokeThickness;
ScreenVector Offset; PlotterThickness Padding.

ArrowAnnotation: DataPoint StartPoint, EndPoint; ScreenVector ArrowDirection;
PlotterColor Color; double HeadLength, HeadWidth, Veeness, StrokeThickness;
LineStyle LineStyle; LineJoin LineJoin.

LineAnnotation: LineAnnotationType Type (Horizontal | Vertical |
LinearEquation); double X, Y, Slope, Intercept.

FunctionAnnotation: FunctionAnnotationType Type (EquationX | EquationY);
Func<double, double> Equation; int Resolution.

PolylineAnnotation: List<DataPoint> Points; IInterpolationAlgorithm
InterpolationAlgorithm.

PolygonAnnotation: List<DataPoint> Points; LineStyle LineStyle; LineJoin
LineJoin; double MinimumSegmentLength.

RectangleAnnotation: double MinimumX, MaximumX, MinimumY, MaximumY.

EllipseAnnotation: double X, Y, Width, Height.

PointAnnotation: double X, Y, Size, TextMargin; MarkerType Shape;
ScreenPoint[] CustomOutline.

ImageAnnotation: PlotterImage ImageSource; PlotLength X, Y, OffsetX, OffsetY,
Width, Height; HorizontalAlignment HorizontalAlignment; VerticalAlignment
VerticalAlignment; double Opacity; bool Interpolate.

PathAnnotation (shared by Line/Function/Polyline annotations): PlotterColor
Color; LineStyle LineStyle; LineJoin LineJoin; double StrokeThickness,
MinimumX, MaximumX, MinimumY, MaximumY, TextMargin, TextPadding,
TextLinePosition, MinimumSegmentLength; AnnotationTextOrientation
TextOrientation (Horizontal | Vertical | AlongLine); PlotterThickness
BorderPadding; PlotterColor BorderBackground, BorderStroke; double
BorderStrokeThickness.

PlotLength is a struct: PlotLength(double value, PlotLengthUnit unit), .Value,
.Unit, with PlotLengthUnit = Data | ScreenUnits | RelativeToViewport |
RelativeToPlotArea.


LEGENDS
=======

Add a Legend (CodeBrix.Plotter.Legends) to model.Legends. A series appears in a
legend when its Title is set and RenderInLegend is true; LegendKey on the series
selects WHICH legend when there is more than one (matched against
LegendBase.Key).

    model.IsLegendVisible = true;
    model.Legends.Add(new Legend
    {
        LegendTitle = "Series",
        LegendPosition = LegendPosition.RightTop,
        LegendPlacement = LegendPlacement.Outside,
        LegendOrientation = LegendOrientation.Vertical,
    });

LegendBase members:

    string Key, LegendTitle, LegendFont, LegendTitleFont
    bool   IsLegendVisible, AllowUseFullExtent, ShowInvisibleSeries
    LegendPosition    LegendPosition     // TopLeft|TopCenter|TopRight|
                                         //   BottomLeft|BottomCenter|BottomRight|
                                         //   LeftTop|LeftMiddle|LeftBottom|
                                         //   RightTop|RightMiddle|RightBottom
    LegendPlacement   LegendPlacement    // Inside | Outside
    LegendOrientation LegendOrientation  // Horizontal | Vertical
    LegendItemOrder   LegendItemOrder    // Normal | Reverse
    LegendSymbolPlacement LegendSymbolPlacement   // Left | Right
    HorizontalAlignment LegendItemAlignment
    PlotterColor LegendBackground, LegendBorder, LegendTextColor,
                 LegendTitleColor
    double LegendBorderThickness, LegendPadding, LegendMargin,
           LegendItemSpacing, LegendLineSpacing, LegendColumnSpacing,
           LegendSymbolLength, LegendSymbolMargin,
           LegendFontSize, LegendFontWeight,
           LegendTitleFontSize, LegendTitleFontWeight,
           LegendMaxWidth, LegendMaxHeight
    PlotterRect LegendArea; PlotterSize LegendSize

Legend adds GroupNameFont, GroupNameFontSize, GroupNameFontWeight (used with
Series.SeriesGroupName), SeriesInvisibleTextColor, and
bool IsPointInLegend(ScreenPoint point).


GEOMETRY, COLOUR AND STYLING VALUE TYPES
========================================

All in the root CodeBrix.Plotter namespace. The geometry types are immutable
structs -- "modify" by constructing a new value.

    DataPoint          DataPoint(double x, double y); .X, .Y; IsDefined();
                       operators + and - with DataVector; DataPoint.Undefined
                       (both coordinates NaN -- breaks a line into segments)
    DataVector         DataVector(double x, double y); .X, .Y, .Length,
                       .LengthSquared; * / + / - operators; DataVector.Undefined
    ScreenPoint        ScreenPoint(double x, double y); .X, .Y;
                       DistanceTo(p), DistanceToSquared(p);
                       static IsUndefined(p); ScreenPoint.Undefined
    ScreenVector       ScreenVector(double x, double y); .X, .Y, .Length,
                       .LengthSquared; Normalize(); operators
    PlotterRect        PlotterRect(left, top, width, height);
                       PlotterRect(ScreenPoint p0, ScreenPoint p1);
                       PlotterRect(ScreenPoint p0, PlotterSize size);
                       static Create(x0, y0, x1, y1);
                       .Left/.Top/.Right/.Bottom/.Width/.Height/.Center/
                       .TopLeft/.TopRight/.BottomLeft/.BottomRight;
                       Contains(x, y), Contains(p), Inflate(dx, dy),
                       Inflate(PlotterThickness), Deflate(PlotterThickness),
                       Intersect(rect), Offset(dx, dy), Clip(rect);
                       PlotterRect.Everything
    PlotterSize        PlotterSize(double width, double height); .Width, .Height;
                       Include(other); PlotterSize.Empty
    PlotterThickness   PlotterThickness(double thickness);
                       PlotterThickness(left, top, right, bottom);
                       .Left/.Top/.Right/.Bottom; Include(other)
    PlotLength         PlotLength(double value, PlotLengthUnit unit)

Colour
------
PlotterColor is an immutable struct with sentinel values, NOT a class:

    static PlotterColor FromRgb(byte r, byte g, byte b)
    static PlotterColor FromArgb(byte a, byte r, byte g, byte b)
    static PlotterColor FromAColor(byte a, PlotterColor color)
    static PlotterColor FromUInt32(uint color)
    static PlotterColor FromHsv(double hue, double sat, double val)
    static PlotterColor FromHsv(double[] hsv)
    static PlotterColor Parse(string value)
    static PlotterColor Interpolate(PlotterColor c1, PlotterColor c2, double t)
    static double ColorDifference(PlotterColor c1, PlotterColor c2)
    static double HueDifference(PlotterColor c1, PlotterColor c2)
    byte A, R, G, B
    bool IsUndefined();  bool IsAutomatic();  bool IsInvisible();  bool IsVisible()
    PlotterColor GetActualColor(PlotterColor defaultColor)

TEST WITH IsUndefined()/IsAutomatic()/IsInvisible(), never against null -- a
struct is never null, and PlotterColors.Undefined / PlotterColors.Automatic are
distinct non-null sentinel values (0x00000000 and 0x00000001).

PlotterColors is a static class of 143 named colours (AliceBlue ... YellowGreen)
plus the two sentinels PlotterColors.Undefined and PlotterColors.Automatic.

PlotterColorExtensions: ChangeIntensity(factor), ChangeSaturation(factor),
ChangeOpacity(factor), Complementary(), ToHsv(), ToUint(), ToByteString(),
ToCode(), GetColorName().

Palettes
--------
    class PlotterPalette
    {
        PlotterPalette()
        PlotterPalette(params PlotterColor[] colors)
        PlotterPalette(IEnumerable<PlotterColor> colors)
        IList<PlotterColor> Colors { get; set; }
        static PlotterPalette Interpolate(int paletteSize, params PlotterColor[] colors)
    }

PlotterPalettes is a static class. MOST OF ITS MEMBERS ARE METHODS TAKING A
COLOUR COUNT, not properties -- that is the usual mistake:

    // methods -- pass the number of colours you want
    PlotterPalettes.BlackWhiteRed(int numberOfColors)
    PlotterPalettes.BlueWhiteRed(int numberOfColors)
    PlotterPalettes.Cool(int numberOfColors)
    PlotterPalettes.Gray(int numberOfColors)
    PlotterPalettes.Hot(int numberOfColors)
    PlotterPalettes.Hue(int numberOfColors)
    PlotterPalettes.HueDistinct(int numberOfColors)
    PlotterPalettes.Jet(int numberOfColors)
    PlotterPalettes.Rainbow(int numberOfColors)

    // the matplotlib colormaps -- the count is optional and defaults to 256
    PlotterPalettes.Cividis(int numberOfColors = 256)
    PlotterPalettes.Inferno(int numberOfColors = 256)
    PlotterPalettes.Magma(int numberOfColors = 256)
    PlotterPalettes.Plasma(int numberOfColors = 256)
    PlotterPalettes.Viridis(int numberOfColors = 256)

    // the only three that are PROPERTIES, pre-sized as their names say
    PlotterPalettes.BlueWhiteRed31
    PlotterPalettes.Hot64
    PlotterPalettes.Hue64

LinearColorAxis and LogarithmicColorAxis default their Palette to
PlotterPalettes.Viridis().

Pens and strokes
----------------
    class PlotterPen
    {
        PlotterPen(PlotterColor color, double thickness = 1,
                   LineStyle lineStyle = LineStyle.Solid,
                   LineJoin lineJoin = LineJoin.Miter)
        PlotterColor Color { get; set; }   double Thickness { get; set; }
        LineStyle LineStyle { get; set; }  LineJoin LineJoin { get; set; }
        double[] DashArray { get; set; }   double[] ActualDashArray { get; }
        static PlotterPen Create(...)      // returns null for an invisible pen
    }

Styling enumerations (root namespace unless noted)
--------------------------------------------------
    LineStyle          Solid, Dash, Dot, DashDot, DashDashDot, DashDotDot,
                       DashDashDotDot, LongDash, LongDashDot, LongDashDotDot,
                       None, Automatic
                       (LineStyleHelper.GetDashArray(this LineStyle) gives the
                        dash pattern)
    LineJoin           Miter, Round, Bevel
    MarkerType         None, Circle, Square, Diamond, Triangle, Cross, Plus,
                       Star, Custom
    HorizontalAlignment  Left = -1, Center = 0, Right = 1
    VerticalAlignment    Top = -1, Middle = 0, Bottom = 1
    EdgeRenderingMode  Automatic, Adaptive, PreferSharpness, PreferSpeed,
                       PreferGeometricAccuracy
    PlotType           XY, Cartesian, Polar
    TitleHorizontalAlignment  CenteredWithinPlotArea, CenteredWithinView
    FontWeights        static class: FontWeights.Normal = 400,
                       FontWeights.Bold = 700 (font weights are doubles)
    SelectionMode      All, Single, Multiple
    CursorType         Default, Pan, ZoomRectangle, ZoomHorizontal, ZoomVertical
    ImageFormat        Png, Bmp, Jpeg, Unknown
    InterpolationAlgorithms  static class: CanonicalSpline, CatmullRomSpline,
                       UniformCatmullRomSpline, ChordalCatmullRomSpline
                       (all IInterpolationAlgorithm)

    // CodeBrix.Plotter.Axes
    AxisPosition       None, Left, Right, Top, Bottom, All
    AxisLayer          BelowSeries, AboveSeries
    TickStyle          Crossing, Inside, Outside, None
    AxisChangeTypes    Zoom, Pan, Reset
    DateTimeIntervalType  Auto, Manual, Milliseconds, Seconds, Minutes, Hours,
                       Days, Weeks, Months, Years

    // CodeBrix.Plotter.Series
    LabelPlacement     Outside, Inside, Middle, Base
    VolumeStyle        None, Combined, Stacked, PositiveNegative
    LineLegendPosition None, Start, End
    HeatMapCoordinateDefinition  Center, Edge
    HeatMapRenderMethod          Bitmap, Rectangles

    // CodeBrix.Plotter.Annotations
    AnnotationLayer           BelowAxes, BelowSeries, AboveSeries
    AnnotationTextOrientation Horizontal, Vertical, AlongLine
    LineAnnotationType        Horizontal, Vertical, LinearEquation
    FunctionAnnotationType    EquationX, EquationY

    // CodeBrix.Plotter.Legends
    LegendPlacement, LegendPosition, LegendOrientation, LegendItemOrder,
    LegendSymbolPlacement       (members listed under LEGENDS above)

    // CodeBrix.Plotter.Skia
    RenderTarget       Screen, PixelGraphic, VectorGraphic


RENDERING
=========

IRenderContext -- the drawing abstraction
-----------------------------------------
Everything in the library draws through IRenderContext, so anything that can
draw can host a plot by implementing it:

    public interface IRenderContext
    {
        void DrawEllipse(PlotterRect extents, PlotterColor fill, PlotterColor stroke,
                         double thickness, EdgeRenderingMode edgeRenderingMode);
        void DrawEllipses(IList<PlotterRect> extents, PlotterColor fill,
                          PlotterColor stroke, double thickness,
                          EdgeRenderingMode edgeRenderingMode);
        void DrawLine(...);
        void DrawLineSegments(...);
        void DrawPolygon(...);
        void DrawPolygons(...);
        void DrawRectangle(PlotterRect rectangle, PlotterColor fill,
                           PlotterColor stroke, double thickness,
                           EdgeRenderingMode edgeRenderingMode);
        void DrawRectangles(IList<PlotterRect> rectangles, PlotterColor fill,
                            PlotterColor stroke, double thickness,
                            EdgeRenderingMode edgeRenderingMode);
        void DrawText(...);
        void DrawImage(PlotterImage source, double srcX, double srcY,
                       double srcWidth, double srcHeight, double destX, double destY,
                       double destWidth, double destHeight, double opacity,
                       bool interpolate);
        PlotterSize MeasureText(string text, string fontFamily = null,
                                double fontSize = 10, double fontWeight = 500);
        void SetToolTip(string text);
        void PushClip(PlotterRect clippingRectangle);
        void PopClip();
        void CleanUp();
    }

RenderContextBase is the abstract helper that implements most of the interface
in terms of DrawLine/DrawPolygon/DrawText plus bool RendersToScreen, and offers
static IsStraightLine(p1, p2) / IsStraightLine(points). ClippingRenderContext
adds clip-stack bookkeeping. XkcdRenderingDecorator wraps another IRenderContext
to draw in a hand-drawn style (DistortionFactor, InterpolationDistance,
FontFamily, ThicknessScale) -- assign it through model.RenderingDecorator.

RenderingExtensions adds convenience drawing over any IRenderContext:
DrawReducedLine, DrawReducedPolygon, DrawMultilineText, DrawMarker, DrawMarkers,
DrawCircle, DrawImage, DrawLine, DrawLineSegments, and
EdgeRenderingMode.GetActual(defaultValue). MathRenderingExtensions adds
DrawMathText / MeasureMathText for sub/superscript notation.

SkiaRenderContext -- the SkiaSharp implementation
-------------------------------------------------
CodeBrix.Plotter.Skia. It is IRenderContext + IDisposable.

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

    // read-only
    bool RendersToScreen => RenderTarget == RenderTarget.Screen;
    int  ClipCount;
    TypefaceResolver TypefaceResolver { get; set; }

RendersToScreen is derived from RenderTarget and cannot be assigned; set
RenderTarget instead.

SkiaRenderContext caches SKTypeface, SKFont and SKShaper instances per font
descriptor, so keep ONE alive across frames when rendering repeatedly, and
dispose it when finished. It does NOT own the SKCanvas and will not dispose it;
you may reassign SkCanvas between frames.

SkiaExtensions offers PlotterColor.ToSKColor() and SKColor.ToPlotterColor().


FONT RESOLUTION AND THE TypefaceResolver CONTRACT
=================================================

By default SkiaRenderContext resolves a font family name through the system font
lookup (SKTypeface.FromFamilyName). The TypefaceResolver property replaces that
lookup. Its delegate type is also named TypefaceResolver, in the same namespace:

    public delegate SKTypeface TypefaceResolver(string fontFamily, double fontWeight);

    context.TypefaceResolver = (fontFamily, fontWeight) =>
        myFonts.TryGetValue(fontFamily, out SKTypeface found)
            ? found
            : myDefaultTypeface;

The contract, precisely:

  * While a resolver is assigned it owns resolution COMPLETELY. Every font
    family a draw or measure call names goes to the resolver; the system font
    lookup is never consulted, not even when the resolver cannot resolve the
    family. A host that must never render system fonts enforces that rule by
    assigning a resolver.
  * A resolver that cannot resolve a family should return its own default or
    fallback typeface rather than null. A null return is treated as
    SKTypeface.Default -- still no system family lookup.
  * fontWeight arrives as the numeric weight (400 normal, 700 bold -- the
    FontWeights constants).
  * Results are cached per (family, weight) pair, exactly like system-resolved
    typefaces, so the resolver runs once per distinct pair, not once per draw.
  * Assigning a different resolver (or null) clears the typeface and shaper
    caches. Re-assigning the delegate already in place is a no-op.
  * OWNERSHIP: typefaces returned by a resolver stay owned by the resolver --
    the render context never disposes them, neither on resolver change nor in
    Dispose(). (Typefaces from the system lookup are owned and disposed by the
    context, as always.)
  * TypefaceResolver is null by default, which is exactly the pre-existing
    behaviour.


EXPORTERS
=========

Every exporter implements:

    public interface IExporter { void Export(IPlotModel model, Stream stream); }

The exporters call Update(true) and Render() on the model themselves, so a
PlotModel can be handed straight to them with no cast.

CodeBrix.Plotter.Skia -- SkiaSharp-backed, and the ones to prefer:

    class PngExporter : IExporter
    {
        int   Width { get; set; }       int Height { get; set; }
        float Dpi { get; set; } = 96;   bool UseTextShaping { get; set; } = true
        void Export(IPlotModel model, Stream stream);
        static void Export(IPlotModel model, string path, int width, int height,
                           float dpi = 96);
        static void Export(IPlotModel model, Stream stream, int width, int height,
                           float dpi = 96);
    }

    class JpegExporter : IExporter        // same shape, plus quality
    {
        int Width, Height; float Dpi = 96; int Quality { get; set; } = 90;
        void Export(IPlotModel model, Stream stream);
        static void Export(IPlotModel model, string path, int width, int height,
                           int quality, float dpi = 96);
        static void Export(IPlotModel model, Stream stream, int width, int height,
                           int quality, float dpi = 96);
    }

    class PdfExporter : IExporter         // vector PDF through SKDocument
    {
        float Width, Height; bool UseTextShaping = true;
        void Export(IPlotModel model, Stream stream);
        static void Export(IPlotModel model, string path, float width, float height);
        static void Export(IPlotModel model, Stream stream, float width, float height);
    }

    class SvgExporter : IExporter         // vector SVG through SKSvgCanvas
    {
        float Width, Height;
        void Export(IPlotModel model, Stream stream);
    }

THERE IS NO ExportToBitmap METHOD on any exporter. To get pixels rather than a
file, render onto an SKBitmap yourself through SkiaRenderContext -- see COMPLETE
EXAMPLES.

CodeBrix.Plotter (root namespace) -- the dependency-free SVG writer inherited
from the upstream core, which needs no SkiaSharp at all:

    class SvgExporter : IExporter
    {
        double Width, Height;
        bool IsDocument { get; set; }
        bool UseVerticalTextAlignmentWorkaround { get; set; }
        IRenderContext TextMeasurer { get; set; }
        void   Export(IPlotModel model, Stream stream);
        string ExportToString(IPlotModel model);
        static void   Export(IPlotModel model, Stream stream, double width,
                             double height, bool isDocument,
                             IRenderContext textMeasurer = null,
                             bool useVerticalTextAlignmentWorkaround = false);
        static string ExportToString(IPlotModel model, double width,
                                     double height, bool isDocument,
                                     IRenderContext textMeasurer = null,
                                     bool useVerticalTextAlignmentWorkaround = false);
    }

It writes through SvgRenderContext (an IRenderContext over SvgWriter, which in
turn derives from XmlWriterBase); both are public if you want to emit SVG
fragments directly rather than a whole plot.

That is the SAME TYPE NAME as the Skia one in a different namespace, so with
both `using` directives in scope you must qualify it. There is no PdfExporter in
the root namespace -- only CodeBrix.Plotter.Skia.PdfExporter. The PortableDocument
machinery (PortableDocument, PortableDocumentExtensions, PdfRenderContext,
PortableDocumentFont, PortableDocumentFontFamily, PortableDocumentImage,
PortableDocumentImageUtilities, StandardFonts, PageSize, PageOrientation,
LineCap, ColorSpace, FontEncoding, FontSubType) is still present, because the core
SVG writer uses PdfRenderContext to measure text without SkiaSharp.

The Skia exporters generally produce better text output because they shape and
measure with the real font; the core SVG writer is there when you want zero
native dependencies.


INPUT MODEL, CONTROLLER AND COMMANDS
====================================

The library ships a UI-framework-independent input model so a host view can map
its own events onto plot commands. Everything here is in the ROOT
CodeBrix.Plotter namespace (there is no .Input or .PlotController namespace).

Event arguments
---------------
    abstract class PlotterInputEventArgs : EventArgs
    {
        bool Handled { get; set; }
        PlotterModifierKeys ModifierKeys { get; set; }
        bool IsAltDown { get; }  bool IsControlDown { get; }  bool IsShiftDown { get; }
    }
    class PlotterMouseEventArgs      : PlotterInputEventArgs { ScreenPoint Position; }
    class PlotterMouseDownEventArgs  : PlotterMouseEventArgs
        { PlotterMouseButton ChangedButton; int ClickCount;
          HitTestResult HitTestResult; }
    class PlotterMouseWheelEventArgs : PlotterMouseEventArgs { int Delta; }
    class PlotterKeyEventArgs        : PlotterInputEventArgs { PlotterKey Key; }
    class PlotterTouchEventArgs      : PlotterInputEventArgs
    {
        PlotterTouchEventArgs()
        PlotterTouchEventArgs(ScreenPoint[] currentTouches,
                              ScreenPoint[] previousTouches)
        ScreenPoint Position; ScreenVector DeltaScale; ScreenVector DeltaTranslation;
    }

    enum PlotterMouseButton   { None = 0, Left = 1, Middle = 2, Right = 3,
                                XButton1 = 4, XButton2 = 5 }
    enum PlotterModifierKeys  { None = 0, Control = 1, Alt = 2, Shift = 4, Windows = 8 }
    enum PlotterKey           { ... the usual key set ... }

Gestures
--------
    abstract class PlotterInputGesture : IEquatable<PlotterInputGesture>
    class PlotterMouseDownGesture(
        PlotterMouseButton mouseButton,
        PlotterModifierKeys modifiers = PlotterModifierKeys.None,
        int clickCount = 1)
    class PlotterMouseEnterGesture(
        PlotterModifierKeys modifiers = PlotterModifierKeys.None)
    class PlotterMouseWheelGesture(
        PlotterModifierKeys modifiers = PlotterModifierKeys.None)
    class PlotterKeyGesture(PlotterKey key,
                            PlotterModifierKeys modifiers = PlotterModifierKeys.None)
    class PlotterTouchGesture
    class PlotterShakeGesture

Commands
--------
    interface IViewCommand
        { void Execute(IView view, IController controller,
                       PlotterInputEventArgs args); }
    interface IViewCommand<in T> : IViewCommand where T : PlotterInputEventArgs
        { void Execute(IView view, IController controller, T args); }
    class DelegateViewCommand<T>(Action<IView, IController, T> handler)
        : IViewCommand<T>
    class DelegatePlotCommand<T>(Action<IPlotView, IController, T> handler)
        : DelegateViewCommand<T>                 // the plot-view flavour

PlotCommands is a static class of ready-made commands (all IViewCommand<T>):

    Reset, ResetAt, CopyCode
    PanAt, PanLeft, PanRight, PanUp, PanDown,
    PanLeftFine, PanRightFine, PanUpFine, PanDownFine
    ZoomRectangle, ZoomWheel, ZoomWheelFine,
    ZoomInAt, ZoomOutAt, ZoomIn, ZoomOut, ZoomInFine, ZoomOutFine
    Track, SnapTrack, PointsOnlyTrack,
    HoverTrack, HoverSnapTrack, HoverPointsOnlyTrack
    PanZoomByTouch, SnapTrackTouch, PointsOnlyTrackTouch

Controllers
-----------
    interface IController
    interface IPlotController : IController      // marker; no extra members
    abstract class ControllerBase : IController
    class PlotController : ControllerBase, IPlotController   // default bindings

    // ControllerBase
    List<InputCommandBinding> InputCommandBindings { get; }
    bool HandleMouseDown / HandleMouseMove / HandleMouseUp / HandleMouseEnter /
         HandleMouseLeave / HandleMouseWheel / HandleTouchStarted /
         HandleTouchDelta / HandleTouchCompleted / HandleKeyDown
         (IView view, <matching args>)
    bool HandleGesture(IView view, PlotterInputGesture gesture,
                       PlotterInputEventArgs args)
    void AddMouseManipulator(IView, ManipulatorBase<PlotterMouseEventArgs>,
                             PlotterMouseDownEventArgs)
    void AddHoverManipulator(IView, ManipulatorBase<PlotterMouseEventArgs>,
                             PlotterMouseEventArgs)
    void AddTouchManipulator(IView, ManipulatorBase<PlotterTouchEventArgs>,
                             PlotterTouchEventArgs)
    void Bind(PlotterMouseDownGesture, IViewCommand<PlotterMouseDownEventArgs>)
    void Bind(PlotterMouseEnterGesture, IViewCommand<PlotterMouseEventArgs>)
    void Bind(PlotterMouseWheelGesture, IViewCommand<PlotterMouseWheelEventArgs>)
    void Bind(PlotterTouchGesture, IViewCommand<PlotterTouchEventArgs>)
    void Bind(PlotterKeyGesture, IViewCommand<PlotterKeyEventArgs>)
    void Unbind(PlotterInputGesture); void Unbind(IViewCommand); void UnbindAll()

    class InputCommandBinding
    {
        InputCommandBinding(PlotterInputGesture gesture, IViewCommand command)
        InputCommandBinding(PlotterKey key, PlotterModifierKeys modifiers,
                            IViewCommand command)
        InputCommandBinding(PlotterMouseButton mouseButton,
                            PlotterModifierKeys modifiers, IViewCommand command)
        PlotterInputGesture Gesture { get; }  IViewCommand Command { get; }
    }

ControllerExtensions is the convenient way to bind:

    controller.BindMouseDown(PlotterMouseButton button,
                             IViewCommand<PlotterMouseDownEventArgs> command)
    controller.BindMouseDown(button, PlotterModifierKeys modifiers, command)
    controller.BindMouseDown(button, modifiers, int clickCount, command)
    controller.BindKeyDown(PlotterKey key, IViewCommand<PlotterKeyEventArgs> command)
    controller.BindKeyDown(key, PlotterModifierKeys modifiers, command)
    controller.BindMouseEnter(IViewCommand<PlotterMouseEventArgs> command)
    controller.BindMouseWheel(IViewCommand<PlotterMouseWheelEventArgs> command)
    controller.BindMouseWheel(PlotterModifierKeys modifiers, command)
    controller.BindTouchDown(IViewCommand<PlotterTouchEventArgs> command)
    controller.UnbindMouseDown(button, modifiers = None, clickCount = 1)
    controller.UnbindKeyDown(key, modifiers = None)
    controller.UnbindMouseEnter(); UnbindTouchDown(); UnbindMouseWheel()

A binding example:

    var controller = new PlotController();

    // Left-drag pans, right-drag draws a zoom rectangle, wheel zooms,
    // Ctrl+wheel zooms in fine steps, left-click shows the tracker.
    controller.UnbindAll();
    controller.BindMouseDown(PlotterMouseButton.Left, PlotCommands.PanAt);
    controller.BindMouseDown(PlotterMouseButton.Right, PlotCommands.ZoomRectangle);
    controller.BindMouseWheel(PlotCommands.ZoomWheel);
    controller.BindMouseWheel(PlotterModifierKeys.Control, PlotCommands.ZoomWheelFine);
    controller.BindMouseDown(PlotterMouseButton.Left, PlotterModifierKeys.Shift,
                             PlotCommands.SnapTrack);
    controller.BindKeyDown(PlotterKey.A, PlotCommands.Reset);

    // A command of your own:
    var myCommand = new DelegateViewCommand<PlotterMouseDownEventArgs>(
        (view, ctrl, args) =>
        {
            var plotView = (IPlotView)view;
            plotView.ActualModel.ZoomAllAxes(2);
            plotView.InvalidatePlot(false);
            args.Handled = true;
        });
    controller.BindMouseDown(PlotterMouseButton.Middle, myCommand);

Manipulators
------------
    abstract class ManipulatorBase<T> where T : PlotterInputEventArgs
        { IView View { get; }
          void Started(T e); void Delta(T e); void Completed(T e); }
    abstract class PlotManipulator<T> : ManipulatorBase<T>
        { IPlotView PlotView { get; }  AxisPreference AxisPreference { get; } }
    abstract class MouseManipulator : PlotManipulator<PlotterMouseEventArgs>
        { ScreenPoint StartPosition { get; } }

    class PanManipulator(IPlotView plotView)             : MouseManipulator
    class ZoomRectangleManipulator(IPlotView plotView)   : MouseManipulator
    class ZoomStepManipulator(IPlotView plotView)        : MouseManipulator
        { bool FineControl; double Step; }
    class TrackerManipulator(IPlotView plotView)         : MouseManipulator
        { bool PointsOnly, Snap, LockToInitialSeries, CheckDistanceBetweenPoints;
          double FiresDistance; }
    class TouchManipulator(IPlotView plotView)
        : PlotManipulator<PlotterTouchEventArgs>
    class TouchTrackerManipulator(IPlotView plotView)    : TouchManipulator
        { same tracker properties as TrackerManipulator }

    enum AxisPreference { None, X, Y }

TrackerHitResult is what a tracker command produces and what a host view is
asked to display:

    class TrackerHitResult
    {
        DataPoint DataPoint { get; set; }
        object Item { get; set; }         double Index { get; set; }
        PlotterRect LineExtents { get; set; }
        PlotModel PlotModel { get; set; } ScreenPoint Position { get; set; }
        Series Series { get; set; }       string Text { get; set; }
        Axis XAxis { get; }               Axis YAxis { get; }
    }

Series.GetNearestPoint(ScreenPoint point, bool interpolate) returns one directly
if you want tracking without a controller.

Host view interfaces
--------------------
    interface IView
    {
        Model ActualModel { get; }
        IController ActualController { get; }
        PlotterRect ClientArea { get; }
        void SetCursorType(CursorType cursorType);
        void ShowZoomRectangle(PlotterRect rectangle);
        void HideZoomRectangle();
    }

    interface IPlotView : IView
    {
        new PlotModel ActualModel { get; }
        void InvalidatePlot(bool updateData = true);
        void ShowTracker(TrackerHitResult trackerHitResult);
        void HideTracker();
        void SetClipboardText(string text);
    }

This package does not implement IPlotView. Implement it in your host if you want
the controller to drive a live plot, and connect the model with
((IPlotModel)model).AttachPlotView(myView).

NOTE: there are NO public events anywhere in this library. PlotModel.MouseDown,
Element.MouseMove, Axis.AxisChanged, PlotModel.TrackerChanged,
ElementCollection<T>.CollectionChanged and the rest of the legacy event family
were deprecated upstream and are not part of this port. (AxisChangedEventArgs,
TrackerEventArgs and ElementCollectionChangedEventArgs<T> still exist as types,
but nothing raises them.) Handle
input through PlotController and the gesture-to-command bindings.


SELECTION AND HIT-TESTING
=========================

Hit-testing
-----------
    class HitTestArguments
    {
        HitTestArguments(ScreenPoint point, double tolerance)
        ScreenPoint Point { get; }  double Tolerance { get; }
    }

    class HitTestResult
    {
        HitTestResult(Element element, ScreenPoint nearestHitPoint,
                      object item = null, double index = 0)
        Element Element { get; }  ScreenPoint NearestHitPoint { get; }
        object Item { get; }      double Index { get; }
    }

    // on Element
    HitTestResult HitTest(HitTestArguments args)
    // on Model (PlotModel's base)
    IEnumerable<HitTestResult> HitTest(HitTestArguments args)
    object SyncRoot { get; }
    PlotterColor SelectionColor { get; set; }

    // PlotModel derives from Model, so HitTest is available directly.
    var hits = model.HitTest(new HitTestArguments(new ScreenPoint(x, y), 10));

Selection
---------
Selection state lives on Element (through the Element.Selectable partial):

    bool Selectable { get; set; }                 // default true
    SelectionMode SelectionMode { get; set; }     // All | Single | Multiple
    void Select();          void Unselect();
    void SelectItem(int index);  void UnselectItem(int index);
    void ClearSelection();
    bool IsSelected();      bool IsItemSelected(int index);
    IEnumerable<int> GetSelectedItems()

    class Selection
    {
        static Selection Everything { get; }
        bool IsEverythingSelected()
        IEnumerable<int> GetSelectedItems()
        IEnumerable<int> GetSelectedItems(Enum feature)
        bool IsItemSelected(int index, Enum feature = null)
        void Select(int index, Enum feature = null)
        void Unselect(int index, Enum feature = null)
        void Clear()
        struct SelectionItem : IEquatable<SelectionItem>
    }

Selected elements are drawn in model.SelectionColor. ScatterErrorSeries also
offers SelectAll(Func<ScatterErrorPoint, bool>).


IMAGE CODECS
============

The library carries its own managed PNG/BMP/JPEG handling so that ImageAnnotation
and IRenderContext.DrawImage need no third-party imaging dependency. All in the
root CodeBrix.Plotter namespace.

    class PlotterImage
    {
        PlotterImage(byte[] bytes)
        PlotterImage(Stream s)
        static PlotterImage Create(PlotterColor[,] pixels, ImageFormat format,
                                   ImageEncoderOptions encoderOptions = null)
        static PlotterImage Create(byte[,] pixels, PlotterColor[] palette,
                                   ImageFormat format,
                                   ImageEncoderOptions encoderOptions = null)
        ImageFormat Format { get; }
        int Width { get; }  int Height { get; }  int BitsPerPixel { get; }
        double DpiX { get; }  double DpiY { get; }
        byte[] GetData()                  // the encoded bytes
        PlotterColor[,] GetPixels()       // decoded, indexed [x, y], [0,0] top-left
    }

    class PlotterImageInfo { int Width, Height, BitsPerPixel; double DpiX, DpiY; }

    interface IImageEncoder
    {
        byte[] Encode(PlotterColor[,] pixels);
        byte[] Encode(byte[,] pixels, PlotterColor[] palette);
    }
    interface IImageDecoder
    {
        PlotterImageInfo GetImageInfo(byte[] bytes);
        PlotterColor[,] Decode(byte[] bytes);
    }

    class PngEncoder(PngEncoderOptions options) : IImageEncoder
    class BmpEncoder(BmpEncoderOptions options) : IImageEncoder
    class PngDecoder  : IImageDecoder
    class BmpDecoder  : IImageDecoder
    class JpegDecoder : IImageDecoder          // decode only; also JpegDecoder.ExifTags

    abstract class ImageEncoderOptions
        { double DpiX { get; set; }  double DpiY { get; set; } }
    class PngEncoderOptions : ImageEncoderOptions
    class BmpEncoderOptions : ImageEncoderOptions

    enum ImageFormat { Png, Bmp, Jpeg, Unknown }

PNG and BMP can be both encoded and decoded; JPEG can only be DECODED. The PNG
support types ColorType, CompressionMethod, FilterMethod, InterlaceMethod and the
Deflate/BitReader/ByteBitReader helpers are also public.


HELPER UTILITIES
================

Root CodeBrix.Plotter namespace (NOT .Utilities -- see KEY NAMESPACES):

    Decimator.Decimate(List<ScreenPoint> input, List<ScreenPoint> output)
        Reduces a screen-space polyline for drawing. Assign it to
        LineSeries.Decimator.
    ArrayBuilder.CreateVector(x0, x1, int n) / CreateVector(x0, x1, double dx)
    ArrayBuilder.Evaluate(Func<double,double,double> f, double[] x, double[] y)
    ArrayBuilder.Fill(this double[] array, double value) / Fill2D
    ArrayExtensions.MaxOrDefault / MinOrDefault / Max2D / Min2D
    ScreenPointHelper.FindNearestPointOnPolyline / FindPointOnLine /
        FindPositionOnLine / IsPointInPolygon / ResamplePoints / GetCentroid
    StringHelper.Format / CreateValidFormatString / SplitLines
    FractionHelper.ConvertToFractionString
    ComparerHelper.CreateComparer<T>(Comparison<T>)
    HashCodeBuilder.GetHashCode(IEnumerable<object>)
    ReflectionPath("A.B.C").GetValue(instance) / TryGetValue(instance, out result)
    PlotterSizeExtensions.GetBounds / GetPolygon
        Bounding box and outline of a rotated, aligned text block.
    ListBuilder<T>, TypeExtensions, StreamExtensions, BinaryReaderExtensions,
    XmlWriterBase, Conrec, Arrays (the Deflate helper's array utilities)

CodeBrix.Plotter.Utilities namespace -- ONE public type:

    Helpers.Swap<T>(ref T, ref T)
    Helpers.ArgMin<T, TComparable>(IEnumerable<T>, Func<T, TComparable>)
    Helpers.LinearInterpolation(x0, y0, x1, y1, value)

Code generation. Every item type implements ICodeGenerating (string ToCode()),
and PlotModel.ToCode() emits C# that rebuilds the whole model -- useful for
turning an interactively built plot into source. CodeGenerator,
CodeGenerationAttribute and CodeGeneratorStringExtensions support it.


================================================================================
COMPLETE EXAMPLES
================================================================================

1 -- End to end: build a model and write a PNG file
---------------------------------------------------
    using System;
    using System.IO;
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Axes;
    using CodeBrix.Plotter.Legends;
    using CodeBrix.Plotter.Series;
    using CodeBrix.Plotter.Skia;

    public static class Program
    {
        public static void Main()
        {
            var model = new PlotModel
            {
                Title = "Trigonometric functions",
                Subtitle = "sin and cos",
                Background = PlotterColors.White,
            };

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "x",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "y",
                Minimum = -1.5,
                Maximum = 1.5,
            });

            model.Series.Add(new FunctionSeries(Math.Sin, -10, 10, 0.05, "sin(x)"));
            model.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.05, "cos(x)")
            {
                Color = PlotterColors.IndianRed,
                LineStyle = LineStyle.Dash,
            });

            model.IsLegendVisible = true;
            model.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.RightTop,
                LegendPlacement = LegendPlacement.Outside,
            });

            // The exporter calls Update() and Render() itself.
            var exporter = new PngExporter { Width = 800, Height = 480, Dpi = 96 };
            using var stream = File.Create("trig.png");
            exporter.Export(model, stream);

            // Or, in one call:
            PngExporter.Export(model, "trig-2.png", 800, 480);
        }
    }

2 -- Render onto an SKCanvas you already own (and reuse the context)
-------------------------------------------------------------------
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Skia;
    using SkiaSharp;

    public sealed class PlotRenderer : IDisposable
    {
        // One context, kept alive across frames: it caches typefaces,
        // fonts and shapers.
        private readonly SkiaRenderContext context =
            new SkiaRenderContext { RenderTarget = RenderTarget.Screen };

        public void Draw(SKCanvas canvas, PlotModel model, int width, int height,
                         float dpiScale, bool dataChanged)
        {
            context.SkCanvas = canvas;      // the context does NOT own the canvas
            context.DpiScale = dpiScale;

            var plot = (IPlotModel)model;   // Update/Render are explicit
            plot.Update(dataChanged);
            canvas.Clear(model.Background.ToSKColor());
            plot.Render(context, new PlotterRect(0, 0, width / dpiScale,
                                                       height / dpiScale));
        }

        public void Dispose() => this.context.Dispose();
    }

3 -- Render to an SKBitmap (there is no ExportToBitmap)
------------------------------------------------------
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Skia;
    using SkiaSharp;

    public static SKBitmap RenderToBitmap(PlotModel model, int width, int height)
    {
        var bitmap = new SKBitmap(width, height);

        using (var canvas = new SKCanvas(bitmap))
        using (var context = new SkiaRenderContext
               {
                   RenderTarget = RenderTarget.PixelGraphic,   // unhinted text
                   SkCanvas = canvas,
               })
        {
            var plot = (IPlotModel)model;
            plot.Update(true);
            canvas.Clear(model.Background.ToSKColor());
            plot.Render(context, new PlotterRect(0, 0, width, height));
        }

        return bitmap;      // caller disposes
    }

4 -- Bar chart (horizontal bars) and column chart (vertical bars)
-----------------------------------------------------------------
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Axes;
    using CodeBrix.Plotter.Series;

    // ---- horizontal bars -------------------------------------------------
    var bars = new PlotModel { Title = "Revenue by region" };

    var categories = new CategoryAxis { Position = AxisPosition.Left };
    categories.Labels.Add("North");
    categories.Labels.Add("South");
    categories.Labels.Add("East");
    categories.Labels.Add("West");
    bars.Axes.Add(categories);
    bars.Axes.Add(new LinearAxis
    {
        Position = AxisPosition.Bottom,
        MinimumPadding = 0,
        MaximumPadding = 0.06,
        ExtraGridlines = new[] { 0d },
    });

    var y2025 = new BarSeries { Title = "2025", LabelFormatString = "{0}",
                                LabelPlacement = LabelPlacement.Inside };
    y2025.Items.Add(new BarItem(120));
    y2025.Items.Add(new BarItem(95));
    y2025.Items.Add(new BarItem(140));
    y2025.Items.Add(new BarItem(70));

    var y2026 = new BarSeries { Title = "2026", LabelFormatString = "{0}" };
    y2026.Items.Add(new BarItem(150));
    y2026.Items.Add(new BarItem(88));
    y2026.Items.Add(new BarItem(165));
    y2026.Items.Add(new BarItem(102));

    bars.Series.Add(y2025);
    bars.Series.Add(y2026);
    // Stack them instead of clustering:
    //   y2025.IsStacked = y2026.IsStacked = true;
    //   y2025.StackGroup = y2026.StackGroup = "revenue";

    // ---- vertical columns: same series type, transposed axes -------------
    var cols = new PlotModel { Title = "Revenue by region" };

    var colCategories = new CategoryAxis { Position = AxisPosition.Bottom,
                                           Key = "categories" };
    colCategories.Labels.Add("North");
    colCategories.Labels.Add("South");
    cols.Axes.Add(colCategories);
    cols.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = "values" });

    var columns = new BarSeries { Title = "2026",
                                  XAxisKey = "values",       // X is the VALUE axis
                                  YAxisKey = "categories" }; // Y is the CATEGORY axis
    columns.Items.Add(new BarItem(150));
    columns.Items.Add(new BarItem(88));
    cols.Series.Add(columns);

5 -- Candlestick chart with a volume pane
-----------------------------------------
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Axes;
    using CodeBrix.Plotter.Series;

    var model = new PlotModel { Title = "OHLC" };

    // Price occupies the top 70% of the plot area, volume the bottom 25%.
    model.Axes.Add(new DateTimeAxis
    {
        Position = AxisPosition.Bottom,
        StringFormat = "yyyy-MM-dd",
        Key = "t",
    });
    model.Axes.Add(new LinearAxis
    {
        Position = AxisPosition.Left, Title = "Price",
        StartPosition = 0.30, EndPosition = 1.0, Key = "price",
    });
    model.Axes.Add(new LinearAxis
    {
        Position = AxisPosition.Left, Title = "Volume",
        StartPosition = 0.0, EndPosition = 0.25, Key = "volume",
    });

    var candles = new CandleStickSeries
    {
        XAxisKey = "t", YAxisKey = "price",
        IncreasingColor = PlotterColors.DarkGreen,
        DecreasingColor = PlotterColors.Red,
        CandleWidth = 4,
    };
    var volume = new VolumeSeries
    {
        XAxisKey = "t", YAxisKey = "volume",
        VolumeStyle = VolumeStyle.PositiveNegative,
    };

    foreach (var bar in myBars)     // your own data
    {
        double t = DateTimeAxis.ToDouble(bar.Timestamp);
        candles.Items.Add(new HighLowItem(t, bar.High, bar.Low, bar.Open, bar.Close));
        volume.Items.Add(new OhlcvItem(t, bar.Open, bar.High, bar.Low, bar.Close,
                                       bar.BuyVolume, bar.SellVolume));
    }

    model.Series.Add(candles);
    model.Series.Add(volume);

6 -- Histogram from raw samples
-------------------------------
    using System.Collections.Generic;
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Axes;
    using CodeBrix.Plotter.Series;

    var samples = new List<double>();          // your measurements

    var binBreaks = HistogramHelpers.CreateUniformBins(0, 100, 20);
    var items = HistogramHelpers.Collect(
        samples,
        binBreaks,
        new BinningOptions(BinningOutlierMode.CountOutliers,
                           BinningIntervalType.InclusiveLowerBound,
                           BinningExtremeValueMode.IncludeExtremeValues));

    var histogram = new HistogramSeries
    {
        Title = "Distribution",
        FillColor = PlotterColors.SteelBlue,
        StrokeColor = PlotterColors.Black,
        StrokeThickness = 1,
        // Colour tall bins differently:
        ColorMapping = item => item.Height > 10
            ? PlotterColors.IndianRed
            : PlotterColors.SteelBlue,
    };
    histogram.Items.AddRange(items);

    var model = new PlotModel { Title = "Histogram" };
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Value" });
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Density" });
    model.Series.Add(histogram);

7 -- Heat map with a colour axis
--------------------------------
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Axes;
    using CodeBrix.Plotter.Series;

    var data = new double[100, 80];
    for (int x = 0; x < 100; x++)
    for (int y = 0; y < 80; y++)
    {
        data[x, y] = System.Math.Sin(x / 10.0) * System.Math.Cos(y / 8.0);
    }

    var model = new PlotModel { Title = "Heat map" };
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
    model.Axes.Add(new LinearColorAxis
    {
        Position = AxisPosition.Right,
        Palette = PlotterPalettes.Jet(200),      // a METHOD taking a colour count
        HighColor = PlotterColors.Gray,
        LowColor  = PlotterColors.Black,
        Key = "colors",
    });

    model.Series.Add(new HeatMapSeries
    {
        X0 = 0, X1 = 99, Y0 = 0, Y1 = 79,
        Data = data,
        Interpolate = true,
        CoordinateDefinition = HeatMapCoordinateDefinition.Center,
        RenderMethod = HeatMapRenderMethod.Bitmap,
        ColorAxisKey = "colors",
    });

8 -- Scatter plot with per-point size and colour, plus annotations
------------------------------------------------------------------
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Annotations;
    using CodeBrix.Plotter.Axes;
    using CodeBrix.Plotter.Series;

    var model = new PlotModel { Title = "Measurements" };
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });
    model.Axes.Add(new LinearColorAxis
    {
        Position = AxisPosition.Right,
        Palette = PlotterPalettes.Viridis(),      // count defaults to 256
        Key = "colors",
    });

    var scatter = new ScatterSeries
    {
        MarkerType = MarkerType.Circle,
        ColorAxisKey = "colors",
    };
    scatter.Points.Add(new ScatterPoint(1, 2, size: 6, value: 0.2));
    scatter.Points.Add(new ScatterPoint(2, 4, size: 12, value: 0.9));
    model.Series.Add(scatter);

    model.Annotations.Add(new LineAnnotation
    {
        Type = LineAnnotationType.Horizontal,
        Y = 3,
        Color = PlotterColors.Red,
        LineStyle = LineStyle.Dash,
        Text = "threshold",
        TextOrientation = AnnotationTextOrientation.Horizontal,
        Layer = AnnotationLayer.AboveSeries,
    });
    model.Annotations.Add(new TextAnnotation
    {
        Text = "outlier",
        TextPosition = new DataPoint(2, 4),
        TextHorizontalAlignment = HorizontalAlignment.Left,
        TextVerticalAlignment = VerticalAlignment.Bottom,
        Offset = new ScreenVector(8, -8),
    });

9 -- Binding to your own objects instead of adding points
---------------------------------------------------------
    public sealed class Reading
    {
        public double Seconds { get; set; }
        public double Volts   { get; set; }
    }

    var readings = new List<Reading>();          // filled elsewhere

    var series = new LineSeries
    {
        Title = "Channel A",
        ItemsSource = readings,
        DataFieldX = nameof(Reading.Seconds),
        DataFieldY = nameof(Reading.Volts),
        MarkerType = MarkerType.None,
        StrokeThickness = 1.5,
    };

    // Alternative: no reflection, an explicit mapping delegate.
    series.DataFieldX = null;
    series.DataFieldY = null;
    series.Mapping = o => new DataPoint(((Reading)o).Seconds, ((Reading)o).Volts);

    // Alternative: make Reading implement IDataPointProvider and set only
    // ItemsSource.

10 -- A font-isolated render (no system fonts at all)
-----------------------------------------------------
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Skia;
    using SkiaSharp;

    // Typefaces YOU own; the resolver never disposes them and neither does
    // the render context.
    using var regular = SKTypeface.FromFile("Fonts/Roboto-Regular.ttf");
    using var bold    = SKTypeface.FromFile("Fonts/Roboto-Bold.ttf");

    using var context = new SkiaRenderContext
        { RenderTarget = RenderTarget.PixelGraphic };
    context.TypefaceResolver = (fontFamily, fontWeight) =>
        fontWeight >= FontWeights.Bold ? bold : regular;   // never returns null

    var model = new PlotModel { Title = "Isolated fonts", DefaultFont = "Roboto" };
    // ... axes and series ...

    using var bitmap = new SKBitmap(400, 300);
    using var canvas = new SKCanvas(bitmap);
    context.SkCanvas = canvas;
    canvas.Clear(SKColors.White);

    var plot = (IPlotModel)model;
    plot.Update(true);
    plot.Render(context, new PlotterRect(0, 0, 400, 300));


================================================================================
MINIMUM VIABLE PROJECT
================================================================================

A console application that writes a PNG. Two files.

MyPlotApp.csproj
----------------
    <Project Sdk="Microsoft.NET.Sdk">

      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>disable</Nullable>
      </PropertyGroup>

      <ItemGroup>
        <PackageReference Include="CodeBrix.Plotter.MitLicenseForever" />
        <!-- The native SkiaSharp binaries for the platform you run on.
             Pick the one(s) you need; SkiaSharp brings Win32 and macOS in
             transitively for net10.0, Linux must be named explicitly. -->
        <PackageReference Include="SkiaSharp.NativeAssets.Linux" />
        <!-- Only needed if any text is shaped through HarfBuzz on Linux. -->
        <PackageReference Include="HarfBuzzSharp.NativeAssets.Linux" />
      </ItemGroup>

    </Project>

(Add version attributes to taste; this file deliberately states none.)

Program.cs
----------
    using System;
    using System.IO;
    using CodeBrix.Plotter;
    using CodeBrix.Plotter.Axes;
    using CodeBrix.Plotter.Series;
    using CodeBrix.Plotter.Skia;

    var model = new PlotModel { Title = "Hello, plot" };
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

    var line = new LineSeries { Title = "Data", MarkerType = MarkerType.Circle };
    line.Points.Add(new DataPoint(0, 0));
    line.Points.Add(new DataPoint(1, 3));
    line.Points.Add(new DataPoint(2, 1));
    model.Series.Add(line);

    PngExporter.Export(model, "hello.png", 640, 400);
    Console.WriteLine(Path.GetFullPath("hello.png"));


================================================================================
PERFORMANCE TIPS
================================================================================

  * KEEP ONE SkiaRenderContext ALIVE across frames. It caches SKTypeface,
    SKFont and SKShaper instances per font descriptor; a new context per frame
    throws that cache away and re-resolves every font on every draw. Reassign
    SkCanvas instead of recreating the context, and Dispose() only when you are
    finished plotting.
  * Update(false) when only the layout changed. Update(true) re-reads every
    ItemsSource, re-projects every point and recomputes every axis range. A
    resize, a scroll or a re-render of unchanged data only needs
    Update(false) -- or InvalidatePlot(false) through the view.
  * Set UseTextShaping = false on SkiaRenderContext (and on Png/Pdf exporters)
    when the plot text is plain Latin. Shaping through HarfBuzz costs real time
    and buys nothing for ASCII labels.
  * Decimate long polylines. Assign LineSeries.Decimator = Decimator.Decimate
    (or your own) so a series with hundreds of thousands of points collapses to
    one screen-space segment per column of pixels.
  * Raise MinimumSegmentLength on LineSeries, ContourSeries, VectorSeries and
    PolygonAnnotation to drop sub-pixel segments before they reach the canvas.
  * Use ScatterSeries.BinSize for dense scatter plots. It bins points into
    screen-space cells so overlapping markers are drawn once.
  * HeatMapSeries: RenderMethod = HeatMapRenderMethod.Bitmap is much faster than
    Rectangles for large grids. Mutating Data in place needs a call to
    Invalidate() -- but that is still cheaper than rebuilding the series.
  * Prefer .Points / .Items over ItemsSource + DataFieldX/DataFieldY when the
    data is already in memory. The DataField path resolves properties by
    reflection (ReflectionPath) on every update; a Mapping delegate avoids the
    reflection, and filling Points directly avoids both.
  * Set explicit Minimum/Maximum on axes for streaming data. Auto-ranging walks
    every point of every series on every Update(true).
  * RenderTarget.Screen enables hinting and subpixel text, which is what you
    want on a live canvas; RenderTarget.PixelGraphic / VectorGraphic disable
    hinting, which is what you want for a deterministic exported image.
  * EdgeRenderingMode.PreferSpeed on a series or on the model skips the
    pixel-snapping work that Adaptive / PreferSharpness do.
  * The core (non-Skia) SvgExporter measures text through PdfRenderContext and
    the built-in font metrics. It is slower and less accurate than the Skia SVG
    exporter; use it only when you must avoid the native dependency.


================================================================================
COMMON PITFALLS TO AVOID
================================================================================

  * Forgetting to Update() the model. A freshly built PlotModel has no resolved
    axis ranges; rendering it without ((IPlotModel)model).Update(true) produces
    an empty or wrong plot. (The exporters do this for you; hand-rolled
    rendering does not.)
  * Calling Update()/Render() directly on a PlotModel variable. They are
    EXPLICIT IPlotModel implementations -- you must cast. Passing a PlotModel to
    something that takes IPlotModel (every exporter) needs no cast.
  * Looking for ColumnSeries. It does not exist. One transposable BarSeries
    draws both bars and columns; the axis positions decide the orientation, and
    the transposed form is the one where the series' X axis is the VALUE axis on
    the Left and its Y axis is the CategoryAxis on the Bottom.
  * Looking for PngExporter.ExportToBitmap(model) or any other ExportToBitmap.
    No exporter has one. Use Export(IPlotModel, Stream), the static
    Export(model, path|stream, width, height, ...) overloads, or render onto an
    SKBitmap yourself.
  * Treating PlotterPalettes members as properties. Almost all of them are
    METHODS taking a colour count -- PlotterPalettes.Jet(200), not
    PlotterPalettes.Jet. Only BlueWhiteRed31, Hot64 and Hue64 are properties.
  * Assuming folder == namespace. There is no CodeBrix.Plotter.Input,
    .Rendering, .Graphics, .Imaging, .PlotController, .PlotModel, .PlotView,
    .Pdf, .Svg or .Foundation namespace, and CodeBrix.Plotter.Utilities holds
    only the Helpers class -- Decimator, HistogramHelpers, BinningOptions and
    the rest are in the ROOT namespace.
  * Comparing PlotterColor against null. It is a struct with sentinel values;
    use IsUndefined(), IsAutomatic(), IsInvisible(), IsVisible().
  * Adding a BarSeries without a CategoryAxis, or adding more category labels
    than there are items (or fewer). Every BarItem lands in the next free
    category slot unless you set BarItemBase.CategoryIndex explicitly.
  * Mixing up which axis a bar series' XAxisKey names. XAxisKey is always the
    series' X axis, which for a vertical column chart is the VALUE axis.
  * Ambiguity between CodeBrix.Plotter.SvgExporter (the dependency-free core
    writer) and CodeBrix.Plotter.Skia.SvgExporter (the SkiaSharp one). With both
    namespaces imported you must qualify the type name.
  * Looking for a core PdfExporter. There is none; use
    CodeBrix.Plotter.Skia.PdfExporter.
  * Looking for the legacy input EVENTS. PlotModel.MouseDown, Element.MouseMove,
    Axis.AxisChanged, PlotModel.TrackerChanged,
    ElementCollection<T>.CollectionChanged and the rest of that family were
    deprecated upstream and are NOT part of this library -- there are no public
    events at all. Handle input through PlotController and gesture-to-command
    bindings.
  * Expecting a view. This library renders; it does not host. There is no
    PlotView control here, and IPlotView is an interface for YOU to implement.
  * Mutating a PlotModel from several threads, or while it is rendering. It is
    not thread-safe. Model.SyncRoot is there to lock on.
  * Reusing a SkiaRenderContext across canvases without reassigning SkCanvas --
    or disposing the SKCanvas expecting the context to have taken ownership. It
    never owns the canvas.
  * Expecting a TypefaceResolver miss to fall back to the system font lookup. It
    never does. Return a default typeface from the resolver instead of null, and
    do not dispose a resolver-supplied typeface while the render context might
    still draw with it -- the context caches it, but the resolver owns it.
  * Forgetting the native asset package. Without SkiaSharp.NativeAssets.<os>
    (and HarfBuzzSharp.NativeAssets.Linux for shaped text on Linux) the first
    render throws a DllNotFoundException, not a plotting error.
  * Swallowing render errors. Exceptions raised while a plot renders are
    captured; check model.GetLastPlotException() when a plot comes out blank.
  * Using OxyPlot names. There is no OxyPlot namespace and no Oxy* type in this
    library; they are CodeBrix.Plotter and Plotter*.
  * Nullable reference types. This library is compiled with NRT OFF, so its
    public surface carries no ? / ! annotations. Consuming it from an
    NRT-enabled project is fine, but do not expect null-state analysis on its
    API.


================================================================================
WHAT THIS PACKAGE DOES NOT DO
================================================================================

  * It provides no UI control for any framework. The upstream OxyPlot.Wpf,
    OxyPlot.WindowsForms, OxyPlot.Avalonia and similar view packages are not
    part of this port. IPlotView and IView are interfaces for a host to
    implement; nothing here implements them.
  * It does not render through System.Drawing, ImageSharp or PdfSharp. The
    upstream OxyPlot.Core.Drawing, OxyPlot.ImageSharp and OxyPlot.Pdf packages
    are not part of this port.
  * It does not ship native SkiaSharp or HarfBuzz binaries. A consuming
    application adds the SkiaSharp.NativeAssets.* (and, on Linux,
    HarfBuzzSharp.NativeAssets.Linux) package for its own platform.
  * It cannot ENCODE JPEG through its own codecs. PngEncoder and BmpEncoder
    exist; JpegDecoder is decode-only. JPEG output comes from
    CodeBrix.Plotter.Skia.JpegExporter, which encodes through SkiaSharp.
  * It does not import plots. There is no reader for SVG, PDF or any chart
    file format -- only writers.
  * It has no data-binding infrastructure beyond ItemsSource +
    DataField*/Mapping. There is no INotifyPropertyChanged or
    INotifyCollectionChanged plumbing; after mutating data you call
    InvalidatePlot(true) yourself.
  * It exposes no public events; see COMMON PITFALLS.
  * It is net10.0-only. There is no netstandard, net8.0 or .NET Framework build,
    and it does not multi-target.
  * It does not do 3D, geographic projection, or map tiles.


================================================================================
WORKING EXAMPLES ON GITHUB
================================================================================

The library's own test suite is the largest body of working, compiling usage.
Browse it at:

  https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests

Feature-to-file map:

  Building models, axes, series, annotations of every kind (hundreds of small
  complete PlotModel factory methods, one per chart variation):
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/ExampleLibrary
    ExampleLibrary/Series/     one file per series type
                               (BarSeriesExamples.cs, BoxPlotSeriesExamples.cs,
                                HistogramSeriesExamples.cs, HeatMapSeriesExamples.cs,
                                ContourSeriesExamples.cs, PieSeriesExamples.cs,
                                ScatterSeriesExamples.cs, VectorSeriesExamples.cs,
                                TornadoBarSeriesExamples.cs, ...
                                and FinancialSeries/ for candlestick and volume)
    ExampleLibrary/Axes/       one file per axis type
    ExampleLibrary/Annotations/ one file per annotation type
    ExampleLibrary/Showcases/  complete, realistic charts
    ExampleLibrary/CustomSeries/ how to subclass Series

  Axis behaviour, ranges, ticks, DateTime and TimeSpan conversion:
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Axes

  SkiaRenderContext, the TypefaceResolver contract, and every exporter:
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Skia
    Skia/SkiaRenderContextTests.cs  render-to-bitmap, context reuse, resolver
    Skia/PngExporterTests.cs        instance and static Export overloads
    Skia/JpegExporterTests.cs, Skia/PdfExporterTests.cs, Skia/SvgExporterTests.cs

  The controller, commands and gesture bindings:
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/PlotController

  PlotModel lifecycle, ElementCollection, colour and geometry value types:
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/PlotModel
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Graphics
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Foundation

  Image codecs and the core SVG / PDF writers:
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Imaging
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Svg
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Pdf

  Decimation, binning, histogram helpers, array and reflection utilities:
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Utilities
    https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests/Rendering

A larger, real application that streams live data into a plot -- including a
reusable SkiaRenderContext, an SKCanvas host and an axis-rescaling strategy --
is in the repository's sample:

  https://github.com/ellisnet/CodeBrix.Plotter/tree/main/samples/PicoScope

The ExampleLibrary and the samples are NOT part of the NuGet package; they are
reference material only.


================================================================================
QUICK REFERENCE CARD
================================================================================

INSTALL
    dotnet add package CodeBrix.Plotter.MitLicenseForever
    + SkiaSharp.NativeAssets.<Linux|Win32|macOS>
    + HarfBuzzSharp.NativeAssets.Linux   (shaped text on Linux)

USINGS
    CodeBrix.Plotter | .Axes | .Series | .Annotations | .Legends |
    .Axes.Rendering | .Utilities | .Skia

LIFECYCLE
    var model = new PlotModel { Title = "..." };
    model.Axes.Add(...); model.Series.Add(...);
    model.Annotations.Add(...); model.Legends.Add(...);
    ((IPlotModel)model).Update(updateData: true);
    ((IPlotModel)model).Render(renderContext, new PlotterRect(0, 0, w, h));
    model.InvalidatePlot(updateData);          // ask an attached view to redraw
    model.GetLastPlotException();              // why the plot came out blank

EXPORT
    new PngExporter  { Width, Height, Dpi, UseTextShaping }.Export(model, stream)
    new JpegExporter { Width, Height, Dpi, Quality }.Export(model, stream)
    new PdfExporter  { Width, Height, UseTextShaping }.Export(model, stream)
    new SvgExporter  { Width, Height }.Export(model, stream)      // .Skia
    PngExporter.Export(model, path, width, height, dpi = 96)      // static
    JpegExporter.Export(model, path, width, height, quality, dpi = 96)
    PdfExporter.Export(model, path, width, height)
    CodeBrix.Plotter.SvgExporter.ExportToString(model, w, h, isDocument)
    (there is NO ExportToBitmap anywhere)

RENDER TO A CANVAS
    using var ctx = new SkiaRenderContext
        { RenderTarget = RenderTarget.Screen, SkCanvas = canvas, DpiScale = 1f,
          UseTextShaping = true, MiterLimit = 10 };
    ctx.TypefaceResolver = (family, weight) => myTypeface;   // owns resolution
    color.ToSKColor();  skColor.ToPlotterColor();            // SkiaExtensions

AXES (CodeBrix.Plotter.Axes)
    LinearAxis, LogarithmicAxis, CategoryAxis, DateTimeAxis, TimeSpanAxis,
    AngleAxis(+FullPlotArea), MagnitudeAxis(+FullPlotArea),
    LinearColorAxis, LogarithmicColorAxis, CategoryColorAxis, RangeColorAxis
    Position | Title | Key | Minimum/Maximum | MajorStep/MinorStep |
    MajorGridlineStyle | StringFormat | LabelFormatter | StartPosition/EndPosition |
    AbsoluteMinimum/AbsoluteMaximum | IsZoomEnabled/IsPanEnabled | TickStyle | Layer
    DateTimeAxis.ToDouble/ToDateTime/CreateDataPoint; TimeSpanAxis.ToDouble/ToTimeSpan

SERIES -> ITEM TYPE (CodeBrix.Plotter.Series)
    LineSeries/Area/TwoColor*/ThreeColorLine/Extrapolation/StairStep/Stem/
      LinearBar/FunctionSeries   -> .Points, DataPoint
    ScatterSeries                -> .Points, ScatterPoint
    ScatterErrorSeries           -> .Points, ScatterErrorPoint
    BarSeries                    -> .Items,  BarItem
    ErrorBarSeries               -> .Items,  ErrorBarItem
    IntervalBarSeries            -> .Items,  IntervalBarItem
    TornadoBarSeries             -> .Items,  TornadoBarItem
    RectangleBarSeries           -> .Items,  RectangleBarItem
    BoxPlotSeries                -> .Items,  BoxPlotItem
    HistogramSeries              -> .Items,  HistogramItem
    RectangleSeries              -> .Items,  RectangleItem
    VectorSeries                 -> .Items,  VectorItem
    HighLowSeries/CandleStick    -> .Items,  HighLowItem
    VolumeSeries                 -> .Items,  OhlcvItem
    PieSeries                    -> .Slices, PieSlice
    HeatMapSeries                -> .Data (double[,])
    ContourSeries                -> .Data + .ColumnCoordinates + .RowCoordinates
    NO ColumnSeries -- BarSeries transposes (X axis vertical => transposed)

ANNOTATIONS (CodeBrix.Plotter.Annotations)
    Text, Arrow, Line, Function, Polyline, Polygon, Rectangle, Ellipse, Point,
    Image  -- Layer = BelowAxes | BelowSeries | AboveSeries

LEGENDS (CodeBrix.Plotter.Legends)
    model.IsLegendVisible = true; model.Legends.Add(new Legend { ... });
    LegendPosition | LegendPlacement | LegendOrientation | LegendItemOrder |
    LegendSymbolPlacement | LegendMaxWidth/MaxHeight | Key <-> Series.LegendKey

CONTROLLER (root namespace)
    var c = new PlotController();
    c.BindMouseDown(PlotterMouseButton.Left, PlotCommands.PanAt);
    c.BindMouseWheel(PlotCommands.ZoomWheel);
    c.BindKeyDown(PlotterKey.A, PlotCommands.Reset);
    c.Bind(new PlotterMouseDownGesture(PlotterMouseButton.Right),
           PlotCommands.ZoomRectangle);
    PlotCommands: Reset/ResetAt/CopyCode, PanAt/Pan{Left,Right,Up,Down}(Fine),
      ZoomRectangle/ZoomWheel(Fine)/Zoom{In,Out}(At|Fine), Track/SnapTrack/
      PointsOnlyTrack, Hover*, PanZoomByTouch, SnapTrackTouch, PointsOnlyTrackTouch
    Manipulators: Pan, ZoomRectangle, ZoomStep, Tracker, Touch, TouchTracker
    TrackerHitResult; CursorType; IPlotView (you implement it)

SELECTION / HIT-TESTING
    model.HitTest(new HitTestArguments(point, tolerance))   // PlotModel : Model
    element.Select()/SelectItem(i)/IsSelected()/ClearSelection(); SelectionMode
    Selection.Everything; model.SelectionColor

COLOUR
    PlotterColor.FromRgb/FromArgb/FromAColor/FromHsv/FromUInt32/Parse/Interpolate
    IsUndefined()/IsAutomatic()/IsInvisible()/IsVisible()  -- never == null
    PlotterColors.<143 names>, .Undefined, .Automatic
    PlotterPalettes.Jet(n)/Rainbow(n)/Hot(n)/Cool(n)/Gray(n)/Hue(n)/
      HueDistinct(n)/BlackWhiteRed(n)/BlueWhiteRed(n)         -- METHODS
    PlotterPalettes.Viridis()/Plasma()/Inferno()/Magma()/Cividis()  -- n = 256
    PlotterPalettes.BlueWhiteRed31/Hot64/Hue64                -- properties

ENUMS
    LineStyle | LineJoin | MarkerType | HorizontalAlignment | VerticalAlignment |
    EdgeRenderingMode | PlotType | TitleHorizontalAlignment | SelectionMode |
    CursorType | ImageFormat | AxisPosition | AxisLayer | TickStyle |
    DateTimeIntervalType | LabelPlacement | VolumeStyle | LineLegendPosition |
    HeatMapCoordinateDefinition | HeatMapRenderMethod | AnnotationLayer |
    AnnotationTextOrientation | LineAnnotationType | FunctionAnnotationType |
    RenderTarget | PlotterMouseButton | PlotterModifierKeys | PlotterKey |
    AxisPreference | Binning{OutlierMode,IntervalType,ExtremeValueMode}
    FontWeights.Normal = 400, FontWeights.Bold = 700 (doubles)

IMAGES
    new PlotterImage(bytes|stream); PlotterImage.Create(pixels, ImageFormat.Png)
    PngEncoder/BmpEncoder (encode), PngDecoder/BmpDecoder/JpegDecoder (decode)

REMEMBER
    Update() and Render() are EXPLICIT IPlotModel members -- cast first.
    Exporters do Update()+Render() for you.
    No ColumnSeries. No ExportToBitmap. No public events. No view control.
    PlotterPalettes are mostly methods. Folder != namespace.


================================================================================
END OF AGENT-README
================================================================================
