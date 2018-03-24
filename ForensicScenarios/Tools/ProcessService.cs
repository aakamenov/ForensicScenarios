using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ForensicScenarios.Tools
{
    public static class ProcessService
    {
        private static List<Process> processes;

        static ProcessService()
        {
            processes = new List<Process>();
        }

        public static Process CreateCmdProcess(string path, bool createWindow = true, bool redirectInput = false, bool redirectOutput = false)
        {
            var processStartInfo = new ProcessStartInfo("cmd", path + "/c ")
            { 
                RedirectStandardOutput = redirectOutput,
                RedirectStandardInput = redirectInput,
                UseShellExecute = false,
                CreateNoWindow = !createWindow,
            };

            var process = new Process()
            { 
                EnableRaisingEvents = true,
                StartInfo = processStartInfo
            };

            processes.Add(process);
            process.Exited += Process_Exited;

            return process;
        }

        public static Process CreateProcess(string path, string arguments)
        {
            var processStartInfo = new ProcessStartInfo(path, arguments)
            {
                UseShellExecute = false
            };

            var process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = processStartInfo                
            };

            processes.Add(process);
            process.Exited += Process_Exited;

            return process;
        }

        public static void KillAll()
        {
            foreach (var process in processes)
            {
                if (process is null)
                    continue;

                process.Exited -= Process_Exited;

                try
                {
                    if (!process.HasExited)
                        process.Kill();
                }
                catch(Exception) { }
            }

            processes.Clear();
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            processes.Remove(sender as Process);
        }
    }
}
