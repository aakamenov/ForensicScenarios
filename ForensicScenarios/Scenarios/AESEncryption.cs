using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AESEncryption : PropertyChangedBase, IScenario
    {
        public string Name => "AES Encryption";

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

        private const string FILENAME = "PlainText.txt";
        private const string FILENAME_ENCRYPTED = "EncryptedText.txt";
        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\AESEncryption\\";
        private const string TEXT_INPUT = "Malware, short for malicious software, is any software used to disrupt computer operations,\ngather sensitive information, gain access to private computer systems, or display unwanted advertising.\nMalware is defined by its malicious intent, acting against the requirements of the computer user,\nand does not include software that causes unintentional harm due to some deficiency.\nThe term badware is sometimes used, and applied to both true (malicious) malware and unintentionally harmful software\nReference: https://en.wikipedia.org/wiki/Malware";
        private readonly TextFieldPromptViewModel prompt;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        public AESEncryption(
            TextFieldPromptViewModel textFieldPrompt,
            IWindowManager manager,
            IEventAggregator aggregator)
        {
            prompt = textFieldPrompt;
            windowManager = manager;
            eventAggregator = aggregator;
            
            Description = "The Advanced Encryption Standard or AES is a symmetric block cipher used by the U.S. government to protect classified information and is implemented in software and hardware throughout the world to encrypt sensitive data.\nThis will create a folder in the forensic bot folder on your desktop. Within this folder, a plain text file will be created. This file will then be encrypted using a password.\n";
        }

        public void Run()
        {
            SetupPrompt();

            ClrPrevious();
            CreateFolder();
            PlainTextFile();
            EncryptedTextFile();

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
            if (prompt.TextBoxContents == currentPassword)
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

        public void CreateFolder()
        {
            var msg = string.Empty;

            try
            {
                Directory.CreateDirectory(path);

                msg = "Directory Created...✔";
            }
            catch
            {
                msg = "Directory Created...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        private void PlainTextFile()
        {
            var fullPath = path + FILENAME;
            var msg = string.Empty;

            try
            {
                using (StreamWriter text = File.CreateText(fullPath))
                    text.WriteLine(TEXT_INPUT);

                msg = "File with plain text was created...✔";
            }
            catch
            {
                msg = "File with plain text was created...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        public void EncryptedTextFile()
        {
            var rnd = new Random();
            currentPassword = ResourcesManager.GetRandomPassword();

            string fullPath = path + FILENAME_ENCRYPTED;
            string str = EncryptText(TEXT_INPUT, currentPassword);
            var msg = string.Empty;

            try
            {
                using (StreamWriter text = File.CreateText(fullPath))
                    text.WriteLine(str);

                msg = "File with encrypted text was created...✔";
            }
            catch
            {
                msg = "File with encrypted text was created...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        private string EncryptText(string textinput, string password)
        {
            var sha256 = SHA256.Create();
            var encrypted = Encrypt(Encoding.UTF8.GetBytes(textinput),
                                    sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));

            return Convert.ToBase64String(encrypted);
        }

        public byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] salt = new byte[8]
            {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8
            };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
                    rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
                    rijndaelManaged.Mode = CipherMode.CBC;

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                                        rijndaelManaged.CreateEncryptor(),
                                                                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cryptoStream.Close();
                    }

                    return memoryStream.ToArray();
                }
            }
        }
    }
}
