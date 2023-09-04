using System;
using Server.Custom.Evolution;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Mobiles
{
	public partial class CustomPlayerMobile : PlayerMobile
	{
		private double PreciseExperience;

		[CommandProperty(AccessLevel.GameMaster)]
		public double Experience
		{
			get
			{
				return Math.Round(PreciseExperience);
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public DateTime LastExperienceGain { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan ExperienceFatigue { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int SkillPoints { get; set; } = 1;

		[CommandProperty(AccessLevel.Administrator)]
		public DateTime HordeInvulnerabilityStart { get; set; }

		public CustomPlayerMobile()
		{
		}

		public CustomPlayerMobile(Serial s)
			: base(s)
		{
		}

		public void AdjustSkillCaps()
		{
			var LevelSkillCap = ExperienceSystem.GetLevelSpec(this).SkillCap;

			foreach (var Skill in Skills)
			{
				Skill.Cap = LevelSkillCap;

				Skill.Base = Math.Min(Skill.Base, Skill.Cap);
			}
		}

		public double AddExperience(double Amount)
		{
			var PreviousLevel = ExperienceSystem.GetLevel(this);

			PreciseExperience += Math.Max(Amount, 0.0);

			if (Math.Round(Amount) > 0)
				SendMessage("Vous avez gagné {0} point(s) d'expérience!", Math.Round(Amount));

			var NewLevel = ExperienceSystem.GetLevel(this);
			if (NewLevel > PreviousLevel)
			{
				AdjustSkillCaps();
				GiveSkillPoints(NewLevel - PreviousLevel);
				SendMessage("Vous êtes maintenant au niveau {0}!", NewLevel);
			}

			return Experience;
		}

		public void GiveSkillPoints(int LevelGap)
		{
			SkillPoints += LevelGap * ExperienceSystem.SkillPointsPerLevel;
		}

		public bool CanLearnSkill(SkillName SkillName)
		{
			if (SkillPoints <= 0)
			{
				return false;
			}

			Skill Skill = Skills[SkillName];

			return Skill.Base < Skill.Cap;
		}

		public bool LearnSkill(SkillName SkillName)
		{
			if (!CanLearnSkill(SkillName))
			{
				return false;
			}

			SkillPoints--;

			Skill Skill = Skills[SkillName];

			Skill.Base = Skill.Cap;

			return true;
		}

		public override void Deserialize(GenericReader Reader)
		{
			base.Deserialize(Reader);

			// Version
			Reader.ReadInt();

			PreciseExperience = Reader.ReadDouble();
			LastExperienceGain = Reader.ReadDateTime();
			ExperienceFatigue = Reader.ReadTimeSpan();
			SkillPoints = Reader.ReadInt();
		}

		public override void Serialize(GenericWriter Writer)
		{
			base.Serialize(Writer);

			// Version
			Writer.Write(0);

			Writer.Write(PreciseExperience);
			Writer.Write(LastExperienceGain);
			Writer.Write(ExperienceFatigue);
			Writer.Write(SkillPoints);
		}
	}
}