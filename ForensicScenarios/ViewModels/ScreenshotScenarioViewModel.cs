using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class ScreenshotScenarioViewModel : ScenarioCategoryViewModel
    {
        protected override void Initialize()
        {
            DisplayName = "Screenshots";

            Scenarios.Add(new Screenshot1());
            Scenarios.Add(new Screenshot2());
        }
    }
}
