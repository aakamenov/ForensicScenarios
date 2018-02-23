﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Security.Cryptography;
using Caliburn.Micro;
using ForensicScenarios.Events;
using ForensicScenarios.ViewModels;

namespace ForensicScenarios.Scenarios
{
    public class DESEncryption : PropertyChangedBase, IScenario
    {
        public string Description { get; set; }

        public string Status
        {
            get => status;
            private set
            {
                status = value;
                NotifyOfPropertyChange(nameof(Status));
            }
        }

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
        private string status;
        private string[] passwords;
        private string currentPassword;

        private const string NAME = "DES Decryption"; //Used to control the text displayed in the listbox
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

            Description = "The DES Encryption Scenario\nThe Data Encryption Standard (DES) is an outdated symmetric-key method of data encryption. It works by using the same key to encrypt and decrypt a message, so both the sender and the receiver must know and use the same private key. Replaced by more secure Advanced Encryption Standard (AES) algorithm.\nThis will create a folder in the forensic bot folder on your desktop. Within this folder, a plain text file will be created. This file will then be encrypted using a password.";

            SetupPrompt();
        }

        public void Run()
        {
            if (passwords is null)
            {
                passwords = Properties.Resources.Passwords.Split(new string[] { Environment.NewLine },
                                                                 StringSplitOptions.RemoveEmptyEntries);
            }

            ClrPrevious();
            CreateFolderFile();
            EncryptFile();

            File.Delete(path + FILENAME);

            windowManager.ShowWindow(prompt);
        }

        public override string ToString()
        {
            return NAME;
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
            prompt.Title = NAME;
            prompt.Label = "Enter the cracked password below:";
            prompt.ButtonText = "Submit";

            prompt.Deactivated += PromptDeactivated;
            prompt.Submitted += PromptSubmitted;
        }

        private void ClrPrevious()
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Status += "Removing previous files...✔\n";
                }                
            }
            catch
            {
                Status += "Removing previous files...✖\n";
            }
        }

        public void CreateFile()
        {
            using (StreamWriter text = File.CreateText(path + FILENAME))
                text.WriteLine("This is example of file text. Now this file will be encrypted. Look at RAM > " + path + FILENAME);
        }

        public void CreateFolderFile()
        {
            try
            {

                Directory.CreateDirectory(path);

                using (StreamWriter text = File.CreateText(path + FILENAME))
                    text.WriteLine("This is an example text file. Now this file will be encrypted. Look at RAM > " + path + FILENAME);

                Status += "Directory has been created...✔\n";
                Status += "File with plain text was created...✔\n";
            }
            catch(Exception)
            {
                Status += "Directory has been created...✖\n";
                Status += "File with plain text was created...✖\n";
            }
        }

        public void EncryptFile()
        {
            try
            {
                var rnd = new Random();
                var index = rnd.Next(0, passwords.Length - 1);
                currentPassword = passwords[index];

                Encrypt(path + FILENAME, path + FILENAME_ENCRYPTED, currentPassword);

                Status += "File with encrypted text was created...✔\n";
            }
            catch(Exception)
            {
                Status += "File with encrypted text was created...✖\n";
            }
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

