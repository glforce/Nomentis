#region References
using Server.Network;
using System.Reflection;
using System;
using System.Collections.Generic;
using Server.Items;
using System.Collections;
using Server.Custom;
using Server.Movement;
using Server.Gumps;
using Server.Multis;
using Server.Scripts.Commands;
using Server.Commands;


#endregion

namespace Server.Mobiles
{


	public partial class CustomPlayerMobile : PlayerMobile
	{

        private int m_TotalNormalFE;
		private int m_TotalRPFE;
        private TimeSpan m_nextFETime;
		private DateTime m_LastFERP;
        private DateTime m_lastLoginTime;
        private int m_Niveau;
        private Metier m_Metier;
        private Classe m_Classe;

        [CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextFETime
		{
			get { return m_nextFETime; }
			set { m_nextFETime = value; }
		}

        [CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastFERP
		{
			get { return m_LastFERP; }
			set { m_LastFERP = value; }
		}


        [CommandProperty(AccessLevel.GameMaster)]
		public int FE { get { return m_TotalRPFE + m_TotalNormalFE; } }


        [CommandProperty(AccessLevel.GameMaster)]
		public int FENormalTotal 
        { 
            get { return m_TotalNormalFE; } 
            set 
            { 
                m_TotalNormalFE = value; 
                CheckLevel();
            }
        }


        [CommandProperty(AccessLevel.GameMaster)]
		public int FERPTotal 
        { 
            get { return m_TotalRPFE; } 
            set 
            { 
                m_TotalRPFE = value; 
                CheckLevel();

            }             
        }

        [CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastLoginTime
		{
			get { return m_lastLoginTime; }
			set { m_lastLoginTime = value; }
		}

        [CommandProperty(AccessLevel.GameMaster)]
		public int Niveau 
        { 
            get 
            { return m_Niveau; } 
            set 
            { 
                int newValue = value;

                if (newValue > 30 )          
                     newValue = 30;               

                if (m_Niveau != newValue)
                {
                     m_Niveau = newValue; 
                     AdjustLvl();
                }              
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
		public Metier Metier
		{
			get => m_Metier;
			set
			{
		    	m_Metier = value;
                AdjustLvl();
				
			}

		}

        [CommandProperty(AccessLevel.GameMaster)]
		public Classe Classe
		{
			get => m_Classe;
			set
			{
		    	m_Classe = value;
                AdjustLvl();
				
			}

		}

    	[CommandProperty(AccessLevel.GameMaster)]
		public int Armure { get => m_Classe.Armor; }

       



        public CustomPlayerMobile()
		{
			
		}

        public CustomPlayerMobile(Serial s)
			: base(s)
		{
			
		}

        public override bool OnEquip(Item item)
		{
			if (this.AccessLevel > AccessLevel.Player)
			{
				return true;
			}

            if (item is BaseArmor)
			{
				int req = 10;

				BaseArmor armor = (BaseArmor)item;

				switch (armor.MaterialType)
				{
					case ArmorMaterialType.Cloth:
						req = 0;
						break;
					case ArmorMaterialType.Leather:
						req = 1;
						break;
					case ArmorMaterialType.Studded:
						req = 2;
						break;
					case ArmorMaterialType.Bone:
						req = 3;
						break;
					case ArmorMaterialType.Ringmail:
						req = 4;
						break;
					case ArmorMaterialType.Chainmail:
						req = 5;
						break;
					case ArmorMaterialType.Plate:
						req = 6;
						break;
					case ArmorMaterialType.Dragon:
						req = 6;
						break;
					case ArmorMaterialType.Wood:
						req = 4;
						break;
					case ArmorMaterialType.Stone:
						req = 4;
						break;
					default:
						req = 10;
						break;
				}

				if (Armure < req)
				{
					SendMessage("Armure requise : " + req);
					return false;
				}
			}

            return base.OnEquip(item);
		}

        public void CheckLevel()
        {
            if (Niveau + 1 > 30)
            {
                return;
            }

            int NewLvl = Niveau;

            while (FE >= XPLevel.GetLevel(NewLvl).FeRequis && NewLvl <= 30)
            {
               NewLvl++;              
            }

            if (NewLvl - 1 > Niveau)
            {
                 SendMessage("Félicitation ! Vous venez de gagner un niveau !");
                 Niveau = NewLvl - 1;

                 if (CanEvolveClass())
                 {
                    SendMessage("Vous pouvez maintenant choisir une nouvelle classe !");
                 }
            }         
        }

        public void AdjustLvl()
        {
            double skillcap =  XPLevel.GetLevel(Niveau).MaxSkill;

            for (int i = 0; i < Skills.Length; ++i)
            {	 
              double skillSpecCap = skillcap;

              if (Metier.IsMetierSkill(Skills[i].SkillName))
              {
                double metierSkill = Metier.GetSkillValue(Skills[i].SkillName);

                if (skillSpecCap > metierSkill)
                {
                    skillSpecCap = metierSkill;
                }
              }
              else
              {
                 double ClasseSkill = Classe.GetSkillValue(Skills[i].SkillName);

                if (skillSpecCap > ClasseSkill)
                {
                    skillSpecCap = ClasseSkill;
                }
              }

              Skills[i].Cap = skillSpecCap;

              if (Skills[i].Value > skillSpecCap)
              {
                Skills[i].Base = skillSpecCap;
              }
            }
        }

        public bool CanEvolveClass()
        {
            if(Classe.LevelToEvolve(m_Classe.ClasseLvl + 1 ) >= m_Niveau)
                return true;
            else
                return false;
        }


        public bool CanEvolveTo(Classe evolution)
        {
            if (this.AccessLevel > AccessLevel.Player)
			{
				return true;
			}
            else if (!CanEvolveClass())
            {
                return false;
            }
            else if(!m_Classe.Evolution.Contains(evolution.ClasseID))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

			switch (version)
			{
        			case 0:
                    {
                        m_Classe = Classe.GetClasse(reader.ReadInt());
                        m_Metier = Metier.GetMetier(reader.ReadInt());
                        m_Niveau = reader.ReadInt();
                        m_lastLoginTime = reader.ReadDateTime();    
                        m_TotalRPFE = reader.ReadInt();
                        m_TotalNormalFE = reader.ReadInt();
                        m_nextFETime = reader.ReadTimeSpan();
                        m_LastFERP = reader.ReadDateTime();				
                        break;
                    }
            }
		}

        public override void Serialize(GenericWriter writer)
        {        
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write(m_Classe.ClasseID);
            writer.Write(m_Metier.MetierID);
            writer.Write(m_Niveau);
            writer.Write(m_lastLoginTime);
            writer.Write(m_TotalRPFE); 
            writer.Write(m_TotalNormalFE); 
            writer.Write(m_nextFETime); 
            writer.Write(m_LastFERP); 
         
        }
    }

}