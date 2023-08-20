using System;
using System.Linq;
using System.Runtime.InteropServices;
using Server.Items;
using System.Collections.Generic;

namespace Server.Misc
{
 		
        class MetierInit 
        {
				public static void Configure()
					{
						Metier.RegisterMetier(new Metier(0, "Aucun",new Dictionary<SkillName, double>() {  }));
						Metier.RegisterMetier(new Metier(1, "Forgeron",new Dictionary<SkillName, double>() 
						{  
							{SkillName.Blacksmith, 100},
							{SkillName.Mining, 100},
							{SkillName.ArmsLore, 100}
						}));
						Metier.RegisterMetier(new Metier(2, "Alchemiste",new Dictionary<SkillName, double>() 
						{  
							{SkillName.Alchemy, 100},
							{SkillName.Fishing, 100},
							{SkillName.Cooking, 100},
							{SkillName.TasteID, 100}
						}));
						Metier.RegisterMetier(new Metier(3, "Menusier",new Dictionary<SkillName, double>() 
						{  
							{SkillName.Fletching, 100},
							{SkillName.Carpentry, 100},
							{SkillName.Lumberjacking, 100},
							{SkillName.ArmsLore, 100}
						}));
						Metier.RegisterMetier(new Metier(4, "Enchanteur",new Dictionary<SkillName, double>() 
						{  
							{SkillName.Imbuing, 100},
							{SkillName.Inscribe, 100},
							{SkillName.Cartography, 100}
						}));
						Metier.RegisterMetier(new Metier(5, "Couturier",new Dictionary<SkillName, double>() 
						{  
							{SkillName.Tailoring, 100},
							{SkillName.Tinkering, 100},
							{SkillName.Lumberjacking, 50},
							{SkillName.Mining, 50}
						}));
						Metier.RegisterMetier(new Metier(6, "Extracteur",new Dictionary<SkillName, double>() 
						{  			
							{SkillName.Lumberjacking, 100},
							{SkillName.Mining, 100}, 
							{SkillName.Fishing, 100}
						}));
					}
		}
}
