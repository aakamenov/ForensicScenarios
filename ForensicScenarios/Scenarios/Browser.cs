using System;
using Forms = System.Windows.Forms;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class Browser : BrowserScenarioBase, IScenario
    {
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(nameof(IsSelected));
            }
        }

        public string Name => "Browser Search";

        public string Description { get; set; }

        private bool isSelected;
        private IEventAggregator eventAggregator;

        public Browser(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "In order to simulate a user activity, a search engine will be opened in private browsing mode using the default browser. A random word search will then be performed and a link may be navigated to.\n\nRunning this scenario will close currently open default browser windows.";
        }

        public async void Run()
        {
            var browserPath = GetDefaultBrowserPath();

            if(string.IsNullOrEmpty(browserPath))
            {
                var msg = "Finding a default browser to use...✖";

                eventAggregator.SendStatusInfo(this, msg);
                eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));

                return;
            }

            (string parameter, string name) = GetParametersForBrowser(browserPath);

            try
            {
                await CloseOpenInstancesAsync(name);
                eventAggregator.SendStatusInfo(this, "Closing open browser instances...✔");
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, "Closing open browser instances...✖");
            }

            try
            {
                var browser = ProcessService.CreateProcess(browserPath, parameter + " google.com");
                var searchWord = ResourcesManager.GetRandomWord();

                browser.Start();

                await Wait.ForTimeAsync(TimeSpan.FromSeconds(10));
                Forms.SendKeys.SendWait(searchWord);
                Forms.SendKeys.SendWait("{ENTER}");

                var randomTabNumber = new Random().Next(20, 50);

                for (int i = 0; i < randomTabNumber; i++)
                {
                    Forms.SendKeys.SendWait("{TAB}");
                    await Wait.ForTimeAsync(TimeSpan.FromMilliseconds(200));
                }

                Forms.SendKeys.SendWait("{ENTER}");
                await Wait.ForTimeAsync(TimeSpan.FromSeconds(5));

                eventAggregator.SendStatusInfo(this, "Performing search functions...✔");
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, $"Performing search functions...✖");
            }

            try
            {
                await CloseOpenInstancesAsync(name);
                eventAggregator.SendStatusInfo(this, "Closing browser...✔");
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, "Closing browser...✖");
            }

            eventAggregator.SendStatusInfo(this, "Browser Search complete...✔");
            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }
    }
}
