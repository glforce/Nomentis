using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Server.Accounting;
using Server.Commands;
using Server.Engines.Quests;

namespace Server.Custom.NarrativeQuestSystem
{
	abstract class NarrativeQuestStepCondition
	{
		public NarrativeQuestStep OwningStep { get; set; }
		public string Name { get; }
		public string QualifiedName
		{
			get
			{
				return string.Format("{0}.{1}", OwningStep.QualifiedName, Name);
			}
		}

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
		private readonly HashSet<string> Flags;

		public FlagNarrativeQuestStepCondition(string Name, HashSet<string> Flags) : base(Name)
		{
			this.Flags = Flags;
		}

		public override bool IsComplete()
		{
			return Flags.Contains(QualifiedName);
		}
	}

	class NarrativeQuestStep
	{
		public NarrativeQuest OwningQuest { get; set; }
		public string Name { get; }
		public string QualifiedName { 
			get
			{
				return string.Format("{0}.{1}", OwningQuest.Name, Name);
			}
		}

		private readonly List<NarrativeQuestStepCondition> Conditions;
		private readonly bool RequiresAny = false;

		public NarrativeQuestStep(string Name, List<NarrativeQuestStepCondition> Conditions, bool RequiresAny = false)
		{
			this.Name = Name;

			this.Conditions = Conditions
				.Select(Condition =>
				{
					Condition.OwningStep = this;
					return Condition;
				})
				.ToList();

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

		public NarrativeQuestStepCondition GetConditionByName(string ConditionName)
		{
			return Conditions.FirstOrDefault(Condition => Condition.Name == ConditionName);
		}
	}

	class NarrativeQuest
	{
		public string Name { get; }
		private readonly List<NarrativeQuestStep> Steps;

		public NarrativeQuest(string Name, List<NarrativeQuestStep> Steps)
		{
			this.Name = Name;
			this.Steps = Steps
				.Select(Step =>
				{
					Step.OwningQuest = this;
					return Step;
				})
				.ToList();
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

		public NarrativeQuestStep GetStepByName(string StepName)
		{
			return Steps.FirstOrDefault(Step => Step.Name == StepName);
		}
	}

	public class NarrativeQuestSystem
	{
		private static readonly string ConfigFilePath = Path.Combine("Config", "NarrativeQuests.xml");
		private static readonly string SaveFilePath = Path.Combine("Saves", "Quests", "NarrativeQuestSystem.bin");

		private static readonly HashSet<string> SavedFlags = new HashSet<string>();

		private static List<NarrativeQuest> Quests = new List<NarrativeQuest>();

		public static void Configure()
		{
			Quests = ReadQuests();

			EventSink.WorldSave += OnSave;
			EventSink.WorldLoad += OnLoad;

			CommandSystem.Register("ToggleQuestStepCondition", AccessLevel.GameMaster, OnToggleQuestStepCondition);
			CommandSystem.Register("GetNextQuestStep", AccessLevel.GameMaster, OnGetNextQuestStep);
			CommandSystem.Register("GetNextQuestStepIncompleteConditions", AccessLevel.GameMaster, OnGetNextQuestStepIncompleteConditions);
			CommandSystem.Register("IsQuestComplete", AccessLevel.GameMaster, OnIsQuestComplete);
		}

		private static List<NarrativeQuest> ReadQuests()
		{
			XDocument Document = XDocument.Load(ConfigFilePath);

			return Document.Root.Elements("quest")
				.Select(QuestNode =>
				{
					string Name = QuestNode.Attribute("name").Value;

					List<NarrativeQuestStep> Steps = QuestNode.Elements("step")
					.Select(StepNode => ReadQuestStep(StepNode))
					.ToList();

					return new NarrativeQuest(Name, Steps);
				})
				.ToList();
		}

		private static NarrativeQuestStep ReadQuestStep(XElement Node)
		{
			string Name = Node.Attribute("name").Value;
			bool RequiresAny = bool.Parse(Node.Attribute("any")?.Value ?? "false");

			return new NarrativeQuestStep(Name, ReadConditions(Node), RequiresAny);
		}

		private static List<NarrativeQuestStepCondition> ReadConditions(XElement ConditionsNode)
		{
			return ConditionsNode.Elements()
				.Select<XElement, NarrativeQuestStepCondition>(ConditionNode =>
				{
					string Name = ConditionNode.Name.ToString();

					switch (Name)
					{
						case "locked":
							return new LockedNarrativeQuestStepCondition(Name, bool.Parse(ConditionNode.Value ?? "false"));
						case "flag":
							return new FlagNarrativeQuestStepCondition(Name, SavedFlags);
						case "item":
							return new ItemOwnedNarrativeQuestStepCondition(Name, int.Parse(ConditionNode.Value ?? "0"));
						default:
							throw new InvalidDataException(string.Format("Unexpected condition type: {0}", Name));
					}
				})
				.ToList();
		}

		public static void OnSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
			   SaveFilePath,
			   Writer =>
			   {
				   // Version
				   Writer.Write(0);

				   Writer.Write(SavedFlags.Count);
				   foreach (string Flag in SavedFlags)
				   {
					   Writer.Write(Flag);
				   }
			   });
		}

		public static void OnLoad()
		{
			SavedFlags.Clear();

			Persistence.Deserialize(
			   SaveFilePath,
			   Reader =>
			   {
				   // Version
				   Reader.ReadInt();

				   int Count = Reader.ReadInt();
				   for (int i = 0; i < Count; ++i)
				   {
					   SavedFlags.Add(Reader.ReadString());
				   }
			   });
		}

		[Usage("ToggleQuestStepCondition")]
		private static void OnToggleQuestStepCondition(CommandEventArgs e)
		{
			if (e.Arguments.Length < 3)
			{
				e.Mobile.SendMessage("Missing parameters.");
				return;
			}

			NarrativeQuest Quest = GetQuestByName(e.Arguments[0]);
			if (Quest == null)
			{
				e.Mobile.SendMessage("Quest not found.");
				return;
			}

			NarrativeQuestStep Step = Quest.GetStepByName(e.Arguments[1]);
			if (Step == null)
			{
				e.Mobile.SendMessage("Quest step not found.");
				return;
			}

			NarrativeQuestStepCondition Condition = Step.GetConditionByName(e.Arguments[2]);
			if (Condition == null)
			{
				e.Mobile.SendMessage("Quest step condition not found.");
				return;
			}

			if (!(Condition is FlagNarrativeQuestStepCondition))
			{
				e.Mobile.SendMessage("Quest step condition is not togglable.");
				return;
			}

			bool Toggle = bool.Parse(e.Arguments.ElementAtOrDefault(3) ?? "true");
			
			if (SetFlag(Condition.QualifiedName, Toggle))
			{
				e.Mobile.SendMessage("Quest step condition {0} toggled to {1}.", Condition.QualifiedName, Toggle);
			}
			else
			{
				e.Mobile.SendMessage("Quest step condition {0} unchanged.", Condition.QualifiedName);
			}
		}

		[Usage("GetNextQuestStep")]
		private static void OnGetNextQuestStep(CommandEventArgs e)
		{
			if (e.Arguments.Length < 1)
			{
				e.Mobile.SendMessage("Missing quest name.");
				return;
			}

			NarrativeQuest Quest = GetQuestByName(e.Arguments[0]);
			if (Quest == null)
			{
				e.Mobile.SendMessage("Quest not found.");
				return;
			}

			NarrativeQuestStep Step = Quest.GetNextStep();
			if (Step == null)
			{
				e.Mobile.SendMessage("Quest has no remaining steps.");
				return;
			}

			e.Mobile.SendMessage("Next quest step: {0}", Step.QualifiedName);
		}

		[Usage("GetNextQuestStepIncompleteConditions")]
		private static void OnGetNextQuestStepIncompleteConditions(CommandEventArgs e)
		{
			if (e.Arguments.Length < 1)
			{
				e.Mobile.SendMessage("Missing quest name.");
				return;
			}

			NarrativeQuest Quest = GetQuestByName(e.Arguments[0]);
			if (Quest == null)
			{
				e.Mobile.SendMessage("Quest not found.");
				return;
			}

			List<NarrativeQuestStepCondition> IncompleteConditions = Quest.GetNextStepIncompleteConditions();
			if (IncompleteConditions.Count == 0)
			{
				e.Mobile.SendMessage("Quest has no remaining incomplete step conditions.");
				return;
			}

			e.Mobile.SendMessage("Next quest step incomplete conditions: {0}", string.Join(", ", IncompleteConditions.Select(Condition => Condition.Name)));
		}

		[Usage("IsQuestComplete")]
		private static void OnIsQuestComplete(CommandEventArgs e)
		{
			if (e.Arguments.Length < 1)
			{
				e.Mobile.SendMessage("Missing quest name.");
				return;
			}

			NarrativeQuest Quest = GetQuestByName(e.Arguments[0]);
			if (Quest == null)
			{
				e.Mobile.SendMessage("Quest not found.");
				return;
			}

			e.Mobile.SendMessage("Quest {0} is {1}.", Quest.Name, Quest.IsComplete() ? "complete" : "incomplete");
		}

		public static bool SetFlag(string Name, bool Value)
		{
			if (Value)
			{
				return SavedFlags.Add(Name);
			}
			else
			{
				return SavedFlags.Remove(Name);
			}
		}

		private static NarrativeQuest GetQuestByName(string Name)
		{
			return Quests.FirstOrDefault(Quest => Quest.Name == Name);
		}

		public static bool IsQuestComplete(string Name)
		{
			return GetQuestByName(Name)?.IsComplete() == true;
		}

		public static string GetNextStep(string Name)
		{
			return GetQuestByName(Name)?.GetNextStep()?.Name ?? "";
		}

		public static string GetNextStepIncompleteConditions(string Name)
		{
			return string.Join(", ", GetQuestByName(Name)?.GetNextStepIncompleteConditions()?.Select(Condition => Condition.Name) ?? new List<string>());
		}
	}
}
