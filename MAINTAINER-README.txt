================================================================================
MAINTAINER-README: CodeBrix.Plotter
Notes for people and agents MAINTAINING this repository — not for package
consumers
================================================================================

If you are CONSUMING the NuGet package, read AGENT-README.txt instead. This file
covers building, testing, packaging and the port's provenance.


PURPOSE AND SCOPE
=================

This repository produces exactly one NuGet package:

    PackageId  : CodeBrix.Plotter.MitLicenseForever
    Project    : src/CodeBrix.Plotter/CodeBrix.Plotter.csproj
    Assembly   : CodeBrix.Plotter
    Namespace  : CodeBrix.Plotter (root) plus .Axes, .Axes.Rendering,
                 .Annotations, .Legends, .Series, .Skia, .Utilities
    License    : MIT
    Documented : AGENT-README.txt (repo root) -- and that file is packed INTO
                 the nupkg, so it must stay accurate

The library consolidates the upstream OxyPlot.Core and OxyPlot.SkiaSharp
packages into one assembly: the platform-independent plot model plus the
SkiaSharp render context and exporters.

Nothing else in the repository ships. The tests, the ExampleLibrary and the
samples/ tree are development material; see EXTRAS-README.txt for the samples.


REPOSITORY LAYOUT
=================

    CodeBrix.Plotter.slnx           library + tests (the everyday solution)
    CodeBrix.Plotter.Windows.slnx   the same, plus the PicoScope sample heads
                                    (Windows-only projects; do not open this on
                                    Linux or macOS)
    AGENT-README.txt                consumer documentation (packed into the nupkg)
    MAINTAINER-README.txt           this file
    EXTRAS-README.txt               the samples and other non-package content
    README-INDEX.txt                map of the README files
    README.md                       human-facing overview (GitHub + nuget.org)
    LICENSE                         MIT
    THIRD-PARTY-NOTICES.txt         upstream attribution and the exact baseline
    icon-codebrix-128.png           package icon

    src/CodeBrix.Plotter/
        InternalsVisibleTo.cs   grants CodeBrix.Plotter.Tests access to internals
        Annotations/      annotation types            -> .Annotations
        Axes/             axis types                  -> .Axes
          Rendering/      axis renderers              -> .Axes.Rendering
        Foundation/       DataPoint, DataVector, PlotLength, MarkerType,
                          IExporter, IDataPointProvider, and CodeGenerator/
        Graphics/         Element, Model, ElementCollection, controller base,
                          selection and hit-testing, IView, CursorType
        Imaging/          PlotterImage, the PNG/BMP/JPEG codecs, Deflate/
        Input/            key/button/modifier enums and Gestures/
        Legends/          Legend, LegendBase and the legend enums  -> .Legends
        Pdf/              the dependency-free PortableDocument PDF writer
        PlotController/   PlotController, PlotCommands, EventArgs/, Manipulators/
        PlotModel/        PlotModel and its partials, PlotElement, the
                          IPlotModel / IPlotElement / IXyAxisPlotElement /
                          ITransposablePlotElement interfaces
        PlotView/         IPlotView
        Rendering/        IRenderContext, RenderContextBase, the colour, rect,
                          size, thickness, pen and palette types, RenderContext/
                          and Utilities/ (splines, decimator)
        Series/           series types                -> .Series
          BarSeries/        the bar family and its item types
          FinancialSeries/  candlestick, high/low and volume
        Skia/             SkiaRenderContext, RenderTarget, TypefaceResolver,
                          SkiaExtensions and the four exporters   -> .Skia
        Svg/              the dependency-free SVG writer
        Utilities/        binning, histogram, array, string and reflection
                          helpers

    tests/CodeBrix.Plotter.Tests/   mirrors the library folders, plus Skia/,
                                    ExampleLibrary/, Schemas/ and Imaging/TestImages/

    samples/PicoScope/              the oscilloscope sample (see EXTRAS-README.txt)

FOLDER IS NOT NAMESPACE. Only Annotations/, Axes/ (+ Axes/Rendering/), Legends/,
Series/ and Skia/ carry sub-namespaces; every other folder's types live in the
root CodeBrix.Plotter namespace. Utilities/ is the odd one out -- Helpers.cs and
TrackerHelper.cs are in CodeBrix.Plotter.Utilities while everything else in that
folder is in the root namespace. That is inherited from upstream; do not "fix"
it, because it would be a breaking change for consumers.


BUILDING
========

    dotnet restore CodeBrix.Plotter.slnx
    dotnet build   CodeBrix.Plotter.slnx

Target framework is net10.0 only, and GenerateDocumentationFile is on, so CS1591
fires on any public or protected member without an XML doc comment. A clean
build is 0 warnings and 0 errors -- fix warnings at the source, never with
<NoWarn>.

GeneratePackageOnBuild is TRUE on the library project, so every build also
produces a .nupkg (see PACKAGING AND PUBLISHING for why that matters).

CodeBrix.Plotter.Windows.slnx additionally builds the PicoScope sample, whose
WPF and WinUI heads target net10.0-windows10.0.19041.0. Building that solution
requires Windows; use CodeBrix.Plotter.slnx everywhere else.


TESTING
=======

    dotnet test CodeBrix.Plotter.slnx

The suite is the upstream OxyPlot test suite -- the tests for OxyPlot.Core and
OxyPlot.SkiaSharp only -- ported to xUnit.v3 + SilverAssertions.

Test project prerequisites, all wired in the csproj:

  * Native SkiaSharp assets are selected from the BUILD HOST's OS, because the
    tests render for real: SkiaSharp.NativeAssets.Linux (plus
    HarfBuzzSharp.NativeAssets.Linux, which is NOT transitive) on Linux,
    SkiaSharp.NativeAssets.macOS on macOS, SkiaSharp.NativeAssets.Win32 on
    Windows.
  * The TypefaceResolver tests need real font FILES, so the project references
    the Roboto font package with ExcludeAssets="all" + GeneratePathProperty and
    copies Roboto-Regular.ttf and Roboto-Bold.ttf into the output's Fonts/
    folder. If those tests start failing with a missing-file error, that copy
    step is what broke.
  * ExampleLibrary data files (Bergensbanen.csv, DodgyContourData.tsv, west0479.mtx,
    WorldPopulation.xml, X.txt, Y.txt and an image) are EmbeddedResources whose
    names must keep resolving as
    <RootNamespace>.ExampleLibrary.Resources.<file>.
  * The SVG schemas (Schemas/ and Svg/: svg.xsd, xlink.xsd, xml.xsd) and the
    Imaging/TestImages/ bitmaps are copied to the output directory and opened
    from the working directory.

tests/CodeBrix.Plotter.Tests/ExampleLibrary/ is the upstream ExampleLibrary,
ported as TEST-SUPPORT CODE ONLY. Several tests enumerate every example model
and assert that updating, rendering and exporting it throws nothing. It is NOT
part of the shipped library and NOT in the NuGet package.

EXPECT THE FULL SUITE TO TAKE TENS OF MINUTES. Several tests enumerate EVERY
example model, in its normal, transposed and reversed forms, and export each one
to SVG, PDF, PNG or JPEG -- thousands of real renders. That breadth is the point
of those tests: they are a smoke test that no series, axis or annotation type
throws while being laid out and drawn. To iterate on one area, filter:

    dotnet test CodeBrix.Plotter.slnx --filter "FullyQualifiedName~AxisTests"

Tests that render or export write their output under the test run's working
directory (bin/Debug/net10.0/), in per-test folders.

PlotterAssert.AreEqual(plot, name) compares a rendered SVG against a file in a
baseline/ folder. ON THE FIRST RUN FOR A GIVEN NAME THERE IS NO BASELINE, so it
writes one and passes; later runs compare against it. It therefore catches
CHANGES between runs on the same machine, not correctness in the absolute -- a
clean checkout always passes. PngAssert compares exported PNGs the same way.
Treat a PlotterAssert failure as "rendering changed", then decide whether the
change was intended and delete the stale baseline if so.

Any call inside a test that accepts a CancellationToken must be passed
TestContext.Current.CancellationToken (xUnit1051).


PACKAGING AND PUBLISHING
========================

The library project packs itself: GeneratePackageOnBuild is true, so a plain
`dotnet build` of src/CodeBrix.Plotter produces the .nupkg. There is no separate
pack driver script.

Versioning is DATE-STAMPED and auto-incrementing, computed in the csproj from
System.DateTime.UtcNow as 1.<x>.<y>.<z>:

    1  major     pinned to 1 for this library
    x  minor     whole years since _VersionBaseYear (2026 => 0)
    y  build     day of year, UTC, 1-based (Jan 1 = 1)
    z  revision  minute of day, UTC, 0..1439

Consequences to remember:

  * The value depends on the clock, so EVERY BUILD PRODUCES A NEW VERSION and a
    fresh .nupkg.
  * Two builds in the SAME UTC minute produce the SAME version -- do not publish
    two packages from within one minute.
  * This is not SemVer: major/minor do not signal API compatibility.
  * To re-baseline the minor number, change _VersionBaseYear in the csproj.

What ships in the nupkg, beyond the assembly and its XML documentation file:

    icon-codebrix-128.png     PackageIcon
    README.md                 PackageReadmeFile
    AGENT-README.txt          consumer documentation for AI agents
    THIRD-PARTY-NOTICES.txt   upstream attribution

MAINTAINER-README.txt, EXTRAS-README.txt and README-INDEX.txt are NOT packed;
they are repository documentation. If you add another packed file, add it to the
<None Include=... Pack="true"> item group in the csproj.

Other packaging metadata: PackageLicenseExpression MIT,
PackageRequireLicenseAcceptance true, PackageProjectUrl and RepositoryUrl point
at https://github.com/ellisnet/CodeBrix.Plotter.

Git tags are expected to match the published NuGet version.


PROVENANCE AND VENDORED SOURCES
===============================

The whole of src/ is a port. THIRD-PARTY-NOTICES.txt is the authoritative
statement; the short form:

  * Upstream: OxyPlot, https://github.com/oxyplot/oxyplot, MIT,
    Copyright (c) 2014 OxyPlot contributors.
  * Baseline: branch `develop`, commit 6b49a4ee ("Feature/upgrade
    oxyplot.skiasharp", #2161) -- the released v2.2.0 tag (commit 989df42a) plus
    the three subsequent develop commits that add net10.0 target frameworks and
    upgrade the SkiaSharp renderer.
  * Source/OxyPlot           -> src/CodeBrix.Plotter/       (273 files)
  * Source/OxyPlot.SkiaSharp -> src/CodeBrix.Plotter/Skia/  (7 files)
  * Source/OxyPlot.Tests           -> tests/CodeBrix.Plotter.Tests/
  * Source/OxyPlot.SkiaSharp.Tests -> tests/CodeBrix.Plotter.Tests/Skia/
  * Source/Examples/ExampleLibrary -> tests/CodeBrix.Plotter.Tests/ExampleLibrary/

Renaming rules applied by the port, which any new or updated file must follow:

  * Namespace OxyPlot* -> CodeBrix.Plotter*.
  * Public types prefixed "Oxy" -> prefixed "Plotter" (PlotterColor,
    PlotterRect, PlotterPalette, PlotterMouseEventArgs, ...). Types upstream did
    not prefix keep their names.
  * Every ported file records where it came from: the namespace line carries a
    `//was previously: <upstream-namespace>;` comment, and the file header keeps
    the upstream OxyPlot copyright notice VERBATIM. Do not remove either.
  * Do not fabricate top-of-file banners on ported files.

Files added by this port (no upstream original) are Skia/TypefaceResolver.cs and
InternalsVisibleTo.cs.

When pulling a fix from upstream, port the change into the renamed file rather
than replacing the file wholesale, so the `//was previously:` marker and the
CodeBrix naming survive.


CODING CONVENTIONS
==================

These apply to any change made to this repository.

  * Target framework is net10.0 only. No multi-targeting, ever.
  * Nullable reference types are OFF. Never write `?` on a reference type
    (`string?`, `MyClass?`, `object?`) and never use the null-forgiveness `!`
    operator. Value-type nullables (`int?`, `bool?`, `DateTime?`) are fine.
  * No `#nullable` directives anywhere.
  * No `global using` and no ImplicitUsings. Every file declares its own `using`
    directives, at the top, in one contiguous block, System.* first.
  * File-scoped namespaces only (`namespace X;`). Never block-scoped.
  * XML doc comments are required on every public and protected member --
    GenerateDocumentationFile is on, so CS1591 fires otherwise. Fix it by
    writing the comment, never by suppressing the warning.
  * No warning suppression at project level: no <NoWarn>, no
    <WarningLevel>0</WarningLevel>, no <TreatWarningsAsErrors>false</>. A clean
    build is 0 warnings and 0 errors.
  * Tests use xUnit.v3 with SilverAssertions. Test classes are named
    `<ClassUnderTest>Tests`; test methods are `<Member>_<snake_case>` or plain
    snake_case; multi-statement bodies carry `//Arrange` `//Act` `//Assert`
    comments; single-statement tests are expression-bodied.
  * There are no public events in the library and no [Obsolete] members. The
    upstream deprecated event family was dropped in the port; do not reintroduce
    it.
  * SkiaSharp and SkiaSharp.HarfBuzz move TOGETHER when their version is bumped
    -- they ship as a matched set -- and the library and test projects stay in
    lock-step. The version is shared across the CodeBrix family so that a
    CodeBrix.Platform application can consume this library without a conflict.


NOTES
=====

  * AGENT-README.txt is shipped inside the nupkg. Any API change must be
    reflected there in the same commit; a wrong signature in that file becomes a
    wrong signature in every consumer's package cache.
  * Do not rename or re-home types to "tidy up" the folder-vs-namespace
    mismatch, or to give the Utilities folder a consistent namespace. Both would
    be breaking changes and both are documented as-is in AGENT-README.txt.
  * The core (non-Skia) SVG writer depends on PdfRenderContext to measure text,
    which is why the whole PortableDocument PDF machinery is still present even
    though the upstream core PdfExporter was dropped. Do not delete Pdf/ on the
    assumption that it is dead code.
  * The two SvgExporter types (root namespace and .Skia) are deliberately
    identically named, matching upstream. Consumers must qualify; keep it that
    way rather than renaming one.
