# PicoScope 2204A — a C# reference implementation

A working sample that streams live oscilloscope traces from a **PicoScope 2204A**
into a [CodeBrix.Plotter](../../README.md) chart, on .NET 10, as a
[CodeBrix.Platform](https://github.com/ellisnet/CodeBrix.Platform) application
with Linux and Windows heads. One device library drives the scope through Pico's
`ps2000` driver on both operating systems.

It is also intended as reference material. Everything the `ps2000` driver can do
appears here, named and documented, so that "how do I do X with my scope" can be
answered without reading `ps2000.h`. Where the API behaves surprisingly — and it
does, in several places — the surprise is written down next to the code that
handles it.

Every capability claim below was **measured against a physical PicoScope 2204A**
(serial 10066/1927), not read off a datasheet. See
[How these facts were established](#how-these-facts-were-established).

---

## Contents

- [Start here: which driver?](#start-here-which-driver)
- [Quick start](#quick-start)
- [Verified capability map](#verified-capability-map)
- [The seven traps](#the-seven-traps)
- [Getting the driver to load](#getting-the-driver-to-load)
- [Architecture](#architecture)
- [Cookbook](#cookbook)
- [Timebase reference table](#timebase-reference-table)
- [Complete ps2000 API reference](#complete-ps2000-api-reference)
- [Troubleshooting](#troubleshooting)
- [Licensing](#licensing)
- [How these facts were established](#how-these-facts-were-established)
- [Running the sample](#running-the-sample)

---

## Start here: which driver?

**A PicoScope 2204A uses the `ps2000` driver, not `ps2000a`.**

This is the single most expensive thing to get wrong, because the naming points
the other way. The "A" in `ps2000a` means *second-generation API*, not
*A-suffix model*.

| Driver | Covers |
|---|---|
| `ps2000` | 2104, 2105, 2202, 2203, **2204, 2204A, 2205, 2205A** |
| `ps2000a` | 2205 MSO, 2206/2207/2208 and their A/B variants, 2405A, 2406B, 2407B, 2408B |

Confirmed on the device itself: `ps2000_open_unit` returns a valid handle and
reports variant `2204A`, while `ps2000aOpenUnit` returns `0x00000003`
(`PICO_NOT_FOUND`).

The consequence for this repository: Pico's own C# examples modernised the
*newer* drivers to SDK-style projects, and left the legacy ones on .NET
Framework. There is no .NET Core `ps2000` example upstream. That absence is a
packaging gap, not a technical limitation — the P/Invoke layer is plain blittable
marshalling and runs perfectly on modern .NET, as this sample demonstrates.

---

## Quick start

```csharp
using PicoScope.ScopeData;
using PicoScope.ScopeData.Model;
using PicoScope.ScopeData.Simulation;
using PicoScope.ScopeData.Ps2000;

// Register what's available. Order doesn't matter.
ScopeDeviceFinder.Register(new Ps2000ScopeDataDevice());
ScopeDeviceFinder.Register(new SimulatedScopeDataDevice());

// Real hardware if it opens, simulator otherwise.
IScopeDataDevice scope = ScopeDeviceFinder.FindBest();

scope.SetChannel(ChannelId.ChannelA, new ChannelSettings(true, Coupling.Dc, VoltageRange.Range5V));

TimebaseInfo tb = scope.FindTimebase(desiredIntervalNanoseconds: 1000, sampleCount: 2000);
CaptureBlock block = await scope.RunBlockAsync(2000, tb.Timebase);

ChannelSamples a = block.Channels[ChannelId.ChannelA];
for (int i = 0; i < a.Count; i++)
{
    Console.WriteLine($"{block.TimeSecondsAt(i):F6} s  {a.VoltsAt(i):F4} V");
}

scope.CloseScope();
```

**Prerequisites**

- .NET 10 SDK
- A 64-bit process (see [bitness](#trap-5-the-driver-is-64-bit-only))
- The `ps2000` driver: on Windows, PicoSDK **or** the PicoScope desktop
  application; on Linux, the `libps2000` package, which the `picoscope`
  application package brings with it. See
  [Getting the driver to load](#getting-the-driver-to-load).
- **The PicoScope desktop application must be closed.** Only one process may hold
  the device open.

---

## Verified capability map

Everything in this table was probed on the physical unit. `Ps2000ScopeDataDevice`
rediscovers all of it at open time rather than matching on the model name, so the
code stays correct if a different 2000-series scope is attached.

### Inputs

| Property | Value |
|---|---|
| Channels | **A and B only.** C, D and the external trigger input are rejected |
| Resolution | 8-bit ADC, always scaled to 16 bits by the driver |
| Full scale | **±32767** ADC counts (`PS2000_MAX_VALUE`) |
| "No data" sentinel | −32768 (`PS2000_LOST_DATA`) — distinct from −32767 |
| Ranges | **±50 mV to ±20 V** (9 of the 12 the enum defines) |
| Rejected ranges | ±10 mV, ±20 mV, ±50 V |
| Coupling | AC and DC, both supported |

Range enum ordinals, since the accepted set is not contiguous from zero:

| Ordinal | Range | 2204A |
|---|---|---|
| 0 | ±10 mV | ✗ |
| 1 | ±20 mV | ✗ |
| 2 | ±50 mV | ✓ |
| 3 | ±100 mV | ✓ |
| 4 | ±200 mV | ✓ |
| 5 | ±500 mV | ✓ |
| 6 | ±1 V | ✓ |
| 7 | ±2 V | ✓ |
| 8 | ±5 V | ✓ |
| 9 | ±10 V | ✓ |
| 10 | ±20 V | ✓ |
| 11 | ±50 V | ✗ |

### Timing

| Property | Value |
|---|---|
| Timebase range | **0–23** |
| Timebase 0 | Valid **only when exactly one channel is enabled** |
| Timebase 1 | The fastest available with both channels on |
| Sampling interval | `10 × 2^timebase` nanoseconds (10 ns to 83.9 ms) |
| Fastest sampling | 100 MS/s (one channel), 50 MS/s (two channels) |
| Block buffer | **8064** samples (one channel), **3968** (two channels) |
| Max oversample | **4.** The header advertises 256 for the series; the 2204A rejects 8 and above |

Oversampling divides the buffer: at oversample 2 the block holds 4032 samples,
at 4 it holds 2016.

### Triggering

| Property | Value |
|---|---|
| Simple trigger sources | **Channel A, Channel B, None.** No external trigger input |
| Directions | Rising, falling |
| Advanced trigger | Supported — properties, conditions, directions, delay |
| Pulse-width qualifier | Supported |
| Window comparison | Supported (via threshold mode) |
| Hysteresis | Supported |

### Equivalent-time sampling (ETS)

| Property | Value |
|---|---|
| Supported | Yes, both Fast and Slow modes |
| Effective interval | **500 ps** at 250 cycles / 40 interleave |
| Max cycles | 250 |
| Max interleave | 40 |

ETS reconstructs a **repetitive** waveform at an effective resolution far beyond
the real 10 ns floor, by capturing it many times with deliberate sub-sample
offsets and interleaving the results. On a single-shot event it produces
nonsense, because it is stitching separate captures together.

### Streaming

| Property | Value |
|---|---|
| Compatible streaming | Supported. Millisecond intervals, polled, capped at 60000 samples |
| Fast streaming | Supported. Sub-microsecond intervals, callback-driven, millions of samples |

Use fast streaming for anything live.

### Signal generator

| Property | Value |
|---|---|
| Built-in generator | Yes |
| Waveforms | **All 9**: Sine, Square, Triangle, RampUp, RampDown, DcVoltage, Gaussian, Sinc, HalfSine |
| Frequency range | 0.1 Hz to **100 kHz**. 200 kHz is rejected |
| Amplitude | Up to **4 V peak-to-peak**. 8 Vpp is rejected |
| Configurable during acquisition | **No** — see [Trap 7](#trap-7-the-signal-generator-is-refused-during-acquisition) |
| Sweeps | Supported (Up, Down, UpDown, DownUp) |
| AWG | Yes, **up to 4096 samples**, 8-bit. 8192 is rejected |
| AWG DDS clock | 48 MHz |
| AWG phase accumulator | 32-bit |

Note the `ps2000` wave-type ordinals differ from `ps2000a`: here `DcVoltage` is
5 and `Gaussian` is 6, whereas the A-series driver places them at 8 and 6.

### Miscellaneous

| Feature | 2204A |
|---|---|
| `ps2000_flash_led` | ✓ Supported (returns 64 — any non-zero means success) |
| `ps2000_set_led` | ✗ Returns 0 |
| `ps2000_set_light` | ✗ Returns 0 |
| `ps2000_last_button_press` | ✗ No button on this model (that is the handheld 2104/2105) |
| `ps2000PingUnit` | ✓ |
| Unit info lines | 9, including the loaded driver's full path on line 8 |

---

## The seven traps

These are the things that cost real time. Each is handled and documented in the
code, but they are worth knowing before you write your own.

### Trap 1: the timebase interval is always nanoseconds

`ps2000_get_timebase` gives you both a `time_interval` and a `time_units`. It is
natural — and wrong — to read the interval as being expressed in those units.

> **`time_interval` is always in nanoseconds.**
> **`time_units` describes the timestamp array**, not the interval.

Measured on the 2204A at timebase 18: the driver reports an interval of
`2621440` with units of **microseconds**. Read literally that is 2.6 seconds per
sample, and an 8000-sample capture would take almost six hours.

| Reading | Predicted duration for 8000 samples |
|---|---|
| As microseconds | 20,971,520 ms (5.8 hours) |
| As nanoseconds | 20,971 ms |
| **Actually measured** | **21,156 ms** |

The driver picks the finest timestamp unit in which the whole capture still fits
a 32-bit integer, which is why the reported unit drifts from picoseconds to
microseconds as the timebase slows. Getting this wrong mis-scales every time axis
by a factor of 1000.

### Trap 2: success is 1, failure is 0

Most `ps2000` entry points return `int16_t` where **zero means failure** and any
non-zero value means success. That is the inverse of the `PICO_STATUS` convention
used by every newer Pico driver, where zero (`PICO_OK`) means success.

```csharp
// ps2000 — legacy convention
if (Ps2000Api.ps2000_set_channel(handle, ch, 1, 1, range) == 0) { /* FAILED */ }

// ps2000a and newer — PICO_STATUS convention
if (Imports.SetChannel(handle, ch, 1, coupling, range) != 0) { /* FAILED */ }
```

Exceptions to the rule, which return values rather than flags:

| Function | Returns |
|---|---|
| `ps2000_open_unit` | A handle: `>0` success, `0` no device, `-1` found but unusable |
| `ps2000_get_values` / `ps2000_get_times_and_values` | The sample count |
| `ps2000_set_ets` | The effective interval in picoseconds (0 = unsupported) |
| `ps2000_get_streaming_values*` | The value count |
| `ps2000_open_unit_progress` | `1` complete, `0` pending, `-1` failed |

### Trap 3: full scale is 32767, not 32512

`ps2000` uses **32767**. The `ps2000a` driver uses **32512**. Copying scaling code
between the two introduces a silent error of about 0.8% — small enough to look
plausible and never be noticed.

```csharp
double volts = adcCount * range.FullScaleInVolts() / 32767.0;
```

### Trap 4: the streaming callback must be rooted

`ps2000_get_streaming_last_values` invokes your callback *synchronously* before
it returns, so the data pointers are valid only for the duration of the call and
must be copied out. But the delegate itself must be kept alive by managed code
for as long as streaming runs:

```csharp
// WRONG — allocates a fresh delegate per poll and leaves the thunk's
// lifetime to chance
Ps2000Api.ps2000_get_streaming_last_values(handle, StreamingCallback);

// RIGHT — one delegate, rooted in a field, for the life of the stream
private Ps2000Api.GetOverviewBuffersMaxMin _streamingCallback;
...
unsafe { _streamingCallback = StreamingCallback; }   // pointers in the signature
Ps2000Api.ps2000_get_streaming_last_values(handle, _streamingCallback);
```

Binding a method group to a delegate whose signature contains pointers requires
an `unsafe` context even though no pointer is dereferenced at that point.

The overview buffers arrive as `short**`, **two per channel** — maximum then
minimum. Channel A's maxima are index 0, its minima index 1, channel B's maxima
index 2, and so on. Without aggregation the two are identical.

### Trap 5: the driver is 64-bit only

A current PicoScope install ships a 64-bit `ps2000.dll` (PE machine `0x8664`),
and the Linux `libps2000.so` is 64-bit too. An x86 build fails with
`BadImageFormatException`. Set `<PlatformTarget>x64`.

### Trap 6: exclusive access

Only one process may hold a device open. `ps2000_open_unit` returns 0 while the
PicoScope desktop application is running. Equally, a handle you fail to close
keeps the device locked against every other process until yours exits — which is
why `IScopeDataDevice` implements `IDisposable` as a safety net on top of the explicit
`CloseScope`.

### Trap 7: the signal generator is refused during acquisition

**Every signal-generator call returns 0 while an acquisition is running** — both
fast streaming and an in-flight block capture. The driver gives no reason:
unit-info line 6 still reads `"0"`, so the failure is indistinguishable from a
bad argument, and the obvious conclusion — that the frequency or amplitude was
out of range — is wrong.

Measured on the 2204A with `ps2000_set_sig_gen_built_in`, sine, 5 kHz, 2 Vpp:

| State | Result |
|---|---|
| Idle | ✓ 1 |
| During `ps2000_run_streaming_ns` | ✗ 0 |
| During an in-flight `ps2000_run_block` | ✗ 0 |
| Immediately after `ps2000_stop` | ✓ 1 |

So the sequence is **stop → configure → restart**:

```csharp
Ps2000Api.ps2000_stop(handle);
Ps2000Api.ps2000_set_sig_gen_built_in(handle, 0, 2_000_000, 0, 5000f, 5000f, 0f, 0f, 0, 0);
Ps2000Api.ps2000_run_streaming_ns(handle, 100, 3, 1_000_000, 0, 1, 50000);   // restart
```

`Ps2000ScopeDataDevice` does this for you: `SetSignalGenerator`,
`SetArbitraryWaveform` and `StopSignalGenerator` all pause any running
acquisition, apply the change and restart it, carrying the running sample total
across so the stream looks continuous to the caller. There is a brief gap in the
data, which is unavoidable.

While you are here: **the 2204A caps generator amplitude at 4 V peak-to-peak.**
8 Vpp is rejected — with the same silent zero.

---

## Getting the driver to load

This deserves its own section because on Windows it is the first thing that
fails, and the failure looks like the driver isn't installed when it is. On
Linux it is the section that explains why nothing special is needed.

The P/Invokes name the library by its bare name — `[DllImport("ps2000")]` — so
the runtime probes for `ps2000.dll` on Windows and `libps2000.so` on Linux from
one declaration. The entry points, signatures and behaviours are the same on
both: the two drivers are built from the same `ps2000.h`, and the same
`PicoScope.ScopeData.Ps2000` assembly runs unchanged on either.

### Windows

**`ps2000.dll` is usually not where the SDK says it should be.** On a machine
with a current PicoScope 7 install, `C:\Program Files\Pico Technology\SDK\lib`
may contain only `psospa.dll`, while the legacy drivers live inside the PicoScope
application's own folder:

```
C:\Program Files\Pico Technology\PicoScope 7 T&M Stable\ps2000.dll
```

That folder is **not on `PATH`**, so a bare P/Invoke throws
`DllNotFoundException` even though the desktop application is using the driver
happily.

The fix is `NativeLibrary.SetDllImportResolver` — which exists on modern .NET and
**not** on .NET Framework. The supposedly "unsupported on .NET Core" legacy
driver is in fact easier to load there:

```csharp
NativeLibrary.SetDllImportResolver(typeof(Ps2000Api).Assembly, (name, asm, path) =>
{
    if (!string.Equals(name, "ps2000", StringComparison.OrdinalIgnoreCase))
    {
        return IntPtr.Zero;
    }

    foreach (string dir in candidateDirectories)
    {
        string candidate = Path.Combine(dir, "ps2000.dll");
        if (File.Exists(candidate) && NativeLibrary.TryLoad(candidate, out IntPtr h))
        {
            return h;
        }
    }
    return IntPtr.Zero;   // fall through to the default search
});
```

Loading by **absolute path** matters: it lets the OS resolve the driver's own
dependency (`picoipp.dll`) from beside it, which a name-based load would not do.

`Ps2000DriverLoader` searches, in order: any paths you add to
`AdditionalSearchPaths`, then both SDK `lib` folders, then every
`Program Files*\Pico Technology\PicoScope*` directory (the channel suffix varies --
"Stable", "Beta", "Early Access"). `GetSearchPaths()` gives you the list for a
diagnostic message.

### Linux

The `libps2000` package — from Pico's apt repository, on its own or as a
dependency of the `picoscope` application package — installs the driver as
`/opt/picoscope/lib/libps2000.so` and adds that folder to the system loader
configuration (`/etc/ld.so.conf.d/picoscope.conf`). The runtime's default probe
therefore finds it with no help at all. `Ps2000DriverLoader` still lists
`/opt/picoscope/lib` as a fallback, for a machine where that configuration was
not applied.

The device needs no group membership and no `sudo`: the package's udev rule
(`95-pico.rules`) opens Pico's USB devices to every user. `lsusb` lists the scope
under vendor `0ce9`. The driver's own dependencies are ordinary system libraries
(`libusb-1.0`, `libudev`), which the package pulls in.

### Confirming what loaded

On either OS the device itself will tell you: **unit info line 8 is the loaded
driver's full path.** The sample logs it at start-up. `Ps2000DriverLoader.LoadedFrom`
is set only when the resolver, rather than the default probe, did the loading.

---

## Architecture

```
five platform heads ──┬──> PicoScope.Core ──────────────> PicoScope.ScopeData
  LinuxX11            │    MainViewModel, ScopePlot        IScopeDataDevice, model types
  LinuxWayland        │    PlotterView add-in              ScopeDeviceFinder
  LinuxFrameBuffer    │    (brings CodeBrix.Plotter in)    SimulatedScopeDataDevice
  Win32Skia           │                                          ▲
  WinWpfSkia          └──> PicoScope.ScopeData.Ps2000 ───────────┘
                           ps2000 interop, driver loader
                           Ps2000ScopeDataDevice
```

| Project | Contents |
|---|---|
| `src/libs/PicoScope.ScopeData` | `IScopeDataDevice`, the model types, `SimulatedScopeDataDevice`, `ScopeDeviceFinder`. No package references at all. |
| `src/libs/PicoScope.ScopeData.Ps2000` | All 33 P/Invokes (`Ps2000Api`), `Ps2000DriverLoader`, `Ps2000ScopeDataDevice`. Plain `net10.0`: the one assembly runs on Windows and Linux. |
| `src/PicoScope.Core` | `MainViewModel`, `ScopePlot`, `HostHelper`. References the CodeBrix.Platform PlotterView add-in, which brings CodeBrix.Plotter in with it. |
| `src/PicoScope.UI` | The shared XAML: `App` and `MainPage`, whose `PlotterControl` shows `ScopePlot.Model`. |
| `src/PicoScope.LinuxX11`, `.LinuxWayland`, `.LinuxFrameBuffer`, `.Win32Skia`, `.WinWpfSkia` | One head per platform. Each registers the device implementations in its `Program.cs`. |
| `tests/libs/PicoScope.ScopeData.Tests` | xUnit tests for the contract, the simulator and the model types, driven through a scripted `TestScopeDataDevice`. |

`PicoScope.ScopeData` is device-agnostic **by construction** — it has no interop,
no package references and no platform reference, so it cannot grow a hardware
or UI dependency by accident. Implementations are supplied from outside via
`ScopeDeviceFinder.Register`, and it is each head, not the shared code, that
decides what to register:

```csharp
// Program.cs of every head. Order does not matter: FindBest() prefers real
// hardware and falls back to the simulator when nothing is plugged in.
ScopeDeviceFinder.Register(new Ps2000ScopeDataDevice());
ScopeDeviceFinder.Register(new SimulatedScopeDataDevice());
```

### One device library, not one per operating system

Nothing in `Ps2000ScopeDataDevice` is Windows-specific, and neither is anything
in the P/Invoke declarations: the Linux `libps2000.so` exports every entry point
the Windows `ps2000.dll` does, with the same signatures, because both are built
from the same header. The only code that differs by OS is *where to look for the
driver*, and `Ps2000DriverLoader` branches on that at run time. So the sample
ships one device assembly rather than a Windows one and a Linux one that would
be identical but for a 150-line loader.

### SimulatedScopeDataDevice

Not a stub. It implements the whole interface, enforces the same lifecycle rules,
and imposes the **real 2204A capability limits** — a capture the hardware would
reject is rejected here too. Code developed against the simulator behaves the
same way against the physical scope.

It synthesises a sine with third-harmonic content on channel A and a slower
phase-shifted companion on channel B, with a little noise so it looks like a
measurement rather than a formula. If the signal generator is configured, channel
A follows it, as though the generator output were looped back into the input.

### The chart

`ScopePlot` (in `PicoScope.Core`) owns a CodeBrix.Plotter `PlotModel` and turns
blocks and streaming batches into it. The page shows that model in a
`PlotterControl` from the CodeBrix.Platform PlotterView add-in, which supplies
the rendering and the pan/zoom/tracker interaction on every head. `ScopePlot`
mutates the model under `PlotModel.SyncRoot` and the control renders under the
same lock, so streaming batches are applied on the driver's polling thread
without a dispatcher hop.

### The earlier Windows-only heads

`PicoScope.Wpf`, `PicoScope.WinUI` and the `Shared/` folder are the earlier WPF
and WinUI 3 heads. They are kept in the tree but are not wired to the
reorganised libraries and are not part of `PicoScope.slnx`.

---

## Cookbook

### Open and interrogate

```csharp
using var scope = new Ps2000ScopeDataDevice();
if (!scope.OpenScope())
{
    Console.WriteLine("No device found (or the PicoScope app has it open).");
    return;
}

Console.WriteLine(scope.UnitInfo);
// PicoScope 2204A (serial 10066/1927, calibrated 05Feb24)

ScopeCapabilities c = scope.Capabilities;
Console.WriteLine($"{c.ChannelCount} channels, {c.SupportedRanges.Count} ranges");
Console.WriteLine($"Driver: {scope.GetUnitInfoLine(UnitInfoLine.DriverPath)}");
```

`OpenScope` returns **false** rather than throwing when there is simply no device
— that is an ordinary condition an application handles by falling back to a
simulator. It throws only when a device is present but something went genuinely
wrong.

### Configure channels

```csharp
scope.SetChannel(ChannelId.ChannelA, new ChannelSettings(true, Coupling.Dc, VoltageRange.Range5V));
scope.SetChannel(ChannelId.ChannelB, new ChannelSettings(true, Coupling.Ac, VoltageRange.Range1V));

// Disabling B unlocks timebase 0 and doubles the block buffer
scope.SetChannel(ChannelId.ChannelB, ChannelSettings.Default.AsDisabled());
```

Disabling a channel is not cosmetic: with one channel enabled you get 100 MS/s
and an 8064-sample buffer; with two you get 50 MS/s and 3968.

### Pick a timebase

```csharp
// By index
TimebaseInfo info = scope.GetTimebase(timebase: 8, sampleCount: 2000);
Console.WriteLine($"{info.IntervalNanoseconds} ns/sample, {info.SampleRateHz/1e3:F1} kS/s");

// Or by the interval you actually want — returns the fastest timebase
// whose interval is at least this
TimebaseInfo tb = scope.FindTimebase(desiredIntervalNanoseconds: 1000, sampleCount: 2000);
if (!tb.IsValid) { /* nothing fits */ }
```

### Block capture

```csharp
CaptureBlock block = await scope.RunBlockAsync(
    sampleCount: 2000,
    timebase: tb.Timebase,
    oversample: 1,
    includeTimestamps: false);

foreach ((ChannelId id, ChannelSamples s) in block.Channels)
{
    if (block.DidOverflow(id)) { Console.WriteLine($"{id} clipped!"); }

    for (int i = 0; i < s.Count; i++)
    {
        double t = block.TimeSecondsAt(i);
        double v = s.VoltsAt(i);
    }
}
```

`includeTimestamps: true` fills `block.Timestamps` in `block.TimestampUnits`. It
is usually unnecessary — the interval is constant and known, so
`TimeSecondsAt(i)` is exact and cheaper.

Always check `DidOverflow`: an overflowed channel's samples are clipped and its
readings untrustworthy.

### Triggering

```csharp
// Simple edge trigger at 500 mV
scope.SetTrigger(TriggerSettings.RisingEdge(
    ChannelId.ChannelA, VoltageRange.Range5V, millivolts: 500));

// Manual construction, with pre-trigger data
scope.SetTrigger(new TriggerSettings(
    Source: ChannelId.ChannelA,
    ThresholdAdc: VoltageRange.Range5V.MillivoltsToAdc(500),
    Direction: TriggerDirection.Rising,
    DelaySamples: -50f,              // trigger sits half way through the block
    AutoTriggerMilliseconds: 1000)); // 0 waits forever — will hang if it never fires

scope.DisableTrigger();
```

`AutoTriggerMilliseconds: 0` waits **indefinitely**. Prefer a non-zero value
unless you specifically want to block.

`DelaySamples` positions the trigger within the block as a percentage from −100
to +100. Zero puts it at the start; negative values give you pre-trigger data.

### Advanced trigger and pulse-width qualifier

```csharp
scope.SetAdvancedTrigger(new AdvancedTriggerSettings
{
    ChannelProperties = new[]
    {
        new TriggerChannelProperties(
            ChannelId.ChannelA,
            ThresholdMajor: VoltageRange.Range5V.MillivoltsToAdc(500),
            ThresholdMinor: VoltageRange.Range5V.MillivoltsToAdc(-500),
            Hysteresis: 256,
            Mode: ThresholdMode.Window)
    },
    ChannelAState = TriggerState.True,
    ChannelADirection = ThresholdDirection.Rising,
    AutoTriggerMilliseconds = 1000
});

scope.SetPulseWidthQualifier(new PulseWidthQualifier(
    ChannelAState: TriggerState.True,
    ChannelBState: TriggerState.DontCare,
    Direction: ThresholdDirection.Rising,
    LowerSamples: 100,
    UpperSamples: 500,
    Type: PulseWidthType.InRange));
```

`ThresholdDirection`'s level and window values deliberately **share ordinals** —
`Inside` is the same value as `Above`, `Enter` the same as `Rising`. Which
meaning applies is decided by the channel's `ThresholdMode`, not by the direction
value alone.

### Streaming

```csharp
scope.SamplesAvailable += (s, e) =>
{
    // Arrives on the polling thread, NOT the UI thread.
    if (e.BufferOverrun) { /* samples were lost */ }

    ChannelSamples a = e.Channels[ChannelId.ChannelA];
    // ... copy what you need, quickly
};

scope.StartStreaming(new StreamingSettings
{
    Mode = StreamingMode.Fast,
    SampleInterval = 100,
    IntervalUnits = TimeUnits.Microseconds,
    MaxSamples = 10_000_000,
    AutoStop = false,
    SamplesPerAggregate = 1,
    OverviewBufferSize = 50000,
    PollIntervalMilliseconds = 20
});

// ... later
scope.StopStreaming();
```

Sizing the overview buffer: it must be comfortably larger than the number of
samples that arrive between polls, or data is overwritten before you read it. At
100 µs and a 20 ms poll that is 200 samples per poll, so 50000 is generous. If
`BufferOverrun` reports true, increase the buffer or poll more often.

A freshly started stream can report an overrun on its very first status check;
treat the first one as noise.

`SamplesPerAggregate` above 1 makes the driver combine that many raw samples into
each reported value, carrying a minimum and a maximum. That is how you draw a
long window without losing spikes.

### Equivalent-time sampling

```csharp
EtsResult ets = scope.SetEts(EtsMode.Fast, cycles: 250, interleave: 40);
if (ets.IsSupported)
{
    Console.WriteLine($"{ets.EffectiveIntervalPicoseconds} ps effective");  // 500
}
// ... run blocks as normal; the driver interleaves them
scope.SetEts(EtsMode.Off, 0, 0);
```

### Signal generator

```csharp
scope.SetSignalGenerator(SignalGeneratorSettings.Tone(WaveType.Sine, 5000, peakToPeakVolts: 2.0));

// Sweep 1 kHz → 10 kHz in 100 Hz steps, 10 ms per step, 5 times
scope.SetSignalGenerator(new SignalGeneratorSettings(
    WaveType.Sine,
    OffsetMicrovolts: 0,
    PeakToPeakMicrovolts: 2_000_000,
    StartFrequencyHz: 1000,
    StopFrequencyHz: 10000,
    IncrementHz: 100,
    DwellTimeSeconds: 0.01,
    SweepType: SweepType.Up,
    SweepCount: 5));

scope.StopSignalGenerator();   // holds the output at 0 V DC
```

Amplitudes and offsets are in **microvolts**, and cap at 4 Vpp (`4_000_000`) on
a 2204A. `SweepCount: 0` sweeps continuously.

These calls are safe to make while streaming — `Ps2000ScopeDataDevice` pauses and
resumes the acquisition around them, because the device itself refuses them
mid-capture. See [Trap 7](#trap-7-the-signal-generator-is-refused-during-acquisition).

To see the generator on your own input, run a cable from the generator output
into Channel A. The simulator does this implicitly.

### Arbitrary waveform generator

```csharp
var wave = new byte[4096];                    // 0..255 spans the full output swing
for (int i = 0; i < wave.Length; i++)
{
    wave[i] = (byte)(127.5 + 127.5 * Math.Sin(2 * Math.PI * i / wave.Length));
}

scope.SetArbitraryWaveform(
    ArbitraryWaveformSettings.Fixed(wave, frequencyHz: 1000, peakToPeakVolts: 2.0));
```

The DDS phase increment is

```
deltaPhase = frequency × 2³² / 48 MHz
```

The **buffer length cancels out** of that expression — it sets the waveform's
resolution, not its repetition rate. This is worth stating because multiplying by
the buffer length is the obvious thing to reach for and it is wrong.

### Charting

```csharp
var plot = new ScopePlot { StreamWindowSeconds = 2.0, MaxPointsPerChannel = 2000 };

plot.ShowBlock(block);                 // one-shot
plot.AppendStreaming(batch);           // live, scrolling window

// plot.Model is a CodeBrix.Plotter PlotModel — render it however you like
var exporter = new PngExporter { Width = 1000, Height = 520 };
using var fs = File.Create("plot.png");
exporter.Export(plot.Model, fs);
```

Two things to know:

- **`PlotModel` is not thread-safe.** Streaming callbacks do not arrive on the UI
  thread. `ScopePlot` mutates the model under `PlotModel.SyncRoot`, and the
  `PlotterControl` renders under that same lock, so batches can be applied on
  the thread they arrive on. A hand-rolled renderer must do the same, or marshal
  everything to one thread.
- **Set `PlotModel.Background` explicitly.** If you style text and gridlines for
  a dark theme but leave the background unset, an exported PNG comes out white
  and everything is invisible. The on-screen canvas clears its own background, so
  this bug hides until you export.

Incoming data is decimated to `MaxPointsPerChannel`, which keeps redraw cost flat
regardless of sample rate:

```
step = StreamWindowSeconds / (intervalSeconds × MaxPointsPerChannel)
```

---

## Timebase reference table

Measured on the 2204A. Interval is `10 × 2^timebase` ns. "TS units" is what the
driver reports in `time_units` — remember it describes the timestamp array, not
the interval.

| Timebase | Interval | Sample rate | TS units | 1 ch | 2 ch |
|---:|---:|---:|:--|:-:|:-:|
| 0 | 10 ns | 100 MS/s | ps | ✓ | ✗ |
| 1 | 20 ns | 50 MS/s | ps | ✓ | ✓ |
| 2 | 40 ns | 25 MS/s | ps | ✓ | ✓ |
| 3 | 80 ns | 12.5 MS/s | ps | ✓ | ✓ |
| 4 | 160 ns | 6.25 MS/s | ps | ✓ | ✓ |
| 5 | 320 ns | 3.125 MS/s | ps | ✓ | ✓ |
| 6 | 640 ns | 1.5625 MS/s | ps | ✓ | ✓ |
| 7 | 1.28 µs | 781.25 kS/s | ps | ✓ | ✓ |
| 8 | 2.56 µs | 390.6 kS/s | ns | ✓ | ✓ |
| 9 | 5.12 µs | 195.3 kS/s | ns | ✓ | ✓ |
| 10 | 10.24 µs | 97.7 kS/s | ns | ✓ | ✓ |
| 11 | 20.48 µs | 48.8 kS/s | ns | ✓ | ✓ |
| 12 | 40.96 µs | 24.4 kS/s | ns | ✓ | ✓ |
| 13 | 81.92 µs | 12.2 kS/s | ns | ✓ | ✓ |
| 14 | 163.84 µs | 6.10 kS/s | ns | ✓ | ✓ |
| 15 | 327.68 µs | 3.05 kS/s | ns | ✓ | ✓ |
| 16 | 655.36 µs | 1.53 kS/s | ns | ✓ | ✓ |
| 17 | 1.311 ms | 763 S/s | ns | ✓ | ✓ |
| 18 | 2.621 ms | 381 S/s | µs | ✓ | ✓ |
| 19 | 5.243 ms | 191 S/s | µs | ✓ | ✓ |
| 20 | 10.49 ms | 95.4 S/s | µs | ✓ | ✓ |
| 21 | 20.97 ms | 47.7 S/s | µs | ✓ | ✓ |
| 22 | 41.94 ms | 23.8 S/s | µs | ✓ | ✓ |
| 23 | 83.89 ms | 11.9 S/s | µs | ✓ | ✓ |

Timebases 24 and above are rejected.

---

## Complete ps2000 API reference

All 32 exported functions, as declared in `Interop/Ps2000Api.cs`. Unless noted,
**non-zero is success**.

### Lifecycle and identity

| Function | Notes |
|---|---|
| `ps2000_open_unit` | Returns a handle: `>0` ok, `0` none found, `-1` unusable |
| `ps2000_open_unit_async` | Begins a non-blocking open |
| `ps2000_open_unit_progress` | Polls it: `1` done, `0` pending, `-1` failed |
| `ps2000_close_unit` | Releases the device |
| `ps2000_get_unit_info` | 9 lines; line 3 is the model, line 6 the error code, line 8 the driver path |
| `ps2000PingUnit` | Liveness check |
| `ps2000_flash_led` | Flashes until the next driver call |
| `ps2000_set_led` | ✗ on 2204A |
| `ps2000_set_light` | ✗ on 2204A |
| `ps2000_last_button_press` | `0` none, `1` short, `2` long. Always 0 on a 2204A |

### Configuration

| Function | Notes |
|---|---|
| `ps2000_set_channel` | Enable, coupling (1 = DC), range |
| `ps2000_get_timebase` | **1 = success.** Interval always ns; see [Trap 1](#trap-1-the-timebase-interval-is-always-nanoseconds) |

### Triggering

| Function | Notes |
|---|---|
| `ps2000_set_trigger` | Simple trigger, integer delay |
| `ps2000_set_trigger2` | Same with a `float` delay — prefer this |
| `ps2000SetAdvTriggerChannelProperties` | Per-channel thresholds. Struct is `Pack = 1` |
| `ps2000SetAdvTriggerChannelConditions` | The logical combination |
| `ps2000SetAdvTriggerChannelDirections` | Per-channel edge/window condition |
| `ps2000SetAdvTriggerDelay` | Delay and pre-trigger fraction |
| `ps2000SetPulseWidthQualifier` | Pulse-width condition |

### Acquisition

| Function | Notes |
|---|---|
| `ps2000_run_block` | Starts a block; reports how long it will take |
| `ps2000_ready` | Poll for completion — there is no completion event |
| `ps2000_stop` | Stops any acquisition. Call before reconfiguring |
| `ps2000_get_values` | **Returns the sample count** |
| `ps2000_get_times_and_values` | Same plus timestamps |
| `ps2000_set_ets` | **Returns the effective interval in ps**, 0 = unsupported |

### Streaming

| Function | Notes |
|---|---|
| `ps2000_run_streaming` | Compatible mode, ms intervals, ≤60000 samples |
| `ps2000_run_streaming_ns` | Fast mode |
| `ps2000_get_streaming_last_values` | Invokes the callback synchronously |
| `ps2000_overview_buffer_status` | Reports whether samples were lost |
| `ps2000_get_streaming_values` | Aggregated min/max retrieval |
| `ps2000_get_streaming_values_no_aggregation` | Unaggregated retrieval |

### Signal generation

| Function | Notes |
|---|---|
| `ps2000_set_sig_gen_built_in` | Amplitudes in µV; ≤100 kHz on a 2204A |
| `ps2000_set_sig_gen_arbitrary` | ≤4096 samples on a 2204A |

### Structs

All three advanced-trigger structs are declared `#pragma pack(1)` in the header,
so the managed declarations must specify `Pack = 1`. Default packing silently
corrupts every field after the first.

| Struct | Members |
|---|---|
| `PS2000_TRIGGER_CHANNEL_PROPERTIES` | thresholdMajor, thresholdMinor, hysteresis, channel, thresholdMode |
| `PS2000_TRIGGER_CONDITIONS` | A, B, C, D, external, pulseWidthQualifier |
| `PS2000_PWQ_CONDITIONS` | A, B, C, D, external (no pulse-width member) |

### Calling convention

The header declares these `__stdcall` on Win32. That matters only for 32-bit
code — x64 has a single calling convention on each operating system, so default
marshalling is correct on Windows and Linux alike.

---

## Troubleshooting

| Symptom | Cause |
|---|---|
| `DllNotFoundException` (Windows) | The driver isn't on the search path. See [Getting the driver to load](#getting-the-driver-to-load) |
| `DllNotFoundException` (Linux) | `libps2000` isn't installed, or `/opt/picoscope/lib` isn't in the loader path — check `ldconfig -p \| grep ps2000` |
| `open_unit` returns 0 on Linux with the device plugged in | The udev rule is missing, so the USB device isn't writable. Reinstall `libps2000`, or add `ATTRS{idVendor}=="0ce9", MODE="0666"` to a rules file |
| `BadImageFormatException` | 32-bit process, 64-bit driver. Set `<PlatformTarget>x64` |
| `open_unit` returns 0 | No device, **or the PicoScope application has it open** |
| `open_unit` returns −1 | Device found but not usable — often a half-closed handle from a previous run |
| `get_timebase` returns 0 | Timebase invalid for the current channel count or oversample. Timebase 0 needs a single channel; oversample above 4 is rejected |
| Time axis off by 1000× | Reading `time_interval` in `time_units`. It is always ns |
| Voltages off by ~0.8% | Scaling with 32512 instead of 32767 |
| `set_channel` returns 0 | Unsupported channel or range — a 2204A rejects ±10 mV, ±20 mV, ±50 V, and channels C/D |
| Streaming callback never fires | The delegate was collected. Root it in a field |
| `BufferOverrun` constantly true | Overview buffer too small or polling too slowly |
| Device locked after a crash | The handle was never closed. The lock clears when the process exits |
| `set_sig_gen_built_in` returns 0 | An acquisition is running — stop it first. Or the amplitude exceeds 4 Vpp, or the frequency exceeds 100 kHz |
| `set_sig_gen_arbitrary` returns 0 | Same, plus a buffer longer than 4096 samples |

---

## Licensing

Three different things with three different terms:

| | Licence | Redistribute? |
|---|---|---|
| This sample | Repository terms (MIT) | Yes |
| Pico's C# examples this derives from | **ISC** | Yes — keep the copyright and permission notice |
| `ps2000.dll` and `picoipp.dll`; `libps2000.so` and `libpicoipp.so` | Pico proprietary SDK terms | Permitted, but **not** MIT |

Pico's [`picosdk-c-sharp-examples`](https://github.com/picotech/picosdk-c-sharp-examples)
is ISC-licensed — a full grant to modify and redistribute commercially, provided
the copyright and permission notices survive. ISC is compatible with MIT.

Pico's SDK licence permits redistributing the driver ("You may copy and
distribute the SDK without restriction, as long as you do not remove any Pico
Technology copyright statements"), and Pico support has advised shipping
`ps2000.dll` plus `picoipp.dll` alongside an application. **But the DLL does not
become MIT** — it keeps a field-of-use restriction ("for use only with Pico
products or with data collected using Pico products") and a mission-critical
exclusion, neither of which is open-source-compatible.

This sample therefore **does not commit the driver**. It resolves it at run time
from your own PicoSDK or PicoScope install on Windows, or from the `libps2000`
package on Linux. That keeps the repository cleanly licensed and avoids pinning
a driver version that would drift out of step with whatever your machine has.

---

## How these facts were established

Nothing here was taken on trust from documentation covering a dozen models with
footnotes. The capability map came from purpose-built probes run against the
physical unit:

1. **Every enum value was offered to the device** and the accept/reject answer
   recorded — all 12 ranges, all channels, all trigger sources, all 9 waveforms,
   timebases 0–25, oversample 1–256, AWG buffers of 1024/2048/4096/8192, and
   generator frequencies from 0.1 Hz to 1 MHz.
2. **The timebase-units question was settled by wall clock.** An 8000-sample
   capture at timebase 18 was timed with a stopwatch and compared against what
   each interpretation predicted. Nanoseconds predicted 20,971 ms; the measured
   time was 21,156 ms; microseconds predicted 5.8 hours.
3. **The whole stack was then exercised end to end** against the device —
   capability discovery, timebase lookup, block capture, two seconds of fast
   streaming (25 batches, 19,198 samples), ETS, triggering, the AWG, and the
   guard clauses — before any UI was built on top of it.

`Ps2000ScopeDataDevice.DiscoverCapabilities()` performs the same interrogation at
open time, so the values are re-derived from whatever hardware is actually
attached rather than hardcoded from this table. Running it on Linux, through
`libps2000.so`, produced the identical capability map for the same unit.

---

## Running the sample

`PicoScope.slnx` holds every head, both libraries and the tests, and builds on
Linux, macOS and Windows with the plain .NET SDK:

```bash
dotnet build samples/PicoScope/PicoScope.slnx
```

Run the head that matches your desktop:

```bash
dotnet run --project samples/PicoScope/src/PicoScope.LinuxX11
dotnet run --project samples/PicoScope/src/PicoScope.LinuxWayland
dotnet run --project samples/PicoScope/src/PicoScope.Win32Skia
dotnet run --project samples/PicoScope/src/PicoScope.WinWpfSkia
```

The console shows what the view model found — the device, the driver it loaded
from, the discovered capabilities — and each acquisition it starts. With no
scope attached, every head falls back to `SimulatedScopeDataDevice` and the
status bar reads `SIMULATED`.

The tests need no hardware:

```bash
dotnet test --project samples/PicoScope/tests/libs/PicoScope.ScopeData.Tests/PicoScope.ScopeData.Tests.csproj
```

If that reports "Zero tests ran" (a known runner quirk on some SDK builds), run
the built test assembly directly, which prints the xUnit summary:

```bash
dotnet samples/PicoScope/tests/libs/PicoScope.ScopeData.Tests/bin/Debug/net10.0/PicoScope.ScopeData.Tests.dll
```
