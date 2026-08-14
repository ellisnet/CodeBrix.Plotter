using System;
using System.Collections.Generic;
using System.Linq;

namespace PicoScope.Scope;

/// <summary>
/// The registry of available scope implementations.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="PicoScope.Scope"/> deliberately knows nothing about any real
/// device -- it has no interop and cannot acquire one. Implementations are
/// supplied from outside and registered here, which is what lets the
/// device-agnostic half of the sample stay device-agnostic.
/// </para>
/// <para>
/// A typical application registers a real implementation and a simulator at
/// startup and then asks for the best available:
/// </para>
/// <code>
/// PicoScopeFinder.Register(new WindowsPicoScope());
/// PicoScopeFinder.Register(new SimulatedPicoScope());
///
/// IPicoScope scope = PicoScopeFinder.FindBest();   //real hardware if present
/// </code>
/// <para>
/// Registration order does not matter: <see cref="FindBest"/> prefers real
/// hardware that opens successfully, and falls back to a simulator so the
/// application still runs with nothing plugged in.
/// </para>
/// </remarks>
public static class PicoScopeFinder
{
    private static readonly object SyncRoot = new object();
    private static readonly List<IPicoScope> Registered = new List<IPicoScope>();

    /// <summary>
    /// Registers a scope implementation as available for use.
    /// </summary>
    /// <param name="scope">The implementation to register.</param>
    /// <exception cref="ArgumentNullException"><paramref name="scope"/> is null.</exception>
    /// <remarks>
    /// Registering the same instance twice is a no-op. Registration does not
    /// open the scope; that happens in <see cref="FindBest"/>, or explicitly via
    /// <see cref="IPicoScope.OpenScope"/>.
    /// </remarks>
    public static void Register(IPicoScope scope)
    {
        if (scope == null) { throw new ArgumentNullException(nameof(scope)); }

        lock (SyncRoot)
        {
            if (!Registered.Contains(scope))
            {
                Registered.Add(scope);
            }
        }
    }

    /// <summary>
    /// Removes a previously registered implementation.
    /// </summary>
    /// <param name="scope">The implementation to remove.</param>
    /// <returns>True when it had been registered.</returns>
    public static bool Unregister(IPicoScope scope)
    {
        if (scope == null) { return false; }

        lock (SyncRoot)
        {
            return Registered.Remove(scope);
        }
    }

    /// <summary>
    /// Every registered implementation, in registration order.
    /// </summary>
    /// <returns>A snapshot of the registered implementations.</returns>
    public static IReadOnlyList<IPicoScope> GetRegistered()
    {
        lock (SyncRoot)
        {
            return Registered.ToArray();
        }
    }

    /// <summary>
    /// Whether any real-hardware implementation has been registered.
    /// </summary>
    /// <remarks>
    /// This reports only that an implementation exists, not that a device is
    /// actually plugged in. Use <see cref="FindBest"/> to find out whether one
    /// opens.
    /// </remarks>
    public static bool HasRealScopeImplementation
    {
        get
        {
            lock (SyncRoot)
            {
                return Registered.Any(s => !s.IsSimulated);
            }
        }
    }

    /// <summary>
    /// Returns the best available scope, opened and ready to use: real hardware
    /// when a device can be opened, otherwise a simulator.
    /// </summary>
    /// <param name="allowSimulatedFallback">
    /// Whether to fall back to a simulator when no real device opens. Set false
    /// to require real hardware.
    /// </param>
    /// <returns>
    /// An open scope, or null when nothing could be opened.
    /// </returns>
    /// <remarks>
    /// Real implementations are tried first. One that fails to open is left
    /// closed and skipped rather than treated as an error, since "no scope
    /// plugged in" is an ordinary condition.
    /// </remarks>
    public static IPicoScope FindBest(bool allowSimulatedFallback = true)
    {
        IPicoScope[] candidates;
        lock (SyncRoot)
        {
            candidates = Registered.ToArray();
        }

        //Real hardware first: a physical device beats a simulation whenever one
        //  is actually there.
        foreach (IPicoScope scope in candidates.Where(s => !s.IsSimulated))
        {
            if (scope.IsOpen) { return scope; }

            try
            {
                if (scope.OpenScope()) { return scope; }
            }
            catch (PicoScopeException)
            {
                //A present-but-unusable device should not stop us falling back
                //  to the simulator, so swallow and keep looking.
            }
        }

        if (!allowSimulatedFallback) { return null; }

        foreach (IPicoScope scope in candidates.Where(s => s.IsSimulated))
        {
            if (scope.IsOpen) { return scope; }
            if (scope.OpenScope()) { return scope; }
        }

        return null;
    }

    /// <summary>
    /// Closes and forgets every registered implementation. Intended for
    /// application shutdown and for test isolation.
    /// </summary>
    public static void Reset()
    {
        IPicoScope[] toDispose;
        lock (SyncRoot)
        {
            toDispose = Registered.ToArray();
            Registered.Clear();
        }

        foreach (IPicoScope scope in toDispose)
        {
            try
            {
                scope.Dispose();
            }
            catch (Exception)
            {
                //Shutdown must not throw; a device that is already gone is fine.
            }
        }
    }
}
