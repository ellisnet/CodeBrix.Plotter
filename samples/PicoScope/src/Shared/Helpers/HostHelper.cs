using CodeBrix.Platform.Simple;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace PicoScope.Helpers;

/// <summary>
/// Supplies the generic host builder that CodeBrix.Platform's service resolver
/// needs at start-up.
/// </summary>
public static class HostHelper
{
    private class HostBuilderProvider : IHostBuilderProvider
    {
        public IHostBuilder CreateDefaultBuilder() => Host.CreateDefaultBuilder();
        public IHostBuilder CreateDefaultBuilder(string[] args) => Host.CreateDefaultBuilder(args);
    }

    private static readonly HostBuilderProvider Provider = new HostBuilderProvider();

    /// <summary>
    /// Returns the host builder provider.
    /// </summary>
    /// <returns>The provider instance.</returns>
    public static IHostBuilderProvider GetHost() => Provider;
}
