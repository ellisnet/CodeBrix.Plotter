using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CodeBrix.Platform.Simple;
using CodeBrix.Plotter;
using PicoScope.Scope;
using PicoScope.Scope.Charting;
using PicoScope.Scope.Model;

// ReSharper disable once CheckNamespace
namespace PicoScope.ViewModels;

/// <summary>
/// Lets the hosting page hand the view model a way to invalidate the Skia
/// canvas the chart is drawn on, and a way to get back onto the UI thread.
/// </summary>
/// <remarks>
/// Streaming callbacks arrive on a polling thread, and a
/// <see cref="PlotModel"/> may only be touched from one thread. Each head wires
/// these up with its own dispatcher -- <c>Dispatcher.BeginInvoke</c> on WPF,
/// <c>DispatcherQueue.TryEnqueue</c> on WinUI.
/// </remarks>
public interface IChartHost
{
    /// <summary>Invalidates the hosting page's chart canvas so it repaints.</summary>
    Action InvalidateChart { get; set; }

    /// <summary>Runs the given action on the UI thread.</summary>
    Action<Action> RunOnUiThread { get; set; }
}

#if HAS_WINUI
[Microsoft.UI.Xaml.Data.Bindable]
#endif
/// <summary>
/// Drives the PicoScope sample: finds a scope, configures it, runs captures and
/// streams, and keeps a <see cref="ScopePlot"/> up to date for the view to draw.
/// </summary>
public class MainViewModel : SimpleViewModel, IChartHost
{
    private IPicoScope _scope;

    /// <summary>
    /// Creates the view model. Call <see cref="InitializeAsync"/> once the view
    /// has wired up <see cref="InvalidateChart"/> and <see cref="RunOnUiThread"/>.
    /// </summary>
    public MainViewModel()
    {
        Plot = new ScopePlot();
    }

    /// <inheritdoc />
    public Action InvalidateChart { get; set; }

    /// <inheritdoc />
    public Action<Action> RunOnUiThread { get; set; }

    /// <summary>
    /// The chart. Its <see cref="ScopePlot.Model"/> is what the view renders.
    /// </summary>
    public ScopePlot Plot { get; }

    /// <summary>The voltage ranges the attached scope accepts.</summary>
    public ObservableCollection<VoltageRange> AvailableRanges { get; } = new ObservableCollection<VoltageRange>();

    /// <summary>The waveforms the attached scope's generator can produce.</summary>
    public ObservableCollection<WaveType> AvailableWaveTypes { get; } = new ObservableCollection<WaveType>();

    //Every setter passes notifyOnMainThread: true. Several of these are written
    //  from the driver polling thread or from an async continuation, and a
    //  PropertyChanged raised off the UI thread breaks XAML binding on both
    //  WPF and WinUI.
    private string _statusText = "Starting up...";

    /// <summary>A one-line status message for the view.</summary>
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value ?? string.Empty, notifyOnMainThread: true);
    }

    private string _deviceText = string.Empty;

    /// <summary>A description of the attached device.</summary>
    public string DeviceText
    {
        get => _deviceText;
        private set => SetProperty(ref _deviceText, value ?? string.Empty, notifyOnMainThread: true);
    }

    private string _capabilityText = string.Empty;

    /// <summary>A summary of what the attached device can do.</summary>
    public string CapabilityText
    {
        get => _capabilityText;
        private set => SetProperty(ref _capabilityText, value ?? string.Empty, notifyOnMainThread: true);
    }

    private bool _isSimulated = true;

    /// <summary>Whether the active scope is a simulator rather than real hardware.</summary>
    public bool IsSimulated
    {
        get => _isSimulated;
        private set
        {
            SetProperty(ref _isSimulated, value, notifyOnMainThread: true);
            NotifyPropertyChanged(nameof(SimulatedBadge), true);
        }
    }

    /// <summary>
    /// A badge for the status bar: "SIMULATED" when no real device is attached,
    /// otherwise empty.
    /// </summary>
    /// <remarks>
    /// Deliberately a string rather than a bool bound to Visibility. WPF would
    /// need a DataTrigger and WinUI a converter, and both would have to be
    /// written twice; an empty string renders as nothing in either framework
    /// with no extra machinery.
    /// </remarks>
    public string SimulatedBadge => _isSimulated ? "SIMULATED" : string.Empty;

    private bool _isReady;

    /// <summary>Whether a scope is open and ready.</summary>
    [AffectsCommands(nameof(CaptureCommand), nameof(StartStreamingCommand),
        nameof(StopStreamingCommand), nameof(SignalGeneratorCommand), nameof(FlashLedCommand))]
    public bool IsReady
    {
        get => _isReady;
        private set => SetProperty(ref _isReady, value, notifyOnMainThread: true);
    }

    private bool _isStreaming;

    /// <summary>Whether a stream is currently running.</summary>
    [AffectsCommands(nameof(CaptureCommand), nameof(StartStreamingCommand), nameof(StopStreamingCommand))]
    public bool IsStreaming
    {
        get => _isStreaming;
        private set => SetProperty(ref _isStreaming, value, notifyOnMainThread: true);
    }

    private bool _showChannelA = true;

    /// <summary>
    /// Whether channel A's trace is drawn. Hiding a channel affects the chart
    /// only -- it keeps acquiring, so unhiding reveals the history that arrived
    /// meanwhile.
    /// </summary>
    public bool ShowChannelA
    {
        get => _showChannelA;
        set
        {
            if (_showChannelA == value) { return; }
            SetProperty(ref _showChannelA, value, notifyOnMainThread: true);
            Plot.SetChannelVisible(ChannelId.ChannelA, value);
            InvalidateChart?.Invoke();
        }
    }

    private bool _showChannelB = true;

    /// <summary>Whether channel B's trace is drawn.</summary>
    public bool ShowChannelB
    {
        get => _showChannelB;
        set
        {
            if (_showChannelB == value) { return; }
            SetProperty(ref _showChannelB, value, notifyOnMainThread: true);
            Plot.SetChannelVisible(ChannelId.ChannelB, value);
            InvalidateChart?.Invoke();
        }
    }

    private VoltageRange _selectedRange = VoltageRange.Range5V;

    /// <summary>The input range applied to both channels.</summary>
    public VoltageRange SelectedRange
    {
        get => _selectedRange;
        set
        {
            //SetEnumProperty is the enum-specific entry point; the generic
            //  SetProperty is constrained to reference types.
            if (_selectedRange == value) { return; }
            SetEnumProperty(ref _selectedRange, value, notifyOnMainThread: true);
            ApplyChannelSettings();
        }
    }

    private WaveType _selectedWaveType = WaveType.Sine;

    /// <summary>The waveform the signal generator produces.</summary>
    public WaveType SelectedWaveType
    {
        get => _selectedWaveType;
        set => SetEnumProperty(ref _selectedWaveType, value, notifyOnMainThread: true);
    }

    private double _generatorFrequencyHz = 5000;

    /// <summary>The frequency the signal generator produces, in hertz.</summary>
    public double GeneratorFrequencyHz
    {
        get => _generatorFrequencyHz;
        set
        {
            //There is no double overload of SetProperty, so this one is done by
            //  hand rather than routed through a helper that does not fit.
            if (Math.Abs(_generatorFrequencyHz - value) < double.Epsilon) { return; }
            _generatorFrequencyHz = value;
            NotifyPropertyChanged(nameof(GeneratorFrequencyHz), true);
        }
    }

    private bool _generatorRunning;

    /// <summary>Whether the signal generator is currently driving its output.</summary>
    public bool GeneratorRunning
    {
        get => _generatorRunning;
        private set => SetProperty(ref _generatorRunning, value, notifyOnMainThread: true);
    }

    /// <summary>
    /// Finds the best available scope, configures it, and starts streaming so the
    /// chart is live immediately.
    /// </summary>
    /// <returns>A task that completes once the scope is ready.</returns>
    /// <remarks>
    /// Each head registers its own implementations with
    /// <see cref="PicoScopeFinder"/> before calling this, which is what keeps
    /// this view model free of any device-specific reference.
    /// </remarks>
    public async Task InitializeAsync()
    {
        _scope = PicoScopeFinder.FindBest();

        if (_scope == null)
        {
            StatusText = "No scope available -- nothing registered with PicoScopeFinder.";
            return;
        }

        IsSimulated = _scope.IsSimulated;
        DeviceText = _scope.UnitInfo.ToString();

        ScopeCapabilities c = _scope.Capabilities;
        CapabilityText =
            $"{c.ChannelCount} channels | " +
            $"{c.SupportedRanges.Count} ranges ({c.SupportedRanges[0].ToDisplayString()} to " +
            $"{c.SupportedRanges[^1].ToDisplayString()}) | " +
            $"timebase {c.MinTimebaseSingleChannel}-{c.MaxTimebase} | " +
            $"{(c.SupportsFastStreaming ? "fast streaming" : "no fast streaming")} | " +
            $"{(c.SupportsEts ? $"ETS {c.EtsEffectiveIntervalPicoseconds} ps" : "no ETS")} | " +
            $"{(c.SupportsSignalGenerator ? $"siggen to {c.MaxSignalGeneratorFrequencyHz / 1000:0} kHz" : "no siggen")}";

        AvailableRanges.Clear();
        foreach (VoltageRange range in c.SupportedRanges) { AvailableRanges.Add(range); }
        _selectedRange = c.Supports(VoltageRange.Range5V) ? VoltageRange.Range5V : c.SupportedRanges[^1];
        NotifyPropertyChanged(nameof(SelectedRange), true);

        AvailableWaveTypes.Clear();
        foreach (WaveType wave in c.SupportedWaveTypes) { AvailableWaveTypes.Add(wave); }

        ApplyChannelSettings();

        _scope.SamplesAvailable += OnSamplesAvailable;
        IsReady = true;
        StatusText = _scope.IsSimulated
            ? "Simulated scope -- no hardware needed."
            : $"Connected to {_scope.Name}.";

        //Show something immediately rather than an empty chart.
        await CaptureOnceAsync().ConfigureAwait(false);
        DoStartStreaming();
    }

    private void ApplyChannelSettings()
    {
        if (_scope == null || !_scope.IsOpen) { return; }

        bool wasStreaming = _scope.IsStreaming;
        if (wasStreaming) { _scope.StopStreaming(); }

        foreach (ChannelId channel in _scope.Capabilities.SupportedChannels)
        {
            _scope.SetChannel(channel, new ChannelSettings(true, Coupling.Dc, _selectedRange));
        }

        if (wasStreaming) { DoStartStreaming(); }
    }

    private void OnSamplesAvailable(object sender, StreamingSamplesEventArgs e)
    {
        //Arrives on the polling thread. The plot model is not thread-safe, so
        //  hop to the UI thread before touching it.
        Action apply = () =>
        {
            Plot.AppendStreaming(e);
            InvalidateChart?.Invoke();
        };

        if (RunOnUiThread != null) { RunOnUiThread(apply); }
        else { apply(); }
    }

    #region | Commands and their implementations |

    private SimpleCommand _captureCommand;

    /// <summary>Captures a single block and displays it.</summary>
    public SimpleCommand CaptureCommand =>
        _captureCommand ??= new SimpleCommand(() => IsReady && !IsStreaming, DoCapture);

    private void DoCapture() => _ = CaptureOnceAsync();

    private async Task CaptureOnceAsync()
    {
        if (_scope == null || !_scope.IsOpen) { return; }

        try
        {
            //Roughly one microsecond per sample gives a couple of milliseconds
            //  across the block, which suits a 1-10 kHz signal nicely.
            TimebaseInfo tb = _scope.FindTimebase(1000, 2000);
            if (!tb.IsValid)
            {
                StatusText = "No usable timebase for a single capture.";
                return;
            }

            int samples = Math.Min(2000, tb.MaxSamples);
            CaptureBlock block = await _scope.RunBlockAsync(samples, tb.Timebase).ConfigureAwait(false);

            Action apply = () =>
            {
                Plot.ShowBlock(block);
                InvalidateChart?.Invoke();
                StatusText = $"Captured {block.SampleCount} samples at " +
                             $"{tb.SampleRateHz / 1000:0.#} kS/s.";
            };

            if (RunOnUiThread != null) { RunOnUiThread(apply); }
            else { apply(); }
        }
        catch (PicoScopeException ex)
        {
            StatusText = "Capture failed: " + ex.Message;
        }
    }

    private SimpleCommand _startStreamingCommand;

    /// <summary>Starts a live stream.</summary>
    public SimpleCommand StartStreamingCommand =>
        _startStreamingCommand ??= new SimpleCommand(() => IsReady && !IsStreaming, DoStartStreaming);

    private void DoStartStreaming()
    {
        if (_scope == null || !_scope.IsOpen || _scope.IsStreaming) { return; }

        try
        {
            Plot.Clear();
            _scope.StartStreaming(new StreamingSettings
            {
                Mode = _scope.Capabilities.SupportsFastStreaming
                    ? StreamingMode.Fast
                    : StreamingMode.Compatible,
                SampleInterval = 100,
                IntervalUnits = TimeUnits.Microseconds,
                MaxSamples = 10_000_000,
                AutoStop = false,
                OverviewBufferSize = 50000,
                PollIntervalMilliseconds = 20
            });

            IsStreaming = true;
            StatusText = "Streaming.";
        }
        catch (PicoScopeException ex)
        {
            StatusText = "Could not start streaming: " + ex.Message;
        }
    }

    private SimpleCommand _stopStreamingCommand;

    /// <summary>Stops a running stream.</summary>
    public SimpleCommand StopStreamingCommand =>
        _stopStreamingCommand ??= new SimpleCommand(() => IsStreaming, DoStopStreaming);

    private void DoStopStreaming()
    {
        if (_scope == null) { return; }

        _scope.StopStreaming();
        IsStreaming = false;
        StatusText = "Stopped.";
    }

    private SimpleCommand _signalGeneratorCommand;

    /// <summary>Turns the signal generator on or off.</summary>
    public SimpleCommand SignalGeneratorCommand =>
        _signalGeneratorCommand ??= new SimpleCommand(
            () => IsReady && _scope != null && _scope.Capabilities.SupportsSignalGenerator,
            DoToggleSignalGenerator);

    private void DoToggleSignalGenerator()
    {
        if (_scope == null || !_scope.IsOpen) { return; }

        try
        {
            if (GeneratorRunning)
            {
                _scope.StopSignalGenerator();
                GeneratorRunning = false;
                StatusText = "Signal generator off.";
            }
            else
            {
                _scope.SetSignalGenerator(
                    SignalGeneratorSettings.Tone(SelectedWaveType, GeneratorFrequencyHz, 2.0));
                GeneratorRunning = true;
                StatusText = $"Signal generator: {SelectedWaveType} at " +
                             $"{GeneratorFrequencyHz:N0} Hz, 2 Vpp. " +
                             (IsSimulated
                                 ? "Channel A follows it."
                                 : "Loop the generator output into Channel A to see it.");
            }
        }
        catch (PicoScopeException ex)
        {
            StatusText = "Signal generator error: " + ex.Message;
        }
    }

    private SimpleCommand _flashLedCommand;

    /// <summary>Flashes the device LED so the physical unit can be identified.</summary>
    public SimpleCommand FlashLedCommand =>
        _flashLedCommand ??= new SimpleCommand(
            () => IsReady && _scope != null && _scope.Capabilities.SupportsFlashLed,
            DoFlashLed);

    private void DoFlashLed()
    {
        _scope?.FlashLed();
        StatusText = "Flashing the device LED.";
    }

    #endregion

    /// <summary>
    /// Stops acquisition and releases the scope. Call from the view's unload or
    /// window-closed handler -- a handle left open keeps the device locked
    /// against every other process until this one exits.
    /// </summary>
    public void Shutdown()
    {
        if (_scope == null) { return; }

        _scope.SamplesAvailable -= OnSamplesAvailable;
        _scope.StopStreaming();
        _scope.StopSignalGenerator();
        PicoScopeFinder.Reset();
        _scope = null;
        IsReady = false;
        IsStreaming = false;
    }
}
