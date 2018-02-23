using System.Collections.ObjectModel;
using Caliburn.Micro;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public abstract class ScenarioCategoryViewModel : Screen, IChild
    {
        public ObservableCollection<IScenario> Scenarios { get; set; }

        public ScenarioCategoryViewModel()
        {
            Scenarios = new ObservableCollection<IScenario>();         
        }
    }
}
