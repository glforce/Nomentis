using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using System.Xml;

namespace Server.Services.Horde
{
	public struct HordeConfig
	{
		public TimeSpan Duration { get; set; }
		public uint WaveCount { get; set; }
		public TimeSpan DelayBetweenWaves { get; set; }
		public List<Tuple<Type, int>> SpawnedTypes { get; set; }
	}

	public class HordeSystem
	{
		private static readonly string SaveFilePath = Path.Combine("Saves", "Misc", "HordeSystem.bin");
		private static readonly string ConfigsFilePath = Path.Combine("Config", "Hordes.xml");

		private static Dictionary<string, HordeConfig> HordeConfigs = new Dictionary<string, HordeConfig>();

		private static Horde CurrentHorde = null;

		public static void Configure()
		{
			EventSink.WorldSave += OnSave;
			EventSink.WorldLoad += OnLoad;

			LoadHordeConfigs();

			CommandSystem.Register("StartHorde", AccessLevel.Administrator, StartHorde);
			CommandSystem.Register("EndHorde", AccessLevel.Administrator, EndHorde);

			CommandSystem.Register("HordeConfigs", AccessLevel.Administrator, ListHordeConfigs);
		}

		private static void LoadHordeConfigs()
		{
			XmlDocument XmlDocument = new XmlDocument();
			XmlDocument.Load(ConfigsFilePath);
			foreach (XmlNode Node in XmlDocument.GetElementsByTagName("hordes")[0].ChildNodes)
			{
				HordeConfig Config = new HordeConfig()
				{
					Duration = TimeSpan.FromSeconds(double.Parse(Node.Attributes["duration"]?.Value ?? "1")),
					WaveCount = uint.Parse(Node.Attributes["waves"]?.Value ?? "0"),
					DelayBetweenWaves = TimeSpan.FromSeconds(double.Parse(Node.Attributes["delay"]?.Value ?? "0")),
					SpawnedTypes = LoadHordeConfigSpawnedTypes(Node)
				};

				HordeConfigs.Add(Node.Name, Config);
			}
		}

		private static List<Tuple<Type, int>> LoadHordeConfigSpawnedTypes(XmlNode HordeNode)
		{
			List<Tuple<Type, int>> SpawnedTypes = new List<Tuple<Type, int>>();

			foreach(XmlNode Node in HordeNode.ChildNodes)
			{
				Type Type = SpawnerType.GetType(Node.Attributes["name"].Value);
				if (Type != null && Type.IsSubclassOf(typeof(BaseCreature)))
				{
					SpawnedTypes.Add(new Tuple<Type, int>(Type, int.Parse(Node.Attributes["weight"].Value)));
				}
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
				   {
					   Writer.Write(false);
				   }
			   });
		}

		public static void OnLoad()
		{
			Persistence.Deserialize(
			   SaveFilePath,
			   Reader =>
			   {
				   if(Reader.ReadBool())
				   {
					   CurrentHorde = new Horde();
					   CurrentHorde.Deserialize(Reader);

					   CurrentHorde.Resume();
				   }
			   });
		}

		[Usage("StartHorde")]
		private static void StartHorde(CommandEventArgs e)
		{
			PlayerMobile PlayerMobile = e.Mobile as PlayerMobile;

			if (IsCurrentHordeActive())
			{
				PlayerMobile.SendMessage("Horde already in progress.");
				return;
			}

			string ConfigName = e.Arguments.ElementAtOrDefault(0) ?? "default";
			HordeConfig Config;
			if (HordeConfigs.TryGetValue(ConfigName, out Config))
			{
				CurrentHorde = new Horde(Config);
				CurrentHorde.Start();
			}
			else
			{
				PlayerMobile.SendMessage("Could not find horde config named {0}", ConfigName);
			}
		}

		[Usage("EndHorde")]
		private static void EndHorde(CommandEventArgs e)
		{
			if (CurrentHorde?.IsActive() == true)
			{
				CurrentHorde?.End();
				CurrentHorde = null;
			}
			else
			{
				(e.Mobile as PlayerMobile).SendMessage("No horde in progress.");
			}
		}

		[Usage("HordeConfigs")]
		private static void ListHordeConfigs(CommandEventArgs e)
		{
			string ConfigNames = String.Join(", ", HordeConfigs.Keys);

			(e.Mobile as PlayerMobile).SendMessage("Available horde configs: {0}", ConfigNames);
		}

		private static bool IsCurrentHordeActive()
		{
			return CurrentHorde?.IsActive() == true;
		}

		public class Horde
		{
			private static readonly int NumberOfSpawnLocationsByPlayer = Config.Get("Horde.NumberOfSpawnLocationsByPlayer", 10);
			private static readonly int SpawnDistanceFromPlayer = Config.Get("Horde.SpawnDistanceFromPlayer", 10);

			private TimeSpan Duration;
			private uint WaveCount = 0;
			private TimeSpan DelayBetweenWaves;
			private List<Type> SpawnedTypes;

			private DateTime StartTime;
			private DateTime EndTime;

			private Timer WaveTimer = null;

			private List<BaseCreature> SpawnedCreatures = new List<BaseCreature>();

			public Horde() { }

			public Horde(HordeConfig Config)
			{
				Setup(
					Config.Duration,
					Config.WaveCount > 0 ? Config.WaveCount : int.MaxValue,
					Config.DelayBetweenWaves != TimeSpan.Zero ? Config.DelayBetweenWaves : TimeSpan.FromTicks(Config.Duration.Ticks / (WaveCount + 1)),
					GetSpawnedTypesFromWeightedList(Config.SpawnedTypes)
				);
			}

			private static List<Type> GetSpawnedTypesFromWeightedList(List<Tuple<Type, int>> WeightedList)
			{
				List<Type> SpawnedTypes = new List<Type>();

				if (WeightedList.Count == 0)
				{
					SpawnedTypes.Add(typeof(CorruptedHorror));
				}
				else
				{
					foreach(Tuple<Type, int> Entry in WeightedList)
					{
						for(int i = 0; i < Entry.Item2; i++)
						{
							SpawnedTypes.Add(Entry.Item1);
						}
					}
				}

				return SpawnedTypes;
			}

			private void Setup(TimeSpan Duration, uint WaveCount, TimeSpan DelayBetweenWaves, List<Type> SpawnedTypes)
			{
				this.Duration = Duration;
				this.WaveCount = WaveCount;
				this.DelayBetweenWaves = DelayBetweenWaves;
				this.SpawnedTypes = SpawnedTypes;

				StartTime = DateTime.Now;
				EndTime = StartTime.Add(Duration);
			}

			public void Start()
			{
				World.Broadcast(0xff, false, AccessLevel.Player, "The horde is shuffling...");

				SpawnWave();
			}

			public void Resume()
			{
				SpawnWave();
			}

			public void End()
			{
				WaveTimer?.Stop();

				DestroySpawnedCreatures();

				World.Broadcast(0xff, false, AccessLevel.Player, "The horde recedes...");
			}

			public bool IsActive() {
				return WaveCount > 0 && DateTime.Now < EndTime;
			}

			private void SpawnWave()
			{
				if (IsActive())
				{
					WaveCount--;

					SpawnCreatures();

					WaveTimer = Timer.DelayCall(DelayBetweenWaves, SpawnWave);
				}
				else
				{
					End();
				}
			}

			private void SpawnCreatures()
			{
				foreach (NetState Instance in NetState.Instances)
				{
					if (Instance.Mobile is PlayerMobile)
					{
						Map Map = Instance.Mobile.Map;
						for (int i = 0; i < NumberOfSpawnLocationsByPlayer; ++i)
						{
							BaseCreature Creature = Activator.CreateInstance(SpawnedTypes[Utility.Random(SpawnedTypes.Count)]) as BaseCreature;

							Point2D SpawnLocation = SafeZones.GetLocationOutsideOfSafeZone(Instance.Mobile, 10, 20);
							Creature.MoveToWorld(new Point3D(SpawnLocation.X, SpawnLocation.Y, Map.GetAverageZ(SpawnLocation.X, SpawnLocation.Y)), Map);

							SpawnedCreatures.Add(Creature);
						}
					}
				}
			}

			private void DestroySpawnedCreatures()
			{
				foreach(BaseCreature Creature in SpawnedCreatures)
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
				foreach(Type Type in SpawnedTypes)
				{
					Writer.WriteObjectType(Type);
				}

				Writer.Write(SpawnedCreatures.Count);
				foreach (BaseCreature Creature in SpawnedCreatures)
				{
					if (!Creature.Deleted)
					{
						Writer.Write(Creature.Serial);
					}
				}
			}

			public virtual void Deserialize(GenericReader Reader)
			{
				Setup(Reader.ReadTimeSpan(), Reader.ReadUInt(), Reader.ReadTimeSpan(), ReadSpawnedTypes(Reader));

				for(int i = 0; i < Reader.ReadInt(); ++i)
				{
					Mobile CreatureMobile = World.FindMobile(Reader.ReadInt());
					if (CreatureMobile != null && CreatureMobile is BaseCreature && CreatureMobile.Alive)
					{
						SpawnedCreatures.Add(CreatureMobile as BaseCreature);
					}
				}
			}

			private static List<Type> ReadSpawnedTypes(GenericReader Reader)
			{
				int Count = Reader.ReadInt();

				List<Type> SpawnedTypes = new List<Type>(Count);

				for (int i = 0; i < Count; ++i) 
				{
					SpawnedTypes.Add(Reader.ReadObjectType());
				}

				return SpawnedTypes;
			}
		}
	}
}
