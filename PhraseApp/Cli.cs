using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Text.RegularExpressions;

namespace PhraseApp
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
            return this.solutionDir + "/" + Path.GetFileName(this.solutionDir) + "/.phraseapp.yml";
        }

        public Boolean ConfigFileExists()
        {
            return File.Exists(this.ConfigFilePath());
        }

        public void Info()
        {
            this.Exec("info");
        }

        public void Push()
        {
            this.Exec("push");
        }

        public void Pull()
        {
            this.Exec("pull");
        }

        public String Exec(String action)
        {

            if(!this.ConfigFileExists())
            {
                MessageBox.Show("Could not find a .phraseapp.yml configuration file");
                return "";
            }

            String dir = Path.GetFileName(this.solutionDir);
            Process process = new Process();
            process.StartInfo.FileName = this.CliToolPath;
            process.StartInfo.Arguments = action;
            process.StartInfo.WorkingDirectory = this.solutionDir+"/"+dir;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.EnvironmentVariables["PHRASEAPP_USER_AGENT"] = "VisualStudio";
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if(err != ""){
                output = err;
            }

            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var paneGuid = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            IVsOutputWindowPane pane;
            outputWindow.CreatePane(paneGuid, "PhraseApp", 1, 0);
            outputWindow.GetPane(paneGuid, out pane);
            pane.OutputString(output);

            return output;

        }

    }
}
