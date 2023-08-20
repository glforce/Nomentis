#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server
{
	[Parsable]
	public class Metier
	{
		public static List<SkillName> MetierSkill = new List<SkillName>()
		{
			SkillName.Alchemy,
			SkillName.ArmsLore,
			SkillName.Cooking,
			SkillName.Fletching,
			SkillName.Blacksmith,
			SkillName.Carpentry,
			SkillName.Cartography,
			SkillName.Fishing,
			SkillName.Fletching,
			SkillName.Imbuing,
			SkillName.Inscribe,
			SkillName.Lumberjacking,
			SkillName.Mining,
			SkillName.Tailoring,
			SkillName.TasteID,
			SkillName.Tinkering	
		};



		private static readonly List<Metier> m_AllMetier = new List<Metier>();

		public static List<Metier> AllMetier => m_AllMetier;

		private readonly int m_MetierID;
		private string m_Name;
		private Dictionary<SkillName, double> m_Skill = new Dictionary<SkillName, double>();

		public static Metier GetMetier(int Id)
		{
			foreach (Metier item in m_AllMetier)
			{
				if (item.MetierID == Id)
				{
					return item;
				}
			}

			return null;
		}
		public static bool IsMetierSkill(SkillName skillN)
		{
			return MetierSkill.Contains(skillN);
		}


		public int MetierID => m_MetierID;

		public string Name { get => m_Name; set => m_Name = value; }

		public Dictionary<SkillName, double> Skill { get => m_Skill; set => m_Skill = value; }

		public override string ToString()
		{
			return m_Name;
		}


		public Metier(
			int MetierID,
			string name,
			Dictionary<SkillName, double> skill
			)
		{
			m_MetierID = MetierID;
			
			m_Name = name;
			m_Skill = skill;
		}
		public static void RegisterMetier(Metier Metier)
		{
			Metier.AllMetier.Add(Metier);
		}



		public double GetSkillValue(SkillName sname)
		{
			if (Skill.ContainsKey(sname))
			{
				return Skill[sname];
			}
			return 0;
		}



		public static string[] GetMetiersNames()
		{

			List<string> MetierName = new List<string>();

			foreach (Metier item in AllMetier)
			{
				MetierName.Add(item.Name);
			}

			return MetierName.ToArray();
		}

	



	}
}
