using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Accounting;

namespace Server.Custom.NarrativeQuestSystem
{
	abstract class NarrativeQuestStep
	{
		private List<NarrativeQuestStep> Prerequesites = new List<NarrativeQuestStep>();

		public NarrativeQuestStep(List<NarrativeQuestStep> Prerequesites)
		{
			this.Prerequesites = Prerequesites;
		}

		protected abstract bool CheckIsComplete();

		public bool IsComplete()
		{
			return Prerequesites.All(Step => Step.IsComplete())
				&& CheckIsComplete();
		}
	}

	class TriggeredNarrativeQuestStep : NarrativeQuestStep
	{
		public bool Triggered { get; set; }

		public TriggeredNarrativeQuestStep(bool Triggered, List<NarrativeQuestStep> Prerequesites)
			: base(Prerequesites)
		{
			this.Triggered = Triggered;
		}

		protected override bool CheckIsComplete()
		{
			return Triggered;
		}
	}

	class ItemOwnedNarrativeQuestStep : NarrativeQuestStep
	{
		private Item Item;

		public ItemOwnedNarrativeQuestStep(Item Item, List<NarrativeQuestStep> Prerequesites)
			: base(Prerequesites)
		{
			this.Item = Item;
		}

		protected override bool CheckIsComplete()
		{
			return Accounts.GetAccounts()
				.Where(Account => Account.AccessLevel == AccessLevel.Player)
				.Any(Account =>
				{
					for (int i = 0; i < Account.Count; i++)
					{
						if (Account[i]?.Items.Contains(Item) == true)
						{
							return true;
						}
					}

					return false;
				});
		}
	}

	class NarrativeQuest
	{
		private TriggeredNarrativeQuestStep Root = new TriggeredNarrativeQuestStep(true, new List<NarrativeQuestStep>());

		public NarrativeQuest()
		{
		}
	}

	public class NarrativeQuestSystem
	{
		private static List<NarrativeQuestStep> Quests = new List<NarrativeQuestStep>();

		public static void Initalize()
		{
			
		}
	}
}
