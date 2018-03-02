using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public class EncryptionScenarioViewModel : ScenarioCategoryViewModel
    {
        public EncryptionScenarioViewModel(
            DESEncryption desEncryption,
            AESEncryption aesEncryption) : base()
        {
            DisplayName = "Encryption";

            Scenarios.Add(desEncryption);
            Scenarios.Add(aesEncryption);
        }
    }
}
