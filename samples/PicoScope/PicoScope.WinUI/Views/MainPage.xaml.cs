using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PicoScope.Rendering;
using PicoScope.ViewModels;

namespace PicoScope.WinUI.Views;

/// <summary>
/// The WinUI page: hosts the Skia canvas the chart is drawn on, and wires the
/// view model to this thread's dispatcher queue.
/// </summary>
public sealed partial class MainPage : Page
{
    private readonly ScopeChartRenderer _renderer = new ScopeChartRenderer();

    private MainViewModel ViewModel => DataContext as MainViewModel;

    /// <summary>
    /// Creates the page.
    /// </summary>
    public MainPage()
    {
        //Wire the bridge before InitializeComponent(), since that is what sets
        //  the data context from the XAML.
        DataContextChanged += (_, _) =>
        {
            if (DataContext is IChartHost host)
            {
                host.InvalidateChart = () => ChartCanvas.Invalidate();

                //Streaming batches arrive on the driver polling thread. The plot
                //  model may only be touched from one thread, so everything is
                //  marshalled onto the dispatcher queue here.
                host.RunOnUiThread = action =>
                {
                    if (DispatcherQueue.HasThreadAccess) { action(); }
                    else { DispatcherQueue.TryEnqueue(() => action()); }
                };
            }
        };

        InitializeComponent();

        ChartCanvas.PaintSurface += (_, e) =>
        {
            MainViewModel viewModel = ViewModel;
            if (viewModel == null) { return; }

            //SKXamlCanvas reports its surface in physical pixels while the
            //  control is measured in effective pixels, so the ratio is the
            //  scale the render context needs.
            float dpiScale = ChartCanvas.ActualWidth > 0
                ? (float)(e.Info.Width / ChartCanvas.ActualWidth)
                : 1.0f;

            _renderer.Render(viewModel.Plot.Model, e.Surface, e.Info, dpiScale);
        };

        Loaded += async (_, _) =>
        {
            if (ViewModel != null) { await ViewModel.InitializeAsync(); }
        };

        Unloaded += (_, _) =>
        {
            //Releasing the handle matters: a scope left open stays locked
            //  against every other process until this one exits.
            ViewModel?.Shutdown();
            _renderer.Dispose();
        };
    }
}
