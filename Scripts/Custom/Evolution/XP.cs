using System;
using System.IO;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Gumps;
using System.Collections.Generic;
using Server.Custom;

namespace Server
{
  public class XP
  {
    public static TimeSpan m_IntervaleXP = TimeSpan.FromMinutes(10);

    public static void Initialize()
    {
      new XPTimer().Start();
    }

    public class XPTimer : Timer
    {
      public XPTimer()
          : base(m_IntervaleXP, m_IntervaleXP)
      {
        Priority = TimerPriority.OneSecond;
      }

      protected override void OnTick()
      {

    	 int day = (int)(DateTime.Now - CustomPersistence.Ouverture).TotalDays + 1;


        foreach (NetState state in NetState.Instances)
        {
          Mobile m = state.Mobile;

          if (m != null && m is CustomPlayerMobile pm )
		  {

			if (pm.NextFETime <= TimeSpan.FromMinutes(10))
			{
				if (pm.FENormalTotal < day * 3)
				{
					GainFE(pm);
				}

				ResetFETime(pm);
			}		  
		    else
		    {
				  if (pm.LastLoginTime < DateTime.Now - TimeSpan.FromMinutes(10))
				  {
					pm.NextFETime -= TimeSpan.FromMinutes(10);
				  }
				  else
				  {
					pm.NextFETime -= DateTime.Now - pm.LastLoginTime;
				  }
		    }
          }
        }
      }
    }

    public static void ResetFETime(CustomPlayerMobile pm)
    {
			pm.NextFETime = TimeSpan.FromMinutes(30);    
    }


    public static void GainFE(CustomPlayerMobile pm)
    {
      if (pm == null)
        return;

	  pm.FENormalTotal++;

	  pm.SendMessage("Vous obtenez une nouvelle FE !");
    
    }
  }
	/*  public static void SetSkills(CustomPlayerMobile from, int skillcaptotal, double skillcapind)
	  {
		from.SkillsCap = skillcaptotal;

		for (int i = 0; i < from.Skills.Length; ++i)
		{
		  //if (!IsLoreSkill(from.Skills[i]))
		  from.Skills[i].Cap = (double)skillcapind;
		}
	  }
	*/


	/*   public static int GetNeededFE(CustomPlayerMobile pm)
	   {
		 if (pm == null)
		   return 0;

		 int neededFE = 0;

		 if (pm.Niveau > 30)
		 {
		   neededFE = 210 + (12 * (pm.Niveau - 30));
		 }
		 else if (pm.Niveau > 40)
		 {
		   neededFE = 340 + (20 * (pm.Niveau - 40));
		 }
		 else
		 {
		   neededFE = m_FeReqTable[pm.Niveau - 1];
		 }

		 return neededFE;
	   }*/

	/*  public static bool CanEvolve(Mobile from)
	  {
		try
		{
		  if (from is CustomPlayerMobile)
		  {
			CustomPlayerMobile pm = from as CustomPlayerMobile;

			int currentXP = pm.FE;
			int neededXP = GetNeededFE(pm);

			if (currentXP > neededXP)
			{
			  return true;
			}
		  }
		}
		catch (Exception ex)
		{
		  Console.WriteLine(ex.ToString());
		}

		return false;
	  }

	  public static void Evolve(Mobile from)
	  {
		try
		{
		  if (from is CustomPlayerMobile)
		  {
		   CustomPlayerMobile pm = from as CustomPlayerMobile;

			int currentFE = pm.FE;
			int neededFE = GetNeededFE(pm);

			if (currentFE > neededFE)
			{
			  pm.Niveau++;

			  int SkillsCaps = 100;
			  if (pm.Niveau > 0 && pm.Niveau < 31)
			  {
				SkillsCaps = m_SkillCapTotalTable[pm.Niveau - 1];
			  }
			  else if (pm.Niveau > 30)
			  {
				SkillsCaps = 800 + ((pm.Niveau - 30) * 10);
			  }
			  else if (pm.Niveau > 40)
			  {
				SkillsCaps = 900 + ((pm.Niveau - 40) * 5);
			  }

			  double SkillsInd = 45;
			  if (pm.Niveau > 0 && pm.Niveau < 31)
			  {
				SkillsInd = m_SkillCapIndividuelTable[pm.Niveau - 1];
			  }
			  else if (pm.Niveau > 30)
			  {
				SkillsInd = 100;
			  }

			  if (SkillsInd > 100)
				SkillsInd = 100;

			  /*if (SkillsCaps > 800)
				SkillsCaps = 800;*/
	/*
			  SetSkills(pm, SkillsCaps, SkillsInd);
			  SetPAs(pm);

			  pm.SendMessage("Vous gagnez un niveau !");
			}
			else
			  pm.SendMessage("Il vous manque des points d'experiences pour gagner votre niveau !");
		  }
		}
		catch (Exception ex)
		{
		  Console.WriteLine(ex.ToString());
		}
	  }*/
	//  }

}