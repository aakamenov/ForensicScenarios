using System;
using System.Diagnostics;
using System.IO;
using Caliburn.Micro;
using System.Threading.Tasks;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class Shellbag : PropertyChangedBase, IScenario
    {
        public string Name => "Shellbag";

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

        public Shellbag(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "This scenario will create randomly named folders and files. The files will be copied, moved and in some cases deleted. This will cause changes to be made to the ShellBags information in the registry.";
        }

        public void Run()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\Shellbag";

            string str1 = "a" + DateTime.Now.ToString("mmssfff");
            string str2 = "b" + DateTime.Now.ToString("mmssfff");
            string str3 = "c" + DateTime.Now.ToString("mmssfff");

            string s1 = "A" + DateTime.Now.ToString("mmssfff");
            string s2 = "B" + DateTime.Now.ToString("mmssfff");
            string s3 = "C" + DateTime.Now.ToString("mmssfff");

            ClrPrevious(path);

            CreateFolder(path + "\\", s1);
            CreateFolder(path + "\\", s2);            
            CreateFolder(path + "\\", s3);

            CreateFile("this is text for file a in folder A", "Shellbag\\" + s1 + "\\", str1 + ".txt");
            CreateFile("this is text for file a in folder B", "Shellbag\\" + s2 + "\\", str2 + ".txt");
            CreateFile("this is text for file a in folder C", "Shellbag\\" + s3 + "\\", str3 + ".txt");

            CopyFile("\\" + s1 + "\\" + str1 + ".txt", "\\" + s3 + "\\" + str1 + ".txt");
            MoveFile("\\" + s2 + "\\" + str2 + ".txt", "\\" + s3 + "\\" + str2 + ".txt");
            DeleteFile("\\" + s1 + "\\", "*.txt");

            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }

        private void ClrPrevious(string path)
        {
            if (Directory.Exists(path))
            {
                var msg = string.Empty;

                try
                {
                    Directory.Delete(path, true);

                    msg = "Removing previous files...✔";
                }
                catch (Exception)
                {
                    msg = "Removing previous files...✖";
                }

                eventAggregator.SendStatusInfo(this, msg);
            }
        }

        private void DeleteFile(string f, string file)
        {
            string str = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\Shellbag";
            var msg = string.Empty;

            try
            {
                ExecuteCommandSync((object)("del  " + str + f + file));

                msg = "File deleted...✔";
            }
            catch (Exception)
            {
                msg = "File deleted...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        private void MoveFile(string f, string d)
        {
            string str = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\Shellbag";
            var msg = string.Empty;

            try
            {
                ExecuteCommandSync((object)("move  " + str + f + " " + str + d));

                msg = "File moved...✔";
            }
            catch (Exception)
            {
                msg = "File moved...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        private void CopyFile(string f, string d)
        {
            string str = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\Shellbag";
            var msg = string.Empty;

            try
            {
                ExecuteCommandSync((object)("copy  " + str + f + " " + str + d));

                msg = "File Copied...✔";
            }
            catch (Exception)
            {
                msg = "File Copied...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        private void CreateFile(string t, string p, string f)
        {
            ExecuteCommandSync((object)("echo " + t + " >" + Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ForensicBot\\" + p + f));

            var msg = "File " + f + " created...✔";
            eventAggregator.SendStatusInfo(this, msg);
        }

        private void CreateFolder(string path, string s)
        {
            var msg = string.Empty;
        
            try
            {
                Directory.CreateDirectory(path + s);
                msg = "Folder " + s + " created...✔";
            }
            catch (Exception)
            {
                msg = "Folder " + s + " created...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }

        private void ExecuteCommandSync(object command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + command);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            
            Console.WriteLine(process.StandardOutput.ReadToEnd());
        }
    }
}
