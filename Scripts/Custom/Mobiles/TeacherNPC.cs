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
	public abstract class TeacherNPC : BaseVendor
	{
		protected TeacherNPC(string Title) : base(Title)
		{
		}

		public TeacherNPC(Serial Serial) : base(Serial)
		{
		}

		protected abstract List<SkillName> TeachedSkills { get; }

		public override void GetContextMenuEntries(Mobile From, List<ContextMenuEntry> List)
		{
			base.GetContextMenuEntries(From, List);

			if (From is CustomPlayerMobile Mobile)
			{
				List.AddRange(
					TeachedSkills
					.Where(Skill => Mobile.CanLearnSkill(Skill))
					.Select(Skill => new TeachSkillContextMenuEntry(Skill, Mobile))
					);
			}
		}

		public override void Serialize(GenericWriter Writer)
		{
			base.Serialize(Writer);
		}

		public override void Deserialize(GenericReader Reader)
		{
			base.Deserialize(Reader);
		}
	}
}
