using Server.Custom.Class;
using Server.Custom.Mobiles;
using Server.Network;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
	public class ClasseIndexGump : CustomBaseGump
	{
        private CustomPlayerMobile From;
        private CustomPlayerMobile Target;

        public ClasseIndexGump(CustomPlayerMobile From, CustomPlayerMobile Target)
            : base("Classes Index", 560, 622, true)
        {
			From.CloseGump(typeof(ClasseIndexGump));

            this.From = From;
            this.Target = Target;

			int x = XBase;
			int y = YBase;

			From.InvalidateProperties();
			Target.InvalidateProperties();

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

			int yLine = 0;

			CharacterClasses.MainCharacterClasses[Target.Race.RaceID].Values
				.Where(Class =>
				{
					return Class.Level == 0 && !Class.Hidden;
				})
				.ToList()
				.ForEach(Class =>
				{
					AddButtonHtml(x + 10, y + yLine * 20 + 40, 1000 + Class.ID, Class.Name, "#FFFFFF");
					yLine++;
				});
		}

		public override void OnResponse(NetState Sender, RelayInfo Info)
        {
			if (Info.ButtonID >= 1000)
			{
				int NewClassId = Info.ButtonID - 1000;

				From.SendGump(new CharacterClassGump(From, Target, new List<int>() { NewClassId }, 0));
			}
		}
    }
}
