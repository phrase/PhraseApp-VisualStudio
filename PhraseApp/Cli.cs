using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PhraseApp
{
    class Cli
    {
        private String CliToolPath;
        private String solutionDir;

        public Cli(String CliToolPath, String solutionDir)
        {
            this.CliToolPath = CliToolPath;
            this.solutionDir = solutionDir;
        }

        public Boolean ConfigFileExists()
        {
            //
            return true;
        }

        public Boolean ClientValid()
        {
            return this.Info().Contains("PhraseApp");
        }

        public String Info()
        {
            return this.Exec("info");
        }

        public String Push()
        {
            return this.Exec("push");
        }

        public String Pull()
        {
            return this.Exec("pull");
        }

        public String Exec(String action)
        {
            String projectDir = this.solutionDir + "/" + Path.GetFileName(this.solutionDir);

            Process process = new Process();
            process.StartInfo.FileName = this.CliToolPath;
            process.StartInfo.Arguments = action;
            process.StartInfo.WorkingDirectory = projectDir;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return output;
        }

    }
}
