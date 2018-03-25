using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using ForensicScenarios.ViewModels;
using ForensicScenarios.Scenarios;

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
            container.Singleton<IEventAggregator, EventAggregator>();
            
            container.PerRequest<MainWindowViewModel>();
            container.PerRequest<ShellbagScenarioViewModel>();
            container.PerRequest<EncryptionScenarioViewModel>();
            container.PerRequest<InternetSecurityViewModel>();
            container.PerRequest<ScreenshotScenarioViewModel>();
            container.PerRequest<TextFieldPromptViewModel>();

            container.PerRequest<AESEncryption>();
            container.PerRequest<DESEncryption>();
            container.PerRequest<ReverseShell>();
            container.PerRequest<Screenshot1>();
            container.PerRequest<Screenshot2>();
            container.PerRequest<Shellbag>();
            container.PerRequest<SQLInjection>();
            container.PerRequest<Browser>();
            container.PerRequest<BrowserNavigation>();
            container.PerRequest<TrueCrypt>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
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
