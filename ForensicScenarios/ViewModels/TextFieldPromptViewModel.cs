using Caliburn.Micro;

namespace ForensicScenarios.ViewModels
{
    public class TextFieldPromptViewModel : Screen
    {
        public System.Action Submitted { get; set; }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyOfPropertyChange(nameof(Title));
            }
        }

        public string Label
        {
            get => label;
            set
            {
                label = value;
                NotifyOfPropertyChange(nameof(Label));
            }
        }

        public string TextBoxContents
        {
            get => textBoxContents;
            set
            {
                textBoxContents = value;
                NotifyOfPropertyChange(nameof(CanSubmit));
                NotifyOfPropertyChange(nameof(TextBoxContents));
            }
        }

        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;
                NotifyOfPropertyChange(nameof(ButtonText));
            }
        }

        public bool CanSubmit => !string.IsNullOrWhiteSpace(textBoxContents);

        private string title;
        private string label;
        private string textBoxContents;
        private string buttonText;

        public void Submit()
        {
            if (Submitted != null)
                Submitted();
        }
    }
}
