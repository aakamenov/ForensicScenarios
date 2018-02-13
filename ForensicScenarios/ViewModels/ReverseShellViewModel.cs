using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    class ReverseShellViewModel : ScenarioCategoryViewModel
    {
        protected override void Initialize()
        {
            DisplayName = "Reverse Shell";

            Scenarios.Add(new ReverseShell());
        }
    }
}
