using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Server.Custom.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Custom.Gump
{
	public abstract class CustomPlayerMobileGump : Gumps.Gump
	{
		protected CustomPlayerMobile From;
		protected ContainerGumpElement Root = new ContainerGumpElement().WithSize(GumpElement.GUMP_SIZE_WRAP_CONTENT, GumpElement.GUMP_SIZE_WRAP_CONTENT);

		private GumpBuilder Builder;

		public CustomPlayerMobileGump(CustomPlayerMobile From, int X, int Y, string Title) : base(X, Y)
		{
			this.From = From;

			From.CloseGump(GetType());

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			Builder = new GumpBuilder(X, Y);

			Builder
				.WithBackground(9270)
				.WithPadding(20)
				.WithChild(new ContainerGumpElement(ContainerGumpElement.GumpContainerDisposition.Vertical)
				.WithSize(GumpElement.GUMP_SIZE_WRAP_CONTENT, GumpElement.GUMP_SIZE_WRAP_CONTENT)
				.WithChildren(new List<GumpElement> {
					new TextGumpElement(Title)
					.WithWidth(GumpElement.GUMP_SIZE_FIT_PARENT)
					.WithCentered(true),
					Root
				}));
		}

		protected void Build()
		{
			Builder.Build(this);
		}

		virtual protected void OnButtonClicked(int ID)
		{

		}

		public override void OnResponse(NetState Sender, RelayInfo Info)
		{
			if (Info.ButtonID > 0)
			{
				OnButtonClicked(Info.ButtonID);
			}
		}

		protected void OpenGump(Func<CustomPlayerMobile, Gumps.Gump> GumpCreationFunc)
		{
			From.SendGump(GumpCreationFunc(From));
		}

		protected GumpElement BuildTable<T>(List<string> Headers, List<T> Rows, params Func<int, T, GumpElement>[] BuildRowElementFunc)
		{
			return new ContainerGumpElement(ContainerGumpElement.GumpContainerDisposition.Horizontal)
				.WithSize(GumpElement.GUMP_SIZE_WRAP_CONTENT, GumpElement.GUMP_SIZE_WRAP_CONTENT)
				.WithChildren(
				Headers.Select((Header, ColumnIndex) =>
				{
					if (ColumnIndex >= BuildRowElementFunc.Length)
					{
						throw new ArgumentOutOfRangeException();
					}

					return new ContainerGumpElement(ContainerGumpElement.GumpContainerDisposition.Vertical)
					.WithSize(GumpElement.GUMP_SIZE_WRAP_CONTENT, GumpElement.GUMP_SIZE_WRAP_CONTENT)
					.WithChild(new TextGumpElement(Header))
						.WithChildren(Rows.Select(
							(Row, Index) =>
							{
								return BuildRowElementFunc[ColumnIndex](Index, Row);
							})
							.ToList()
						);
				})
				.ToList<GumpElement>());
		}
	}
}
