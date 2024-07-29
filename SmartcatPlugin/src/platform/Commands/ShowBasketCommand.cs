using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowBasketCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            SheerResponse.ShowModalDialog(
                "/sitecore modules/shell/Basket/BasketModal.aspx", "900", "400", "Basket", false);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}