using Caliburn.Micro;

namespace ForensicScenarios.Scenarios
{
    public class NmapScanST : NmapBase
    {
        public override string Name => "Nmap Scan -sT";

        public override string Description { get; set; }

        public NmapScanST(IEventAggregator aggregator) : base(aggregator)
        {
            Description = "Nmap is an open-source network mapping tool used for network discovery and security auditing.Nmap can also use sophisticated scanning methods to detect which services are running on a computer, making it valuable for attackers.\n\nRunning this scenario will cause this machine to attempt to scan the victim computer for open ports, and which services may be running on these ports.";
        }

        public override async void Run()
        {
            await RunNmap("-sT");
        }
    }
}
