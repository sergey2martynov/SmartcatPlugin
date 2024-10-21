using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowAddItemCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            SheerResponse.ShowModalDialog(
                "/sitecore modules/shell/Smartcat/AddItem/AddItemModal.aspx", "900", "600", "ShowSmartcatModalPipeline", false);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}
