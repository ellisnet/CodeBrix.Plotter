using System;
using CodeBrix.Platform.Simple;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PicoScope.Helpers;
using PicoScope.Scope;
using PicoScope.Scope.Simulation;
using PicoScope.Scope.Windows;

namespace PicoScope.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object, sets up CodeBrix.Platform's
    /// service resolver, and registers the scope implementations this head can
    /// offer.
    /// </summary>
    public App()
    {
        SimpleServiceResolver.CreateInstance(HostHelper.GetHost(), services =>
        {
            //Nothing extra to register: the view model resolves its scope
            //  through PicoScopeFinder rather than through DI, so that the
            //  device-agnostic library never has to know about the container.
        });
        SimpleViewModel.SetIsDesignMode(false);

        //Registration order does not matter -- FindBest() prefers real hardware
        //  and falls back to the simulator when nothing is plugged in.
        PicoScopeFinder.Register(new WindowsPicoScope());
        PicoScopeFinder.Register(new SimulatedPicoScope());

        this.InitializeComponent();
    }

    /// <summary>The application's main window.</summary>
    protected Window MainWindow { get; private set; }

    /// <summary>The app's main window, exposed for anything that needs its HWND.</summary>
    public static Window CurrentWindow { get; private set; }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window
        {
            Title = "PicoScope - CodeBrix.Plotter sample"
        };
        CurrentWindow = MainWindow;

        if (MainWindow.Content is not Frame rootFrame)
        {
            rootFrame = new Frame();
            MainWindow.Content = rootFrame;
            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (rootFrame.Content == null)
        {
            rootFrame.Navigate(typeof(Views.MainPage), args.Arguments);
        }

        MainWindow.Activate();
    }

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }
}
