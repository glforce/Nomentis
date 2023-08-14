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

		private static void OnRoll(CommandEventArgs e)
		{
			e.Mobile.Say("Rolling rolling...");

			int Result = Utility.Random(0, 20) + 1;

			if (e.Arguments.Length > 0)
			{
				int Required = int.Parse(e.Arguments[0]);
				e.Mobile.Say("Roll {0}", Result >= Required ? "Success" : "Failed");
			}
			else
			{
				e.Mobile.Say("Rolled {0}", Result.ToString());
			}
		}
	}
}
