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
       complete ps2000 API reference and a troubleshooting guide)
Solution: samples/PicoScope/PicoScope.Windows.slnx  (the sample on its own,
       with CodeBrix.Plotter referenced from src/)
       -- or CodeBrix.Plotter.Windows.slnx at the repository root, which opens
       the library, the tests and the sample together.

What it is
----------
A working application that streams live oscilloscope traces from a PicoScope
2204A into a CodeBrix.Plotter chart, on .NET 10, with both a WPF head and a
WinUI 3 head. It doubles as reference material for the Pico `ps2000` driver:
every capability claim in its README was measured against physical hardware.

Projects
--------
    PicoScope.Scope          net10.0
        Device-agnostic by construction -- no interop, no Windows reference.
        Owns IPicoScope, the model types (ChannelId, ChannelSettings,
        VoltageRange, TimebaseInfo, TriggerSettings, CaptureBlock,
        StreamingSettings, SignalGeneratorSettings, EtsSettings,
        ScopeCapabilities, UnitInfo), PicoScopeFinder, SimulatedPicoScope, and
        Charting/ScopePlot.cs.
    PicoScope.Scope.Windows  net10.0-windows
        The ps2000 P/Invokes, the driver loader and WindowsPicoScope.
    PicoScope.Wpf            net10.0-windows10.0.19041.0
        WPF head, hosting the plot in an SKElement (SkiaSharp.Views.WPF).
    PicoScope.WinUI          net10.0-windows10.0.19041.0
        WinUI 3 head, hosting the plot in an SKXamlCanvas
        (SkiaSharp.Views.WinUI).
    Shared/                  linked as source into both heads
        The view model, the chart renderer and host helpers.

What it demonstrates for CodeBrix.Plotter consumers
---------------------------------------------------
    * ScopePlot (PicoScope.Scope/Charting/ScopePlot.cs) builds and owns a
      PlotModel and exposes it as a property -- a clean separation between chart
      state and the UI head.
    * Live data: appending streaming samples, capping points per channel
      (MaxPointsPerChannel), keeping a rolling time window
      (StreamWindowSeconds), auto-scaling the voltage axis, showing and hiding
      channels, and clearing.
    * Rendering the same PlotModel through two different SkiaSharp canvas hosts
      without changing the model code.

How to run it
-------------
Windows only, because the driver interop and both UI heads are Windows-targeted.

    1. .NET 10 SDK, and a 64-bit process (the ps2000 driver is 64-bit only).
    2. Install PicoSDK or the PicoScope desktop application, so that ps2000.dll
       is resolvable at run time. The driver is NOT committed to this repository
       -- see the Licensing section of samples/PicoScope/README.md for why.
    3. CLOSE the PicoScope desktop application. Only one process may hold the
       device open.
    4. Open samples/PicoScope/PicoScope.Windows.slnx and run PicoScope.Wpf or
       PicoScope.WinUI (WinUI is configured for the x64 platform).

Without hardware, the sample still runs: PicoScopeFinder falls back to
SimulatedPicoScope, which implements the whole interface, enforces the same
lifecycle rules and imposes the real 2204A capability limits, so code developed
against it behaves the same way against a physical scope.

Licensing note
--------------
The sample itself is under the repository's MIT terms. It derives from Pico's
ISC-licensed picosdk-c-sharp-examples (keep the copyright and permission
notices). The Pico driver DLLs are proprietary and are deliberately not
committed; they are resolved from your own install at run time.


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
