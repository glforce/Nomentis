using Server.ContextMenus;
using Server.Custom.Mobiles;

namespace Server.Custom.Context_Entries
{
	public class TeachSkillContextMenuEntry : ContextMenuEntry
	{
		private readonly SkillName Skill;
		private readonly CustomPlayerMobile Mobile;

		public TeachSkillContextMenuEntry(SkillName Skill, CustomPlayerMobile Mobile) : base(6000 + (int)Skill)
		{
			this.Skill = Skill;
			this.Mobile = Mobile;
		}

		public override void OnClick()
		{
			bool Result = Mobile.LearnSkill(Skill);

			if (Result)
			{
				Mobile.SendMessage("Vous avez progressé dans le skill {0}", Skill.ToString());
			}
			else
			{
				Mobile.SendMessage("Vous ne pouvez pas progresser dans le skill {0}", Skill.ToString());
			}
		}
	}
}
