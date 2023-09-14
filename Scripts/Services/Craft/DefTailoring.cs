using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
    public enum TailorRecipe
    {
        ElvenQuiver = 501,
        QuiverOfFire = 502,
        QuiverOfIce = 503,
        QuiverOfBlight = 504,
        QuiverOfLightning = 505,

        SongWovenMantle = 550,
        SpellWovenBritches = 551,
        StitchersMittens = 552,

        JesterShoes = 560,
        ChefsToque = 561,
        GuildedKilt = 562,
        CheckeredKilt = 563,
        FancyKilt = 564,
        FloweredDress = 565,
        EveningGown = 566,

        TigerPeltChest = 570,
        TigerPeltCollar = 571,
        TigerPeltHelm = 572,
        TigerPeltLegs = 573,
        TigerPeltShorts = 574,
        TigerPeltBustier = 575,
        TigerPeltLongSkirt = 576,
        TigerPeltSkirt = 577,

        DragonTurtleHideArms = 580,
        DragonTurtleHideChest = 581,
        DragonTurtleHideHelm = 582,
        DragonTurtleHideLegs = 583,
        DragonTurtleHideBustier = 584,

        // doom
        CuffsOfTheArchmage = 585,

        KrampusMinionHat = 586,
        KrampusMinionBoots = 587,
        KrampusMinionTalons = 588,

        MaceBelt = 1100,
        SwordBelt = 1101,
        DaggerBelt = 1102,
        ElegantCollar = 1103,
        CrimsonMaceBelt = 1104,
        CrimsonSwordBelt = 1105,
        CrimsonDaggerBelt = 1106,
        ElegantCollarOfFortune = 1107,
        AssassinsCowl = 1108,
        MagesHood = 1109,
        CowlOfTheMaceAndShield = 1110,
        MagesHoodOfScholarlyInsight = 1111,

    }

    public class DefTailoring : CraftSystem
    {
        #region Statics
        private static readonly Type[] m_TailorColorables = new Type[]
   		{
            typeof(GozaMatEastDeed), typeof(GozaMatSouthDeed),
            typeof(SquareGozaMatEastDeed), typeof(SquareGozaMatSouthDeed),
            typeof(BrocadeGozaMatEastDeed), typeof(BrocadeGozaMatSouthDeed),
            typeof(BrocadeSquareGozaMatEastDeed), typeof(BrocadeSquareGozaMatSouthDeed),
            typeof(SquareGozaMatDeed)
   		};

        private static readonly Type[] m_TailorClothNonColorables = new Type[]
        {
            typeof(DeerMask), typeof(BearMask), typeof(OrcMask), typeof(TribalMask), typeof(HornedTribalMask), typeof(CuffsOfTheArchmage)
        };

        // singleton instance
        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefTailoring();

                return m_CraftSystem;
            }
        }
        #endregion

        #region Constructor
        private DefTailoring()
            : base(1, 1, 1.25)// base( 1, 1, 4.5 )
        {
        }

        #endregion

        #region Overrides
        public override SkillName MainSkill => SkillName.Tailoring;

        public override int GumpTitleNumber => 1044005;

        public override CraftECA ECA => CraftECA.ChanceMinusSixtyToFourtyFive;

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.NameNumber == 1157348 || item.NameNumber == 1159225 || item.NameNumber == 1159213 || item.NameNumber == 1159212 ||
                item.NameNumber == 1159211 || item.NameNumber == 1159228 || item.NameNumber == 1159229)
                return 0.05; // 5%

            return 0.5; // 50%
        }

        public override int CanCraft(Mobile from, ITool tool, Type itemType)
        {
            int num = 0;

            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
                return 1044038; // You have worn out your tool!
            else if (!tool.CheckAccessible(from, ref num))
                return num; // The tool must be on your person to use.

            return 0;
        }

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            if (type != typeof(Cloth) && type != typeof(UncutCloth) && type != typeof(AbyssalCloth))
                return false;

            type = item.ItemType;

            bool contains = false;

            for (int i = 0; !contains && i < m_TailorColorables.Length; ++i)
                contains = (m_TailorColorables[i] == type);

            return contains;
        }

        public override bool RetainsColorFromException(CraftItem item, Type type)
        {
            if (item == null || type == null)
                return false;

            if (type != typeof(Cloth) && type != typeof(UncutCloth) && type != typeof(AbyssalCloth))
                return false;

            bool contains = false;

            for (int i = 0; !contains && i < m_TailorClothNonColorables.Length; ++i)
                contains = (m_TailorClothNonColorables[i] == item.ItemType);

            return contains;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x248);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if (makersMark && quality == 2)
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;









            #region jupes

            AddCraft(typeof(Skirt), "Jupes", 1025398, 29.0, 54.0, typeof(Cloth), 1044455, 10, 1044287);
         //   AddCraft(typeof(FurSarong), "Jupes", 1028971, 35.0, 60.0, typeof(Cloth), 1044455, 12, 1044287); // Bug lorsque mis sur le paperdoll...
            index = AddCraft(typeof(Hakama), "Jupes", 1030213, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);
            index = AddCraft(typeof(TattsukeHakama), "Jupes", 1030214, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);

			index = AddCraft(typeof(JupeCourte7), "Jupes", "Jupe Courte Droite", 10.0, 30.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe11), "Jupes", "Jupe Sombre", 10.0, 30.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(JupeCourte3), "Jupes", "Mini jupe à Ceinture", 15.0, 35.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe5), "Jupes", "Jupe à Motifs", 20.0, 40.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(JupeLacee), "Jupes", "Jupe Lacée", 20.0, 40.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe8), "Jupes", "Jupe Provocante à Volants", 30.0, 50.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Pareo), "Jupes", "Jupe Ouverte", 35.0, 55.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Jupe9), "Jupes", "Jupe à Saccoche", 40.0, 60.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(JupeVoiles2), "Jupes", "Jupe à Volant", 45.0, 65.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe), "Jupes", "Jupe à Bande", 45.0, 65.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(JupeLacee3), "Jupes", "Jupe Lacée Droite", 50.0, 70.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
		//	index = AddCraft(typeof(JupeOndule2), "Jupes", "Jupe Ondulée froufrou", 55.0, 75.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(JupeCourte4), "Jupes", "Jupe Barbare", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe7), "Jupes", "Jupe Provocante", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(JupeLacee2), "Jupes", "Jupe Lacée Sombre", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe3), "Jupes", "Jupe Droite", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(JupeCourte5), "Jupes", "Jupe Courte à Vollant", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe6), "Jupes", "Jupe à Plis", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(JupeCourte2), "Jupes", "Jupe Quadrillée", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
          	index = AddCraft(typeof(Jupe4), "Jupes", "Jupe Artisane", 70.0, 90.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Jupe10), "Jupes", "Jupe à Jartelles", 75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Jupe2), "Jupes", "Jupe Délicate", 75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(JupeVoiles), "Jupes", "Jupe à Banquet", 80.0, 100.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(JupeCourte6), "Jupes", "Jupe Courte à Vollant Unie", 80.0, 100.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(PareoCourt), "Jupes", "Paréo", 85.0, 105.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(JupeCourte), "Jupes", "Jupe Courte Lacée", 90.0, 110.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	

      //      index = AddCraft(typeof(GuildedKilt), "Jupes", 1109619, 82.8, 97.8, typeof(Cloth), 1044455, 8, 1044287); // Epee quand equipe...
      //       AddRecipe(index, (int)TailorRecipe.GuildedKilt);

       //     index = AddCraft(typeof(CheckeredKilt), "Jupes", 1109620, 41.4, 56.4, typeof(Cloth), 1044455, 8, 1044287);
       //     AddRecipe(index, (int)TailorRecipe.CheckeredKilt);

       //     index = AddCraft(typeof(FancyKilt), "Jupes", 1109621, 20.7, 25.7, typeof(Cloth), 1044455, 8, 1044287);
       //     AddRecipe(index, (int)TailorRecipe.FancyKilt);





            


            #endregion

            #region Haut


          











            AddCraft(typeof(JesterSuit), "Hauts", 1028095, 8.2, 33.2, typeof(Cloth), 1044455, 24, 1044287);
            AddCraft(typeof(Doublet), "Hauts", 1028059, 0, 25.0, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(Shirt), "Hauts", 1025399, 20.7, 45.7, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(FancyShirt), "Hauts", 1027933, 24.8, 49.8, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(Tunic), "Hauts", 1028097, 00.0, 25.0, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(Surcoat), "Hauts", 1028189, 8.2, 33.2, typeof(Cloth), 1044455, 14, 1044287);
            index = AddCraft(typeof(ElvenShirt), "Hauts", 1032661, 80.0, 105.0, typeof(Cloth), 1044455, 10, 1044287);
            index = AddCraft(typeof(ElvenDarkShirt), "Hauts", 1032662, 80.0, 105.0, typeof(Cloth), 1044455, 10, 1044287);
			index = AddCraft(typeof(Tabar3), "Hauts", "Tabar à cape", 10.0, 30.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Veste3), "Hauts", "Manteau Propre", 10.0, 30.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(TuniqueCeinture), "Hauts", "Tunique à Ceinture", 10.0, 30.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ManteauDore), "Hauts", "Manteau Doré", 20.0, 40.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Bustier), "Hauts", "Bustier ailé", 20.0, 40.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar9), "Hauts", "Grand tabar ouvert", 20.0, 40.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique3), "Hauts", "Tunique à plis", 20.0, 40.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chemise), "Hauts", "Chemise propre", 20.0, 40.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Chemise2), "Hauts", "Chemise noble", 30.0, 50.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Doublet6), "Hauts", "Doublet à bouton", 30.0, 50.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Uniforme), "Hauts", "Manteau d'uniforme",	30.0,	50.0,	typeof(Cloth),"Tissus",	8	,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar5), "Hauts", "Tabar sombre à griffon", 30.0, 50.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar8), "Hauts", "Tabar doré capitoné", 30.0, 50.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar11), "Hauts", "Grand tabar simple", 40.0, 60.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar6), "Hauts", "Tabar à arbre", 40.0, 60.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Veste), "Hauts", "Veste", 40.0, 60.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail10), "Hauts", "Chandail manche ample", 40.0, 60.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chemise6), "Hauts", "Chemise lacée", 40.0, 60.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique10), "Hauts", "Tunique à ceinture", 50.0, 70.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
		//	index = AddCraft(typeof(ChemiseRiche), "Hauts", "Chemise Riche", 50.0, 70.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(VestonRiche), "Hauts", "Veston Riche", 50.0, 70.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChemiseCorset1), "Hauts", "Chemise à Corset 1", 50.0, 70.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChemiseCorset2), "Hauts", "Chemise à Corset 2", 50.0, 70.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CorsetCuir), "Hauts", "Corset de Cuir", 55.0, 75.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CorsetEpaule), "Hauts", "Corset Épaule", 55.0, 75.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CorsetSimple), "Hauts", "Corset Simple", 55.0, 75.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
		//	index = AddCraft(typeof(CorsetTissus), "Hauts", "Corset de Tissus", 55.0, 75.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar7), "Hauts", "Tabar doré", 55.0, 75.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique), "Hauts", "Tunique en peaux", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Tabar1), "Hauts", "Tabar orné", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar10), "Hauts", "Grand tabar de toile", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Doublet4), "Hauts", "Doublet lacé croisé", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique6), "Hauts", "Tunique à voilant", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Doublet5), "Hauts", "Doublet ajusté", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Manteau2), "Hauts", "Manteau ample", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar), "Hauts", "Tabar simple", 65.0, 85.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail7), "Hauts", "Grand chandail", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique2), "Hauts", "Tunique bouffante", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Tunique5), "Hauts", "Tunique sans manche", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique7), "Hauts", "Tunique propre", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Camisole), "Hauts", "Chandail de travail", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Tunique4), "Hauts", "Tunique à cordon", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail), "Hauts", "Chandail distingué", 65.0, 85.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Chandail6), "Hauts", "Doublet demi - manche", 70.0, 90.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Surcoat), "Hauts", "Tunique ajustée", 70.0, 90.0, typeof(Cloth), "Tissus", 14  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Bustier3), "Hauts", "Bustier demi - manche", 70.0, 90.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail4), "Hauts", "Chandail ample", 70.0, 90.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail5), "Hauts", "Chandail propre", 75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Veste2), "Hauts", "Veste manche courte", 75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail2), "Hauts", "Tunique ornée",  75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Doublet2), "Hauts", "Doublet à épaulette", 75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail9), "Hauts", "Chandail bouffant", 75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chemise3), "Hauts", "Chemise longue lacée", 75.0, 95.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique8), "Hauts", "Tunique élégante", 80.0, 100.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Bustier2), "Hauts", "Bustier provocant", 80.0, 100.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail3), "Hauts", "Corset Bouffant", 80.0, 100.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chemise5), "Hauts", "Chemise ajustée", 80.0, 100.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tunique9), "Hauts", "Tunique sombre", 85.0, 105.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Tabar4), "Hauts", "Tabar sombre", 85.0, 105.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Doublet3), "Hauts", "Doublet lacé", 85.0, 105.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chandail8), "Hauts", "Chandail Manche Longue", 85.0, 105.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chemise7), "Hauts", "Manteau élégant", 90.0, 110.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chemise4), "Hauts", "Chemise simple", 90.0, 110.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");

           
       //     AddCraft(typeof(FormalShirt), "Hauts", 1028975, 26.0, 51.0, typeof(Cloth), 1044455, 16, 1044287);
            index = AddCraft(typeof(ClothNinjaJacket), "Hauts", 1030207, 75.0, 100.0, typeof(Cloth), 1044455, 12, 1044287);
            index = AddCraft(typeof(Kamishimo), "Hauts", 1030212, 75.0, 100.0, typeof(Cloth), 1044455, 15, 1044287);
            index = AddCraft(typeof(HakamaShita), "Hauts", 1030215, 40.0, 65.0, typeof(Cloth), 1044455, 14, 1044287);
            index = AddCraft(typeof(MaleKimono), "Hauts", 1030189, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);
            index = AddCraft(typeof(FemaleKimono), "Hauts", 1030190, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);
            index = AddCraft(typeof(JinBaori), "Hauts", 1030220, 30.0, 55.0, typeof(Cloth), 1044455, 12, 1044287);

            #endregion 

            #region  Robes

                       
            AddCraft(typeof(PlainDress), "Robes et toges", 1027937, 12.4, 37.4, typeof(Cloth), 1044455, 10, 1044287);
            AddCraft(typeof(FancyDress), "Robes et toges", 1027935, 33.1, 58.1, typeof(Cloth), 1044455, 12, 1044287);
     //      AddCraft(typeof(GildedDress), "Robes et toges", 1028973, 37.5, 62.5, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(MaleElvenRobe), "Robes et toges", 1032659, 80.0, 105.0, typeof(Cloth), 1044455, 30, 1044287);
            index = AddCraft(typeof(FemaleElvenRobe), "Robes et toges", 1032660, 80.0, 105.0, typeof(Cloth), 1044455, 30, 1044287);

          
			index = AddCraft(typeof(RobeProvocante5), "Robes et toges", "Robe provocante légère", 10.0, 30.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe14), "Robes et toges", "Robe Provocante délicate", 15.0, 35.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe15), "Robes et toges", "Robe légère", 20.0, 40.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(RobeProvocante2), "Robes et toges", "Robe Provocante ornée", 20.0, 40.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe6), "Robes et toges", "Robe à volants", 30.0, 50.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe11), "Robes et toges", "Robe Délicate sombre", 35.0, 55.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe19), "Robes et toges", "Robe sans manche", 40.0, 60.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe2), "Robes et toges", "Robe provocante", 40.0, 60.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe13), "Robes et toges", "Robe Ouverte", 40.0, 60.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(RobeProvocante3), "Robes et toges", "Robe dorée sombre", 45.0, 65.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe9), "Robes et toges", "Robe d'érudit",    45.0,	65.0,	typeof(Cloth),"Tissus",	16	,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeProvocante), "Robes et toges", "Robe Sombre", 50.0, 70.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(RobeProvocante6), "Robes et toges", "Robe dorée", 50.0, 70.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe18), "Robes et toges", "Robe à Col", 50.0, 70.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(RobeCourteLacet), "Robes et toges", "Robe Courte Lacet", 55.0, 75.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeBleudecolte), "Robes et toges", "Robe Bleue decoltée", 55.0, 75.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeLacetCuir), "Robes et toges", "Robe Lacet Cuir", 55.0, 75.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeDessin), "Robes et toges", "Robe à motifs", 60.0, 80.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeCorset), "Robes et toges", "Robe Corset", 60.0, 80.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeNobleDecolte), "Robes et toges", "Robe Noble Decoltée", 60.0, 80.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeDoree), "Robes et toges", "Robe Dorée", 60.0, 80.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RobeMoulante), "Robes et toges", "Robe Moulante", 60.0, 80.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
		//	index = AddCraft(typeof(MaleElvenRobe), "Robes et toges", "Robe à Capuchon", 65.0, 85.0, typeof(Cloth), "Tissus", 30  ,"Vous n'avez pas assez de tissus.");
		//	index = AddCraft(typeof(FemaleElvenRobe), "Robes et toges", "Grande Robe Toge", 65.0, 85.0, typeof(Cloth), "Tissus", 30  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Robe5), "Robes et toges", "Robe artisane", 65.0, 85.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(RobeCourte), "Robes et toges", "Robe Courte", 70.0, 90.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(RobeProvocante4), "Robes et toges", "Robe décoltée", 70.0, 90.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe16), "Robes et toges", "Robe provocante Sombre", 75.0, 95.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe8), "Robes et toges", "Robe lacée large", 75.0, 95.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe12), "Robes et toges", "Robe délicate", 80.0, 100.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe3), "Robes et toges", "Robe manches courtes", 80.0, 100.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");
		//	index = AddCraft(typeof(RobeNim), "Robes et toges", "Robe Nimunique", 80.0, 100.0, typeof(Cloth), "Tissus", 16, "Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Robe7), "Robes et toges", "Robe Simple", 85.0, 105.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Robe10), "Robes et toges", "Robe manches amples", 90.0, 110.0, typeof(Cloth), "Tissus", 16  ,"Vous n'avez pas assez de tissus.");

        
        
            index = AddCraft(typeof(FloweredDress), "Robes et toges", 1109622, 75.0, 90.0, typeof(Cloth), 1044455, 18, 1044287);
            AddRecipe(index, (int)TailorRecipe.FloweredDress);

            index = AddCraft(typeof(EveningGown), "Robes et toges", 1109625, 75, 90.0, typeof(Cloth), 1044455, 18, 1044287);
            AddRecipe(index, (int)TailorRecipe.EveningGown);

            index = AddCraft(typeof(RobeofRite), "Robes et toges", "Robe rituelle", 101.5, 120.0, typeof(Leather), 1044462, 6, 1044253);
            AddRes(index, typeof(FireRuby), 1032695, 1, 1044253);
            AddRes(index, typeof(GoldDust), "Poudre d'or", 5, 1044253);
            AddRes(index, typeof(AbyssalCloth), 1113350, 6, 1044253);
            ForceNonExceptional(index);


            #endregion





            #region Toge

            AddCraft(typeof(Robe), "Robes et toges", 1027939, 53.9, 78.9, typeof(Cloth), 1044455, 16, 1044287);
			index = AddCraft(typeof(Toge), "Robes et toges", "Toge Souple", 50.0, 70.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Toge3), "Robes et toges", "Toge à épaulettes", 55.0, 75.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Toge8), "Robes et toges", "Toge Sombre", 55.0, 75.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(TogeKoraine), "Robes et toges", "Toge Évasée", 60.0, 80.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(TogeSparte), "Robes et toges", "Toge Stylée", 60.0, 80.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Toge5), "Robes et toges", "Toge à ceinture large", 60.0, 80.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Manteau), "Robes et toges", "Toge ornée", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Toge4), "Robes et toges", "Toge en toile", 65.0, 85.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Toge2), "Robes et toges", "Toge Propre", 65.0, 85.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Toge6), "Robes et toges", "Toge à ceinture dorée", 65.0, 85.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Toge7), "Robes et toges", "Toge en voile", 70.0, 90.0, typeof(Cloth), "Tissus", 18  ,"Vous n'avez pas assez de tissus.");	

            #endregion

            #region Pantalon


            AddCraft(typeof(ShortPants), "Pantalons", 1025422, 24.8, 49.8, typeof(Cloth), 1044455, 6, 1044287);
            AddCraft(typeof(LongPants), "Pantalons", 1025433, 24.8, 49.8, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(Kilt), "Pantalons", 1025431, 20.7, 45.7, typeof(Cloth), 1044455, 8, 1044287);
            index = AddCraft(typeof(ElvenPants), "Pantalons", 1032665, 80.0, 105.0, typeof(Cloth), 1044455, 12, 1044287);
			index = AddCraft(typeof(Pantalon7), "Pantalons", "Short Droit", 55.0, 75.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon8), "Pantalons", "Short Ample", 55.0, 75.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon4), "Pantalons", "Pantalon à Poches", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Kilt3), "Pantalons", "Grand Kilt", 60.0, 80.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Pantalon12), "Pantalons", "Pantalon de Toile", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon2), "Pantalons", "Pantalon à Motifs", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon5), "Pantalons", "Pantalon de Cuir", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon6), "Pantalons", "Pantalon Court à Ceinture", 65.0, 85.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon10), "Pantalons", "Pantalon Sombre", 65.0, 85.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon11), "Pantalons", "Pantalon Jupe", 65.0, 85.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Kilt2), "Pantalons", "Kilt à Bandouillère", 70.0, 90.0, typeof(Cloth), "Tissus", 8   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Pantalon9), "Pantalons", "Short de Toile", 70.0, 90.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			
			index = AddCraft(typeof(Salopette), "Pantalons", "Salopette", 80.0, 100.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(CulotteLeopard), "Pantalons", "Pantalons Léopard", 80.0, 100.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Pantalon1), "Pantalons", "Pantalon Ample", 85.0, 105.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Pantalon3), "Pantalons", "Pantalon à Fourrure", 90.0, 110.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	


            index = AddCraft(typeof(WoodlandBelt), "Pantalons", 1032639, 80.0, 105.0, typeof(Cloth), 1044455, 10, 1044287);





            #endregion

            #region Chapeaux

            AddCraft(typeof(SkullCap), "Chapeaux", 1025444, 0.0, 25.0, typeof(Cloth), 1044455, 2, 1044287);
            AddCraft(typeof(Bandana), "Chapeaux", 1025440, 0.0, 25.0, typeof(Cloth), 1044455, 2, 1044287);
            AddCraft(typeof(FloppyHat), "Chapeaux", 1025907, 6.2, 31.2, typeof(Cloth), 1044455, 11, 1044287);
            AddCraft(typeof(Cap), "Chapeaux", 1025909, 6.2, 31.2, typeof(Cloth), 1044455, 11, 1044287);
            AddCraft(typeof(WideBrimHat), "Chapeaux", 1025908, 6.2, 31.2, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(StrawHat), "Chapeaux", 1025911, 6.2, 31.2, typeof(Cloth), 1044455, 10, 1044287);
            AddCraft(typeof(TallStrawHat), "Chapeaux", 1025910, 6.7, 31.7, typeof(Cloth), 1044455, 13, 1044287);
            AddCraft(typeof(WizardsHat), "Chapeaux", 1025912, 7.2, 32.2, typeof(Cloth), 1044455, 15, 1044287);
            AddCraft(typeof(Bonnet), "Chapeaux", 1025913, 6.2, 31.2, typeof(Cloth), 1044455, 11, 1044287);
            AddCraft(typeof(FeatheredHat), "Chapeaux", 1025914, 6.2, 31.2, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(TricorneHat), "Chapeaux", 1025915, 6.2, 31.2, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(JesterHat), "Chapeaux", 1025916, 7.2, 32.2, typeof(Cloth), 1044455, 15, 1044287);

     //      AddCraft(typeof(FlowerGarland), "Chapeaux", 1028965, 10.0, 35.0, typeof(Cloth), 1044455, 5, 1044287);

            AddCraft(typeof(ClothNinjaHood), "Chapeaux", 1030202, 80.0, 105.0, typeof(Cloth), 1044455, 13, 1044287);

            AddCraft(typeof(Kasa), "Chapeaux", 1030211, 60.0, 85.0, typeof(Cloth), 1044455, 12, 1044287);

            AddCraft(typeof(OrcMask), "Chapeaux", 1025147, 75.0, 100.0, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(BearMask), "Chapeaux", 1025445, 77.5, 102.5, typeof(Cloth), 1044455, 15, 1044287);
            AddCraft(typeof(DeerMask), "Chapeaux", 1025447, 77.5, 102.5, typeof(Cloth), 1044455, 15, 1044287);
            AddCraft(typeof(TribalMask), "Chapeaux", 1025449, 82.5, 107.5, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(HornedTribalMask), "Chapeaux", 1025451, 82.5, 107.5, typeof(Cloth), 1044455, 12, 1044287);

      //      index = AddCraft(typeof(ChefsToque), "Chapeaux", 1109618, 6.2, 21.2, typeof(Cloth), 1044455, 11, 1044287);
      //      AddRecipe(index, (int)TailorRecipe.ChefsToque);

    //        index = AddCraft(typeof(KrampusMinionHat), "Chapeaux", 1125639, 100.0, 500.0, typeof(Cloth), 1044455, 8, 1044287);
     //       AddRecipe(index, (int)TailorRecipe.KrampusMinionHat);

        //    index = AddCraft(typeof(AssassinsCowl), "Chapeaux", 1126024, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
        //    AddRes(index, typeof(Leather), 1044462, 5, 1044463);
        //    AddRes(index, typeof(VileTentacles), 1113333, 5, 1044253);
        //    AddRecipe(index, (int)TailorRecipe.AssassinsCowl);

   /*         index = AddCraft(typeof(MagesHood), "Chapeaux", 1159227, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(VoidCore), 1113334, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.MagesHood);

            index = AddCraft(typeof(CowlOfTheMaceAndShield), "Chapeaux", 1159228, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(MaceAndShieldGlasses), 1073381, 1, 1044253);
            AddRes(index, typeof(VileTentacles), 1113333, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CowlOfTheMaceAndShield);
            ForceExceptional(index);

            index = AddCraft(typeof(MagesHoodOfScholarlyInsight), "Chapeaux", 1159229, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(TheScholarsHalo), 1157354, 1, 1044253);
            AddRes(index, typeof(VoidCore), 1113334, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.MagesHoodOfScholarlyInsight);
            ForceExceptional(index);*/

        	index = AddCraft(typeof(Capuche), "Chapeaux", "Capuche", 0.0, 20.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");	
			
			index = AddCraft(typeof(Capuche1), "Chapeaux", "Capuche", 15.0, 35.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChapeauPlume1), "Chapeaux", "Chapeau à Plume Longue", 20.0, 40.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChapeauToc), "Chapeaux", "Petite Toque", 20.0, 40.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChapeauPlume2), "Chapeaux", "Chapeau à Plume Courte", 30.0, 50.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ToquePlume), "Chapeaux", "Toque à Plume", 35.0, 55.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChapeauPirate), "Chapeaux", "Chapeau de Pirate", 40.0, 60.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
		//	index = AddCraft(typeof(CapucheToile), "Chapeaux", "Capuche à Toile", 40.0, 60.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChapeauFoulard), "Chapeaux", "Chapeau Foulard", 40.0, 60.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChapeauMousquetaire), "Chapeaux", "Chapeau Mousquetaire", 45.0, 65.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ChapeauPlume3), "Chapeaux", "Chapeau à Plume", 45.0, 65.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Chale1), "Chapeaux", "Chale voilé", 50.0, 70.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CoiffeEgypte), "Chapeaux", "Coiffe Égyptienne", 50.0, 70.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CoiffeColore), "Chapeaux", "Coiffe Colorée", 50.0, 70.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ToqueBouffon), "Chapeaux", "Toque Bouffon", 55.0, 75.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(MasqueEpouvantail), "Chapeaux", "Masque Epouvantail", 55.0, 75.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");	

			index = AddCraft(typeof(ChapeauMage), "Chapeaux", "majisto", 70.0, 90.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
	//		index = AddCraft(typeof(ClothNinjaHood), "Chapeaux", "Capuche de ninja", 75.0, 95.0, typeof(Cloth), "Tissus", 13  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(VoileTete), "Chapeaux", "Voile", 75.0, 95.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Toque), "Chapeaux", "Toque", 80.0, 100.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Turban), "Chapeaux", "Turban", 80.0, 100.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(VoileTeteLong), "Chapeaux", "Long voile", 85.0, 105.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Capuche2), "Chapeaux", "Grande Capuche", 90.0, 110.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
           
            #endregion

            #region Capes 



            AddCraft(typeof(Cloak), "Capes", 1025397, 41.4, 66.4, typeof(Cloth), 1044455, 14, 1044287);
         //   AddCraft(typeof(FurCape), "Capes", 1028969, 35.0, 60.0, typeof(Cloth), 1044455, 13, 1044287);	
			index = AddCraft(typeof(Cape10), "Capes", "Demi cape en cuir", 40.0, 60.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape), "Capes", "Cape à Vollets", 45.0, 65.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape7), "Capes", "Cape à bande doré", 45.0, 65.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape2), "Capes", "Cape à plumes", 50.0, 70.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape6), "Capes", "Cape en fourrure", 50.0, 70.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape9), "Capes", "Demi cape Distinguée", 55.0, 75.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape5), "Capes", "Cape à rabat", 55.0, 75.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape11), "Capes", "Demi cape élégante", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(CapeBleu), "Capes", "Cape Bleue", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CapePaon), "Capes", "Cape Paon", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Drapee), "Capes", "Drapée", 60.0, 80.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Cape4), "Capes", "Cape à épaulière", 65.0, 85.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(Cape8), "Capes", "Demi cape avec cuir", 65.0, 85.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");	
			index = AddCraft(typeof(VoileDos), "Capes", "Voile De Dos", 65.0, 85.0, typeof(Cloth), "Tissus", 6   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Cape3), "Capes", "Cape à voiles", 70.0, 90.0, typeof(Cloth), "Tissus", 12  ,"Vous n'avez pas assez de tissus.");
	
            #endregion

            #region Accessoire


            index = AddCraft(typeof(CutUpCloth), "Divers", 1044458, 0.0, 0.0, typeof(BoltOfCloth), 1044453, 1, 1044253);
            AddCraftAction(index, CutUpCloth);

            index = AddCraft(typeof(CombineCloth), "Divers", 1044459, 0.0, 0.0, typeof(Cloth), 1044455, 1, 1044253);
            AddCraftAction(index, CombineCloth);

            index = AddCraft(typeof(PowderCharge), "Divers", 1116160, 0.0, 50.0, typeof(Cloth), 1044455, 1, 1044253);
            AddRes(index, typeof(BlackPowder), 1095826, 4, 1044253);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(AbyssalCloth), "Divers", 1113350, 110.0, 160.0, typeof(Cloth), 1044455, 50, 1044253);
            AddRes(index, typeof(CrystallineBlackrock), 1077568, 1, 1044253);
            SetItemHue(index, 2075);

            AddCraft(typeof(OilCloth), "Divers", 1041498, 74.6, 99.6, typeof(Cloth), 1044455, 1, 1044287);



            AddCraft(typeof(BodySash), "Divers", 1025441, 4.1, 29.1, typeof(Cloth), 1044455, 4, 1044287);
            AddCraft(typeof(HalfApron), "Divers", 1025435, 20.7, 45.7, typeof(Cloth), 1044455, 6, 1044287);
            AddCraft(typeof(FullApron), "Divers", 1025437, 29.0, 54.0, typeof(Cloth), 1044455, 10, 1044287);



            AddCraft(typeof(GozaMatEastDeed), "Divers", 1030404, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(GozaMatSouthDeed), "Divers", 1030405, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(SquareGozaMatEastDeed), "Divers", 1030407, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(SquareGozaMatSouthDeed), "Divers", 1030406, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeGozaMatEastDeed), "Divers", 1030408, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeGozaMatSouthDeed), "Divers", 1030409, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeSquareGozaMatEastDeed), "Divers", 1030411, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeSquareGozaMatSouthDeed), "Divers", 1030410, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(SquareGozaMatDeed), "Divers", 1113621, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
			index = AddCraft(typeof(Foulard), "Divers", "Foulard", 0.0, 20.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(BourseCeinture), "Divers", "Bourse de Ceinture", 40.0, 60.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CacheOeil1), "Divers", "Cache Oeil Droit", 40.0, 60.0, typeof(Cloth), "Tissus", 3   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CacheOeil2), "Divers", "Cache Oeil Gauche", 40.0, 60.0, typeof(Cloth), "Tissus", 3   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CacheOeil3), "Divers", "Bandeau Oeil droit", 45.0, 65.0, typeof(Cloth), "Tissus", 3   ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(LargeDiamondPillow), "Divers", "Coussin Diamant", 60.0, 80.0, typeof(Cloth), "Tissus", 25  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(LargeSquarePillow), "Divers", "Coussin Carré", 60.0, 80.0, typeof(Cloth), "Tissus", 20  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(ThrowPillow), "Divers", "Coussin à Motifs", 60.0, 80.0, typeof(Cloth), "Tissus", 20  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Foulard4), "Divers", "Cache - Visage", 75.0, 95.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Foulard2), "Divers", "Foulard épaule", 80.0, 100.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(Cocarde), "Divers", "Cocarde", 40.0, 700.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(BandagesPieds), "Divers", "Bandages Pieds", 40.0, 70.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(BandagesBras), "Divers", "Bandages Bras", 40.0, 70.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(BandagesTaille), "Divers", "Bandages Taille", 40.0, 70.0, typeof(Cloth), "Tissus", 10  ,"Vous n'avez pas assez de tissus.");
            index = AddCraft(typeof(ElvenQuiver), "Divers", "Petit Carquois", 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRecipe(index, (int)TailorRecipe.ElvenQuiver);

            index = AddCraft(typeof(QuiverOfFire), "Divers", 1073109, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(FireRuby), 1032695, 15, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfFire);

            index = AddCraft(typeof(QuiverOfIce), "Divers", 1073110, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(WhitePearl), 1032694, 15, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfIce);

            index = AddCraft(typeof(QuiverOfBlight), "Divers", 1073111, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(Blight), 1032675, 10, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfBlight);

            index = AddCraft(typeof(QuiverOfLightning), "Divers", 1073112, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(Corruption), 1032676, 10, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfLightning);


            AddCraft(typeof(Obi), "Divers", 1030219, 20.0, 45.0, typeof(Cloth), 1044455, 6, 1044287);

            index = AddCraft(typeof(LeatherContainerEngraver), "Divers", 1072152, 75.0, 100.0, typeof(Bone), 1049064, 1, 1049063);
            AddRes(index, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(SpoolOfThread), 1073462, 2, 1073463);
            AddRes(index, typeof(Dyes), 1024009, 1, 1044253);

        /*    index = AddCraft(typeof(MaceBelt), "Divers", 1126020, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(Lodestone), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.MaceBelt);

            index = AddCraft(typeof(SwordBelt), "Divers", 1126021, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(Lodestone), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.SwordBelt);

            index = AddCraft(typeof(DaggerBelt), "Divers", 1159210, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(Lodestone), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.DaggerBelt);

            index = AddCraft(typeof(ElegantCollar), "Divers", 1159224, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(FeyWings), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.ElegantCollar);

            index = AddCraft(typeof(CrimsonMaceBelt), "Divers", 1159211, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(CrimsonCincture), 1075043, 1, 1044253);
            AddRes(index, typeof(Lodestone), 1113348, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CrimsonMaceBelt);
            ForceExceptional(index);

            index = AddCraft(typeof(CrimsonSwordBelt), "Divers", 1159212, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(CrimsonCincture), 1075043, 1, 1044253);
            AddRes(index, typeof(Lodestone), 1113348, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CrimsonSwordBelt);
            ForceExceptional(index);

            index = AddCraft(typeof(CrimsonDaggerBelt), "Divers", 1159213, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(CrimsonCincture), 1075043, 1, 1044253);
            AddRes(index, typeof(Lodestone), 1113348, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CrimsonDaggerBelt);
            ForceExceptional(index);

            index = AddCraft(typeof(ElegantCollarOfFortune), "Divers", 1159225, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(LeurociansMempoOfFortune), 1071460, 1, 1044253);
            AddRes(index, typeof(FeyWings), 1113332, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.ElegantCollarOfFortune);
            ForceExceptional(index);*/



            #endregion

            #region tapis / rideaux

            index = AddCraft(typeof(BlueDecorativeRugDeed), "Divers", "Tapis décoratif bleu", 40.0, 70.0, typeof(Cloth), "Tissus", 50  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(BlueFancyRugDeed), "Divers", "Tapis huppé bleu", 40.0, 70.0, typeof(Cloth), "Tissus", 50  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(BluePlainRugDeed), "Divers", "Tapis bleu", 40.0, 70.0, typeof(Cloth), "Tissus", 50  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(CinnamonFancyRugDeed), "Divers", "Tapis Cannelle", 40.0, 70.0, typeof(Cloth), "Tissus", 50  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(GoldenDecorativeRugDeed), "Divers", "Tapis Doré", 40.0, 70.0, typeof(Cloth), "Tissus", 50  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(PinkFancyRugDeed), "Divers", "Tapis Huppé Rose", 50.0, 80.0, typeof(Cloth), "Tissus", 50  ,"Vous n'avez pas assez de tissus.");
			index = AddCraft(typeof(RedPlainRugDeed), "Divers", "Tapis Rouge", 50.0, 80.0, typeof(Cloth), "Tissus", 50  ,"Vous n'avez pas assez de tissus.");

        

		
            #endregion




            #region Footwear

            index = AddCraft(typeof(Bottes), "Bottes", "Bottes à talon", 10.0, 30.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes2), "Bottes", "Bottes en cuir", 10.0, 30.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes3), "Bottes", "Bottes ajustées", 20.0, 40.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes4), "Bottes", "Bottes lacées", 20.0, 40.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes5), "Bottes", "Bottes à Sangles", 20.0, 40.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes6), "Bottes", "Bottes en cuir large", 25.0, 45.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes7), "Bottes", "Bottes en fourrure", 25.0, 45.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes8), "Bottes", "Bottes en tissus", 25.0, 45.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes9), "Bottes", "Bottes à rebord", 30.0, 50.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(Bottes10), "Bottes", "Botte haute à sangles", 30.0, 50.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(BottesPoils), "Bottes", "Bottes de Poils", 35.0, 55.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(SoulierTissus), "Bottes", "Soulier en Tissus", 40.0, 60.0, typeof(Leather), 1044462, 15, 1044463);
			index = AddCraft(typeof(SandaleCuir), "Bottes", "Sandales en cuir", 40.0, 60.0, typeof(Leather), 1044462, 10, 1044463);

		//	AddCraft(typeof(LeatherTalons), "Bottes", "Soulier en cuir", 50.0, 70.0, typeof(Leather), 1044462, 15, 1044463);
            index = AddCraft(typeof(ElvenBoots), "Bottes", 1072902, 80.0, 105.0, typeof(Leather), 1044462, 15, 1044463);
            AddCraft(typeof(FurBoots), "Bottes", 1028967, 50.0, 75.0, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(NinjaTabi), "Bottes", 1030210, 70.0, 95.0, typeof(Cloth), 1044455, 10, 1044287);
            AddCraft(typeof(SamuraiTabi), "Bottes", 1030209, 20.0, 45.0, typeof(Cloth), 1044455, 6, 1044287);
            AddCraft(typeof(Sandals), "Bottes", 1025901, 12.4, 37.4, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(typeof(Shoes), "Bottes", 1025904, 16.5, 41.5, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(Boots), "Bottes", 1025899, 33.1, 58.1, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(ThighBoots), "Bottes", 1025906, 41.4, 66.4, typeof(Leather), 1044462, 10, 1044463);
          //  AddCraft(typeof(LeatherTalons), "Bottes", 1095728, 40.4, 65.4, typeof(Leather), 1044462, 6, 1044453);
        //    index = AddCraft(typeof(JesterShoes), "Bottes", 1109617, 20.0, 35.0, typeof(Cloth), 1044455, 6, 1044287);
//            AddRecipe(index, (int)TailorRecipe.JesterShoes);

       /*     index = AddCraft(typeof(KrampusMinionBoots), "Bottes", 1125637, 100.0, 500.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Cloth), 1044455, 4, 1044287);
            AddRecipe(index, (int)TailorRecipe.KrampusMinionBoots);*/

    /*        index = AddCraft(typeof(KrampusMinionTalons), "Bottes", 1125644, 100.0, 500.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Cloth), 1044455, 4, 1044287);
            AddRecipe(index, (int)TailorRecipe.KrampusMinionTalons);*/

            #endregion

            #region Leather Armor

            index = AddCraft(typeof(SpellWovenBritches), 1015293, 1072929, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1044253);
            AddRes(index, typeof(Putrefaction), 1032678, 10, 1044253);
            AddRes(index, typeof(Scourge), 1032677, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.SpellWovenBritches);
            ForceNonExceptional(index);

            index = AddCraft(typeof(SongWovenMantle), 1015293, 1072931, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1044253);
            AddRes(index, typeof(Blight), 1032675, 10, 1044253);
            AddRes(index, typeof(Muculent), 1032680, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.SongWovenMantle);
            ForceNonExceptional(index);

            index = AddCraft(typeof(StitchersMittens), 1015293, 1072932, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(CapturedEssence), 1032686, 1, 1044253);
            AddRes(index, typeof(Corruption), 1032676, 10, 1044253);
            AddRes(index, typeof(Taint), 1032679, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.StitchersMittens);
            ForceNonExceptional(index);

            AddCraft(typeof(LeatherGorget), 1015293, 1025063, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(typeof(LeatherCap), 1015293, 1027609, 6.2, 31.2, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(typeof(LeatherGloves), 1015293, 1025062, 51.8, 76.8, typeof(Leather), 1044462, 3, 1044463);
            AddCraft(typeof(LeatherArms), 1015293, 1025061, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(typeof(LeatherLegs), 1015293, 1025067, 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);
            AddCraft(typeof(LeatherChest), 1015293, 1025068, 70.5, 95.5, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherJingasa), 1015293, 1030177, 45.0, 70.0, typeof(Leather), 1044462, 4, 1044463);

            index = AddCraft(typeof(LeatherMempo), 1015293, 1030181, 80.0, 105.0, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(LeatherDo), 1015293, 1030182, 75.0, 100.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherHiroSode), 1015293, 1030185, 55.0, 80.0, typeof(Leather), 1044462, 5, 1044463);

            index = AddCraft(typeof(LeatherSuneate), 1015293, 1030193, 68.0, 93.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherHaidate), 1015293, 1030197, 68.0, 93.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherNinjaPants), 1015293, 1030204, 80.0, 105.0, typeof(Leather), 1044462, 13, 1044463);

            index = AddCraft(typeof(LeatherNinjaJacket), 1015293, 1030206, 85.0, 110.0, typeof(Leather), 1044462, 13, 1044463);

            index = AddCraft(typeof(LeatherNinjaBelt), 1015293, 1030203, 50.0, 75.0, typeof(Leather), 1044462, 5, 1044463);

            index = AddCraft(typeof(LeatherNinjaMitts), 1015293, 1030205, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherNinjaHood), 1015293, 1030201, 90.0, 115.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(LeafChest), 1015293, 1032667, 75.0, 100.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(LeafArms), 1015293, 1032670, 60.0, 85.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeafGloves), 1015293, 1032668, 60.0, 85.0, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(LeafLegs), 1015293, 1032671, 75.0, 100.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(LeafGorget), 1015293, 1032669, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeafTonlet), 1015293, 1032672, 70.0, 95.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(TigerPeltChest), 1015293, 1109626, 90.0, 115.0, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 4, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltChest);

            index = AddCraft(typeof(TigerPeltLegs), 1015293, 1109628, 90.0, 115.0, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 4, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltLegs);

            index = AddCraft(typeof(TigerPeltShorts), 1015293, 1109629, 90.0, 115.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltShorts);

            index = AddCraft(typeof(TigerPeltHelm), 1015293, 1109632, 90.0, 115.0, typeof(Leather), 1044462, 2, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 1, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltHelm);

            index = AddCraft(typeof(TigerPeltCollar), 1015293, 1109633, 90.0, 115.0, typeof(Leather), 1044462, 2, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 1, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltCollar);

            index = AddCraft(typeof(DragonTurtleHideChest), 1015293, 1109634, 101.5, 116.5, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideChest);

            index = AddCraft(typeof(DragonTurtleHideLegs), 1015293, 1109636, 101.5, 116.5, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 4, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideLegs);

            index = AddCraft(typeof(DragonTurtleHideHelm), 1015293, 1109637, 101.5, 116.5, typeof(Leather), 1044462, 2, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 1, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideHelm);

            index = AddCraft(typeof(DragonTurtleHideArms), 1015293, 1109638, 101.5, 116.5, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideArms);

            #endregion


            #region Studded Armor
            AddCraft(typeof(StuddedGorget), 1015293, 1025078, 78.8, 103.8, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(StuddedGloves), 1015293, 1025077, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(StuddedArms), 1015293, 1025076, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);
            AddCraft(typeof(StuddedLegs), 1015293, 1025082, 91.2, 116.2, typeof(Leather), 1044462, 12, 1044463);
            AddCraft(typeof(StuddedChest), 1015293, 1025083, 94.0, 119.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(StuddedMempo), 1015293, 1030216, 80.0, 105.0, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(StuddedDo), 1015293, 1030183, 95.0, 120.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(StuddedHiroSode), 1015293, 1030186, 85.0, 110.0, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(StuddedSuneate), 1015293, 1030194, 92.0, 117.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(StuddedHaidate), 1015293, 1030198, 92.0, 117.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(HideChest), 1015293, 1032651, 85.0, 110.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(HidePauldrons), 1015293, 1032654, 75.0, 100.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(HideGloves), 1015293, 1032652, 75.0, 100.0, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(HidePants), 1015293, 1032655, 92.0, 117.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(HideGorget), 1015293, 1032653, 90.0, 115.0, typeof(Leather), 1044462, 12, 1044463);

            #endregion

            #region Female Armor
            AddCraft(typeof(LeatherShorts), 1015293, 1027168, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(LeatherSkirt), 1015293, 1027176, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(LeatherBustierArms), 1015293, 1027178, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(StuddedBustierArms), 1015293, 1027180, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(FemaleLeatherChest), 1015293, 1027174, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(FemaleStuddedChest), 1015293, 1027170, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(TigerPeltBustier), 1015293, 1109627, 90.0, 115.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 3, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltBustier);

            index = AddCraft(typeof(TigerPeltLongSkirt), 1015293, 1109630, 90.0, 115.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltLongSkirt);

            index = AddCraft(typeof(TigerPeltSkirt), 1015293, 1109631, 90.0, 115.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltSkirt);

            index = AddCraft(typeof(DragonTurtleHideBustier), 1015293, 1109635, 101.5, 116.5, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 3, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideBustier);

            #endregion

            #region Bone Armor
            index = AddCraft(typeof(BoneHelm), 1015293, 1025206, 85.0, 110.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = AddCraft(typeof(BoneGloves), 1015293, 1025205, 89.0, 114.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = AddCraft(typeof(BoneArms), 1015293, 1025203, 92.0, 117.0, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(Bone), 1049064, 4, 1049063);

            index = AddCraft(typeof(BoneLegs), 1015293, 1025202, 95.0, 120.0, typeof(Leather), 1044462, 10, 1044463);
            AddRes(index, typeof(Bone), 1049064, 6, 1049063);

            index = AddCraft(typeof(BoneChest), 1015293, 1025199, 96.0, 121.0, typeof(Leather), 1044462, 12, 1044463);
            AddRes(index, typeof(Bone), 1049064, 10, 1049063);

            index = AddCraft(typeof(OrcHelm), 1015293, 1027947, 90.0, 115.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Bone), 1049064, 4, 1049063);

            index = AddCraft(typeof(CuffsOfTheArchmage), 1015293, 1157348, 120.0, 120.1, typeof(Cloth), 1044455, 8, 1044287);
            AddRes(index, typeof(MidnightBracers), 1061093, 1, 1044253);
            AddRes(index, typeof(BloodOfTheDarkFather), 1157343, 5, 1044253);
            AddRes(index, typeof(DarkSapphire), 1032690, 4, 1044253);
            ForceNonExceptional(index);
            AddRecipe(index, (int)TailorRecipe.CuffsOfTheArchmage);

            #region Bottes
			
			#endregion

			#region Ceintures

			index = AddCraft(typeof(Ceinture), "Divers", "Ceinture boucle ronde", 10.0, 30.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture2), "Divers", "Ceinture boucle carrée", 10.0, 30.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture3), "Divers", "Ceinture d'artisan", 20.0, 40.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture4), "Divers", "Ceinture à pochettes", 20.0, 40.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture5), "Divers", "Ceinture mince", 30.0, 50.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture6), "Divers", "Ceinture poche à gauche", 30.0, 50.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture7), "Divers", "Ceinture en tissu", 30.0, 50.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture8), "Divers", "Ceinture en bandouillère", 40.0, 60.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Ceinture9), "Divers", "Bourse carrée", 40.0, 60.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(CeintureBaril), "Divers", "Ceinture Barril", 40.0, 60.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(CeintureMetal), "Divers", "Ceinture Métalique", 50.0, 70.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(CentureDoreeLarge), "Divers", "Ceinture Dorée Large", 50.0, 70.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");

			#endregion

			#region Masques
		
			
			index = AddCraft(typeof(CoiffeGuepard), "Chapeaux", "Coiffe Guepard", 30.0, 50.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(CoiffeLoupBlanc), "Chapeaux", "Coiffe Loup Blanc", 30.0, 50.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(CoiffeLion), "Chapeaux", "Coiffe Lion", 30.0, 50.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(CoiffeSanglier), "Chapeaux", "Coiffe Sanglier", 30.0, 50.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(TeteCoyote), "Chapeaux", "Tete de Coyote", 40.0, 60.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(TeteTaureau), "Chapeaux", "Tete de Taureau", 40.0, 60.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque1), "Chapeaux", "Masque ossement de cerf", 10.0, 30.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque2), "Chapeaux", "Masque Ossement d'élan", 10.0, 30.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque3), "Chapeaux", "Masque Crâne", 25.0, 35.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque4), "Chapeaux", "Masque Crâne à piques", 25.0, 35.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque5), "Chapeaux", "Masque du Sage à cornes", 35.0, 55.0, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque6), "Chapeaux", "Masque de plumes fines", 40, 60, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque7), "Chapeaux", "Masque simple", 40, 60, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque8), "Chapeaux", "Masque Vénitien", 40, 60, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque9), "Chapeaux", "Masque-foulard", 40, 60, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque10), "Chapeaux", "Lunettes d'aveugle", 40, 60, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque11), "Chapeaux", "Bandeau oeil droit", 50, 70, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque12), "Chapeaux", "Masque Crâne a foulard", 50, 70, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque13), "Chapeaux", "Masque de soirée", 50, 70, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque14), "Chapeaux", "Masque Festif", 50, 70, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque15), "Chapeaux", "Masque du phénix", 70, 90, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque16), "Chapeaux", "Masque simple à foulard", 70, 90, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque17), "Chapeaux", "Masque doré", 70, 90, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			index = AddCraft(typeof(Masque18), "Chapeaux", "Masque partiel orné", 70, 90, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");
			//index = AddCraft(typeof(Masque19), "Chapeaux", "Masque", 70, 90, typeof(Leather), "cuir", 10, "Vous n'avez pas assez de cuir.");




			#endregion

            #endregion

            // Set the overridable material
            SetSubRes(typeof(Leather), 1049150);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            AddSubRes(typeof(Leather), 1049150, 0.0, 1044462, 1049312);
            AddSubRes(typeof(SpinedLeather), 1049151, 65.0, 1044462, 1049312);
            AddSubRes(typeof(HornedLeather), 1049152, 80.0, 1044462, 1049312);
            AddSubRes(typeof(BarbedLeather), 1049153, 99.0, 1044462, 1049312);

            MarkOption = true;
            Repair = true;
            CanEnhance = true;
            CanAlter = true;
        } 
        #endregion

        private void CutUpCloth(Mobile m, CraftItem craftItem, ITool tool)
        {
            PlayCraftEffect(m);

            Timer.DelayCall(TimeSpan.FromSeconds(Delay), () =>
                {
                    if (m.Backpack == null)
                    {
                        m.SendGump(new CraftGump(m, this, tool, null));
                    }

                    Dictionary<int, int> bolts = new Dictionary<int, int>();
                    List<Item> toConsume = new List<Item>();
                    object num = null;
                    Container pack = m.Backpack;

                    foreach (Item item in pack.Items)
                    {
                        if (item.GetType() == typeof(BoltOfCloth))
                        {
                            if (!bolts.ContainsKey(item.Hue))
                            {
                                toConsume.Add(item);
                                bolts[item.Hue] = item.Amount;
                            }
                            else
                            {
                                toConsume.Add(item);
                                bolts[item.Hue] += item.Amount;
                            }
                        }
                    }

                    if (bolts.Count == 0)
                    {
                        num = 1044253; // You don't have the components needed to make that.
                    }
                    else
                    {
                        foreach (Item item in toConsume)
                        {
                            item.Delete();
                        }

                        foreach (KeyValuePair<int, int> kvp in bolts)
                        {
                            UncutCloth cloth = new UncutCloth(kvp.Value * 50)
                            {
                                Hue = kvp.Key
                            };

                            DropItem(m, cloth, tool);
                        }
                    }

                    if (tool != null)
                    {
                        tool.UsesRemaining--;

                        if (tool.UsesRemaining <= 0 && !tool.Deleted)
                        {
                            tool.Delete();
                            m.SendLocalizedMessage(1044038);
                        }
                        else
                        {
                            m.SendGump(new CraftGump(m, this, tool, num));
                        }
                    }

                    ColUtility.Free(toConsume);
                    bolts.Clear();
                });
        }

        private void CombineCloth(Mobile m, CraftItem craftItem, ITool tool)
        {
            PlayCraftEffect(m);

            Timer.DelayCall(TimeSpan.FromSeconds(Delay), () =>
                {
                    if (m.Backpack == null)
                    {
                        m.SendGump(new CraftGump(m, this, tool, null));
                    }

                    Container pack = m.Backpack;

                    Dictionary<int, int> cloth = new Dictionary<int, int>();
                    List<Item> toConsume = new List<Item>();
                    object num = null;

                    foreach (Item item in pack.Items)
                    {
                        Type t = item.GetType();

                        if (t == typeof(UncutCloth) || t == typeof(Cloth) || t == typeof(CutUpCloth))
                        {
                            if (!cloth.ContainsKey(item.Hue))
                            {
                                toConsume.Add(item);
                                cloth[item.Hue] = item.Amount;
                            }
                            else
                            {
                                toConsume.Add(item);
                                cloth[item.Hue] += item.Amount;
                            }
                        }
                    }

                    if (cloth.Count == 0)
                    {
                        num = 1044253; // You don't have the components needed to make that.
                    }
                    else
                    {
                        foreach (Item item in toConsume)
                        {
                            item.Delete();
                        }

                        foreach (KeyValuePair<int, int> kvp in cloth)
                        {
                            UncutCloth c = new UncutCloth(kvp.Value)
                            {
                                Hue = kvp.Key
                            };

                            DropItem(m, c, tool);
                        }
                    }

                    if (tool != null)
                    {
                        tool.UsesRemaining--;

                        if (tool.UsesRemaining <= 0 && !tool.Deleted)
                        {
                            tool.Delete();
                            m.SendLocalizedMessage(1044038);
                        }
                        else
                        {
                            m.SendGump(new CraftGump(m, this, tool, num));
                        }
                    }

                    ColUtility.Free(toConsume);
                    cloth.Clear();
                });
        }

        private void DropItem(Mobile from, Item item, ITool tool)
        {
            if (tool is Item && ((Item)tool).Parent is Container)
            {
                Container cntnr = (Container)((Item)tool).Parent;

                if (!cntnr.TryDropItem(from, item, false))
                {
                    if (cntnr != from.Backpack)
                        from.AddToBackpack(item);
                    else
                        item.MoveToWorld(from.Location, from.Map);
                }
            }
            else
            {
                from.AddToBackpack(item);
            }
        }
    }
}
