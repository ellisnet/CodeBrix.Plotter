# CodeBrix.Plotter

A fully managed, cross-platform plotting library for .NET that draws charts, graphs and plots onto a SkiaSharp canvas.
You describe a plot as a `PlotModel` — axes, series, annotations, legends — and CodeBrix.Plotter renders it, either straight onto an `SKCanvas` you already have or into a PNG, JPEG, PDF or SVG file.
CodeBrix.Plotter is a port of [OxyPlot](https://github.com/oxyplot/oxyplot) that combines the OxyPlot.Core and OxyPlot.SkiaSharp packages into a single library, and is provided as a .NET 10 library and associated `CodeBrix.Plotter.MitLicenseForever` NuGet package.

CodeBrix.Plotter's only dependencies are SkiaSharp and SkiaSharp.HarfBuzz, pinned to version 4.151.0 to match the rest of the CodeBrix family.

CodeBrix.Plotter supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

## CodeBrix.Plotter supports:

* **Over 40 series types** — line, area, bar, column, scatter, stem, stair-step, pie, heat map, contour, candlestick, box plot, error bar, histogram, rectangle, vector, tornado, two-color and extrapolation variants, and more
* **Axis types** — linear, logarithmic, date/time, time span, category, angle, magnitude, colour and range-colour axes, with full control over ticks, gridlines, formatting and zoom/pan limits
* **Annotations** — text, arrow, line, function, rectangle, ellipse, point, polygon, polyline, image and tile-map annotations
* **Legends** — placement inside or outside the plot area, multi-column layouts, item ordering and custom symbol rendering
* **Rendering to a SkiaSharp canvas** — `SkiaRenderContext` implements `IRenderContext` against any `SKCanvas`, so a plot can be drawn into whatever surface your application already owns
* **Exporting** — PNG, JPEG, PDF and SVG exporters built on SkiaSharp, plus a built-in dependency-free SVG writer inherited from OxyPlot core
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

## Attribution

CodeBrix.Plotter is derived from [OxyPlot](https://github.com/oxyplot/oxyplot), Copyright (c) 2014 OxyPlot contributors, used under the MIT License.
See `THIRD-PARTY-NOTICES.txt` for the full attribution, the exact upstream baseline, and the list of modifications made during the port.

## License

The project is licensed under the MIT License. see: https://en.wikipedia.org/wiki/MIT_License
