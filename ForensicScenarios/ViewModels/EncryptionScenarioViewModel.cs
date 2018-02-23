using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class EncryptionScenarioViewModel : ScenarioCategoryViewModel
    {
        public EncryptionScenarioViewModel(DESEncryption desEncryption) : base()
        {
            DisplayName = "Encryption";

            Scenarios.Add(desEncryption);
        }
    }
}
