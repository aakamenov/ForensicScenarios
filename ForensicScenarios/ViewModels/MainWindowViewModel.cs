using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class MainWindowViewModel : Conductor<ScenarioCategoryViewModel>.Collection.OneActive
    {
        public ObservableCollection<IScenario> SelectedScenarios { get; set; }

        public ScenarioCategoryViewModel SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                NotifyOfPropertyChange(nameof(SelectedTab));
            }
        }

        public string ScenarioDescription
        {
            get => scenarioDescription;
            set
            {
                scenarioDescription = value;
                NotifyOfPropertyChange(nameof(ScenarioDescription));
            }
        }

        public string RunScenariosButtonText => $"Run Scenarios\n({SelectedScenarios.Count}/{totalScenarioCount})";

        public bool CanRunScenarios => SelectedScenarios.Count > 0;

        private ScenarioCategoryViewModel selectedTab;
        private string scenarioDescription;
        private readonly int totalScenarioCount;

        public MainWindowViewModel()
        {
            Items.Add(new ShellbagScenarioViewModel());
            Items.Add(new EncryptionScenarioViewModel());
            Items.Add(new ScreenshotScenarioViewModel());
            Items.Add(new ReverseShellViewModel());

            totalScenarioCount = Items.Select(x => x.Scenarios.Count).Sum(); 

            SelectedTab = Items.FirstOrDefault();
            SelectedScenarios = new ObservableCollection<IScenario>();
        }

        public void ShowDescription(ListView a)
        {
            var item = VisualTreeHelper.HitTest(a, Mouse.GetPosition(a)).VisualHit;

            while (item != null && !(item is ListViewItem))
                item = VisualTreeHelper.GetParent(item);

            if (item != null && item is ListViewItem)
            {
                var listViewItem = item as ListViewItem;
                var scenario = listViewItem.DataContext as IScenario;
                ScenarioDescription = scenario.Description;
            }
        }

        public void EmptyDescription()
        {
            ScenarioDescription = string.Empty;
        }

        public void RunScenarios()
        {
            SelectedScenarios.Apply(x => x.Run());
        }

        public void SelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
                SelectedScenarios.Add(item as IScenario);

            foreach (var item in e.RemovedItems)
                SelectedScenarios.Remove(item as IScenario);

            NotifyOfPropertyChange(nameof(RunScenariosButtonText));
            NotifyOfPropertyChange(nameof(CanRunScenarios));
        }
    }
}
