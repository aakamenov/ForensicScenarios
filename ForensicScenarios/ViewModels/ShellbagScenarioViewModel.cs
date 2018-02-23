using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class ShellbagScenarioViewModel : ScenarioCategoryViewModel
    {
        public ShellbagScenarioViewModel(Shellbag shellbag) : base()
        {
            DisplayName = "Shellbags";

            Scenarios.Add(shellbag);
        }
    }
}
