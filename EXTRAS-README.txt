================================================================================
EXTRAS-README: CodeBrix.Plotter
Samples, tools and other content in this repository that is not part of a NuGet
package
================================================================================

Only src/CodeBrix.Plotter ships as a NuGet package
(CodeBrix.Plotter.MitLicenseForever). Everything described below is development
and reference material: it is not packed, not published, and not referenced by
the library.


THE PICOSCOPE SAMPLE
====================

Path:  samples/PicoScope/
Docs:  samples/PicoScope/README.md  (long-form; capability map, cookbook, a
       complete ps2000 API reference, driver loading on Windows and Linux, and
       a troubleshooting guide)
Solution: samples/PicoScope/PicoScope.slnx  (every head, both libraries and
       the tests; builds with the plain .NET SDK on Linux, macOS and Windows)

What it is
----------
A CodeBrix.Platform application that streams live oscilloscope traces from a
PicoScope 2204A into a CodeBrix.Plotter chart, on .NET 10, with Linux (X11,
Wayland, frame buffer) and Windows (Win32-Skia, WPF-Skia) heads. The chart is
shown through the CodeBrix.Platform PlotterView add-in, which is how a
Platform application consumes this library. It doubles as reference material
for the Pico `ps2000` driver: every capability claim in its README was
measured against physical hardware, on Windows and again on Linux.

Projects
--------
    src/libs/PicoScope.ScopeData         net10.0
        Device-agnostic by construction -- no interop, no package references,
        no platform reference. Owns IScopeDataDevice, the model types
        (ChannelId, ChannelSettings, VoltageRange, TimebaseInfo,
        TriggerSettings, CaptureBlock, StreamingSettings,
        SignalGeneratorSettings, EtsSettings, ScopeCapabilities, UnitInfo),
        ScopeDeviceFinder and SimulatedScopeDataDevice.
    src/libs/PicoScope.ScopeData.Ps2000  net10.0
        The ps2000 P/Invokes (Ps2000Api), the driver loader and
        Ps2000ScopeDataDevice. One assembly for Windows and Linux: the driver
        API is identical on both, and only the loader's search paths differ.
    src/PicoScope.Core                   net10.0
        MainViewModel and Charting/ScopePlot.cs. References the PlotterView
        add-in package, which brings CodeBrix.Plotter in with it.
    src/PicoScope.UI                     shared project
        App.xaml and Views/MainPage.xaml, compiled into every head.
    src/PicoScope.LinuxX11, .LinuxWayland, .LinuxFrameBuffer,
    src/PicoScope.Win32Skia, .WinWpfSkia
        One head per platform. Each registers the ps2000 device and the
        simulator with ScopeDeviceFinder in its Program.cs.
    tests/libs/PicoScope.ScopeData.Tests
        xUnit v3 tests for the contract, the simulator and the model types,
        using a scripted TestScopeDataDevice. No hardware needed.
    src/PicoScope.Wpf, src/PicoScope.WinUI, src/Shared/
        The earlier Windows-only WPF and WinUI 3 heads. Kept in the tree, not
        wired to the reorganised libraries, not in PicoScope.slnx.

What it demonstrates for CodeBrix.Plotter consumers
---------------------------------------------------
    * ScopePlot (src/PicoScope.Core/Charting/ScopePlot.cs) builds and owns a
      PlotModel and exposes it as a property -- a clean separation between
      chart state and the UI head.
    * Live data: appending streaming samples, capping points per channel
      (MaxPointsPerChannel), keeping a rolling time window
      (StreamWindowSeconds), auto-scaling the voltage axis, showing and hiding
      channels, and clearing.
    * The thread-safe update pattern: the model is mutated under
      PlotModel.SyncRoot and then invalidated, from whichever thread the data
      arrives on, and the PlotterView control renders under the same lock.
    * Consuming the library through the CodeBrix.Platform PlotterView add-in
      rather than referencing it directly.

How to run it
-------------
    1. .NET 10 SDK, and a 64-bit process (the ps2000 driver is 64-bit only).
    2. The ps2000 driver. Windows: install PicoSDK or the PicoScope desktop
       application, so that ps2000.dll is resolvable at run time. Linux:
       install the libps2000 package from Pico's apt repository (the picoscope
       application package pulls it in); it puts libps2000.so on the system
       loader path and installs the udev rule, so no group or sudo is needed.
       The driver is NOT committed to this repository -- see the Licensing
       section of samples/PicoScope/README.md for why.
    3. CLOSE the PicoScope desktop application. Only one process may hold the
       device open.
    4. dotnet build samples/PicoScope/PicoScope.slnx, then dotnet run the head
       for your desktop (src/PicoScope.LinuxX11 on an X11 session, and so on).
       The console shows the device found, the driver it loaded from, and the
       discovered capabilities.

Without hardware, the sample still runs: ScopeDeviceFinder falls back to
SimulatedScopeDataDevice, which implements the whole interface, enforces the
same lifecycle rules and imposes the real 2204A capability limits, so code
developed against it behaves the same way against a physical scope.

Licensing note
--------------
The sample itself is under the repository's MIT terms. It derives from Pico's
ISC-licensed picosdk-c-sharp-examples (keep the copyright and permission
notices). The Pico driver binaries (ps2000.dll and picoipp.dll on Windows,
libps2000.so and libpicoipp.so on Linux) are proprietary and are deliberately
not committed; they are resolved from your own install at run time.


THE TEST PROJECT AND THE EXAMPLE LIBRARY
========================================

Path: tests/CodeBrix.Plotter.Tests/

The test project is not shipped, but two parts of it are worth knowing about as
reference material:

    ExampleLibrary/     The upstream OxyPlot ExampleLibrary, ported as
                        TEST-SUPPORT CODE ONLY. Hundreds of small factory
                        methods, each returning a complete PlotModel for one
                        chart variation, organised by feature (Series/, Axes/,
                        Annotations/, Showcases/, CustomSeries/, Misc/,
                        Issues/, Discussions/). This is the fastest way to find
                        a working recipe for any series, axis or annotation
                        type. It is NOT part of the library or the NuGet
                        package.
    Skia/               Working examples of SkiaRenderContext usage, the
                        TypefaceResolver contract, and every exporter.

Optional / generated test data:

    ExampleLibrary/Resources/   embedded data files the examples read
                                (Bergensbanen.csv, DodgyContourData.tsv,
                                west0479.mtx, WorldPopulation.xml, X.txt, Y.txt
                                and an image)
    Schemas/ and Svg/           the SVG XSDs the SVG output is validated against
    Imaging/TestImages/         PNG, BMP and JPEG files the decoders are run over
    baseline/ folders           created on first run by PlotterAssert; they are
                                generated, machine-local and not authoritative

Running the tests is covered in MAINTAINER-README.txt.


TOOLS
=====

There are no build tools, scripts or generators in this repository. Packing is
done by the library project itself (GeneratePackageOnBuild), and there is no
CI workflow committed here.
