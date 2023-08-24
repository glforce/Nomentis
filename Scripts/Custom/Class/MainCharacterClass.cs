using System.Collections.Generic;
using Server.Items;

namespace Server.Custom.Class
{
	public class MainCharacterClass : CharacterClass
	{
		private List<ArmorMaterialType> m_AllowedArmorMaterialTypes;

		public MainCharacterClass(
			int ID,
			string Name,
			Dictionary<SkillName, double> SkillCaps,
			int RequiredLevel,
			List<int> Evolutions,
			bool Hidden,
			List<ArmorMaterialType> AllowedArmorMaterialTypes
			) : base(ID, Name, SkillCaps, RequiredLevel, Evolutions, Hidden)
		{
			m_AllowedArmorMaterialTypes = AllowedArmorMaterialTypes;
		}

		public List<ArmorMaterialType> AllowedArmorMaterialTypes
		{
			get { return m_AllowedArmorMaterialTypes; }
		}

		public bool IsArmorAllowed(BaseArmor Armor)
		{
			return m_AllowedArmorMaterialTypes.Contains(Armor.MaterialType);
		}
	}
}
