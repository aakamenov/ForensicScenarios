using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Forms = System.Windows.Forms;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class BrowserNavigation : BrowserScenarioBase, IScenario
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

        public string Name => "Browser Website Navigation";

        public string Description { get; set; }

        private bool isSelected;
        private IEventAggregator eventAggregator;
        private const string URLS_FILE = "url.txt";
        private readonly string[] defaultWebsites = 
        {
            @"https://www.napier.ac.uk",
            @"http://www.dell.com",
            @"http://www.hp.com",
            @"https://rust-lang.org",
            @"http://vanilla-js.com"
        };

        public BrowserNavigation(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "To simulate user activities, a number of websites will be opened in a private window. A link found on the websites may then be navigated to.\n\nRunning this scenario will close currently open default browser windows.\n\nTo override which websites are browsed, create a text file named \"url.txt\" in the root directory of the tool and enter the websites URLs there, separated by a new line.";
        }

        public async void Run()
        {
            var browserPath = GetDefaultBrowserPath();

            if (string.IsNullOrEmpty(browserPath))
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

            var websites = File.Exists(URLS_FILE) ? await GetUrlsFromFile() : defaultWebsites;

            foreach (string website in websites)
            {
                Uri url = null;

                try
                {
                    url = new Uri(website);
                }
                catch(UriFormatException)
                {
                    eventAggregator.SendStatusInfo(this, $"Skipping \"{website}\": not a valid URL!");
                    continue;
                }

                await RunBrowserNavigation(url.AbsoluteUri, name, browserPath, parameter);
            }

            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }

        private async Task RunBrowserNavigation(string url, string browserName, string browserPath, string parameter)
        {
            try
            {
                var browser = ProcessService.CreateProcess(browserPath, parameter + ' ' + url);

                browser.Start();

                await Wait.ForTimeAsync(TimeSpan.FromSeconds(10));

                var randomTabNumber = new Random().Next(4, 15);

                for (int i = 0; i < randomTabNumber; i++)
                {
                    Forms.SendKeys.SendWait("{TAB}");
                    await Wait.ForTimeAsync(TimeSpan.FromMilliseconds(200));
                }

                Forms.SendKeys.SendWait("{ENTER}");

                await Wait.ForTimeAsync(TimeSpan.FromSeconds(5));

                eventAggregator.SendStatusInfo(this, $"Browsing \"{url}\"...✔");
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, $"Browsing \"{url}\"...✖");
            }
            
            try
            {
                await CloseOpenInstancesAsync(browserName);
                eventAggregator.SendStatusInfo(this, "Closing browser...✔");
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, "Closing browser...✖");
            }

            await Wait.ForTimeAsync(TimeSpan.FromSeconds(1));
        }

        private async Task<IEnumerable<string>> GetUrlsFromFile()
        {
            try
            {
                using (var file = File.Open(URLS_FILE, FileMode.Open))
                {
                    using (var reader = new StreamReader(file))
                    {
                        var contents = await reader.ReadToEndAsync();

                        var urls = contents.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                        eventAggregator.SendStatusInfo(this, "Reading URLs from file...✔");

                        return urls;
                    }
                }
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, "Reading URLs from file...✖");

                return Enumerable.Empty<string>();
            }
        }
    }
}
