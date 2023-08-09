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
		public double Duration { get; set; }
		public uint WaveCount { get; set; }
		public double DelayBetweenWaves { get; set; }
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
					Duration = double.Parse(Node.Attributes["duration"]?.Value ?? "1"),
					WaveCount = uint.Parse(Node.Attributes["waves"]?.Value ?? "0"),
					DelayBetweenWaves = double.Parse(Node.Attributes["delay"]?.Value ?? "0")
				};

				HordeConfigs.Add(Node.Name, Config);
			}
		}

		public static void OnSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
			   SaveFilePath,
			   writer =>
			   {
				   if (IsCurrentHordeActive())
				   {
					   writer.Write(true);

					   CurrentHorde.Serialize(writer);
				   }
				   else
				   {
					   writer.Write(false);
				   }
			   });
		}

		public static void OnLoad()
		{
			Persistence.Deserialize(
			   SaveFilePath,
			   reader =>
			   {
				   if(reader.ReadBool())
				   {
					   CurrentHorde = new Horde();
					   CurrentHorde.Deserialize(reader);

					   CurrentHorde.Resume();
				   }
			   });
		}

		[Usage("StartHorde")]
		public static void StartHorde(CommandEventArgs e)
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
		public static void EndHorde(CommandEventArgs e)
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
		public static void ListHordeConfigs(CommandEventArgs e)
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

			private DateTime StartTime;
			private DateTime EndTime;

			private Timer WaveTimer = null;

			private List<BaseCreature> SpawnedCreatures = new List<BaseCreature>();

			public Horde() { }

			public Horde(HordeConfig Config)
			{
				Setup(
					TimeSpan.FromSeconds(Config.Duration),
					Config.WaveCount > 0 ? Config.WaveCount : int.MaxValue,
					TimeSpan.FromSeconds(Config.DelayBetweenWaves > 0.0 ? Config.DelayBetweenWaves : Config.Duration / (WaveCount + 1))
				);
			}

			private void Setup(TimeSpan Duration, uint WaveCount, TimeSpan DelayBetweenWaves)
			{
				this.Duration = Duration;
				this.WaveCount = WaveCount;
				this.DelayBetweenWaves = DelayBetweenWaves;

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
						Point3D Origin = Instance.Mobile.Location;
						Map Map = Instance.Mobile.Map;
						for (int i = 0; i < NumberOfSpawnLocationsByPlayer; ++i)
						{
							BaseCreature Creature = new CorruptedHorror();
							Creature.MoveToWorld(Map.GetSpawnPosition(Origin, SpawnDistanceFromPlayer), Map);

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

			public virtual void Serialize(GenericWriter writer)
			{
				writer.Write(EndTime - DateTime.Now);
				writer.Write(WaveCount);
				writer.Write(DelayBetweenWaves);
				writer.Write(SpawnedCreatures.Count());
				foreach (BaseCreature Creature in SpawnedCreatures)
				{
					writer.Write(Creature.Serial);
				}
			}

			public virtual void Deserialize(GenericReader reader)
			{
				Setup(reader.ReadTimeSpan(), reader.ReadUInt(), reader.ReadTimeSpan());

				for(int i = 0; i < reader.ReadInt(); ++i)
				{
					Mobile CreatureMobile = World.FindMobile(reader.ReadInt());
					if (CreatureMobile != null && CreatureMobile is BaseCreature && CreatureMobile.Alive)
					{
						SpawnedCreatures.Add(CreatureMobile as BaseCreature);
					}
				}
			}
		}
	}
}
