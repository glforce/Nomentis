using Server.Custom.Horde;

namespace Server.Mobiles.Corrupted
{
	public abstract class BaseCorrupted : BaseCreature
	{
		public BaseCorrupted(AIType AI, FightMode Mode, int RangePerception, int RangeFight, double ActiveSpeed, double PassiveSpeed)
			: base(AI, Mode, RangePerception, RangeFight, ActiveSpeed, PassiveSpeed)
		{

		}

		public BaseCorrupted(Serial Serial):base(Serial)
		{

		}

		public override bool CheckMovementTo(Point3D Location, Map Map)
		{
			return !SafeZones.IsInSafeZone(Map, Location);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
		}
	}
}
