using System;
using System.Web.UI;
using Server.Gumps;
using Server.Network;

namespace Server.Custom.Gump
{
	public abstract class GumpBuilderGump : Gumps.Gump
	{
		private Mobile From;
		private GumpBuilder Builder;

		public GumpBuilderGump(Mobile From, int X, int Y): base(X, Y)
		{
			this.From = From;

			From.CloseGump(GetType());

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			Builder = new GumpBuilder(X, Y);
		}

		protected ContainerGumpElement Root => Builder;

		protected void Build()
		{
			Builder.Build(this);
		}

		protected abstract void OnButtonClicked(int ID);

		public override void OnResponse(NetState Sender, RelayInfo Info)
		{
			if (Info.ButtonID > 0)
			{
				OnButtonClicked(Info.ButtonID);
			}
		}

		protected void OpenGump(Func<Mobile, Gumps.Gump> GumpCreationFunc)
		{
			From.SendGump(GumpCreationFunc(From));
		}
	}
}
