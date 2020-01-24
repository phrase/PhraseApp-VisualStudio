using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Phrase
{
    class Cli
    {
        private String CliToolPath = "";
        private String solutionDir = "";

        public Cli(String CliToolPath, String solutionDir)
        {
            this.CliToolPath = CliToolPath;
            this.solutionDir = solutionDir;
        }

        public String ConfigFilePath()
        {
            return solutionDir + "/" + Path.GetFileName(solutionDir) + "/.phraseapp.yml";
        }

        public Boolean ConfigFileExists()
        {
            return File.Exists(this.ConfigFilePath());
        }

        public void Info()
        {
            Exec("info");
        }

        public void Push()
        {
            Exec("push");
        }

        public void Pull()
        {
            Exec("pull");
        }

        public String Exec(String action)
        {
            if (!File.Exists(CliToolPath))
            {
                MessageBox.Show("Phrase CLI client path not configured (see Tools > Options > Phrase)");
                return "";
            }

            if (!ConfigFileExists())
            {
                MessageBox.Show("Could not find a .phraseapp.yml configuration file");
                return "";
            }

            String dir = Path.GetFileName(solutionDir);
            Process process = new Process();
            process.StartInfo.FileName = CliToolPath;
            process.StartInfo.Arguments = action;
            process.StartInfo.WorkingDirectory = this.solutionDir + "/" + dir;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.EnvironmentVariables["PHRASEAPP_USER_AGENT"] = "VisualStudio";
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (err != "")
            {
                output = err;
            }

            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var paneGuid = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            outputWindow.CreatePane(paneGuid, "Phrase", 1, 0);
            outputWindow.GetPane(paneGuid, out IVsOutputWindowPane pane);
            pane.OutputString(output);

            return output;
        }
    }
}
