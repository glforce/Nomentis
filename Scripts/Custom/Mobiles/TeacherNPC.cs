using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.ContextMenus;
using Server.Custom.Context_Entries;
using Server.Mobiles;

namespace Server.Custom.Mobiles
{
	public abstract class TeacherNPC : BaseCreature
	{
		protected TeacherNPC(string title) : 
			base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 5)
		{
			Title = title;
		}

		public TeacherNPC(Serial serial) : base(serial)
		{
		}

		protected abstract List<SkillName> TeachedSkills { get; }

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from is CustomPlayerMobile Mobile)
			{
				list.AddRange(
					TeachedSkills
					.Where(Skill => Mobile.CanLearnSkill(Skill))
					.Select(Skill => new TeachSkillContextMenuEntry(Skill, Mobile))
					);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
		}
	}
}
