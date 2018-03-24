using System;
using System.Text;
using System.IO;
using System.Windows;
using System.Security.Cryptography;
using Caliburn.Micro;
using ForensicScenarios.Events;
using ForensicScenarios.ViewModels;
using ForensicScenarios.Tools;

namespace ForensicScenarios.Scenarios
{
    public class DESEncryption : PropertyChangedBase, IScenario
    {
        public string Name => "DES Encryption";

        public string Description { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(nameof(IsSelected));
            }
        }

        private bool isSelected;
        private string currentPassword;

        private const string FILENAME = "\\example.txt";
        private const string FILENAME_ENCRYPTED = "\\example.encrypted.txt";
        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\DESEncryption";
        private readonly TextFieldPromptViewModel prompt;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        public DESEncryption(
            TextFieldPromptViewModel textFieldPrompt,
            IWindowManager manager,
            IEventAggregator aggregator)
        {
            prompt = textFieldPrompt;
            windowManager = manager;
            eventAggregator = aggregator;

            Description = "The DES Encryption Scenario\n\nThe Data Encryption Standard (DES) is an outdated symmetric-key method of data encryption. It works by using the same key to encrypt and decrypt a message, so both the sender and the receiver must know and use the same private key. Replaced by more secure Advanced Encryption Standard (AES) algorithm.\nThis will create a folder in the forensic bot folder on your desktop. Within this folder, a plain text file will be created. This file will then be encrypted using a password.";
        }

        public void Run()
        {
            SetupPrompt();

            ClrPrevious();
            CreateFolderFile();
            EncryptFile();

            if(File.Exists(path + FILENAME))
                File.Delete(path + FILENAME);

            windowManager.ShowWindow(prompt);
        }

        private void PromptDeactivated(object sender, DeactivationEventArgs e)
        {
            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }

        private void PromptSubmitted()
        {
            if(prompt.TextBoxContents == currentPassword)
            {
                MessageBox.Show("Password is correct!", "Challenge completed!", MessageBoxButton.OK, MessageBoxImage.Information);
                prompt.TryClose();
            }
            else
            {
                MessageBox.Show("Incorrect password, try again!", "Incorrect password!", MessageBoxButton.OK, MessageBoxImage.Error);
                prompt.TextBoxContents = string.Empty;
            }
        }

        private void SetupPrompt()
        {
            prompt.Title = Name;
            prompt.Label = "Enter the cracked password below:";
            prompt.TextBoxContents = string.Empty;
            prompt.ButtonText = "Submit";

            prompt.Deactivated += PromptDeactivated;
            prompt.Submitted += PromptSubmitted;
        }

        private void ClrPrevious()
        {
            var msg = string.Empty;

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    msg = "Removing previous files...✔";
                }                
            }
            catch
            {
                msg = "Removing previous files...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        public void CreateFile()
        {
            using (StreamWriter text = File.CreateText(path + FILENAME))
                text.WriteLine("This is example of file text. Now this file will be encrypted. Look at RAM > " + path + FILENAME);
        }

        public void CreateFolderFile()
        {
            var msg = string.Empty;

            try
            {
                Directory.CreateDirectory(path);

                using (StreamWriter text = File.CreateText(path + FILENAME))
                    text.WriteLine("This is an example text file. Now this file will be encrypted. Look at RAM > " + path + FILENAME);

                msg = "Directory has been created...✔";
                msg += "File with plain text was created...✔";
            }
            catch(Exception)
            {
                msg = "Directory has been created...✖";
                msg += "File with plain text was created...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        public void EncryptFile()
        {
            var msg = string.Empty;

            try
            {
                currentPassword = ResourcesManager.GetRandomPassword();

                Encrypt(path + FILENAME, path + FILENAME_ENCRYPTED, currentPassword);

                msg = "File with encrypted text was created...✔";
            }
            catch(Exception)
            {
                msg = "File with encrypted text was created...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        public void Encrypt(string input, string output, string strHash)
        {
            var cryptoServiceProvider1 = new TripleDESCryptoServiceProvider();
            var cryptoServiceProvider2 = new MD5CryptoServiceProvider();

            var fileStream1 = new FileStream(input, FileMode.Open, FileAccess.Read);
            var fileStream2 = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write);

            byte[] hash = cryptoServiceProvider2.ComputeHash(Encoding.ASCII.GetBytes(strHash));
            byte[] buffer = File.ReadAllBytes(input);

            cryptoServiceProvider2.Clear();
            cryptoServiceProvider1.Key = hash;
            cryptoServiceProvider1.Mode = CipherMode.ECB;

            CryptoStream cryptoStream = new CryptoStream(fileStream2, cryptoServiceProvider1.CreateEncryptor(), CryptoStreamMode.Write);

            long num = 0;
            long length = fileStream1.Length;

            while (num < length)
            {
                int count = fileStream1.Read(buffer, 0, buffer.Length);
                num += count;

                cryptoStream.Write(buffer, 0, count);
            }

            fileStream1.Close();
            fileStream2.Close();
        }
    }
}

