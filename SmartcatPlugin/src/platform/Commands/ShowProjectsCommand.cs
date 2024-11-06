using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowProjectsCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            var options = new ModalDialogOptions("/sitecore modules/shell/Smartcat/Projects/ProjectListModal.aspx")
            {
                Resizable = false,
                Width = "900",
                Height = "520",
                Header = "Projects",
                Response = false,
                Maximizable = false
            };

            SheerResponse.ShowModalDialog(options);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}