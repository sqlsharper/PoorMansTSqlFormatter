using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using PoorMansTSqlFormatterSSMSLib;
using Task = System.Threading.Tasks.Task;

namespace PoorMansTSqlFormatterSSMSPackage.Commands
{
    internal sealed class SqlOptionsCommand
    {
        public const int CommandId = 0x101;
        public static readonly Guid CommandSet = PackageGuids.guidPoorMansTSqlFormatterSSMSVSPackageCmdSet;
        private readonly AsyncPackage package;

        private SqlOptionsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            new SqlOptionsCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var helper = new GenericVSHelper(true, null, null, null);
            helper.GetUpdatedFormattingOptionsFromUser();
        }
    }
}