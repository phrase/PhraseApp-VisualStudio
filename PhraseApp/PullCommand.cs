//------------------------------------------------------------------------------
// <copyright file="PullCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Windows.Forms;

namespace PhraseApp
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class PullCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("fccdbb59-737e-4dbc-a15b-f28e4118ed32");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="PullCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private PullCommand(Package package)
        {
            this.package = package ?? throw new ArgumentNullException("package");

            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += menuItemBefore_QueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        void menuItemBefore_QueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuItem = sender as OleMenuCommand;
            menuItem.Enabled = true;
            menuItem.Visible = true;
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static PullCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new PullCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            String solutionName = dte.Solution.FullName;
            if (dte.Solution.FullName == "")
            {
                MessageBox.Show("It looks like no project is open. Please open a project to pull translation files");
                return;
            }

            try
            {
                String solutionDir = System.IO.Path.GetDirectoryName(solutionName);
                var opts = package.GetDialogPage(typeof(CliToolOptions)) as CliToolOptions;

                Cli cli = new Cli(opts.CliToolPath, solutionDir);
                cli.Pull();
            }
            catch (Exception exception)
            {
                if (exception.Source != null)
                {
                    Console.WriteLine("Error pulling translations: {0}", exception.Source);
                }
            }
        }
    }
}
