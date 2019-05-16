using System;
using System.Diagnostics;

namespace Graphs.Utility
{
    public static class ImageCreator
    {
        public static string CreateImage(string inputName, string outputName, int size)
        {
            string command = 
                $"dot -Tpng -Gcenter=1 -Gsize=1,1\\! -Gdpi={size} -Gratio=auto -o{outputName} {inputName}";
            string s = ExecuteCommand(command);
            return s;
        }

        private static string ExecuteCommand(string command)
        {
            command = command.Replace("\"", "\"\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            return process.StandardOutput.ReadToEnd();
        }
    }
}