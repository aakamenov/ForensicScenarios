using System.Collections.ObjectModel;
using Caliburn.Micro;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public abstract class ScenarioCategoryViewModel : Screen
    {
        public ObservableCollection<IScenario> Scenarios { get; set; }

        public IScenario SelectedScenario
        {
            get => selectedScenario;
            set
            {
                selectedScenario = value;
                NotifyOfPropertyChange(nameof(SelectedScenario));
                NotifyOfPropertyChange(nameof(selectedScenario.Description));
            }
        }

        private IScenario selectedScenario;

        public ScenarioCategoryViewModel()
        {
            Scenarios = new ObservableCollection<IScenario>();
            Initialize();
        }

        protected abstract void Initialize();
    }
}
