using Server.Network;
using Server.Custom.Evolution;
using System.Linq;
using Server.Custom.Mobiles;
using Server.Custom.Gump;
using System.Collections.Generic;
using Server.Items;
using System;

namespace Server.Gumps
{
	public class ExperienceTableGump : CustomPlayerMobileGump
	{
		private const int BUTTON_ID_PREVIOUS = 1;
		private const int BUTTON_ID_NEXT = 2;

		private int Page;

		public ExperienceTableGump(CustomPlayerMobile From, int Page = 0)
			: base(From, 50, 50, "Table des niveaux")
		{
			this.Page = Page;

			int PlayerLevel = ExperienceSystem.GetLevel(From);

			int FirstIndex = Page * 28;
			var LevelSpecs = ExperienceSystem.LevelSpecs.Skip(FirstIndex)
				.Take(28)
				.ToList();

			Func<int, string> ColorFunction = (int LevelIndex) =>
			{
				return LevelIndex + 1 == PlayerLevel ? "#ffcc00" : "#ffffff";
			};

			var Table = BuildTable(
				new List<string> { "Niveau", "Expérience requise", "Skill cap" },
				LevelSpecs,
				(Index, Spec) =>
				{
					return new TextGumpElement((Index + FirstIndex + 1).ToString())
					.WithColor(ColorFunction(Index));
				},
				(Index, Spec) =>
				{
					return new TextGumpElement(Spec.RequiredExperience.ToString())
					.WithColor(ColorFunction(Index));
				},
				(Index, Spec) =>
				{
					return new TextGumpElement(Spec.SkillCap.ToString())
					.WithColor(ColorFunction(Index));
				}
			);

			var BottomActions = new ContainerGumpElement()
				.WithSize(GumpElement.GUMP_SIZE_FIT_PARENT, GumpElement.GUMP_SIZE_WRAP_CONTENT);

			if (Page != 0)
			{
				BottomActions.WithChild(
					new ContainerGumpElement(ContainerGumpElement.GumpContainerDisposition.Horizontal)
					.WithSize(GumpElement.GUMP_SIZE_FIT_PARENT, GumpElement.GUMP_SIZE_WRAP_CONTENT)
					.WithAlignment(ContainerGumpElement.GumpContainerAlignment.Start, ContainerGumpElement.GumpContainerAlignment.Center)
					.WithChild(new ButtonGumpElement(BUTTON_ID_PREVIOUS)
					.WithSize(50, 50)
					.WithStateID(4506))
					);
			}

			if (ExperienceSystem.LevelSpecs.Count > (Page + 1) * 28)
			{
				BottomActions.WithChild(
					new ContainerGumpElement(ContainerGumpElement.GumpContainerDisposition.Horizontal)
					.WithSize(GumpElement.GUMP_SIZE_FIT_PARENT, GumpElement.GUMP_SIZE_WRAP_CONTENT)
					.WithAlignment(ContainerGumpElement.GumpContainerAlignment.End, ContainerGumpElement.GumpContainerAlignment.Center)
					.WithChild(new ButtonGumpElement(BUTTON_ID_NEXT)
					.WithSize(50, 50)
					.WithStateID(4502))
				);
			}

			Root.WithChild(new ContainerGumpElement(ContainerGumpElement.GumpContainerDisposition.Vertical)
					.WithSize(GumpElement.GUMP_SIZE_WRAP_CONTENT, GumpElement.GUMP_SIZE_WRAP_CONTENT)
					.WithChildren(new List<GumpElement>
					{
						Table,
						BottomActions
					}));

			Build();
		}

		override protected void OnButtonClicked(int ID)
		{
			switch(ID) {
				case BUTTON_ID_PREVIOUS:
					OpenGump(From => new ExperienceTableGump(From, Page - 1));
					break;
				case BUTTON_ID_NEXT:
					OpenGump(From => new ExperienceTableGump(From, Page + 1));
					break;
			}
		}
	}
}
