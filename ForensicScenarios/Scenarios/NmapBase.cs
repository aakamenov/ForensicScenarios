using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public abstract class NmapBase : PropertyChangedBase, IScenario
    {
        public abstract string Name { get; }

        public abstract string Description { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(nameof(IsSelected));
            }
        }

        private bool isSelected;

        protected readonly IEventAggregator eventAggregator;

        public NmapBase(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
        }

        public abstract void Run();

        protected async Task RunNmap(string flag)
        {
            await Task.Run(async () =>
            {
                if (!IsInstalled())
                {
                    eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
                    return;
                }

                var ip = "10.201.0.42";
                var command = $"nmap {flag} {ip}";

                var cmd = ProcessService.CreateCmdProcess(string.Empty, false, redirectInput: true, redirectOutput: true);
                cmd.Start();

                cmd.StandardInput.WriteLine(command);
                await Wait.ForTimeAsync(TimeSpan.FromSeconds(2));
                cmd.StandardInput.WriteLine("exit");

                cmd.WaitForExit();

                if (!cmd.HasExited)
                    cmd.Kill();

                eventAggregator.SendStatusInfo(this, "Nmap completed!");
                eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
            });
        }

        protected bool IsInstalled()
        {
            try
            {
                var isInstalled = LocalSoftware.IsInstalled("Nmap");

                if (!isInstalled)
                {
                    eventAggregator.SendStatusInfo(this, "Could not find Nmap installation.\nTerminating scenario execution...");
                    return false;
                }

                var hasGlobalPath = LocalSoftware.LookupEXEInGlobalPaths("nmap");

                if (!hasGlobalPath)
                {
                    eventAggregator.SendStatusInfo(this, "Nmap is installed but its directory is not in \"Path\". Please, add the Nmap installation directory to the \"Path\" environment variable.\nTerminating scenario execution...");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                if (e is IOException ||
                    e is SecurityException ||
                    e is UnauthorizedAccessException)
                {
                    eventAggregator.SendStatusInfo(this, $"An error occurred: {e.Message}\nTerminating scenario execution...");
                }
                else
                    eventAggregator.SendStatusInfo(this, "An error occurred while trying to locate the Nmap installation.\nTerminating scenario execution...");

                return false;
            }
        }
    }
}
