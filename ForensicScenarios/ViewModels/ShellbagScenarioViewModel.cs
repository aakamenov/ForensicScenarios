using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public sealed class ShellbagScenarioViewModel : ScenarioCategoryViewModel
    {
        public ShellbagScenarioViewModel(Shellbag shellbag) : base()
        {
            DisplayName = "Shellbags";

            Scenarios.Add(shellbag);
        }
    }
}
