using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class TrueCrypt : PropertyChangedBase, IScenario
    {
        public string Name => "TrueCrypt";

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
        private readonly IEventAggregator eventAggregator;
        private char volume = 'M';
        private string application;

        public TrueCrypt(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "TrueCrypt is an example of a freeware encryption utility used for on-the-fly encryption (OTFE). It can create a virtual encrypted disk within a file or encrypt a partition. It supports 32-bit and 64-bit versions of Windows, OS X and Linux operating systems.\n\nTrueCrypt is vulnerable to various known attacks which are also present in other software-based disk encryption software such as BitLocker. To prevent those, requires users to follow various security precautions. Development of TrueCrypt was ended in May 2014.\n\nThis scenario will mount an encrypted drive and manipulate a file within the drive before dismounting it.";
        }

        public void Run()
        {
            if(!IsInstalled())
            {
                eventAggregator.SendStatusInfo(this, "Could not find TrueCrypt installation. Terminating scenario execution...");
                eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
                return;
            }

            ClearPrevious(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\True Crypt");

            CreateFolder();

            if (!LoadDisk())
            {
                eventAggregator.SendStatusInfo(this, "Could not load TrueCrypt disk. Terminating scenario execution...");
                eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));

                return;
            }

            eventAggregator.SendStatusInfo(this, "Copying encrypted file to machine\n");

            if (MountDisk())
            {
                try
                {
                    OpenFile();
                    eventAggregator.SendStatusInfo(this, "Mounting additional disk...✔\n");
                    ReadDrive();
                }
                catch
                {
                    eventAggregator.SendStatusInfo(this, "Mounting additional disk...✖\n");
                }

                try
                {
                    eventAggregator.SendStatusInfo(this, "Unmounting addtional disk...✔\n");
                    UnmountDisk();
                }
                catch
                {
                    eventAggregator.SendStatusInfo(this, "Unmounting addtional disk...✖\n");
                }
            }

            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }

        private bool IsInstalled()
        {
            string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey(name))
            {
                foreach (string subKeyName in registryKey1.GetSubKeyNames())
                {
                    using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName))
                    {
                        if (registryKey2.GetValue("DisplayName") != null)
                        {
                            if (registryKey2.GetValue("DisplayName").ToString().Contains("TrueCrypt"))
                            {
                                application = registryKey2.GetValue("DisplayName").ToString();
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void ClearPrevious(string path)
        {
            try
            {
                Directory.Delete(path, true);
                eventAggregator.SendStatusInfo(this, "Removing previous files...✔\n");
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, "Removing previous files...✖\n");
            }
        }

        private void CreateFolder()
        {
            try
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\True Crypt");
                eventAggregator.SendStatusInfo(this, "Folder created...✔\n");
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, "Folder created...✖\n");
            }
        }

        private bool LoadDisk()
        {
            try
            {
                var encoding = new ASCIIEncoding();
                var bytes = encoding.GetBytes(Properties.Resources.TrueCryptDisk);

                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\True Crypt\\TruCrypDisk", bytes);
            }
            catch
            {
                eventAggregator.SendStatusInfo(this, "There is something wrong with encrypted disk");
            }

            return File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\True Crypt\\TruCrypDisk");
        }

        private bool MountDisk()
        {
            if (!CheckVolume(volume))
                volume = 'K';

            string broken = null;
            var str1 = "/q /v " + FixPath(broken) + " /p marcin11 /l" + volume;
            var str2 = "/q /v \"c:\\Users\\mka11_000\\Desktop\\ForensicBot\\True Crypt\\TruCrypDisk\" /p marcin11 /l" + volume;
            var process = ProcessService.CreateProcess("C:\\Program Files\\TrueCrypt\\TrueCrypt.exe", str1, redirectOutput: true);

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\True Crypt\\TruCrypDisk"))
                broken = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\True Crypt\\TruCrypDisk";

            process.Start();
            process.WaitForExit();
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return !CheckVolume(volume);
        }

        private bool UnmountDisk()
        {
            var process = ProcessService.CreateProcess("C:\\Program Files\\TrueCrypt\\TrueCrypt.exe", "/q /d" + volume, redirectOutput: true);
            process.Start();
            process.WaitForExit();
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return true;
        }

        private bool CheckVolume(char v)
        {
            return !Directory.Exists(Path.GetPathRoot(v.ToString() + ":\\"));
        }

        private void OpenFile()
        {
            ProcessService.CreateProcess("explorer", volume + ":", redirectOutput: true).Start();
        }

        private void ReadDrive()
        {
            ProcessService.CreateProcess("notepad", volume + ":\\\\hello napier.txt", redirectOutput: true).Start();
        }

        private void ExecuteCommandSync(object command)
        {
            try
            {
                var process = ProcessService.CreateCmdProcess(command.ToString(), false, redirectOutput: true);
                process.Start();

                Console.WriteLine(process.StandardOutput.ReadToEnd());
            }
            catch(Exception e)
            {
                eventAggregator.SendStatusInfo(this, $"An unexpected error ocurred: \"{e.Message}\"");
            }
        }

        private string FixPath(string broken)
        {
            string str = "\"";
            char[] charArray = broken.ToCharArray();

            for (int index = 0; index < charArray.Length; ++index)
            {
                str += charArray[index].ToString();
            }

            return str + "\"";
        }
    }
}
