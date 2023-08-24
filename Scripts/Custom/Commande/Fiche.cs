using System;
using Server;
using Server.Commands;
using Server.Custom.Mobiles;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Scripts.Commands
{
	public class Fiche
    {
        public static void Initialize()
        {
            CommandSystem.Register("Fiche", AccessLevel.Player, OpenSheet);		
            CommandSystem.Register("TargetFiche", AccessLevel.GameMaster, OpenTargetSheet);		
		}

        [Usage("Fiche")]
        [Description("Permet d'ouvrir le menu .Fiche")]
        private static void OpenSheet(CommandEventArgs e)
        {
			OnSelectTarget(e.Mobile, e.Mobile);
		}

		[Usage("TargetFiche")]
		[Description("Permet d'ouvrir le menu .Fiche de la cible")]
		private static void OpenTargetSheet(CommandEventArgs e)
		{
			e.Mobile.BeginTarget(12, false, TargetFlags.None, OnSelectTarget);
		}

		private static void OnSelectTarget(Mobile From, object Target)
		{
			if (Target is CustomPlayerMobile)
			{
				From.SendGump(new FicheGump((CustomPlayerMobile)From, (CustomPlayerMobile)Target));
			}
		}
	}
}


