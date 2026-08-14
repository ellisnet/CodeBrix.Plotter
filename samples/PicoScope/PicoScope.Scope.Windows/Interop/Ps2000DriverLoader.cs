using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PicoScope.Scope.Windows.Interop;

/// <summary>
/// Finds and loads <c>ps2000.dll</c> at run time.
/// </summary>
/// <remarks>
/// <para>
/// This exists because the driver is usually <b>not</b> anywhere the default
/// P/Invoke search will look. Installing PicoSDK does not reliably put
/// <c>ps2000.dll</c> in <c>Program Files\Pico Technology\SDK\lib</c> -- on a
/// machine with a current PicoScope 7 install, that folder may contain only
/// <c>psospa.dll</c>, while the legacy drivers live inside the PicoScope
/// application's own folder, which is not on <c>PATH</c>. A bare
/// <c>[DllImport("ps2000.dll")]</c> then fails with
/// <see cref="DllNotFoundException"/> even though the driver is installed and
/// the desktop application is using it happily.
/// </para>
/// <para>
/// The fix is <see cref="NativeLibrary.SetDllImportResolver"/>, which is
/// available on modern .NET but not on .NET Framework -- so the supposedly
/// "unsupported on .NET Core" legacy driver is in fact easier to load there.
/// </para>
/// <para>
/// <b>Bitness.</b> The driver shipped with a 64-bit PicoScope install is 64-bit,
/// so the host process must be 64-bit too. An x86 build fails with
/// <see cref="BadImageFormatException"/>.
/// </para>
/// </remarks>
public static class Ps2000DriverLoader
{
    /// <summary>The driver library file name.</summary>
    public const string DriverFileName = "ps2000.dll";

    private static readonly object SyncRoot = new object();
    private static bool _registered;

    /// <summary>
    /// Additional directories to search before the built-in ones. Add to this
    /// before the first driver call if the driver lives somewhere unusual.
    /// </summary>
    public static IList<string> AdditionalSearchPaths { get; } = new List<string>();

    /// <summary>
    /// The full path of the driver that was loaded, or an empty string if it has
    /// not been loaded yet.
    /// </summary>
    public static string LoadedFrom { get; private set; } = string.Empty;

    /// <summary>
    /// Registers the resolver. Safe to call repeatedly; only the first call does
    /// anything.
    /// </summary>
    /// <remarks>
    /// Every entry point in <see cref="Ps2000"/> calls this first, so callers do
    /// not normally need to.
    /// </remarks>
    public static void EnsureRegistered()
    {
        if (_registered) { return; }

        lock (SyncRoot)
        {
            if (_registered) { return; }

            NativeLibrary.SetDllImportResolver(typeof(Ps2000DriverLoader).Assembly, Resolve);
            _registered = true;
        }
    }

    /// <summary>
    /// Returns every directory that will be searched for the driver, in order.
    /// </summary>
    /// <returns>The candidate directories.</returns>
    /// <remarks>
    /// Useful in a diagnostic message when the driver cannot be found, so a user
    /// can see where it was looked for.
    /// </remarks>
    public static IReadOnlyList<string> GetSearchPaths()
    {
        var paths = new List<string>(AdditionalSearchPaths);

        string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        //The documented SDK location comes first, because that is where a
        //  developer who installed PicoSDK will expect it to be.
        paths.Add(Path.Combine(programFiles, "Pico Technology", "SDK", "lib"));
        paths.Add(Path.Combine(programFilesX86, "Pico Technology", "SDK", "lib"));

        //Then the PicoScope application folders, which is where the legacy
        //  drivers actually live on a current install. The channel suffix
        //  varies ("Stable", "Beta", "Early Access"), so enumerate.
        foreach (string root in new[] { programFiles, programFilesX86 })
        {
            string picoRoot = Path.Combine(root, "Pico Technology");
            if (!Directory.Exists(picoRoot)) { continue; }

            try
            {
                foreach (string dir in Directory.EnumerateDirectories(picoRoot, "PicoScope*"))
                {
                    paths.Add(dir);
                }
            }
            catch (IOException)
            {
                //An unreadable directory is not worth failing over.
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        return paths;
    }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals(libraryName, DriverFileName, StringComparison.OrdinalIgnoreCase))
        {
            return IntPtr.Zero;
        }

        foreach (string directory in GetSearchPaths())
        {
            string candidate = Path.Combine(directory, DriverFileName);
            if (!File.Exists(candidate)) { continue; }

            //Loading by absolute path also lets the OS resolve the driver's own
            //  dependencies (picoipp.dll) from beside it, which a plain
            //  name-based load would not do.
            if (NativeLibrary.TryLoad(candidate, out IntPtr handle))
            {
                LoadedFrom = candidate;
                return handle;
            }
        }

        //Fall through to the default search, which covers the case where the
        //  driver has been copied next to the executable or put on PATH.
        return IntPtr.Zero;
    }
}
