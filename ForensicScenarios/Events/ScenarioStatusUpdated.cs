using ForensicScenarios.Scenarios;

namespace ForensicScenarios.Events
{
    public class ScenarioStatusUpdated
    {
        public IScenario Scenario { get; }
        public string StatusMessage { get; }

        public ScenarioStatusUpdated(IScenario scenario, string message)
        {
            Scenario = scenario;
            StatusMessage = message;
        }
    }
}
