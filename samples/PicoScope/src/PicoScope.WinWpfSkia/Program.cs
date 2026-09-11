using System;
using CodeBrix.Platform.UI.Hosting;
using CodeBrix.Platform.UI.Runtime.Skia.Wpf;
using PicoScope.ScopeData;
using PicoScope.ScopeData.Ps2000;
using PicoScope.ScopeData.Simulation;

namespace PicoScope;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.InitializeLogging();

        //This head decides which scope implementations the application can use.
        //  Registration order does not matter: FindBest() prefers real hardware
        //  and falls back to the simulator when nothing is plugged in.
        ScopeDeviceFinder.Register(new Ps2000ScopeDataDevice());
        ScopeDeviceFinder.Register(new SimulatedScopeDataDevice());

        var host = CodeBrixPlatformHostBuilder.Create()
            .App(() => new App())
            .UseWindowsWpf()
            .UseDirectSkiaCanvasMode() //Experimental - should be safe to leave enabled
            .Build();

        if (host is WpfHost wpfHost)
        {
            wpfHost.RenderSurfaceType = RenderSurfaceType.Software;
        }

        host.Run();
    }
}
