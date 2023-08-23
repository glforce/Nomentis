#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server
{
	[Parsable]
	public class Classe
	{

		private static readonly List<Classe> m_AllClasse = new List<Classe>();

		public static List<Classe> AllClasse => m_AllClasse;

		private readonly int m_ClasseID;
		private string m_Name;
		private bool m_Hidden;
		private int m_ClasseLvl;
		private int m_Armor;
		private List<int> m_Evolution = new List<int>();
		private Dictionary<SkillName, double> m_Skill = new Dictionary<SkillName, double>();

		public static Classe GetClasse(int Id)
		{
			foreach (Classe item in m_AllClasse)
			{
				if (item.ClasseID == Id)
				{
					return item;
				}
			}

			return null;
		}

		public int ClasseID => m_ClasseID;

		public string Name { get => m_Name; set => m_Name = value; }
		public bool Hidden { get => m_Hidden; set => m_Hidden = value; }

		public int ClasseLvl { get => m_ClasseLvl; set => m_ClasseLvl = value; }
		public int Armor { get => m_Armor; set => m_Armor = value; }

		public Dictionary<SkillName, double> Skill { get => m_Skill; set => m_Skill = value; }

		public List<int> Evolution { get => m_Evolution; set => m_Evolution = value; }

		public int NiveauRequis 
		{ 
			get 
			{
				return LevelToEvolve(ClasseLvl);
			} 
			
		}

		public override string ToString()
		{
			return m_Name;
		}

		public Classe(
			int ClasseID,
			string name,
			int classLvl,
			int armor,
			bool hidden,
			List<int> evolution,
			Dictionary<SkillName, double> skill
			)
		{
			m_ClasseID = ClasseID;
			m_Hidden = hidden;
			m_Name = name;
			m_Skill = skill;
			m_ClasseLvl = classLvl;
			m_Armor = armor;
			m_Evolution = evolution;
		}
		public static void RegisterClasse(Classe Classe)
		{
			Classe.AllClasse.Add(Classe);
		}



		public double GetSkillValue(SkillName sname)
		{
			if (Skill.ContainsKey(sname))
			{
				return Skill[sname];
			}
			return 0;
		}



		public static string[] GetClassesNames()
		{

			List<string> ClasseName = new List<string>();

			foreach (Classe item in AllClasse)
			{
				ClasseName.Add(item.Name);
			}

			return ClasseName.ToArray();
		}

		public static int LevelToEvolve(int ClasseLvl)
		{		 
				switch (ClasseLvl)
				{
					
					case 0:					
						return 0; 					
					case 1:
						return 5;
					case 2:				
						return 15; 
					case 3:						          
						return 20;
					case 4:
						return 30;
					default:
						return 99;
				}	
		}

	



	}
}
