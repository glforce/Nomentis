using System;
using System.Linq;
using Server.Commands;
using Server.Engines.Shadowguard;
using Server.Items;
using Server.Menus.Questions;
using Server.Targeting;

namespace Server.Services.Horde
{
	public class CorruptionSystem
	{
		public static void Configure()
		{
			CommandSystem.Register("Corruption", AccessLevel.Administrator, CheckCorruption);
			CommandSystem.Register("IncreaseCorruption", AccessLevel.Administrator, IncreaseCorruption);
			CommandSystem.Register("DecreaseCorruption", AccessLevel.Administrator, DecreaseCorruption);
			CommandSystem.Register("SetCorruption", AccessLevel.Administrator, SetCorruption);
		}

		private static void TargetCorruptionAction(Mobile From, Action<Mobile> Action)
		{
			From.Target = new CorruptionTarget(Target =>
			{
				Action(Target);
				OutputCorruption(From, Target);
			});
		}

		private static void OutputCorruption(Mobile From, Mobile Target)
		{
			From.SendMessage("[{0}] Corruption level: {1}/{2}", Target.Name, Target.Corruption, Mobile.CORRUPTION_MAX);
		}

		private static void CheckCorruption(CommandEventArgs e)
		{
			TargetCorruptionAction(e.Mobile, Target => { });
		}

		private static void IncreaseCorruption(CommandEventArgs e)
		{
			if (e.Arguments.Count() > 0)
			{
				TargetCorruptionAction(e.Mobile, Target => Target.IncreaseCorruption(float.Parse(e.Arguments[0])));
			}
		}

		private static void DecreaseCorruption(CommandEventArgs e)
		{
			if (e.Arguments.Count() > 0)
			{
				TargetCorruptionAction(e.Mobile, Target => Target.IncreaseCorruption(-float.Parse(e.Arguments[0])));
			}
		}

		private static void SetCorruption(CommandEventArgs e)
		{
			if (e.Arguments.Count() > 0)
			{
				TargetCorruptionAction(e.Mobile, Target => Target.Corruption = float.Parse(e.Arguments[0]));
			}
		}

		private class CorruptionTarget : Target
		{
			Action<Mobile> Callback;

			public CorruptionTarget(Action<Mobile> Callback)
				: base(-1, false, TargetFlags.None)
			{
				this.Callback = Callback;
			}

			protected override void OnTarget(Mobile From, object Targeted)
			{
				if (Targeted is Mobile)
				{
					Callback(Targeted as Mobile);
				}
			}
		}
	}
}
