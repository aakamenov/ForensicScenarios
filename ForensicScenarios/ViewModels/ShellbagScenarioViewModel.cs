using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class ShellbagScenarioViewModel : ScenarioCategoryViewModel
    {
        protected override void Initialize()
        {
            DisplayName = "Shellbags";

            Scenarios.Add(new Shellbag());
        }
    }
}
