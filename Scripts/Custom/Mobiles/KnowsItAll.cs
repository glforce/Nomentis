using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Mobiles;

namespace Server.Custom.Mobiles
{
	public class KnowsItAll : TeacherNPC
	{
		[Constructable]
		public KnowsItAll() : base("Knows it all")
		{
			Body = 0x190;
			Name = NameList.RandomName("male");
		}

		protected override List<SkillName> TeachedSkills => Enumerable.Range(0, 58).Select(skillID => (SkillName)skillID).ToList();
	}
}
