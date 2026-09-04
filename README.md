# CodeBrix.Plotter

A fully managed, cross-platform plotting library for .NET that draws charts, graphs and plots onto a SkiaSharp canvas.
You describe a plot as a `PlotModel` — axes, series, annotations, legends — and CodeBrix.Plotter renders it, either straight onto an `SKCanvas` you already have or into a PNG, JPEG, PDF or SVG file.
CodeBrix.Plotter is provided as a .NET 10 library and associated `CodeBrix.Plotter.MitLicenseForever` NuGet package.

CodeBrix.Plotter supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

## Installation

```
dotnet add package CodeBrix.Plotter.MitLicenseForever
```

Note that the NuGet package ID and the namespace are different - there is no package named plain `CodeBrix.Plotter`:

* NuGet package ID: `CodeBrix.Plotter.MitLicenseForever`
* Assembly and primary namespace: `CodeBrix.Plotter` - i.e. `using CodeBrix.Plotter;`

XML documentation (IntelliSense) ships alongside the assembly.

The package pulls in the following automatically; no version pinning is needed in the consuming project:

* `SkiaSharp` - the rendering engine
* `SkiaSharp.HarfBuzz` - text shaping for complex scripts

Both are matched to the versions used by the rest of the CodeBrix family.

SkiaSharp is taken as a managed-only dependency, so that a consuming application chooses the native asset package matching its own target platform. The Windows and macOS native assets arrive transitively; a Linux application must add them itself:

```
dotnet add package SkiaSharp.NativeAssets.Linux
dotnet add package HarfBuzzSharp.NativeAssets.Linux
```

Without those, a Linux build still compiles and then fails at run time on the first render with a native-library load error.

## CodeBrix.Plotter supports:

* **A broad set of series types** — line, area, bar, linear bar, interval bar, scatter, stem, stair-step, pie, heat map, contour, candlestick, box plot, error bar, histogram, rectangle, vector, tornado, volume, high/low, two-colour, three-colour and extrapolation variants
* **Axis types** — linear, logarithmic, date/time, time span, category, angle, magnitude, colour and range-colour axes, with full control over ticks, gridlines, formatting and zoom/pan limits
* **Annotations** — text, arrow, line, function, rectangle, ellipse, point, polygon, polyline, image and tile-map annotations
* **Legends** — placement inside or outside the plot area, multi-column layouts, item ordering and custom symbol rendering
* **Rendering to a SkiaSharp canvas** — `SkiaRenderContext` implements `IRenderContext` against any `SKCanvas`, so a plot can be drawn into whatever surface your application already owns
* **Exporting** — PNG, JPEG, PDF and SVG exporters built on SkiaSharp, plus a built-in dependency-free SVG writer
* **Text shaping** — complex scripts are shaped through SkiaSharp.HarfBuzz
* **An input and controller model** — mouse, touch and keyboard gestures bound to pan, zoom, tracker and reset commands, ready to be wired to a host UI framework
* **Image decoding** — built-in PNG and BMP decoders and a PNG encoder, with no third-party imaging dependency

## Sample Code

### Building a plot and exporting it to a PNG file

```csharp
using System.IO;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Series;
using CodeBrix.Plotter.Skia;

var model = new PlotModel { Title = "Trigonometric functions" };
model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "x" });
model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "y" });

model.Series.Add(new FunctionSeries(Math.Sin, -10, 10, 0.05, "sin(x)"));
model.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.05, "cos(x)")
{
    Color = PlotterColors.IndianRed,
});

var exporter = new PngExporter { Width = 800, Height = 480 };
using var stream = File.Create("trig.png");
exporter.Export(model, stream);
```

### Rendering onto an SKCanvas you already have

```csharp
using CodeBrix.Plotter;
using CodeBrix.Plotter.Skia;
using SkiaSharp;

public void DrawPlot(SKCanvas canvas, PlotModel model, int width, int height)
{
    using var renderContext = new SkiaRenderContext
    {
        RenderTarget = RenderTarget.Screen,
        SkCanvas = canvas,
    };

    ((IPlotModel)model).Update(true);
    ((IPlotModel)model).Render(renderContext, new PlotterRect(0, 0, width, height));
}
```

### Adding data series from your own objects

```csharp
using CodeBrix.Plotter;
using CodeBrix.Plotter.Series;

var series = new LineSeries
{
    Title = "Measurements",
    MarkerType = MarkerType.Circle,
    MarkerSize = 3,
};

foreach (var reading in readings)
{
    series.Points.Add(new DataPoint(reading.Time, reading.Value));
}

model.Series.Add(series);
```

## Documentation

The NuGet package includes `AGENT-README.txt`, a complete API reference and usage guide written for AI coding agents - point your agent at that file when it is writing code against this library.

Additional sample code and usage examples are available in the `CodeBrix.Plotter.Tests` project:
https://github.com/ellisnet/CodeBrix.Plotter/tree/main/tests/CodeBrix.Plotter.Tests

A complete working application - live oscilloscope traces streamed into a `PlotModel` and rendered through two different SkiaSharp canvas hosts - is in the PicoScope sample, which has a long-form guide of its own:
https://github.com/ellisnet/CodeBrix.Plotter/tree/main/samples/PicoScope

## License

CodeBrix.Plotter is licensed under the MIT License - see the
[LICENSE](https://github.com/ellisnet/CodeBrix.Plotter/blob/main/LICENSE) file.

For licensing and provenance information about the open source code included in
this package, see [THIRD-PARTY-NOTICES.txt](https://github.com/ellisnet/CodeBrix.Plotter/blob/main/THIRD-PARTY-NOTICES.txt).
