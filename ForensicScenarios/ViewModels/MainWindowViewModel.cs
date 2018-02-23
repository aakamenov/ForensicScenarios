using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using Caliburn.Micro;
using ForensicScenarios.Scenarios;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.ViewModels
{
    public class MainWindowViewModel : Conductor<ScenarioCategoryViewModel>.Collection.OneActive,
        IHandle<ScenarioCompleted>
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

        public bool TabControlEnabled => runningScenarioCount == 0;

        private ScenarioCategoryViewModel selectedTab;
        private string scenarioDescription;
        private int runningScenarioCount;

        private readonly int totalScenarioCount;
        private readonly IEventAggregator eventAggregator;

        public MainWindowViewModel(
            IEventAggregator aggregator,
            ShellbagScenarioViewModel shellbag,
            EncryptionScenarioViewModel encryption, 
            ScreenshotScenarioViewModel screenshot,
            ReverseShellViewModel reverseShell)
        {
            Items.Add(shellbag);
            Items.Add(encryption);
            Items.Add(screenshot);
            Items.Add(reverseShell);

            totalScenarioCount = Items.Select(x => x.Scenarios.Count).Sum(); 

            SelectedTab = Items.FirstOrDefault();
            SelectedScenarios = new ObservableCollection<IScenario>();

            eventAggregator = aggregator;
            eventAggregator.Subscribe(this);
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
            runningScenarioCount = SelectedScenarios.Count;
            NotifyOfPropertyChange(nameof(TabControlEnabled));

            //https://stackoverflow.com/questions/2329978/the-calling-thread-must-be-sta-because-many-ui-components-require-this
            SelectedScenarios.Apply(async x => 
                                    await Application.Current.Dispatcher.InvokeAsync(new System.Action(x.Run)));

            NotifyOfPropertyChange(nameof(RunScenariosButtonText));
            NotifyOfPropertyChange(nameof(CanRunScenarios));
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

        public void OnClosing(CancelEventArgs e)
        {
            if (!TabControlEnabled)
            {
                var message = $"You have {runningScenarioCount} scenarios currently running.\nExit anyway?";

                var result = MessageBox.Show(message, "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            ProcessService.KillAll();
        }

        public void Handle(ScenarioCompleted message)
        {
            runningScenarioCount--;
            message.CompletedScenario.IsSelected = false;

            if (runningScenarioCount == 0)
                NotifyOfPropertyChange(nameof(TabControlEnabled));
        }

        public void Exit()
        {
            Application.Current.MainWindow.Close();
        }
    }
}
