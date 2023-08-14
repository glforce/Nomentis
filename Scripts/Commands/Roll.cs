using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Commands
{
	public class Roll
	{
		public static void Initialize()
		{
			CommandSystem.Register("Roll", AccessLevel.Player, OnRoll);
		}

		private static readonly int MAX_ROLL = 20;

		private static void OnRoll(CommandEventArgs e)
		{
			e.Mobile.Emote("Rolling rolling... {0}", MAX_ROLL);

			int Result = Utility.Random(0, MAX_ROLL) + 1;

			if (e.Arguments.Length > 0)
			{
				int CheckValue;

				Skill CheckSkill = e.Mobile.Skills.FirstOrDefault(Skill => Skill.Name.Equals(e.Arguments[0], StringComparison.OrdinalIgnoreCase));
				if (CheckSkill != null)
				{
					CheckValue = (int)((1.0 - CheckSkill.Value / CheckSkill.Cap) * MAX_ROLL);
				}
				else
				{
					CheckValue = int.Parse(e.Arguments[0]);
				}

				e.Mobile.Emote("Roll {0}", Result >= CheckValue ? "Success" : "Failed");
			}
			else
			{
				e.Mobile.Emote("Rolled {0}", Result.ToString());
			}
		}
	}
}
