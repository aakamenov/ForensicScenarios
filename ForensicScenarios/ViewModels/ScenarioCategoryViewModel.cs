using System.Collections.ObjectModel;
using Caliburn.Micro;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public abstract class ScenarioCategoryViewModel : Screen
    {
        public ObservableCollection<IScenario> Scenarios { get; set; }

        public ScenarioCategoryViewModel()
        {
            Scenarios = new ObservableCollection<IScenario>();

            Initialize();
        }

        protected abstract void Initialize();
    }
}
