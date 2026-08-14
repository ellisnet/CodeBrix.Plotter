// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScatterSeriesExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Calculates the Least squares fit of a list of DataPoints.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Annotations;
using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Legends;
using CodeBrix.Plotter.Series;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("ScatterSeries")]
[Tags("Series")]
public class ScatterSeriesExamples
{
    [Example("Correlated points")]
    [DocumentationExample("Series/ScatterSeries")]
    public static PlotModel CorrelatedScatter()
    {
        return CreateCorrelatedScatter(1000);
    }

    [Example("Random points")]
    public static PlotModel RandomScatter()
    {
        return RandomScatter(32768, 0);
    }

    [Example("Random points (BinSize=2)")]
    public static PlotModel RandomScatter2()
    {
        return RandomScatter(32768, 2);
    }

    [Example("Random points (BinSize=4)")]
    public static PlotModel RandomScatter4()
    {
        return RandomScatter(32768, 4);
    }

    [Example("Random points (BinSize=6)")]
    public static PlotModel RandomScatter6()
    {
        return RandomScatter(32768, 6);
    }

    [Example("Random points (BinSize=8)")]
    public static PlotModel RandomScatter8()
    {
        return RandomScatter(32768, 8);
    }

    [Example("Random points (BinSize=10)")]
    public static PlotModel RandomScatter10()
    {
        return RandomScatter(32768, 10);
    }

    [Example("Two ScatterSeries")]
    public static PlotModel TwoScatterSeries()
    {
        var model = new PlotModel { Title = "Two ScatterSeries (with and without values)", Subtitle = "With values (squares), without values (triangles)" };
        var colorAxis = new LinearColorAxis { Position = AxisPosition.Right, Key = "ColorAxis", Palette = PlotterPalettes.Jet(30), Minimum = -1, Maximum = 1 };
        model.Axes.Add(colorAxis);
        model.Series.Add(CreateRandomScatterSeries(50, MarkerType.Triangle, false, false, null));
        model.Series.Add(CreateRandomScatterSeries(50, MarkerType.Square, false, true, colorAxis));
        return model;
    }

    [Example("LabelFormatString")]
    public static PlotModel LabelFormatString()
    {
        var model = new PlotModel { Title = "ScatterSeries with LabelFormatString" };
        var s = CreateRandomScatterSeries(50, MarkerType.Square, false, false, null);
        s.LabelFormatString = "{1:0.###}";
        model.Series.Add(s);
        return model;
    }

    private static PlotModel CreateRandomScatterSeriesWithColorAxisPlotModel(int n, PlotterPalette palette, MarkerType markerType, AxisPosition colorAxisPosition, PlotterColor highColor, PlotterColor lowColor)
    {
        var model = new PlotModel { Title = string.Format("ScatterSeries (n={0})", n), Background = PlotterColors.LightGray };
        var colorAxis = new LinearColorAxis { Position = colorAxisPosition, Palette = palette, Minimum = -1, Maximum = 1, HighColor = highColor, LowColor = lowColor };
        model.Axes.Add(colorAxis);
        model.Series.Add(CreateRandomScatterSeries(n, markerType, false, true, colorAxis));
        return model;
    }

    private static ScatterSeries CreateRandomScatterSeries(int n, MarkerType markerType, bool setSize, bool setValue, LinearColorAxis colorAxis)
    {
        var s1 = new ScatterSeries
        {
            MarkerType = markerType,
            MarkerSize = 6,
            ColorAxisKey = colorAxis != null ? colorAxis.Key : null
        };
        var random = new Random(13);
        for (int i = 0; i < n; i++)
        {
            var p = new ScatterPoint((random.NextDouble() * 2.2) - 1.1, random.NextDouble());
            if (setSize)
            {
                p.Size = (random.NextDouble() * 5) + 5;
            }

            if (setValue)
            {
                p.Value = (random.NextDouble() * 2.2) - 1.1;
            }

            s1.Points.Add(p);
        }

        return s1;
    }

    [Example("Random points with random size")]
    public static PlotModel RandomSize()
    {
        return RandomSize(1000, 8);
    }

    public static PlotModel RandomSize(int n, int binsize)
    {
        var model = new PlotModel { Title = string.Format("ScatterSeries with random MarkerSize (n={0})", n), Subtitle = "BinSize = " + binsize };

        var s1 = new ScatterSeries { Title = "Series 1", MarkerStrokeThickness = 0, BinSize = binsize };
        var random = new Random(13);
        for (int i = 0; i < n; i++)
        {
            s1.Points.Add(new ScatterPoint(random.NextDouble(), random.NextDouble(), 4 + (10 * random.NextDouble())));
        }

        model.Series.Add(s1);
        return model;
    }

    [Example("Random points with least squares fit")]
    public static PlotModel RandomWithFit()
    {
        const int n = 20;
        var model = new PlotModel { Title = string.Format("Random data (n={0})", n) };
        var l = new Legend
        {
            LegendPosition = LegendPosition.LeftTop
        };

        model.Legends.Add(l);

        var s1 = new ScatterSeries { Title = "Measurements" };
        var random = new Random(7);
        double x = 0;
        double y = 0;
        for (int i = 0; i < n; i++)
        {
            x += 2 + (random.NextDouble() * 10);
            y += 1 + random.NextDouble();
            var p = new ScatterPoint(x, y);
            s1.Points.Add(p);
        }

        model.Series.Add(s1);
        double a, b;
        LeastSquaresFit(s1.Points, out a, out b);
        model.Annotations.Add(new LineAnnotation { Slope = a, Intercept = b, Text = "Least squares fit" });
        return model;
    }

    /// <summary>
    /// Calculates the Least squares fit of a list of DataPoints.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="a">The slope.</param>
    /// <param name="b">The intercept.</param>
    public static void LeastSquaresFit(IEnumerable<ScatterPoint> points, out double a, out double b)
    {
        // http://en.wikipedia.org/wiki/Least_squares
        // http://mathworld.wolfram.com/LeastSquaresFitting.html
        // http://web.cecs.pdx.edu/~gerry/nmm/course/slides/ch09Slides4up.pdf

        double Sx = 0;
        double Sy = 0;
        double Sxy = 0;
        double Sxx = 0;
        int m = 0;
        foreach (var p in points)
        {
            Sx += p.X;
            Sy += p.Y;
            Sxy += p.X * p.Y;
            Sxx += p.X * p.X;
            m++;
        }

        double d = Sx * Sx - m * Sxx;
        a = 1 / d * (Sx * Sy - m * Sxy);
        b = 1 / d * (Sx * Sxy - Sxx * Sy);
    }

    [Example("Marker types")]
    public static PlotModel MarkerTypes()
    {
        var model = new PlotModel { Title = "Marker types" };
        var r = new Random(12345);
        model.Series.Add(CreateRandomScatterSeries(r, 10, "Circle", MarkerType.Circle));
        model.Series.Add(CreateRandomScatterSeries(r, 10, "Cross", MarkerType.Cross));
        model.Series.Add(CreateRandomScatterSeries(r, 10, "Diamond", MarkerType.Diamond));
        model.Series.Add(CreateRandomScatterSeries(r, 10, "Plus", MarkerType.Plus));
        model.Series.Add(CreateRandomScatterSeries(r, 10, "Square", MarkerType.Square));
        model.Series.Add(CreateRandomScatterSeries(r, 10, "Star", MarkerType.Star));
        model.Series.Add(CreateRandomScatterSeries(r, 10, "Triangle", MarkerType.Triangle));
        return model;
    }

    [Example("ScatterSeries.Points")]
    public static PlotModel DataPoints()
    {
        var model = new PlotModel { Title = "ScatterSeries (n=1000)", Subtitle = "The scatter points are added to the Points collection." };
        var series = new ScatterSeries();
        series.Points.AddRange(CreateRandomScatterPoints(1000));
        model.Series.Add(series);
        return model;
    }

    [Example("ScatterSeries.ItemsSource")]
    public static PlotModel FromItemsSource()
    {
        var model = new PlotModel { Title = "ScatterSeries (n=1000)", Subtitle = "The scatter points are defined in the ItemsSource property." };
        model.Series.Add(new ScatterSeries
        {
            ItemsSource = CreateRandomScatterPoints(1000),
        });
        return model;
    }

    [Example("ScatterSeries.ItemsSource + Mapping")]
    public static PlotModel FromMapping()
    {
        var model = new PlotModel { Title = "ScatterSeries (n=1000)", Subtitle = "The scatter points are defined by a mapping from the ItemsSource." };
        model.Series.Add(new ScatterSeries
        {
            ItemsSource = CreateRandomDataPoints(1000),
            Mapping = item => new ScatterPoint(((DataPoint)item).X, ((DataPoint)item).Y)
        });
        return model;
    }

    [Example("ScatterSeries.ItemsSource + reflection")]
    public static PlotModel FromItemsSourceReflection()
    {
        var model = new PlotModel { Title = "ScatterSeries (n=1000)", Subtitle = "The scatter points are defined by reflection from the ItemsSource." };
        model.Series.Add(new ScatterSeries
        {
            ItemsSource = CreateRandomDataPoints(1000),
            DataFieldX = "X",
            DataFieldY = "Y"
        });
        return model;
    }

    [Example("ScatterSeries with ColorAxis Rainbow(16)")]
    public static PlotModel ColorMapRainbow16()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Rainbow(16), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Hue(30) Star")]
    public static PlotModel ColorMapHue30()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Hue(30), MarkerType.Star, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Hot(64)")]
    public static PlotModel ColorMapHot64()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Hot(64), MarkerType.Triangle, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Cool(32)")]
    public static PlotModel ColorMapCool32()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Cool(32), MarkerType.Circle, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Gray(32)")]
    public static PlotModel ColorMapGray32()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Gray(32), MarkerType.Diamond, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Jet(32)")]
    public static PlotModel ColorMapJet32()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Jet(32), MarkerType.Plus, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Hot with extreme colors")]
    public static PlotModel ColorMapHot64Extreme()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Hot(64), MarkerType.Square, AxisPosition.Right, PlotterColors.Magenta, PlotterColors.Green);
    }

    [Example("ScatterSeries with ColorAxis Hot (top legend)")]
    public static PlotModel ColorMapHot64ExtremeTopLegend()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Hot(64), MarkerType.Cross, AxisPosition.Top, PlotterColors.Magenta, PlotterColors.Green);
    }
    [Example("ScatterSeries with ColorAxis Hot(16) N=31000")]
    public static PlotModel ColorMapHot16Big()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(31000, PlotterPalettes.Hot(16), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis BlueWhiteRed (3)")]
    public static PlotModel ColorMapBlueWhiteRed3()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.BlueWhiteRed(3), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis BlueWhiteRed (9)")]
    public static PlotModel ColorMapBlueWhiteRed9()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.BlueWhiteRed(9), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis BlueWhiteRed (256)")]
    public static PlotModel ColorMapBlueWhiteRed256()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.BlueWhiteRed(256), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis BlackWhiteRed (9)")]
    public static PlotModel ColorMapBlackWhiteRed9()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.BlackWhiteRed(9), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis BlackWhiteRed (9) top legend")]
    public static PlotModel ColorMapBlackWhiteRed9TopLegend()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.BlackWhiteRed(9), MarkerType.Square, AxisPosition.Top, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Viridis")]
    public static PlotModel ColorMapViridis()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Viridis(), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Plasma")]
    public static PlotModel ColorMapPlasma()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Plasma(), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Magma")]
    public static PlotModel ColorMapMagma()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Magma(), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Inferno")]
    public static PlotModel ColorMapInferno()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Inferno(), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with ColorAxis Cividis")]
    public static PlotModel ColorMapCividis()
    {
        return CreateRandomScatterSeriesWithColorAxisPlotModel(2500, PlotterPalettes.Cividis(), MarkerType.Square, AxisPosition.Right, PlotterColors.Undefined, PlotterColors.Undefined);
    }

    [Example("ScatterSeries with single-selected items")]
    public static PlotModel SingleSelectItems()
    {
        var model = RandomScatter(10, 8);
        model.Subtitle = "Click to select a point";

        model.SelectionColor = PlotterColors.Red;

        var series = model.Series[0];

        series.SelectionMode = SelectionMode.Single;

        series.SelectItem(3);
        series.SelectItem(5);

        return model;
    }

    [Example("ScatterSeries with multi-selected items")]
    public static PlotModel MultiSelectItems()
    {
        var model = RandomScatter(10, 8);
        model.Subtitle = "Click to toggle point selection";

        model.SelectionColor = PlotterColors.Red;

        var series = model.Series[0];

        series.SelectionMode = SelectionMode.Multiple;

        series.SelectItem(3);
        series.SelectItem(5);

        return model;
    }

    [Example("ScatterSeries with SelectionMode.All (no tracker)")]
    public static PlotModel AllSelected()
    {
        return AllSelected(false);
    }

    [Example("ScatterSeries with SelectionMode.All (with tracker)")]
    public static PlotModel AllSelectedWithTracker()
    {
        return AllSelected(true);
    }

    private static PlotModel AllSelected(bool showTracker)
    {
        var model = RandomScatter(10, 8);
        model.Subtitle = "Click to select all points";

        model.SelectionColor = PlotterColors.Red;

        var series = model.Series[0];

        series.SelectionMode = SelectionMode.All;

        return model;
    }

    [Example("TrackerFormatString")]
    public static PlotModel TrackerFormatString()
    {
        var model = new PlotModel { Title = "TrackerFormatString" };

        var s1 = new ScatterSeries { TrackerFormatString = "{Sum:0.0}", DataFieldX = "X", DataFieldY = "Y" };
        var myPoints = new List<MyPoint>
        {
            new MyPoint { X = 10, Y = 40 },
            new MyPoint { X = 40, Y = 20 },
            new MyPoint { X = 60, Y = 30 }
        };
        s1.ItemsSource = myPoints;
        model.Series.Add(s1);
        return model;
    }

    public struct MyPoint
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Sum
        {
            get
            {
                // calculated on request
                return this.X + this.Y;
            }
        }
    }

    private static PlotModel RandomScatter(int n, int binSize)
    {
        var model = new PlotModel { Title = string.Format("ScatterSeries (n={0})", n), Subtitle = binSize > 0 ? "BinSize = " + binSize : "No 'binning'" };

        var s1 = new ScatterSeries()
        {
            Title = "Series 1",
            MarkerType = MarkerType.Diamond,
            MarkerStrokeThickness = 0,
            BinSize = binSize
        };

        var random = new Random(1);
        for (int i = 0; i < n; i++)
        {
            s1.Points.Add(new ScatterPoint(random.NextDouble(), random.NextDouble()));
        }

        model.Series.Add(s1);
        return model;
    }

    private static PlotModel CreateCorrelatedScatter(int n)
    {
        var model = new PlotModel { Title = string.Format("Correlated ScatterSeries (n={0})", n) };

        var s1 = new ScatterSeries
        {
            Title = "Series 1",
            MarkerType = MarkerType.Diamond,
            MarkerStrokeThickness = 0,
        };

        var random = new Random(1);
        for (int i = 0; i < n; i++)
        {
            var x = GetNormalDistributedValue(random);
            var y = 2 * x * x + GetNormalDistributedValue(random);
            s1.Points.Add(new ScatterPoint(x, y));
        }

        model.Series.Add(s1);
        return model;
    }

    private static ScatterSeries CreateRandomScatterSeries(Random r, int n, string title, MarkerType markerType)
    {
        var s1 = new ScatterSeries { Title = title, MarkerType = markerType, MarkerStroke = PlotterColors.Black, MarkerStrokeThickness = 1.0 };
        for (int i = 0; i < n; i++)
        {
            double x = r.NextDouble() * 10;
            double y = r.NextDouble() * 10;
            var p = new ScatterPoint(x, y);
            s1.Points.Add(p);
        }

        return s1;
    }

    private static double GetNormalDistributedValue(Random rnd)
    {
        var d1 = rnd.NextDouble();
        var d2 = rnd.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(d1)) * Math.Sin(2.0 * Math.PI * d2);
    }

    private static List<DataPoint> CreateRandomDataPoints(int n)
    {
        return CreateRandomScatterPoints(n).Select(sp => new DataPoint(sp.X, sp.Y)).ToList();
    }

    private static List<ScatterPoint> CreateRandomScatterPoints(int n)
    {
        var r = new Random(12345);

        var points = new List<ScatterPoint>();
        for (int i = 0; i < n; i++)
        {
            double x = r.NextDouble() * 10;
            double y = r.NextDouble() * 10;
            var p = new ScatterPoint(x, y);
            points.Add(p);
        }

        return points;
    }
}
