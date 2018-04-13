using System;
using System.IO;
using Microsoft.Win32;

namespace ForensicScenarios.Tools
{
    public static class LocalSoftware
    {
        private const string REGISTRY_PATH = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

        /// <summary>
        /// Checks in the registry if the given software is installed.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <returns></returns>
        public static bool IsInstalled(string name)
        {           
            using (var registryKey1 = Registry.LocalMachine.OpenSubKey(REGISTRY_PATH))
            {
                foreach (var subKeyName in registryKey1.GetSubKeyNames())
                {
                    using (var registryKey2 = registryKey1.OpenSubKey(subKeyName))
                    {
                        var displayName = registryKey2.GetValue("DisplayName");

                        if (displayName != null)
                        {
                            if (displayName.ToString().Contains(name))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the directory where the specified software is installed.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A string containing the path where the specified software is installed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static string GetSoftwareInstallationPath(string name)
        {
            using (var registryKey1 = Registry.LocalMachine.OpenSubKey(REGISTRY_PATH))
            {
                foreach (var subKeyName in registryKey1.GetSubKeyNames())
                {
                    using (var registryKey2 = registryKey1.OpenSubKey(subKeyName))
                    {
                        var displayName = registryKey2.GetValue("DisplayName");

                        if (displayName != null)
                        {
                            if (displayName.ToString().Contains(name))
                            {
                                var uninstallString = registryKey2.GetValue("UninstallString");

                                if (uninstallString is null)
                                    return string.Empty;
                                else
                                    return Path.GetDirectoryName(uninstallString.ToString());
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks whether any ".exe" files in the directories listed in the user "Path" variable contain the parameter specified.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static bool LookupEXEInGlobalPaths(string name)
        {
            var pathVar = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)
                                     .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(var entry in pathVar)
            {
                try
                {
                    var files = Directory.GetFiles(entry, "*.exe", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        if (file.ToLower().Contains(name.ToLower()))
                            return true;
                    }
                }
                catch(DirectoryNotFoundException) //Ignore non-existing directories
                {
                    continue;
                }
            }

            return false;
        }
    }
}
