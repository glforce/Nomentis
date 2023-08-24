using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Accounting;
using System.Linq;
using Server.Custom.Mobiles;

namespace Server.Gumps
{
	public class CharactersExperienceGump : CustomBaseGump
	{
		private PlayerMobile From;
		private int Page;
		List<CustomPlayerMobile> PlayerCharacters = new List<CustomPlayerMobile>();

		public CharactersExperienceGump(PlayerMobile From) : this(From, FetchPlayerCharacters(), 0)
		{
		}

		private static List<CustomPlayerMobile> FetchPlayerCharacters()
		{
			return Accounts.GetAccounts()
				.SelectMany(Account =>
				{
					List<CustomPlayerMobile> PlayerCharacters = new List<CustomPlayerMobile>(Account.Length);
					for (int i = 0; i < Account.Length; i++)
					{
						if (Account[i] != null && Account.AccessLevel == AccessLevel.Player)
						{
							if (Account[i] is CustomPlayerMobile Mobile)
							{
								PlayerCharacters.Add(Mobile);
							}
						}
					}
					return PlayerCharacters;
				})
				.OrderByDescending(PlayerCharacter => PlayerCharacter.Experience)
				.ToList();
		}

		private CharactersExperienceGump(PlayerMobile From, List<CustomPlayerMobile> PlayerCharacters, int Page)
			: base("Fe", 560, 622, true)
		{
			From.CloseGump(typeof(CharactersExperienceGump));

			this.From = From;
			this.Page = Page;

			int x = XBase;
			int y = YBase;

			this.PlayerCharacters = PlayerCharacters;

			AddHtmlTexteColored(x + 10, y + 20 * 20, 300, "Personnage", "#ffffff");
			AddHtmlTexteColored(x + 300, y + 20 * 20, 300, "Experience", "#ffffff");

			int Line = 0;

			PlayerCharacters.Skip(Page * 28 - 1)
				.ToList()
				.ForEach(PlayerCharacter =>
				{
					AddHtmlTexteColored(x + 10, y + 40 + Line * 20, 300, PlayerCharacter.Name, "#ffffff");
					AddHtmlTexteColored(x + 300, y + 40 + Line * 20, 300, PlayerCharacter.Experience.ToString(), "#ffffff");
					Line++;
				});

			if (Page != 0)
			{
				AddButton(x + 5, y + 610, 1, 4506);
			}

			if (PlayerCharacters.Count > (Page + 1) * 28)
			{
				AddButton(x + 535, y + 610, 2, 4502);
			}
		}

		public override void OnResponse(NetState Sender, RelayInfo Info)
		{
			switch (Info.ButtonID)
			{
				case 1:
					Sender.Mobile.SendGump(new CharactersExperienceGump(From, PlayerCharacters, Page - 1));
					break;

				case 2:
					Sender.Mobile.SendGump(new CharactersExperienceGump(From, PlayerCharacters, Page + 1));
					break;
			}
		}
	}
}
