using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.IO;
using Caliburn.Micro;
using Microsoft.Win32;
using ForensicScenarios.Scenarios;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.ViewModels
{
    public class MainWindowViewModel : Conductor<ScenarioCategoryViewModel>.Collection.OneActive,
        IHandle<ScenarioCompleted>,
        IHandle<ScenarioStatusUpdated>
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

        public string ScenarioOutput
        {
            get => scenarioOutput;
            set
            {
                scenarioOutput = value;
                NotifyOfPropertyChange(nameof(ScenarioOutput));
            }
        }

        public string RunScenariosButtonText => $"Run Scenarios\n({SelectedScenarios.Count}/{totalScenarioCount})";

        public bool CanRunScenarios => SelectedScenarios.Count > 0 && IsRunning is false;

        public bool CanSaveLogs => !string.IsNullOrEmpty(ScenarioOutput);

        public bool TabControlEnabled => !IsRunning;

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                NotifyOfPropertyChange(nameof(IsRunning));
                NotifyOfPropertyChange(nameof(CanRunScenarios));
                NotifyOfPropertyChange(nameof(TabControlEnabled));
            }
        }

        private ScenarioCategoryViewModel selectedTab;
        private string scenarioDescription;
        private string scenarioOutput;
        private bool isRunning;

        private readonly int totalScenarioCount;
        private readonly IEventAggregator eventAggregator;

        public MainWindowViewModel(
            IEventAggregator aggregator,
            ShellbagScenarioViewModel shellbag,
            EncryptionScenarioViewModel encryption, 
            ScreenshotScenarioViewModel screenshot,
            InternetSecurityViewModel internetSecurity)
        {
            Items.Add(shellbag);
            Items.Add(encryption);
            Items.Add(screenshot);
            Items.Add(internetSecurity);

            totalScenarioCount = Items.Select(x => x.Scenarios.Count).Sum(); 

            SelectedTab = Items.FirstOrDefault();
            SelectedScenarios = new ObservableCollection<IScenario>();

            eventAggregator = aggregator;
            eventAggregator.Subscribe(this);
        }

        public void ShowDescription(ListView a)
        {
            var item = VisualTreeHelper.HitTest(a, Mouse.GetPosition(a))?.VisualHit;

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
            IsRunning = true;
            ScenarioOutput = string.Empty;

            var scenario = SelectedScenarios.First();
            UpdateRunningInfo(scenario);

            //https://stackoverflow.com/questions/2329978/the-calling-thread-must-be-sta-because-many-ui-components-require-this
            Application.Current.Dispatcher.InvokeAsync(scenario.Run);
        }

        public async void SaveLogs()
        {
            var browser = new SaveFileDialog()
            {
                CheckPathExists = true,
                CreatePrompt = true,
                DefaultExt = ".txt",
                Filter = "Text | .txt"
            };

            bool? result = browser.ShowDialog();

            if (result is null || result is false) //If no files were selected or the window was closed
                return;

            var extension = Path.GetExtension(@browser.FileName);

            if (extension != ".txt")
            {
                MessageBox.Show("The file must have \".txt\" extension!",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var file = File.CreateText(browser.FileName))
            {
                var now = DateTime.Now;
                var header = $"Forensic Scenario Bot Log File\n{now.ToShortDateString()} - {now.ToShortTimeString()}\n";

                var split = (header + ScenarioOutput).Split(new char[] { '\n' });
                var contents = string.Join(Environment.NewLine, split);

                await file.WriteAsync(contents);
            }
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
            if (IsRunning)
            {
                var message = $"You have {SelectedScenarios.Count} scenarios currently running.\nExit anyway?";

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
            message.CompletedScenario.IsSelected = false;

            if(SelectedScenarios.Count > 0)
            {
                var scenario = SelectedScenarios.First();
                UpdateRunningInfo(scenario);
                Application.Current.Dispatcher.InvokeAsync(scenario.Run);
            }
            else
            {
                IsRunning = false;
                NotifyOfPropertyChange(nameof(CanSaveLogs));
                EmptyDescription();
            }
        }

        public void Handle(ScenarioStatusUpdated message)
        {
            ScenarioOutput += $"{message.StatusMessage}\n";
        }

        public void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        private void UpdateRunningInfo(IScenario scenario)
        {
            ScenarioDescription = $"Currently running: {scenario.Name}\n";

            if (SelectedScenarios.Count > 1)
            {
                var multiple = SelectedScenarios.Count > 1 ? "s" : string.Empty;
                ScenarioDescription += $"{SelectedScenarios.Count} scenario{multiple} to go";
            }

            ScenarioOutput += $"\n{scenario.Name}:\n";
        }
    }
}
