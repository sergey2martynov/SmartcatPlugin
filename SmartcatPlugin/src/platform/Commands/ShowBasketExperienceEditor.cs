using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowBasketExperienceEditor : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.Start(this, "Run", context.Parameters);
        }

        protected static void Run(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                SheerResponse.ShowModalDialog("/sitecore modules/shell/Smartcat/Basket/BasketModal.aspx",
                    "900", "600", "Basket", false);
                args.WaitForPostBack();
            }
        }
    }
}