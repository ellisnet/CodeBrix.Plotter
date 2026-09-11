using System;
using System.Windows;
using PicoScope.Rendering;
using PicoScope.ViewModels;

namespace PicoScope.Views;

/// <summary>
/// The WPF window: hosts the Skia canvas the chart is drawn on, and wires the
/// view model to this thread's dispatcher.
/// </summary>
public partial class MainWindow : Window
{
    private readonly ScopeChartRenderer _renderer = new ScopeChartRenderer();

    private MainViewModel ViewModel => DataContext as MainViewModel;

    /// <summary>
    /// Creates the window.
    /// </summary>
    public MainWindow()
    {
        //Wire the bridge before InitializeComponent(), since that is what sets
        //  the data context from the XAML.
        DataContextChanged += (_, _) =>
        {
            if (DataContext is IChartHost host)
            {
                host.InvalidateChart = () => ChartCanvas.InvalidateVisual();

                //Streaming batches arrive on the driver polling thread. The plot
                //  model may only be touched from one thread, so everything is
                //  marshalled onto the dispatcher here.
                host.RunOnUiThread = action =>
                {
                    if (Dispatcher.CheckAccess()) { action(); }
                    else { Dispatcher.BeginInvoke(action); }
                };
            }
        };

        InitializeComponent();

        ChartCanvas.PaintSurface += (_, e) =>
        {
            MainViewModel viewModel = ViewModel;
            if (viewModel == null) { return; }

            //The DPI scale is the ratio between the canvas's pixel size and its
            //  logical size, which is how SKElement reports a high-DPI surface.
            float dpiScale = ChartCanvas.ActualWidth > 0
                ? (float)(e.Info.Width / ChartCanvas.ActualWidth)
                : 1.0f;

            _renderer.Render(viewModel.Plot.Model, e.Surface, e.Info, dpiScale);
        };

        Loaded += async (_, _) =>
        {
            if (ViewModel != null) { await ViewModel.InitializeAsync(); }
        };

        Closed += (_, _) =>
        {
            //Releasing the handle matters: a scope left open stays locked
            //  against every other process until this one exits.
            ViewModel?.Shutdown();
            _renderer.Dispose();
        };
    }
}
