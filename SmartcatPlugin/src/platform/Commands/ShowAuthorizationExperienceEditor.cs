using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace SmartcatPlugin.Commands
{
    public class ShowAuthorizationExperienceEditor : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.Start(this, "Run", context.Parameters);
        }

        protected static void Run(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                SheerResponse.ShowModalDialog("/sitecore modules/shell/Smartcat/Authorization/AuthorizationModal.aspx",
                    "600", "300", "Authorization", false);
                args.WaitForPostBack();
            }
        }
    }
}