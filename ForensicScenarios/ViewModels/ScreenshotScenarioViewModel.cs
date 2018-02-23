using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class ScreenshotScenarioViewModel : ScenarioCategoryViewModel
    {
        public ScreenshotScenarioViewModel(
            Screenshot1 screenshot1,
            Screenshot2 screenshot2)
            : base()
        {
            DisplayName = "Screenshots";

            Scenarios.Add(screenshot1);
            Scenarios.Add(screenshot2);
        }
    }
}
