using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowAuthorizationCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            var options = new ModalDialogOptions("/sitecore modules/shell/Smartcat/Authorization/AuthorizationModal.aspx")
            {
                Resizable = false,
                Width = "500",
                Height = "300",
                Header = "Connect workspace",
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