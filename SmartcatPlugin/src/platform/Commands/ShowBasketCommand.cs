using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowBasketCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            var options = new ModalDialogOptions("/sitecore modules/shell/Smartcat/Basket/BasketModal.aspx")
            {
                Resizable = false,
                Width = "800",
                Height = "540",
                Header = "Create project",
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