using System;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RmobDatVisualizer.GUI.Helpers
{
    /// <summary>
    /// Helper class to retrieve version information from the assembly.
    /// </summary>
    public static class VersionHelper
    {
        /// <summary>
        /// Gets the application version from the assembly.
        /// Strips any Git commit hash or metadata (e.g., "1.0.2+abc123def" becomes "1.0.2").
        /// </summary>
        /// <returns>Version string in the format "vX.Y.Z" (clean without hash)</returns>
        public static string GetApplicationVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            if (!string.IsNullOrEmpty(informationalVersion))
            {
                // Remove Git commit hash and metadata (e.g., "1.0.2+abc123" becomes "1.0.2")
                var cleanVersion = Regex.Replace(informationalVersion, @"\+.*$", "");
                return $"v{cleanVersion}";
            }

            var version = assembly.GetName().Version;
            if (version != null)
            {
                return $"v{version.Major}.{version.Minor}.{version.Build}";
            }

            return "vUnknown";
        }

        /// <summary>
        /// Gets the full application title with version.
        /// </summary>
        /// <returns>Title string in the format "Rmob.Dat Visualizer vX.Y.Z"</returns>
        public static string GetApplicationTitle()
        {
            return $"Rmob.Dat Visualizer {GetApplicationVersion()}";
        }
    }
}
