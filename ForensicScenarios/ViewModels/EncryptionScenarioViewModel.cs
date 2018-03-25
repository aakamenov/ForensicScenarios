using ForensicScenarios.Scenarios;

namespace ForensicScenarios.ViewModels
{
    public sealed class EncryptionScenarioViewModel : ScenarioCategoryViewModel
    {
        public EncryptionScenarioViewModel(
            DESEncryption desEncryption,
            AESEncryption aesEncryption,
            TrueCrypt trueCrypt) : base()
        {
            DisplayName = "Encryption";

            Scenarios.Add(desEncryption);
            Scenarios.Add(aesEncryption);
            Scenarios.Add(trueCrypt);
        }
    }
}
