using System.Windows;
using CodeBrix.Platform.Simple;
using PicoScope.Helpers;
using PicoScope.Scope;
using PicoScope.Scope.Simulation;
using PicoScope.Scope.Windows;

namespace PicoScope;

/// <summary>
/// The WPF application entry point.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Sets up CodeBrix.Platform's service resolver and registers the scope
    /// implementations this head can offer.
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
    }
}
