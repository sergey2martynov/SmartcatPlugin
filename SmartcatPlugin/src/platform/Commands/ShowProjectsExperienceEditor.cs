using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartcatPlugin.Commands
{
    public class ShowProjectsExperienceEditor : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.Start(this, "Run", context.Parameters);
        }

        protected static void Run(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                SheerResponse.ShowModalDialog("/sitecore modules/shell/Smartcat/Projects/ProjectListModal.aspx",
                    "1000", "600", "Basket", false);
                args.WaitForPostBack();
            }
        }
    }
}