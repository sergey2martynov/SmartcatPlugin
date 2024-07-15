﻿using Sitecore.Shell.Framework.Commands;

namespace SmartcatPlugin.Commands
{
    public class ShowBasketCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(
                "/sitecore modules/shell/Basket/BasketModal.aspx", "600", "400", "Basket", false);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CommandState.Enabled;
        }
    }
}