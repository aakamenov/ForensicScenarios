using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using ForensicScenarios.ViewModels;

namespace ForensicScenarios
{
    public class Bootstrap : BootstrapperBase
    {
        private SimpleContainer container;

        public Bootstrap()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.PerRequest<MainWindowViewModel>();
            container.PerRequest<ShellbagScenarioViewModel>();
            container.PerRequest<EncryptionScenarioViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}
