using Server.Network;
using Server.Custom.Evolution;
using System.Linq;
using Server.Custom.Mobiles;

namespace Server.Gumps
{
	public class ExperienceTableGump : CustomBaseGump
	{
		private CustomPlayerMobile From;
		private int Page;

		public ExperienceTableGump(CustomPlayerMobile From, int Page = 0)
			: base("Table des niveaux", 560, 622, true)
		{
			From.CloseGump(typeof(ExperienceTableGump));

			this.From = From;
			this.Page = Page;

			int x = XBase;
			int y = YBase;

			AddHtmlTexteColored(x + 10, y + 20, 300, "Niveau", "#ffffff");
			AddHtmlTexteColored(x + 200, y + 20, 300, "FE requises", "#ffffff");
			AddHtmlTexteColored(x + 400, y + 20, 300, "Skill Cap", "#ffffff");

			int PlayerLevel = ExperienceSystem.GetLevel(From);

			int Line = 0;

			ExperienceSystem.LevelSpecs.Skip(Page * 28 - 1)
				.Take(28)
				.ToList()
				.ForEach(Spec =>
				{
					int CurrentLevel = Line + 1;

					string Color = CurrentLevel == PlayerLevel ? "#ffcc00" : "#ffffff";

					AddHtmlTexteColored(x + 10, y + 40 + Line * 20, 300, CurrentLevel.ToString(), Color);
					AddHtmlTexteColored(x + 200, y + 40 + Line * 20, 300, Spec.RequiredExperience.ToString(), Color);
					AddHtmlTexteColored(x + 400, y + 40 + Line * 20, 300, Spec.SkillCap.ToString(), Color);
					Line++;
				});

			if (Page != 0)
			{
				AddButton(x + 5, y + 610, 1, 4506);
			}

			if (ExperienceSystem.LevelSpecs.Count > (Page + 1) * 28)
			{
				AddButton(x + 535, y + 610, 2, 4502);
			}
		}

		public override void OnResponse(NetState Sender, RelayInfo Info)
		{
			switch (Info.ButtonID)
			{
				case 1:
					Sender.Mobile.SendGump(new ExperienceTableGump(From, Page - 1));
					break;
				case 2:
					Sender.Mobile.SendGump(new ExperienceTableGump(From, Page + 1));
					break;
			}
		}
	}
}
