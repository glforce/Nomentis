using System;
using Server.Misc;

namespace Server.Custom
{
	public class CustomRaces
	{
		private static readonly int HumanIndex = 3;
		private static readonly int VampireIndex = 4;
		private static readonly int LycanIndex = 5;

		public static Race Human => Race.Races[HumanIndex];
		public static Race Vampire => Race.Races[VampireIndex];
		public static Race Lycan => Race.Races[LycanIndex];

		public static void Configure()
		{
			RaceDefinitions.RegisterRace(new HumanRace());
			RaceDefinitions.RegisterRace(new VampireRace());
			RaceDefinitions.RegisterRace(new LycanRace());
		}

		private class CustomRace : Race
		{
			public CustomRace(int Index, string Name, string NamePlural) : base(Index, Index, Name, NamePlural, 400, 401, 402, 403)
			{
			}

			public override int ClipFaceHue(int hue)
			{
				return ClipSkinHue(hue);
			}

			public override int ClipHairHue(int hue)
			{
				throw new NotImplementedException();
			}

			public override int ClipSkinHue(int hue)
			{
				return Math.Min(1002, Math.Max(hue, 1058));
			}

			public override int RandomFace(bool female)
			{
				return 15172 + Utility.Random(9);
			}

			public override int RandomFaceHue()
			{
				return RandomSkinHue();
			}

			public override int RandomFacialHair(bool female)
			{
				if (female)
				{
					return 0;
				}

				var rand = Utility.Random(7);

				return (rand < 4 ? 0x203E : 0x2047) + rand;
			}

			public override int RandomHair(bool female)
			{
				switch (Utility.Random(9))
				{
					case 0:
						return 0x203B;  //Short
					case 1:
						return 0x203C;  //Long
					case 2:
						return 0x203D;  //Pony Tail
					case 3:
						return 0x2044;  //Mohawk
					case 4:
						return 0x2045;  //Pageboy
					case 5:
						return 0x2047;  //Afro
					case 6:
						return 0x2049;  //Pig tails
					case 7:
						return 0x204A;  //Krisna
					default:
						return female ? 0x2046 : 0x2048;    //Buns or Receeding Hair
				}
			}

			public override int RandomHairHue()
			{
				return Utility.Random(1102, 48);
			}

			public override int RandomSkinHue()
			{
				return Utility.Random(1002, 57) | 0x8000;
			}

			public override bool ValidateEquipment(Item item)
			{
				return true;
			}

			public override bool ValidateFace(bool female, int itemID)
			{
				return itemID >= 0x3B44 && itemID <= 0x3B4D;
			}

			public override bool ValidateFacialHair(bool female, int itemID)
			{
				if (female)
				{
					return false;
				}

				return itemID == 0
					|| itemID >= 0x203E && itemID <= 0x2041
					|| itemID >= 0x204B && itemID <= 0x204D;
			}

			public override bool ValidateHair(bool female, int itemID)
			{
				if (female)
				{
					if (itemID == 0x2048)
					{
						return false;
					}
				}
				else
				{
					if (itemID == 0x2046)
					{
						return false;
					}
				}

				return itemID == 0
					|| itemID >= 0x203B && itemID <= 0x203D
					|| itemID >= 0x2044 && itemID <= 0x204A;
			}
		}

		private class HumanRace : CustomRace
		{
			public HumanRace() : base(HumanIndex, "Humain", "Humains")
			{
			}
		}

		private class VampireRace : CustomRace
		{
			public VampireRace() : base(VampireIndex, "Vampire", "Vampires")
			{
			}
		}

		private class LycanRace : CustomRace
		{
			public LycanRace() : base(LycanIndex, "Lycan", "Lycans")
			{
			}
		}
	}
}
