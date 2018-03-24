using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public sealed class InternetSecurityViewModel : ScenarioCategoryViewModel
    {
        public InternetSecurityViewModel(
            ReverseShell reverseShell, 
            SQLInjection injection, 
            Browser browser,
            BrowserNavigation browserNavigation)
        {
            DisplayName = "Internet Security";

            Scenarios.Add(reverseShell);
            Scenarios.Add(injection);
            Scenarios.Add(browser);
            Scenarios.Add(browserNavigation);
        }
    }
}
