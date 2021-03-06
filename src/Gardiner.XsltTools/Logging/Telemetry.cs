using System;

using EnvDTE;

using EnvDTE80;

using JetBrains.Annotations;

namespace Gardiner.XsltTools.Logging
{
    #pragma warning disable CA1724 // Type Names Should Not Match Namespaces
    public static class Telemetry
    {
        private static ITelemetryProvider _provider;
        private static DTEEvents _events;

        public static void Initialise([NotNull] ITelemetryProvider provider, [NotNull] DTE2 dte)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (dte == null)
            {
                throw new ArgumentNullException(nameof(dte));
            }

            _provider = provider;
            _events = dte.Events.DTEEvents;

            _events.OnBeginShutdown += Shutdown;
        }

        public static void LogMessage(string message)
        {
            _provider.LogMessage(message);
        }

        public static void Log(Exception ex)
        {
            _provider.LogException(ex);
        }

        public static void Shutdown()
        {
            _events.OnBeginShutdown -= Shutdown;

            _provider.Shutdown();
        }
    }
}