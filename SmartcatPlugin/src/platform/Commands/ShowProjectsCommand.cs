using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowProjectsCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            SheerResponse.ShowModalDialog(
                "/sitecore modules/shell/Smartcat/Projects/ProjectListModal.aspx", "1000", "600", "Basket", false);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}