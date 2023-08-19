using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Scripts.Commands
{
    public class FE
    {
        public static void Initialize()
        {
           
			CommandSystem.Register("Fe", AccessLevel.Player, new CommandEventHandler(Fe_OnCommand));
		}

		[Usage("Fe")]
			[Description("Permet de voir le nombre de Fioles obtenues")]
			public static void Fe_OnCommand(CommandEventArgs e)
			{
			CustomPlayerMobile from = (CustomPlayerMobile)e.Mobile;
			
				from.SendMessage("Vous avez " + from.FE + " Fioles d'Expériences.");

			}
    }
}


