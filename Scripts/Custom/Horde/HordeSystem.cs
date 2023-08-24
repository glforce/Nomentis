using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using System.Xml.Linq;

namespace Server.Custom.Horde
{
	public struct HordePreset
	{
		public TimeSpan Duration { get; set; }
		public int WaveCount { get; set; }
		public TimeSpan DelayBetweenWaves { get; set; }
		public List<Tuple<Type, int>> SpawnedTypes { get; set; }
		public int MaxAliveCreatures { get; set; }
		public TimeSpan WarningDelay { get; set; }
		public int WarningCount { get; set; }
	}

	public class HordeSystem
	{
		private static readonly string SaveFilePath = Path.Combine("Saves", "Misc", "HordeSystem.bin");
		private static readonly string ConfigFilePath = Path.Combine("Config", "Hordes.xml");

		private static Dictionary<string, HordePreset> HordePresets = new Dictionary<string, HordePreset>();
		private static List<string> HordeWarnings = new List<string>();

		private static Horde CurrentHorde = null;

		public static void Configure()
		{
			EventSink.WorldSave += OnSave;
			EventSink.WorldLoad += OnLoad;

			LoadHordeConfig();

			CommandSystem.Register("StartHorde", AccessLevel.Administrator, StartHorde);
			CommandSystem.Register("EndHorde", AccessLevel.Administrator, EndHorde);

			CommandSystem.Register("HordePresets", AccessLevel.Administrator, ListHordePresets);
		}

		private static void LoadHordeConfig()
		{
			var XmlDocument = XDocument.Load(ConfigFilePath);

			foreach (var Node in XmlDocument.Root.Element("hordes").Elements())
			{
				var Config = new HordePreset()
				{
					Duration = TimeSpan.Parse(Node.Attribute("duration")?.Value ?? "00:01:00"),
					WaveCount = int.Parse(Node.Attribute("waves")?.Value ?? "0"),
					DelayBetweenWaves = TimeSpan.Parse(Node.Attribute("delay")?.Value ?? "00:00:00"),
					SpawnedTypes = LoadHordePresetSpawnedTypes(Node),
					MaxAliveCreatures = int.Parse(Node.Attribute("maxalive")?.Value ?? "0"),
					WarningDelay = TimeSpan.Parse(Node.Attribute("warningdelay")?.Value ?? "00:00:30"),
					WarningCount = int.Parse(Node.Attribute("warningcount")?.Value ?? "0")
				};

				HordePresets.Add(Node.Name.ToString(), Config);
			}

			HordeWarnings = XmlDocument.Root.Element("warnings")
				.Descendants()
				.Select(Warning => Warning.Value)
				.ToList();
		}

		private static List<Tuple<Type, int>> LoadHordePresetSpawnedTypes(XElement HordeNode)
		{
			var SpawnedTypes = new List<Tuple<Type, int>>();

			foreach (var Node in HordeNode.Descendants())
			{
				var Type = SpawnerType.GetType(Node.Attribute("name").Value);
				if (Type != null && Type.IsSubclassOf(typeof(BaseCreature)))
					SpawnedTypes.Add(new Tuple<Type, int>(Type, int.Parse(Node.Attribute("weight").Value)));
			}

			return SpawnedTypes;
		}

		public static void OnSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
			   SaveFilePath,
			   Writer =>
			   {
				   if (IsCurrentHordeActive())
				   {
					   Writer.Write(true);

					   CurrentHorde.Serialize(Writer);
				   }
				   else
					   Writer.Write(false);
			   });
		}

		public static void OnLoad()
		{
			Persistence.Deserialize(
			   SaveFilePath,
			   Reader =>
			   {
				   if (Reader.ReadBool())
				   {
					   CurrentHorde = new Horde();
					   CurrentHorde.Deserialize(Reader);

					   CurrentHorde.Resume();
				   }
			   });
		}

		private static void AskForConfirmation(PlayerMobile Mobile, string Message, Action OnConfirm)
		{
			Mobile.SendGump(new ConfirmCallbackGump(Mobile, "Horde", Message, null, null, (_, __) =>
			{
				OnConfirm();
			}));
		}

		[Usage("StartHorde")]
		private static void StartHorde(CommandEventArgs e)
		{
			var PlayerMobile = e.Mobile as PlayerMobile;

			if (IsCurrentHordeActive())
			{
				PlayerMobile.SendMessage("Horde already in progress.");
				return;
			}

			var PresetName = e.Arguments.ElementAtOrDefault(0) ?? "default";
			HordePreset Preset;
			if (HordePresets.TryGetValue(PresetName, out Preset))
				AskForConfirmation(PlayerMobile, string.Format("Vous allez lancer la horde {0}. Êtes-vous sûr?", PresetName), () =>
				{
					CurrentHorde = new Horde(Preset);
					CurrentHorde.Start();
				});
			else
				PlayerMobile.SendMessage("Could not find horde preset named {0}", PresetName);
		}

		[Usage("EndHorde")]
		private static void EndHorde(CommandEventArgs e)
		{
			if (CurrentHorde?.IsActive() == true)
				AskForConfirmation(e.Mobile as PlayerMobile, "Vous allez interrompre la horde en cours. Êtes-vous sûr?", () =>
				{
					CurrentHorde?.End();
					CurrentHorde = null;
				});
			else
				(e.Mobile as PlayerMobile).SendMessage("No horde in progress.");
		}

		[Usage("HordePresets")]
		private static void ListHordePresets(CommandEventArgs e)
		{
			var PresetNames = String.Join(", ", HordePresets.Keys);

			(e.Mobile as PlayerMobile).SendMessage("Available horde presets: {0}", PresetNames);
		}

		private static bool IsCurrentHordeActive()
		{
			return CurrentHorde?.IsActive() == true;
		}

		public class Horde
		{
			private static readonly int SpawnRangeMin = Config.Get("Horde.SpawnRangeMin", 0);
			private static readonly int SpawnRangeMax = Config.Get("Horde.SpawnRangeMax", 10);

			private int WaveCount = 0;
			private TimeSpan DelayBetweenWaves;
			private List<Type> SpawnedTypes;
			private int MaxAliveCreatures = 0;
			private TimeSpan WarningDelay;
			private int WarningCount = 0;
			private int MaxWarning = 0;

			private DateTime StartTime;
			private DateTime EndTime;

			private Timer Timer = null;

			private List<BaseCreature> SpawnedCreatures = new List<BaseCreature>();

			public Horde() { }

			public Horde(HordePreset Preset)
			{
				Setup(
					Preset.Duration,
					Preset.WaveCount > 0 ? Preset.WaveCount : int.MaxValue,
					Preset.DelayBetweenWaves != TimeSpan.Zero ? Preset.DelayBetweenWaves : TimeSpan.FromTicks(Preset.Duration.Ticks / (WaveCount + 1)),
					GetSpawnedTypesFromWeightedList(Preset.SpawnedTypes),
					Preset.MaxAliveCreatures > 0 ? Preset.MaxAliveCreatures : int.MaxValue,
					Preset.WarningDelay,
					Preset.WarningCount
				);
			}

			private static List<Type> GetSpawnedTypesFromWeightedList(List<Tuple<Type, int>> WeightedList)
			{
				var SpawnedTypes = new List<Type>();

				if (WeightedList.Count == 0)
					SpawnedTypes.Add(typeof(CorruptedHorror));
				else
					foreach (var Entry in WeightedList)
						for (var i = 0; i < Entry.Item2; i++)
							SpawnedTypes.Add(Entry.Item1);

				return SpawnedTypes;
			}

			private void Setup(
				TimeSpan Duration,
				int WaveCount,
				TimeSpan DelayBetweenWaves,
				List<Type> SpawnedTypes,
				int MaxAliveCreatures,
				TimeSpan WarningDelay,
				int WarningCount)
			{
				this.WaveCount = WaveCount;
				this.DelayBetweenWaves = DelayBetweenWaves;
				this.SpawnedTypes = SpawnedTypes;
				this.MaxAliveCreatures = MaxAliveCreatures;
				this.WarningDelay = WarningDelay;
				this.WarningCount = WarningCount;

				MaxWarning = WarningCount;

				StartTime = DateTime.Now;
				EndTime = StartTime.Add(Duration);
			}

			public void Start()
			{
				Trigger();
			}

			public void Resume()
			{
				Trigger();
			}

			public void End()
			{
				Timer?.Stop();

				DestroySpawnedCreatures();

				World.Broadcast(0xff, false, AccessLevel.Player, "The horde recedes...");
			}

			public bool IsActive()
			{
				return WaveCount > 0 && DateTime.Now < EndTime;
			}

			private void Trigger()
			{
				if (IsActive())
				{
					TimeSpan DelayForNextWave;
					if (WarningCount > 0)
					{
						var WarningIndex = Math.Min(MaxWarning - WarningCount, HordeWarnings.Count - 1);

						WarningCount--;

						World.Broadcast(0xff, false, AccessLevel.Player, HordeWarnings[WarningIndex]);

						DelayForNextWave = WarningDelay;
					}
					else
					{
						WaveCount--;

						SpawnCreatures();

						DelayForNextWave = DelayBetweenWaves;
					}

					Timer = Timer.DelayCall(DelayForNextWave, Trigger);
				}
				else
					End();
			}

			private void SpawnCreatures()
			{
				if (SpawnedCreatures.Count(Creature => Creature.Alive) >= MaxAliveCreatures)
					return;

				foreach (var Instance in NetState.Instances)
					if (Instance.Mobile is PlayerMobile)
					{
						var Map = Instance.Mobile.Map;

						var Creature = Activator.CreateInstance(SpawnedTypes[Utility.Random(SpawnedTypes.Count)]) as BaseCreature;

						var SpawnLocation = SafeZones.GetLocationOutsideOfSafeZone(Instance.Mobile, SpawnRangeMin, SpawnRangeMax);
						var ValidSpawnLocation = Map.GetSpawnPosition(new Point3D(SpawnLocation.X, SpawnLocation.Y, Map.GetAverageZ(SpawnLocation.X, SpawnLocation.Y)), SpawnRangeMin);

						if (!SafeZones.IsInSafeZone(Map, ValidSpawnLocation))
						{
							Creature.MoveToWorld(ValidSpawnLocation, Map);

							SpawnedCreatures.Add(Creature);
						}
						else
							// Fail-safe in case the valid spawn position was in another safe zone
							Creature.Delete();
					}
			}

			private void DestroySpawnedCreatures()
			{
				foreach (var Creature in SpawnedCreatures)
				{
					World.RemoveMobile(Creature);
					Creature.Delete();
				}

				SpawnedCreatures.Clear();
			}

			public virtual void Serialize(GenericWriter Writer)
			{
				Writer.Write(EndTime - DateTime.Now);
				Writer.Write(WaveCount);
				Writer.Write(DelayBetweenWaves);
				Writer.Write(SpawnedTypes.Count);
				foreach (var Type in SpawnedTypes)
					Writer.WriteObjectType(Type);

				Writer.Write(MaxAliveCreatures);
				Writer.Write(WarningDelay);
				Writer.Write(WarningCount);

				Writer.Write(SpawnedCreatures.Count);
				foreach (var Creature in SpawnedCreatures)
					if (!Creature.Deleted)
						Writer.Write(Creature.Serial);
			}

			public virtual void Deserialize(GenericReader Reader)
			{
				Setup(
					Reader.ReadTimeSpan(),
					Reader.ReadInt(),
					Reader.ReadTimeSpan(),
					ReadSpawnedTypes(Reader),
					Reader.ReadInt(),
					Reader.ReadTimeSpan(),
					Reader.ReadInt()
					);

				for (var i = 0; i < Reader.ReadInt(); ++i)
				{
					var CreatureMobile = World.FindMobile(Reader.ReadInt());
					if (CreatureMobile != null && CreatureMobile is BaseCreature && CreatureMobile.Alive)
						SpawnedCreatures.Add(CreatureMobile as BaseCreature);
				}
			}

			private static List<Type> ReadSpawnedTypes(GenericReader Reader)
			{
				var Count = Reader.ReadInt();

				var SpawnedTypes = new List<Type>(Count);

				for (var i = 0; i < Count; ++i)
					SpawnedTypes.Add(Reader.ReadObjectType());

				return SpawnedTypes;
			}
		}
	}
}
