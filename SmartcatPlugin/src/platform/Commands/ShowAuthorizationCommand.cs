using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowAuthorizationCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            SheerResponse.ShowModalDialog("/sitecore modules/shell/Smartcat/Authorization/AuthorizationModal.aspx",
                "600", "300", "Authorization", false);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}