using Sitecore.Shell.Framework.Commands;

namespace SmartcatPlugin.Commands
{
    public class ShowAddItemCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(
                "/sitecore modules/shell/AddItem/AddItemModal.aspx", "600", "400", "AddItem", false);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}
