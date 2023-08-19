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
}