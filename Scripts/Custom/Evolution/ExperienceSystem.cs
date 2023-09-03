using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Server.Commands;
using Server.Custom.Mobiles;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Custom.Evolution
{
	public class ExperienceSystem
	{
		public struct LevelSpec
		{
			public int RequiredExperience { get; set; }
			public double SkillCap { get; set; }
		}

		private static readonly string ConfigFilePath = Path.Combine("Config", "ExperienceTable.xml");

		private static readonly TimeSpan EXPERIENCE_GAIN_FREQUENCY = Config.Get("ExperienceSystem.ExperienceGainFrequency", TimeSpan.FromSeconds(30));

		private static readonly TimeSpan MAX_OPTIMAL_EXPERIENCE_GAIN_SPAN = Config.Get("ExperienceSystem.MaxOptimalExperienceGainSpan", TimeSpan.FromHours(1));
		private static readonly double EXPERIENCE_GAIN_RATE_BY_MINUTE = Config.Get("ExperienceSystem.ExperienceGainRateByMinute", 0.01);

		public static List<LevelSpec> LevelSpecs = new List<LevelSpec>();

		public static void Configure()
		{
			LoadExperienceTable();

			CommandSystem.Register("Experience", AccessLevel.Player, ShowPlayerExperience);
			CommandSystem.Register("TableNiveaux", AccessLevel.Player, OpenExperienceTable);

			CommandSystem.Register("CharactersExperience", AccessLevel.GameMaster, ShowCharactersExperience);
			CommandSystem.Register("TargetExperience", AccessLevel.GameMaster, ShowTargetExperience);
			CommandSystem.Register("GiveExperience", AccessLevel.GameMaster, GiveExperience);

			Timer.DelayCall(EXPERIENCE_GAIN_FREQUENCY, EXPERIENCE_GAIN_FREQUENCY, GainTimedExperience);
		}

		private static void LoadExperienceTable()
		{
			XDocument XmlDocument = XDocument.Load(ConfigFilePath);

			LevelSpecs = XmlDocument.Root.Descendants("level")
				.Select(Node =>
				{
					return new LevelSpec
					{
						RequiredExperience = int.Parse(Node.Attribute("req")?.Value ?? "0"),
						SkillCap = double.Parse(Node.Attribute("skillcap")?.Value ?? "0")
					};
				})
				.ToList();
		}

		private static void ShowPlayerExperience(CommandEventArgs e)
		{
			if (e.Mobile is CustomPlayerMobile Mobile)
			{
				Mobile.SendMessage("Vous avez {0} point(s) d'expérience.", Mobile.Experience);
			}
		}

		private static void OpenExperienceTable(CommandEventArgs e)
		{
			if (e.Mobile is CustomPlayerMobile Mobile)
			{
				Mobile.SendGump(new ExperienceTableGump(Mobile));
			}
		}

		private static void ShowCharactersExperience(CommandEventArgs e)
		{
			if (e.Mobile is PlayerMobile PlayerMobile)
			{
				PlayerMobile.SendGump(new CharactersExperienceGump(PlayerMobile));
			}
		}

		private static void ShowTargetExperience(CommandEventArgs e)
		{
			e.Mobile.Target = new ShowExperienceTarget();
		}

		private static void GiveExperience(CommandEventArgs e)
		{
			if (e.Arguments.Length > 0)
			{
				double Amount = double.Parse(e.Arguments[0]);

				e.Mobile.Target = new GiveExperienceTarget(Amount);
			}
		}

		private static void GainTimedExperience()
		{
			DateTime Now = DateTime.UtcNow;

			foreach (NetState Instance in NetState.Instances)
			{
				if (Instance.Mobile is CustomPlayerMobile PlayerMobile)
				{
					GainTimedExperience(PlayerMobile, Now, Instance.ConnectedOn);
				}
			}
		}

		private static void GainTimedExperience(CustomPlayerMobile Mobile, DateTime Now, DateTime ConnectedOn)
		{
			if (Mobile.LastExperienceGain < ConnectedOn)
			{
				if (Mobile.LastExperienceGain > DateTime.MinValue)
				{
					Mobile.ExperienceFatigue -= ConnectedOn - Mobile.LastExperienceGain;
				}

				Mobile.LastExperienceGain = ConnectedOn;
			}

			TimeSpan ElapsedSinceLastGain = Now - Mobile.LastExperienceGain;
			Mobile.ExperienceFatigue += ElapsedSinceLastGain;

			double FatigueModifier = 1.0f;
			if (Mobile.ExperienceFatigue > TimeSpan.Zero)
			{
				FatigueModifier = MAX_OPTIMAL_EXPERIENCE_GAIN_SPAN.TotalMinutes / Mobile.ExperienceFatigue.TotalMinutes;
			}

			Mobile.AddExperience(EXPERIENCE_GAIN_RATE_BY_MINUTE * ElapsedSinceLastGain.TotalMinutes * FatigueModifier);
		}

		public static int GetLevel(CustomPlayerMobile PlayerMobile)
		{
			int FoundLevel = LevelSpecs.FindIndex(Spec => PlayerMobile.Experience >= Spec.RequiredExperience);

			return Math.Max(FoundLevel, 0);
		}

		public static LevelSpec GetLevelSpec(CustomPlayerMobile PlayerMobile)
		{
			return LevelSpecs[GetLevel(PlayerMobile)];
		}

		private class ShowExperienceTarget : Target
		{
			public ShowExperienceTarget() 
				: base(15, false, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile From, object Target)
			{
				if (Target is CustomPlayerMobile TargetPlayer)
				{
					From.SendMessage("Le personnage {0} a {1} point(s) d'expérience.", TargetPlayer.Name, TargetPlayer.Experience);
				}
			}
		}

		private class GiveExperienceTarget : Target
		{
			private double Amount;

			public GiveExperienceTarget(double Amount)
				: base(15, false, TargetFlags.None)
			{
				this.Amount = Amount;
			}

			protected override void OnTarget(Mobile From, object Target)
			{
				if (Target is CustomPlayerMobile TargetPlayer)
				{
					TargetPlayer.AddExperience(Amount);

					From.SendMessage("Le personnage {0} a reçu {1} point(s) d'expérience. Total: {2}", TargetPlayer.Name, Amount, TargetPlayer.Experience);
				}
			}
		}
	}
}
