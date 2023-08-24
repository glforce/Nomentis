using System;
using Server.Custom.Class;
using Server.Custom.Evolution;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Mobiles
{
	public partial class CustomPlayerMobile : PlayerMobile
	{
		private MainCharacterClass m_Class;
		private CharacterClass m_Job;
		private double PreciseExperience;

		[CommandProperty(AccessLevel.GameMaster)]
		public MainCharacterClass Class
		{
			get => m_Class;
			set
			{
				m_Class = value;
				AdjustSkillCaps();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public CharacterClass Job
		{
			get => m_Job;
			set
			{
				m_Job = value;
				AdjustSkillCaps();
			}
		}

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

		public CustomPlayerMobile()
		{
		}

		public CustomPlayerMobile(Serial s)
			: base(s)
		{
		}

		public override bool OnEquip(Item Item)
		{
			if (AccessLevel > AccessLevel.Player)
				return true;

			if (Item is BaseArmor Armor)
				if (!m_Class.IsArmorAllowed(Armor))
				{
					SendMessage("Type d'armure requis: {0}.", Armor.MaterialType.ToString());
					return false;
				}

			return base.OnEquip(Item);
		}

		public void AdjustSkillCaps()
		{
			var LevelSkillCap = ExperienceSystem.GetLevelSpec(this).SkillCap;

			foreach (var Skill in Skills)
			{
				double ClassCap = Math.Max(m_Class.GetSkillCap(Skill.SkillName), m_Job.GetSkillCap(Skill.SkillName));

				Skill.Cap = Math.Min(Skill.Cap, Math.Min(LevelSkillCap, ClassCap));

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
				SendMessage("Vous êtes maintenant au niveau {0}!", NewLevel);

				if (CharacterClasses.IsRequiredLevel(m_Class.Level + 1, NewLevel))
					SendMessage("Vous avez maintenant accès à une nouvelle classe!");
			}

			return Experience;
		}

		public bool CanEvolveTo(MainCharacterClass Class)
		{
			return m_Class.Evolutions.Contains(Class.ID)
			   && CharacterClasses.IsRequiredLevel(Class.Level, ExperienceSystem.GetLevel(this));
		}

		public override void Deserialize(GenericReader Reader)
		{
			base.Deserialize(Reader);

			var version = Reader.ReadInt();

			switch (version)
			{
				case 0:
					m_Class = CharacterClasses.GetMainCharacterClass(Reader.ReadInt());
					m_Job = CharacterClasses.GetJobCharacterClass(Reader.ReadInt());
					PreciseExperience = Reader.ReadDouble();
					LastExperienceGain = Reader.ReadDateTime();
					ExperienceFatigue = Reader.ReadTimeSpan();
					break;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
			writer.Write(m_Class.ID);
			writer.Write(m_Job.ID);
			writer.Write(PreciseExperience);
			writer.Write(LastExperienceGain);
			writer.Write(ExperienceFatigue);
		}
	}
}