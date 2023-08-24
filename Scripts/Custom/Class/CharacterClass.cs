using System.Collections.Generic;

namespace Server.Custom.Class
{
	public class CharacterClass
	{
		protected int m_ID;
		protected string m_Name;
		protected Dictionary<SkillName, double> m_SkillCaps;
		protected int m_Level;
		protected List<int> m_Evolutions;
		protected bool m_Hidden;

		public int ID
		{
			get { return m_ID; }
		}

		public string Name
		{
			get { return m_Name; }
		}

		public Dictionary<SkillName, double> SkillCaps
		{
			get { return m_SkillCaps; }
		}

		public int Level
		{
			get { return m_Level; }
		}

		public List<int> Evolutions
		{
			get { return m_Evolutions; }
		}

		public bool Hidden
		{
			get { return m_Hidden; }
		}

		public CharacterClass(
			int ID, 
			string Name, 
			Dictionary<SkillName, double> SkillCaps, 
			int Level, 
			List<int> Evolutions,
			bool Hidden)
		{
			m_ID = ID;
			m_Name = Name;
			m_Level = Level;
			m_SkillCaps = SkillCaps;
			m_Evolutions = Evolutions;
			m_Hidden = Hidden;
		}

		public double GetSkillCap(SkillName SkillName)
		{
			if (m_SkillCaps.ContainsKey(SkillName))
			{
				return m_SkillCaps[SkillName];
			}

			return 0.0;
		}
	}
}
