// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotControllerExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using CodeBrix.Plotter;
using CodeBrix.Plotter.Annotations;
using CodeBrix.Plotter.Axes;
using CodeBrix.Plotter.Legends;
using CodeBrix.Plotter.Series;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

[Examples("PlotController examples")]
public static class PlotControllerExamples
{
    [Example("Basic controller example")]
    public static Example BasicExample()
    {
        var model = new PlotModel { Title = "Basic Controller example", Subtitle = "Panning with left mouse button" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
        var controller = new PlotController();
        controller.UnbindAll();
        controller.BindMouseDown(PlotterMouseButton.Left, PlotCommands.PanAt);
        return new Example(model, controller);
    }

    [Example("Show tracker without clicking")]
    public static Example HoverTracking()
    {
        var model = new PlotModel { Title = "Show tracker without clicking" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
        model.Series.Add(new FunctionSeries(t => (Math.Cos(t) * 5) + Math.Cos(t * 50), t => (Math.Sin(t) * 5) + Math.Sin(t * 50), 0, Math.PI * 2, 20000));

        // create a new plot controller with default bindings
        var controller = new PlotController();

        // add a tracker command to the mouse enter event
        controller.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);

        return new Example(model, controller);
    }

    [Example("Mouse handling example")]
    public static Example MouseHandlingExample()
    {
        var model = new PlotModel { Title = "Mouse handling example" };
        var series = new ScatterSeries();
        model.Series.Add(series);

        // Create a command that adds points to the scatter series
        var command = new DelegatePlotCommand<PlotterMouseDownEventArgs>(
            (v, c, a) =>
            {
                a.Handled = true;
                var point = series.InverseTransform(a.Position);
                series.Points.Add(new ScatterPoint(point.X, point.Y));
                model.InvalidatePlot(true);
            });

        var controller = new PlotController();
        controller.BindMouseDown(PlotterMouseButton.Left, command);

        return new Example(model, controller);
    }

    [Example("Clicking on an annotation")]
    public static Example ClickingOnAnAnnotation()
    {
        var plotModel = new PlotModel { Title = "Clicking on an annotation", Subtitle = "Click on the rectangles" };

        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

        var annotation1 = new RectangleAnnotation { Fill = PlotterColors.Green, Text = "RectangleAnnotation 1", MinimumX = 25, MaximumX = 75, MinimumY = 20, MaximumY = 40 };
        plotModel.Annotations.Add(annotation1);

        var annotation2 = new RectangleAnnotation { Fill = PlotterColors.SkyBlue, Text = "RectangleAnnotation 2", MinimumX = 25, MaximumX = 75, MinimumY = 60, MaximumY = 80 };
        plotModel.Annotations.Add(annotation2);

        EventHandler<PlotterMouseDownEventArgs> handleMouseClick = (s, e) =>
        {
            plotModel.Subtitle = "You clicked " + ((RectangleAnnotation)s).Text;
            plotModel.InvalidatePlot(false);
        };

        var controller = new PlotController();
        var handleClick = new DelegatePlotCommand<PlotterMouseDownEventArgs>(
            (v, c, e) =>
            {
                var args = new HitTestArguments(e.Position, 10);
                var firstHit = v.ActualModel.HitTest(args).FirstOrDefault(x => x.Element is RectangleAnnotation);
                if (firstHit != null)
                {
                    e.Handled = true;
                    plotModel.Subtitle = "You clicked " + ((RectangleAnnotation)firstHit.Element).Text;
                    plotModel.InvalidatePlot(false);
                }
            });
        controller.Bind(new PlotterMouseDownGesture(PlotterMouseButton.Left), handleClick);

        return new Example(plotModel, controller);
    }

    [Example("Preferring an axis for manipulation")]
    public static Example PreferringAnAxisForManipulation()
    {
        var model = new PlotModel
        {
            Title = "Preferring an axis for manipulation",
            Subtitle = "Mouse wheel over plot area prefers X axis",
        };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

        model.Series.Add(new FunctionSeries(Math.Cos, -7, 7, 0.01));

        var controller = new PlotController();
        controller.UnbindAll();
        controller.BindMouseWheel(new DelegatePlotCommand<PlotterMouseWheelEventArgs>((view, _, args) => HandleZoomByWheel(view, args)));
        controller.BindMouseWheel(PlotterModifierKeys.Control, new DelegatePlotCommand<PlotterMouseWheelEventArgs>((view, _, args) => HandleZoomByWheel(view, args, 0.1)));

        return new Example(model, controller);

        void HandleZoomByWheel(IPlotView view, PlotterMouseWheelEventArgs args, double factor = 1)
        {
            var m = new ZoomStepManipulator(view)
            {
                AxisPreference = AxisPreference.X,
                Step = args.Delta * 0.001 * factor,
                FineControl = args.IsControlDown,
            };
            m.Started(args);
        }
    }

    [Example("Show/Hide Legend")]
    public static Example ShowHideLegend()
    {
        var plotModel = new PlotModel { Title = "Show/Hide Legend", Subtitle = "Click on the rectangles" };

        int n = 3;
        for (int i = 1; i <= n; i++)
        {
            var s = new LineSeries { Title = "Series " + i };
            plotModel.Series.Add(s);
            for (double x = 0; x < 2 * Math.PI; x += 0.1)
            {
                s.Points.Add(new DataPoint(x, (Math.Sin(x * i) / i) + i));
            }
        }
        var l = new Legend();

        plotModel.Legends.Add(l);

        var annotation1 = new RectangleAnnotation { Fill = PlotterColors.Green, Text = "Show Legend", MinimumX = 0.5, MaximumX = 1.5, MinimumY = .2, MaximumY = 0.4 };
        plotModel.Annotations.Add(annotation1);

        var annotation2 = new RectangleAnnotation { Fill = PlotterColors.SkyBlue, Text = "Hide Legend", MinimumX = 0.5, MaximumX = 1.5, MinimumY = 0.6, MaximumY = 0.8 };
        plotModel.Annotations.Add(annotation2);

        EventHandler<PlotterMouseDownEventArgs> handleMouseClick = (s, e) =>
        {
            string annotationText = ((RectangleAnnotation)s).Text;
            if (annotationText == "Show Legend")
            {
                plotModel.IsLegendVisible = true;
            }
            else if (annotationText == "Hide Legend")
            {
                plotModel.IsLegendVisible = false;
            }

            plotModel.Subtitle = "You clicked " + ((RectangleAnnotation)s).Text;
            plotModel.InvalidatePlot(false);
        };

        var controller = new PlotController();
        var handleClick = new DelegatePlotCommand<PlotterMouseDownEventArgs>(
            (v, c, e) =>
            {
                var args = new HitTestArguments(e.Position, 10);
                var firstHit = v.ActualModel.HitTest(args).FirstOrDefault(x => x.Element is RectangleAnnotation);
                if (firstHit != null)
                {
                    e.Handled = true;
                    plotModel.Subtitle = "You clicked " + ((RectangleAnnotation)firstHit.Element).Text;
                    plotModel.InvalidatePlot(false);
                }
            });
        controller.Bind(new PlotterMouseDownGesture(PlotterMouseButton.Left), handleClick);

        return new Example(plotModel, controller);
    }
}
