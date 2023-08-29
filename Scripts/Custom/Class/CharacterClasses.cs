using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Items;
using System.Xml.Linq;
using Server.Commands;
using Server.Gumps;
using Server.Custom.Mobiles;
using System.Runtime.InteropServices;

namespace Server.Custom.Class
{
	public class CharacterClasses
	{
		private struct MainClassSpecialization
		{
			public string Name;
			public Dictionary<SkillName, double> SkillCaps;
			public List<ArmorMaterialType> AllowedArmorMaterialTypes;
		}

		private static readonly string ConfigFilePath = Path.Combine("Config", "Classes.xml");

		public static Dictionary<Race, Dictionary<int, MainCharacterClass>> MainCharacterClasses = new Dictionary<Race, Dictionary<int, MainCharacterClass>>();
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

			XElement MainNode = Document.Root.Element("main");

			Dictionary<int, MainCharacterClass> BaseMainCharacterClasses = ReadMainClasses(MainNode, 0)
				.ToDictionary(MainCharacterClass => MainCharacterClass.ID);

			Dictionary<Race, Dictionary<int, MainClassSpecialization>> Specializations = ReadSpecializations(MainNode);

			foreach (Race Race in Race.AllRaces)
			{
				Dictionary<int, MainCharacterClass> RaceMainCharacterClasses = new Dictionary<int, MainCharacterClass>();
				MainCharacterClasses[Race] = RaceMainCharacterClasses;

				foreach (KeyValuePair<int, MainCharacterClass> BaseMainCharacterClass in BaseMainCharacterClasses)
				{
					int ID = BaseMainCharacterClass.Key;

					MainClassSpecialization Specialization = new MainClassSpecialization();
					if (Specializations.ContainsKey(Race))
					{
						if (Specializations[Race].ContainsKey(ID))
						{
							Specialization = Specializations[Race][ID];
						}
					}

					RaceMainCharacterClasses[ID] = SpecializeMainCharacterClass(BaseMainCharacterClass.Value, Specialization);
				}
			}

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

					List<ArmorMaterialType> AllowedArmorMaterialTypes = ReadAllowedArmorMaterialTypes(ClassNode) ?? new List<ArmorMaterialType>();

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

		private static List<ArmorMaterialType> ReadAllowedArmorMaterialTypes(XElement Node)
		{
			XElement ArmorsNode = Node?.Element("armors");
			if (ArmorsNode == null)
			{
				return null;
			}

			return ArmorsNode.Elements("armor")
				.Select(ArmorNode => (ArmorMaterialType)Enum.Parse(typeof(ArmorMaterialType), ArmorNode.Value))
				.ToList();
		}

		private static CharacterClass ReadClassNode(XElement Node, int Depth)
		{
			int ID = int.Parse(Node.Attribute("id")?.Value);
			string Name = Node.Attribute("name")?.Value;

			Dictionary<SkillName, double> SkillCaps = ReadSkillCaps(Node) ?? new Dictionary<SkillName, double>();

			bool Hidden = bool.Parse(Node.Attribute("hidden")?.Value ?? "false");

			return new CharacterClass(ID, Name, SkillCaps, Depth, new List<int>(), Hidden);
		}

		private static Dictionary<SkillName, double> ReadSkillCaps(XElement Node)
		{
			XElement SkillsNode = Node?.Element("skills");
			if (SkillsNode == null)
			{
				return null;
			}

			return SkillsNode.Elements("skill")
				.ToDictionary(
				SkillCap => (SkillName)Enum.Parse(typeof(SkillName), SkillCap.Attribute("name")?.Value),
				SkillCap => double.Parse(SkillCap.Value ?? "0"));
		}

		private static MainCharacterClass SpecializeMainCharacterClass(MainCharacterClass BaseMainCharacterClass, MainClassSpecialization Specialization)
		{
			string Name = Specialization.Name ?? BaseMainCharacterClass.Name;
			Dictionary<SkillName, double> SkillCaps = Specialization.SkillCaps ?? BaseMainCharacterClass.SkillCaps;
			List<ArmorMaterialType> AllowedArmorMaterialTypes = Specialization.AllowedArmorMaterialTypes ?? BaseMainCharacterClass.AllowedArmorMaterialTypes;

			return new MainCharacterClass(
				BaseMainCharacterClass.ID,
				Name,
				SkillCaps,
				BaseMainCharacterClass.Level,
				BaseMainCharacterClass.Evolutions,
				BaseMainCharacterClass.Hidden,
				AllowedArmorMaterialTypes);
		}

		private static Dictionary<Race, Dictionary<int, MainClassSpecialization>> ReadSpecializations(XElement Node)
		{
			return Node.Element("specializations")
				.Elements()
				.ToDictionary(RaceNode => Race.Parse(RaceNode.Attribute("name").Value),
				RaceNode =>
				{
					return RaceNode.Elements("specialization")
					.ToDictionary(SpecializationNode => int.Parse(SpecializationNode.Attribute("id").Value),
					SpecializationNode =>
					{
						return new MainClassSpecialization()
						{
							Name = SpecializationNode.Attribute("name").Value,
							SkillCaps = ReadSkillCaps(SpecializationNode),
							AllowedArmorMaterialTypes = ReadAllowedArmorMaterialTypes(SpecializationNode)
						};
					});
				});
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

		public static MainCharacterClass GetMainCharacterClass(Race Race, int ID)
		{
			LoadClasses();

			if (MainCharacterClasses.ContainsKey(Race))
			{
				if (MainCharacterClasses[Race].ContainsKey(ID))
				{
					return MainCharacterClasses[Race][ID];
				}
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
