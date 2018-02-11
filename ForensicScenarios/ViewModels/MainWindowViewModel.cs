using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class MainWindowViewModel : Conductor<ScenarioCategoryViewModel>.Collection.OneActive
    {
        public ScenarioCategoryViewModel SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                NotifyOfPropertyChange(nameof(SelectedTab));
            }
        }

        private ScenarioCategoryViewModel selectedTab;

        public MainWindowViewModel()
        {
            Items.Add(new ShellbagScenarioViewModel());
            Items.Add(new EncryptionScenarioViewModel());

            SelectedTab = Items.FirstOrDefault();
        }
    }
}
