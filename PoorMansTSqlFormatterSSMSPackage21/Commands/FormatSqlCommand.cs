using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using PoorMansTSqlFormatterSSMSLib;
using Task = System.Threading.Tasks.Task;

namespace PoorMansTSqlFormatterSSMSPackage.Commands
{
    internal sealed class FormatSqlCommand
    {
        public const int CommandId = 0x100;
        public static readonly Guid CommandSet = PackageGuids.guidPoorMansTSqlFormatterSSMSVSPackageCmdSet;
        private readonly AsyncPackage package;

        private FormatSqlCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += this.QueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            new FormatSqlCommand(package, commandService);
        }

        private void QueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var command = sender as OleMenuCommand;
            DTE2 dte = (DTE2)package.GetServiceAsync(typeof(DTE)).Result;
            command.Enabled = dte?.ActiveDocument != null && !dte.ActiveDocument.ReadOnly;
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            DTE2 dte = (DTE2)package.GetServiceAsync(typeof(DTE)).Result;
            var helper = new GenericVSHelper(true, null, null, null); 
            helper.FormatSqlInTextDoc(dte);
        }
    }
}