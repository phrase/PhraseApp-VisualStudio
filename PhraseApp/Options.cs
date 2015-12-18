using System;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using EnvDTE;
using System.IO;
using System.Windows.Forms;

namespace PhraseApp
{

    public class ConfigFileOptions : DialogPage
    {
        String accessToken = "";
        String projectId = "";

        [Category("Configuration File")]
        [DisplayName("Access Token")]
        [Description("Generate a PhraseApp API Access Token in your Account")]
        public String AccessToken
        {
            get { return accessToken; }
            set { accessToken = value; }
        }

        [Category("Configuration File")]
        [DisplayName("Project ID")]
        [Description("The ID of your PhraseApp Project")]
        public String ProjectId
        {
            get { return projectId; }
            set { projectId = value; }
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            String solutionDir = System.IO.Path.GetDirectoryName(dte.Solution.FullName);
            String dir = Path.GetFileName(solutionDir);
            String configFile = solutionDir + "/" + dir + "/.phraseapp.yml";

            if (File.Exists(configFile))
            {
                if (MessageBox.Show(".phraseapp.yml configuration already exists, overwrite?", "PhraseApp", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    WriteConfigFile(configFile);
                }
            }else
            {
                WriteConfigFile(configFile);
            }

        }

        protected void WriteConfigFile(String configFile)
        {
            System.IO.File.WriteAllText(@configFile,
                "phraseapp:\n" +
                "  access_token: " + this.AccessToken + "\n" +
                "  project_id: " + this.ProjectId + "\n" +
                "  file_format: windows8_resource\n" +
                "  push:\n" +
                "    sources:\n" +
                "    - file: ./Strings/<locale_code>/Resources.resw\n" +
                "      params:\n" +
                "        file_format: windows8_resource\n" +
                "  pull:\n" +
                "    targets:\n" +
                "    - file: ./Strings/<locale_code>/Resources.resw\n" +
                "      params:\n" +
                "        file_format: windows8_resource\n"
            );
        }

    }

    public class CliToolOtions : DialogPage
    {
        String cliToolPath = "";

        [Category("CLI Tool Settings")]
        [DisplayName("Path to CLI Tool")]
        [Description("Full Path to the PhraseApp CLI tool")]
        public String CliToolPath
        {
            get { return cliToolPath; }
            set { cliToolPath = value; }
        }
    }
}
