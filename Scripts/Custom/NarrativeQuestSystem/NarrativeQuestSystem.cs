using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Xml.Linq;
using Server.Accounting;

namespace Server.Custom.NarrativeQuestSystem
{
	abstract class NarrativeQuestStepCondition
	{
		public string Name { get; }

		public NarrativeQuestStepCondition(string Name)
		{
			this.Name = Name;
		}

		public abstract bool IsComplete();
	}

	class LockedNarrativeQuestStepCondition : NarrativeQuestStepCondition
	{
		private readonly bool Complete;

		public LockedNarrativeQuestStepCondition(string Name, bool Complete) : base(Name)
		{
			this.Complete = Complete;
		}

		public override bool IsComplete()
		{
			return Complete;
		}
	}

	class ItemOwnedNarrativeQuestStepCondition : NarrativeQuestStepCondition
	{
		private readonly int ItemID;

		public ItemOwnedNarrativeQuestStepCondition(string Name, int ItemID) : base(Name)
		{
			this.ItemID = ItemID;
		}

		public override bool IsComplete()
		{
			return Accounts.GetAccounts()
 				.Where(Account => Account.AccessLevel == AccessLevel.Player)
 				.Any(Account =>
 				{
					 for (int i = 0; i < Account.Count; i++)
					 {
						 if (Account[i]?.Items.Any(Item => Item.ItemID == ItemID) == true)
						 {
							 return true;
						 }
					 }
					 return false;
 				});
		}
	}

	class FlagNarrativeQuestStepCondition : NarrativeQuestStepCondition
	{
		private readonly Dictionary<string, bool> Flags;

		public FlagNarrativeQuestStepCondition(string Name, Dictionary<string, bool> Flags) : base(Name)
		{
			this.Flags = Flags;
		}

		public override bool IsComplete()
		{
			return Flags.ContainsKey(Name) && Flags[Name];
		}
	}

	class NarrativeQuestStep
	{
		public string Name { get; }
		private readonly List<NarrativeQuestStepCondition> Conditions;
		private readonly bool RequiresAny = false;

		public NarrativeQuestStep(string Name, List<NarrativeQuestStepCondition> Conditions, bool RequiresAny = false)
		{
			this.Name = Name;
			this.Conditions = Conditions;
			this.RequiresAny = RequiresAny;
		}

		public bool IsComplete()
		{
			int IncompleteCount = RequiresAny ? Conditions.Count - 1 : 0;
			return GetIncompleteConditions().Count <= IncompleteCount;
		}

		public List<NarrativeQuestStepCondition> GetIncompleteConditions()
		{
			return Conditions
				.Where(Condition => !Condition.IsComplete())
				.ToList();
		}
	}

	class NarrativeQuest
	{
		public string Name { get; }
		private readonly List<NarrativeQuestStep> Steps;

		public NarrativeQuest(string Name, List<NarrativeQuestStep> Steps)
		{
			this.Name = Name;
			this.Steps = Steps;
		}

		public NarrativeQuestStep GetNextStep()
		{
			return Steps.FirstOrDefault(Step => !Step.IsComplete());
		}

		public bool IsComplete()
		{
			return GetNextStep() == null;
		}

		public List<NarrativeQuestStepCondition> GetNextStepIncompleteConditions()
		{
			List<NarrativeQuestStepCondition> NextStepIncompleteConditions = new List<NarrativeQuestStepCondition>();

			NarrativeQuestStep NextStep = GetNextStep();

			if (NextStep != null)
			{
				NextStepIncompleteConditions.AddRange(NextStep.GetIncompleteConditions());
			}

			return NextStepIncompleteConditions;
		}
	}

	public class NarrativeQuestSystem
	{
		private static Dictionary<string, bool> SavedFlags;

		private static List<NarrativeQuest> Quests;

		public static void Initalize()
		{

		}

		private List<NarrativeQuest> ReadQuests()
		{
			XDocument Document = XDocument.Load("");

			return Document.Root.Elements("quest")
				.Select(QuestNode =>
				{
					string Name = QuestNode.Attribute("name").Value;

					List<NarrativeQuestStep> Steps = QuestNode.Elements("step")
					.Select(StepNode => ReadQuestStep(StepNode, Name))
					.ToList();

					return new NarrativeQuest(Name, Steps);
				})
				.ToList();
		}

		private NarrativeQuestStep ReadQuestStep(XElement Node, string QuestName)
		{
			string Name = string.Format("{0}.{1}", QuestName, Node.Attribute("name").Value);
			bool RequiresAny = bool.Parse(Node.Attribute("any").Value ?? "false");

			return new NarrativeQuestStep(Name, ReadConditions(Node, Name), RequiresAny);
		}

		private List<NarrativeQuestStepCondition> ReadConditions(XElement ConditionsNode, string StepName)
		{
			return ConditionsNode.Elements()
				.Select<XElement, NarrativeQuestStepCondition>(ConditionNode =>
				{
					string Name = ConditionNode.Name.ToString();
					string FullName = string.Format("{0}.{1}", StepName, Name);

					switch (Name)
					{
						case "locked":
							return new LockedNarrativeQuestStepCondition(FullName, bool.Parse(ConditionNode.Value ?? "false"));
						case "flag":
							return new FlagNarrativeQuestStepCondition(FullName, SavedFlags);
						case "item":
							return new ItemOwnedNarrativeQuestStepCondition(FullName, int.Parse(ConditionNode.Value ?? "0"));
						default:
							throw new InvalidDataException(string.Format("Unexpected condition type: {0}", Name));
					}
				})
				.ToList();
		}
	}
}
