using System;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;
using ForensicScenarios.Tools;

namespace ForensicScenarios.Scenarios
{
    public class BrowserScenarioBase : PropertyChangedBase
    {
        protected string GetDefaultBrowserPath()
        {
            RegistryKey key = null;
            var browser = string.Empty;

            try
            {
                key = Registry.ClassesRoot.OpenSubKey("\\http\\shell\\open\\command", false);
                browser = key.GetValue(null).ToString();
                var exeSuffix = ".exe";

                if (!browser.EndsWith(exeSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    var lastIndex = browser.LastIndexOf(exeSuffix, StringComparison.OrdinalIgnoreCase);
                    browser = browser.Substring(0, lastIndex + exeSuffix.Length).TrimStart('\"');
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (key != null)
                    key.Close();
            }

            return browser;
        }

        protected (string parameter, string name) GetParametersForBrowser(string path)
        {
            string toLower = path.ToLower();

            if (toLower.Contains("firefox"))
            {
                return (parameter: "-private-window", name: "firefox");
            }
            else if (toLower.Contains("chrome"))
            {
                return (parameter: "--incognito", name: "chrome");
            }
            else if (toLower.Contains("explore"))
            {
                return (parameter: "-private", name: "explore");
            }
            else if (toLower.Contains("edge"))
            {
                return (parameter: "-private", name: "edge");
            }
            else if (toLower.Contains("opera"))
            {
                return (parameter: "--private", name: "opera");
            }
            else
            {
                return (parameter: string.Empty, name: string.Empty);
            }
        }

        /// <summary>
        /// Kills all processes which match the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException"
        /// <exception cref="NotSupportedException"
        protected async Task CloseOpenInstancesAsync(string name)
        {
            var processes = Process.GetProcessesByName(name);

            foreach (var process in processes)
            {
                if (process.MainWindowHandle != IntPtr.Zero) //Check if the process has a window
                {
                    process.CloseMainWindow();
                    await Wait.ForTimeAsync(TimeSpan.FromSeconds(1));
                }
            }           
        }
    }
}
