using System;
using System.IO;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Gumps;
using System.Collections.Generic;
using Server.Custom;

namespace Server
{
  public class XPLevel
  {
		public static Dictionary<int, XPLevel> XpTable = new Dictionary<int, XPLevel>()
		{ 
			{0, new XPLevel(0,30)}, // Classe 1
			{1, new XPLevel(6,35)},
			{2, new XPLevel(12,40)},
			{3, new XPLevel(18,45)},
			{4, new XPLevel(24,47)},
			{5, new XPLevel(33,50)}, //Classe2
			{6, new XPLevel(42,52)},
			{7, new XPLevel(51,54)}, 
			{8, new XPLevel(63,56)},
			{9, new XPLevel(75,58)},
			{10, new XPLevel(90,60)}, 
			{11, new XPLevel(105,63)},
			{12, new XPLevel(123,66)},
			{13, new XPLevel(141,69)},
			{14, new XPLevel(162,72)},
			{15, new XPLevel(183,75)},// classe 3
			{16, new XPLevel(207,77)},
			{17, new XPLevel(231,79)},
			{18, new XPLevel(258,81)},
			{19, new XPLevel(285,83)},
			{20, new XPLevel(312,85)},// Classe 4
			{21, new XPLevel(342,87)},
			{22, new XPLevel(372,89)},
			{23, new XPLevel(402,91)}, 
			{24, new XPLevel(432,93)},
			{25, new XPLevel(462,95)},
			{26, new XPLevel(492,96)},
			{27, new XPLevel(525,97)},
			{28, new XPLevel(558,98)},
			{29, new XPLevel(594,99)},
			{30, new XPLevel(630,100)} // Classe 5
		};

		private int m_FeRequis = 0;
		private double m_MaxSkill = 0;

		public int FeRequis { get => m_FeRequis; set => m_FeRequis = value; }
		public double MaxSkill { get => m_MaxSkill; set => m_MaxSkill = value; }

		public XPLevel(int feRequis, double maxSkill)
		{
				m_FeRequis = feRequis;
				m_MaxSkill = maxSkill;
		}


		public static XPLevel GetLevel(int niveau)
		{
			int lvl = niveau;

			if (lvl > 30)
			{
				lvl = 30;
			}
			else if(lvl < 0)
			{
				lvl = 0;
			}

			return XpTable[lvl];		
		}





  }
}