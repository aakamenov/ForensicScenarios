using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public sealed class InternetSecurityViewModel : ScenarioCategoryViewModel
    {
        public InternetSecurityViewModel(
            ReverseShell reverseShell,
            ReverseShellVictim reverseShellVictim,
            SQLInjection injection, 
            Browser browser,
            BrowserNavigation browserNavigation,
            NmapScan nmap)
        {
            DisplayName = "Internet Security";

            Scenarios.Add(reverseShell);
            Scenarios.Add(reverseShellVictim);
            Scenarios.Add(injection);
            Scenarios.Add(browser);
            Scenarios.Add(browserNavigation);
            Scenarios.Add(nmap);
        }
    }
}
