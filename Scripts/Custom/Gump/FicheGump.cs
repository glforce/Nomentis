using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Misc;
using System.Collections.Generic;
using Server.Custom;
using Server.Custom.Mobiles;
using Server.Custom.Evolution;

namespace Server.Gumps
{
	public class FicheGump : CustomBaseGump
	{
		private CustomPlayerMobile From;
		private CustomPlayerMobile Target;

		public FicheGump(CustomPlayerMobile From, CustomPlayerMobile Target)
			: base("Fiche du personnage", 560, 622, false)
		{
			From.CloseGump(typeof(FicheGump));

			this.From = From;
			this.Target = Target;

			int x = XBase;
			int y = YBase;

			From.InvalidateProperties();
			Target.InvalidateProperties();

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddSection(x - 10, y, 250, 180, "Informations");

			int yLine = 2;

			AddHtmlText(x + 10, y + yLine * 20, 100, "Nom:");
			AddHtmlText(x + 125, y + yLine * 20, 150, Target.Name);
			yLine++;

			AddHtmlText(x + 10, y + yLine * 20, 100, "Race:");
			AddHtmlText(x + 125, y + yLine * 20, 150, Target.Race.Name);
			yLine++;

			AddHtmlText(x + 10, y + yLine * 20, 100, "Niveau:");
			AddHtmlText(x + 125, y + yLine * 20, 150, ExperienceSystem.GetLevel(Target).ToString());
			yLine++;

			AddSection(x - 10, y + 317, 250, 135, "Expériences");

			AddHtmlText(x + 10, y + 355, 150, "Expérience:");
			AddHtmlText(x + 125, y + 355, 100, Target.Experience.ToString());

			AddHtmlText(x + 10, y + 415, 150, "Heures jouées:");
			AddHtmlText(x + 125, y + 415, 100, Math.Round(Target.Account.TotalGameTime.TotalHours, 2).ToString());

			AddHtmlText(x + 10, y + 610, 150, "Faim :");
			AddLabel(x + 130, y + 610, 150, Target.Hunger * 5 + " / 100".ToString());

			AddHtmlText(x + 10, y + 630, 150, "Soif :");
			AddLabel(x + 130, y + 630, 150, Target.Thirst * 5 + " / 100".ToString());
		}
	}
}