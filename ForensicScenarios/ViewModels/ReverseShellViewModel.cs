﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class ReverseShellViewModel : ScenarioCategoryViewModel
    {
        public ReverseShellViewModel(ReverseShell reverseShell) : base()
        {
            DisplayName = "Reverse shell";
            Scenarios.Add(reverseShell);
        }
    }
}
