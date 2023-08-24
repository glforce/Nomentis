using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Items;
using System.Xml.Linq;
using Server.Commands;
using Server.Gumps;
using Server.Custom.Mobiles;

namespace Server.Custom.Class
{
	public class CharacterClasses
	{
		private static readonly string ConfigFilePath = Path.Combine("Config", "Classes.xml");

		public static Dictionary<int, MainCharacterClass> MainCharacterClasses = new Dictionary<int, MainCharacterClass>();
		public static Dictionary<int, CharacterClass> JobCharacterClasses = new Dictionary<int, CharacterClass>();

		private static List<int> Requirements = new List<int>();

		public static void Initialize()
		{
			LoadClasses();
			LoadRequirements();

			CommandSystem.Register("Classe", AccessLevel.Player, OpenCharacterClassGump);
		}

		private static void LoadClasses()
		{
			if (MainCharacterClasses.Count > 0 || JobCharacterClasses.Count > 0)
			{
				return;
			}

			XDocument Document = XDocument.Load(ConfigFilePath);

			MainCharacterClasses = ReadMainClasses(Document.Root.Element("main"), 0)
				.ToDictionary(MainCharacterClass => MainCharacterClass.ID);

			JobCharacterClasses = Document.Root.Element("job")
				.Element("classes")
				.Elements("class")
				.Select(Node => ReadClassNode(Node, 0))
				.ToDictionary(CharacterClass => CharacterClass.ID);
		}

		private static List<MainCharacterClass> ReadMainClasses(XElement Node, int Depth)
		{
			return Node.Element("classes")
				.Elements("class")
				.SelectMany(ClassNode =>
				{
					CharacterClass Base = ReadClassNode(ClassNode, Depth);

					List<ArmorMaterialType> AllowedArmorMaterialTypes = ClassNode.Element("armors")
					.Elements("armor")
					.Select(ArmorNode => (ArmorMaterialType)Enum.Parse(typeof(ArmorMaterialType), ArmorNode.Value))
					.ToList();

					List<MainCharacterClass> Classes = ReadMainClasses(ClassNode, Depth + 1);

					MainCharacterClass MainClass = new MainCharacterClass(
						Base.ID,
						Base.Name,
						Base.SkillCaps,
						Base.Level,
						Classes.Select(Class => Class.ID).ToList(),
						Base.Hidden,
						AllowedArmorMaterialTypes);

					Classes.Add(MainClass);

					return Classes;
				})
				.ToList();
		}

		private static CharacterClass ReadClassNode(XElement Node, int Depth)
		{
			int ID = int.Parse(Node.Attribute("id")?.Value);
			string Name = Node.Attribute("name")?.Value;

			Dictionary<SkillName, double> SkillCaps = Node.Element("skills")
				.Elements("skill")
				.Select(SkillCap =>
				{
					SkillName SkillName = (SkillName)Enum.Parse(typeof(SkillName), SkillCap.Attribute("name")?.Value);

					return new Tuple<SkillName, double>(SkillName, double.Parse(SkillCap.Value ?? "0"));
				})
				.ToDictionary(Entry => Entry.Item1, Entry => Entry.Item2);

			bool Hidden = bool.Parse(Node.Attribute("hidden")?.Value ?? "false");

			return new CharacterClass(ID, Name, SkillCaps, Depth, new List<int>(), Hidden);
		}

		private static void LoadRequirements()
		{
			if (Requirements.Count > 0)
			{
				return;
			}

			XDocument Document = XDocument.Load(ConfigFilePath);

			Requirements = Document.Root.Element("requirements")
				.Elements("requirement")
				.Select(Node => int.Parse(Node.Value))
				.ToList();
		}

		[Usage("Classe")]
		[Description("Permet d'ouvrir l'arbre des classes")]
		public static void OpenCharacterClassGump(CommandEventArgs e)
		{
			if (e.Mobile is CustomPlayerMobile Mobile)
			{
				Mobile.SendGump(new CharacterClassGump(Mobile, Mobile, new List<int> { Mobile.Class.ID }, 0));
			}
		}

		public static MainCharacterClass GetMainCharacterClass(int ID)
		{
			LoadClasses();

			if (MainCharacterClasses.ContainsKey(ID))
			{
				return MainCharacterClasses[ID];
			}

			return null;
		}

		public static CharacterClass GetJobCharacterClass(int ID)
		{
			LoadClasses();

			if (JobCharacterClasses.ContainsKey(ID))
			{
				return JobCharacterClasses[ID];
			}

			return null;
		}

		public static int GetLevelRequirement(int ClassLevel)
		{
			LoadRequirements();

			if (ClassLevel < Requirements.Count)
			{
				return Requirements[ClassLevel];
			}

			return int.MaxValue;
		}

		public static bool IsRequiredLevel(int ClassLevel, int CharacterLevel)
		{
			return CharacterLevel >= GetLevelRequirement(ClassLevel);
		}
	}
}
