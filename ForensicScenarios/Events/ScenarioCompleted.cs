using ForensicScenarios.Scenarios;


namespace ForensicScenarios.Events
{
    public class ScenarioCompleted
    {
        public IScenario CompletedScenario { get; }

        public ScenarioCompleted(IScenario scenario)
        {
            CompletedScenario = scenario;
        }
    }
}
