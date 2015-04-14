using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.ComponentModel;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RestSharp;


namespace PhraseApp.PhraseApp
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidPhraseAppPkgString)]
    [ProvideOptionPage(typeof(OptionPageGrid),
    "PhraseApp", "PhraseApp", 0, 0, true)]
    public sealed class PhraseAppPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public PhraseAppPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

           
            if ( null != mcs )
            {
                // Upload Button
                CommandID uploadID = new CommandID(GuidList.guidPhraseAppCmdSet, (int)PkgCmdIDList.cmdidPhraseAppUpload);
                MenuCommand uploadItem = new MenuCommand(DoUpload, uploadID );
                mcs.AddCommand( uploadItem );

                // Upload Button
                CommandID downloadID = new CommandID(GuidList.guidPhraseAppCmdSet, (int)PkgCmdIDList.cmdidPhraseAppDownload);
                MenuCommand downloadItem = new MenuCommand(DoDownload, downloadID);
                mcs.AddCommand(downloadItem);
            }

           


        }
        #endregion

        // Get the Directory for the current Visual Studio Solution
        String getSolutionDir() {
            try {
                DTE dte = (DTE)GetService(typeof(DTE));
               return System.IO.Path.GetDirectoryName(dte.Solution.FullName);
            } catch (SystemException ex) {
                return (ex.ToString());
            }
        }

        // Show a Message Box
        void popMessage(String msg) {
            MessageBox.Show(msg);
        }
       
        // Upload Action
        private void DoUpload(object sender, EventArgs e) {
            OptionPageGrid options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            PhraseClient phraseClient = new PhraseClient(options.projectID, options.AuthToken, getSolutionDir());
            phraseClient.uploadAll();
            popMessage("Uploaded all locales");
        }

        // Download Action
        private void DoDownload(object sender, EventArgs e){
           OptionPageGrid options = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
           PhraseClient phraseClient = new PhraseClient(options.projectID, options.AuthToken, getSolutionDir());
           phraseClient.downloadAll();
           popMessage("Downloaded all locales");
        }



    }


    // Options Page for PhraseApp Plugin
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionPageGrid : DialogPage {
        private String projectIDValue;
        private String AuthTokenValue;

        [Category("PhraseApp Project Settings")]
        [DisplayName("Project ID")]
        [Description("Project ID")]
        public String projectID {
            get { return projectIDValue; }
            set { projectIDValue = value; }
        }

        [Category("PhraseApp Project Settings")]
        [DisplayName("API Auth Token")]
        [Description("API Auth Token")]
        public String AuthToken {
            get { return AuthTokenValue; }
            set { AuthTokenValue = value; }
        }


    }

    public class PhraseClient {

        String baseURL = "https://api.phraseapp.com/v2/";
        String projectID;
        String authToken;
        String solutionDir;

        // PhraseApp Client
        public PhraseClient(string projectID, string authToken, string solutionDir) {
            this.projectID = projectID;
            this.authToken = authToken;
            this.solutionDir = solutionDir;
        }

        // Build RestClient with PraseApp API BaseURL
        RestClient buildClient() {
            var client = new RestClient(this.baseURL);
            return client;
        }

        // Build Request with Authorization token
        RestRequest buildRequest(String endpoint, Method method) {
            var request = new RestRequest(endpoint, method);
            request.AddHeader("Authorization", "token " + this.authToken);
            return request;
        }

        // Find Strings directory
        String findStringsDir() {
            var allFiles = Directory.GetFiles(this.solutionDir, "Resources.resw", SearchOption.AllDirectories);
            foreach (String f in allFiles) {
                if (isLocaleFile(f)) {
                    return Directory.GetParent(Directory.GetParent(f).ToString()).ToString();
                }
            }
            return "";
        }

        // Check if given file is a locale file
        bool isLocaleFile(string filePath) {
            return (filePath.Contains("\\Strings\\") && filePath.Contains("Resources.resw"));
        }

        // Get List of Locales (lc-CC + locale_id) from PhraseApp
        List<KeyValuePair<string, string>> listLocales() {
            List<KeyValuePair<string, string>> localesList = new List<KeyValuePair<string, string>>();
            var client = buildClient();
            var request = buildRequest("projects/" + this.projectID + "/locales", Method.GET);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            dynamic locales = JsonConvert.DeserializeObject(content);
            foreach (var locale in locales) {
                localesList.Add(new KeyValuePair<string, string>((String)locale.name, (String)locale.id));
            }

            return localesList;
        }

        // Find localeID for a given locale name
        String findLocaleID(String localeName) {
            List<KeyValuePair<string, string>> locales = listLocales();
            foreach (var locale in locales) {
                if (locale.Key.Equals(localeName)) {
                    return locale.Value;
                }
            }
            return "";
        }


        // Upload a locale file
        Boolean uploadLocale(String localeName, String filePath) {
            var client = buildClient();
            var request = buildRequest("projects/" + this.projectID + "/uploads", Method.POST);
            request.AddParameter("locale_id", findLocaleID(localeName));
            request.AddParameter("format", "windows8_resource");
            request.AddFile("file", filePath);

            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

            return true;
        }


        // Download a locale
        Boolean downloadLocale(String localeName) {
            var client = buildClient();
            var request = buildRequest("projects/" + this.projectID + "/locales/" + findLocaleID(localeName) + "/download", Method.GET);
            request.AddParameter("format", "windows8_resource");

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            String savePath = findStringsDir() + "\\" + localeName + "\\" + "Resources.resw";
            File.WriteAllText(savePath, content, Encoding.UTF8);
            return true;
        }

        // Search Directory for all locale files and upload them
        public void uploadAll() {
            int numLocales = 0;
            int uploadErrors = 0;
            var allFiles = Directory.GetFiles(this.solutionDir, "Resources.resw", SearchOption.AllDirectories);
            foreach (String f in allFiles) {
                if (isLocaleFile(f)) {
                    numLocales++;
                    if (!uploadLocale(Directory.GetParent(f).Name, f)) {
                        uploadErrors++;
                    }
                }
            }
        }

        // Download all locales from PhraseApp
        public void downloadAll() {
            int numLocales = 0;
            int downloadErrors = 0;
            List<KeyValuePair<string, string>> locales = listLocales();
            foreach (var locale in locales) {
                numLocales++;
                if (!downloadLocale(locale.Key)) {
                    downloadErrors++;
                }
            }
        }

    }

}
