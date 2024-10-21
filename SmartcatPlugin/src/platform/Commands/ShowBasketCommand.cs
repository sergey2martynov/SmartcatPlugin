using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowBasketCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            SheerResponse.ShowModalDialog("/sitecore modules/shell/Smartcat/Basket/BasketModal.aspx",
                "900", "600", "Basket", false);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}