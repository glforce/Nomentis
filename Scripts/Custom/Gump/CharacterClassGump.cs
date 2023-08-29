using System;
using Server.Network;
using System.Collections.Generic;
using System.Linq;
using Server.Custom.Mobiles;
using Server.Custom.Class;

namespace Server.Gumps
{
	public class CharacterClassGump : CustomBaseGump
	{
		private CustomPlayerMobile From;
		private CustomPlayerMobile Target;
		private List<int> ClassIds;
		private int CurrentClassIndex;
		private MainCharacterClass CurrentClass;

		public CharacterClassGump(CustomPlayerMobile From, CustomPlayerMobile Target, List<int> ClassIds, int CurrentClassIndex)
		: base("Classes", 560, 622, false)
		{
			From.CloseGump(typeof(CharacterClassGump));

			this.From = From;
			this.Target = Target;
			this.ClassIds = ClassIds;
			this.CurrentClassIndex = CurrentClassIndex;

			int x = XBase;
			int y = YBase;

			From.InvalidateProperties();
			Target.InvalidateProperties();

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			CurrentClass = CharacterClasses.GetMainCharacterClass(Target.Race, ClassIds[CurrentClassIndex]);

			AddSection(x - 10, y, 300, 240, "Description");

			int yLine = 2;

			AddHtmlText(x + 10, y + yLine * 20, 100, "Nom:");
			AddHtmlText(x + 150, y + yLine * 20, 150, CurrentClass.Name);
			yLine++;

			AddHtmlText(x + 10, y + yLine * 20, 125, "Niveau de classe:");
			AddHtmlText(x + 150, y + yLine * 20, 150, CurrentClass.Level.ToString());
			yLine++;

			AddHtmlText(x + 10, y + yLine * 20, 125, "Niveau Requis:");
			AddHtmlText(x + 150, y + yLine * 20, 150, CharacterClasses.GetLevelRequirement(CurrentClass.Level).ToString());
			yLine++;

			AddHtmlText(x + 10, y + yLine * 20, 100, "Armure:");
			AddHtmlText(x + 150, y + yLine * 20, 150, string.Join(", ", CurrentClass.AllowedArmorMaterialTypes));
			yLine++;

			AddButtonHtml(x + 10, y + yLine * 20, 4, "Index des classes", "#FFFFFF");

			AddSection(x + 295, y, 300, 240, "Évolutions");

			yLine = 2;

			foreach (int EvolutionID in CurrentClass.Evolutions)
			{
				MainCharacterClass EvolutionClass = CharacterClasses.GetMainCharacterClass(Target.Race,EvolutionID);

				AddButtonHtml(x + 315, y + yLine * 20, 1000 + EvolutionClass.ID, EvolutionClass.Name, "#FFFFFF");

				yLine++;
			}

			string SkillCaps = string.Join("\n",
				CurrentClass.SkillCaps
				.Select(SkillCap => string.Format("{0] - {1}", SkillCap.Key.ToString(), SkillCap.Value))
				.ToList());

			AddSection(x - 10, y + 245, 605, 300, "Compétences", SkillCaps);

			AddBackground(x - 10, y + 550, 605, 55, 9270);

			if (Target.CanEvolveTo(CurrentClass))
			{
				AddButtonHtml(x + 150, y + 568, 3, $"Je veux devenir un {CurrentClass.Name}.", "#FFFFFF");
			}

			AddSection(x - 10, y + 610, 605, 50, CurrentClass.Name);

			if (CurrentClassIndex > 0)
			{
				AddButton(x, y + 610, 1, 4506);
			}
			if (CurrentClassIndex + 1 < ClassIds.Count)
			{
				AddButton(x + 540, y + 610, 2, 4502);
			}
		}

		public override void OnResponse(NetState Sender, RelayInfo Info)
		{
			if (Info.ButtonID == 1)
			{
				From.SendGump(new CharacterClassGump(From, Target, ClassIds, Math.Max(CurrentClassIndex - 1, 0)));
			}
			else if (Info.ButtonID == 2)
			{
				From.SendGump(new CharacterClassGump(From, Target, ClassIds, Math.Min(CurrentClassIndex + 1, ClassIds.Count - 1)));
			}
			else if (Info.ButtonID == 3)
			{
				if (Target.CanEvolveTo(CurrentClass))
				{
					Target.Class = CurrentClass;
					From.SendGump(new CharacterClassGump(From, Target, ClassIds, CurrentClassIndex));
				}
			}
			else if (Info.ButtonID == 4)
			{
				From.SendGump(new ClasseIndexGump(From, Target));
			}
			else if (Info.ButtonID >= 1000)
			{
				int NewClassId = Info.ButtonID - 1000;

				int NextClassIndex = CurrentClassIndex + 1;

				List<int> NewClassIds = ClassIds.Take(NextClassIndex).Append(NewClassId).ToList();

				From.SendGump(new CharacterClassGump(From, Target, NewClassIds, NextClassIndex));
			}
		}
	}
}
