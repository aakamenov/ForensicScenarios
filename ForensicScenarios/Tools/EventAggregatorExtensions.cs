using Caliburn.Micro;
using ForensicScenarios.Events;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.Tools
{
    public static class EventAggregatorExtensions
    {
        public static void SendStatusInfo(this IEventAggregator eventAggregator, 
                                               IScenario sender, string message)
        {
            eventAggregator.BeginPublishOnUIThread(new ScenarioStatusUpdated(sender, message));
        }
    }
}
