using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Server.Gumps;

namespace Server.Custom.Gump
{
	public enum GumpSizeDimension
	{
		Width, Height
	}

	public enum GumpPaddingDimension
	{
		Left, Top, Right, Bottom
	}

	public abstract class GumpElement
	{
		public static readonly int GUMP_SIZE_WRAP_CONTENT = -1;
		public static readonly int GUMP_SIZE_FIT_PARENT = -2;

		protected int[] Size = new int[2];
		protected int[] Padding = new int[4];

		public GumpElement Parent { get; set; } = null;

		public int Width
		{
			get
			{
				return ComputeSize(GumpSizeDimension.Width);
			}
		}

		public int Height
		{
			get
			{
				return ComputeSize(GumpSizeDimension.Height);
			}
		}

		public int ComputeSize(GumpSizeDimension Dimension, bool IncludePadding = false)
		{
			int Value = GetSize(Dimension);

			if (Value == GUMP_SIZE_WRAP_CONTENT)
			{
				Value = GetContentSize(Dimension);
			}
			else if (Value == GUMP_SIZE_FIT_PARENT)
			{
				if (Parent != null && Parent.GetSize(Dimension) != GUMP_SIZE_WRAP_CONTENT)
				{
					return Parent.ComputeSize(Dimension);
				}

				// Invalid
				Value = 0;
			}

			if (IncludePadding)
			{
				Value += GetPadding(Dimension);
			}

			return Value;
		}

		protected abstract int GetContentSize(GumpSizeDimension Dimension);

		public int GetSize(GumpSizeDimension Dimension)
		{
			return Size[(int)Dimension];
		}

		public int GetPadding(GumpSizeDimension Dimension)
		{
			if (Dimension == GumpSizeDimension.Width)
			{
				return GetPadding(GumpPaddingDimension.Left) + GetPadding(GumpPaddingDimension.Right);
			}
			else
			{
				return GetPadding(GumpPaddingDimension.Top) + GetPadding(GumpPaddingDimension.Bottom);
			}
		}

		public int GetPadding(GumpPaddingDimension Dimension)
		{
			return Padding[(int)Dimension];
		}

		public abstract void AddToGump(Gumps.Gump Gump, int X, int Y, int Width, int Height);
	}

	public abstract class GumpElement<T> : GumpElement where T : GumpElement<T>
	{
		public T WithSize(int Width, int Height)
		{
			return WithWidth(Width)
				.WithHeight(Height);
		}

		public T WithWidth(int Width)
		{
			Size[(int)GumpSizeDimension.Width] = Width;
			return (T)this;
		}
		public T WithHeight(int Height)
		{
			Size[(int)GumpSizeDimension.Height] = Height;
			return (T)this;
		}

		public T WithPadding(int Left, int Top, int Right, int Bottom)
		{
			return WithPadding(GumpPaddingDimension.Left, Left)
				.WithPadding(GumpPaddingDimension.Top, Top)
				.WithPadding(GumpPaddingDimension.Right, Right)
				.WithPadding(GumpPaddingDimension.Bottom, Bottom);
		}

		public T WithPadding(int Value)
		{
			return WithPadding(Value, Value, Value, Value);
		}

		public T WithPadding(GumpPaddingDimension Dimension, int Value)
		{
			Padding[(int)Dimension] = Value;
			return (T)this;
		}
	}

	public class ContainerGumpElement : GumpElement<ContainerGumpElement>
	{
		public enum GumpContainerDisposition
		{
			Stacked,
			Horizontal,
			Vertical
		}

		private GumpContainerDisposition ContainerDisposition;
		private List<GumpElement> Children = new List<GumpElement>();
		private int SpaceBetween = 0;
		private int Background = -1;

		public ContainerGumpElement(GumpContainerDisposition ContainerDisposition = GumpContainerDisposition.Stacked)
		{
			this.ContainerDisposition = ContainerDisposition;
		}

		public ContainerGumpElement WithChild(GumpElement Child)
		{
			Child.Parent = this;
			Children.Add(Child);
			return this;
		}

		public ContainerGumpElement WithChildren(List<GumpElement> Children)
		{
			Children.ForEach(Child => WithChild(Child));
			return this;
		}

		public ContainerGumpElement WithSpaceBetween(int SpaceBetween)
		{
			this.SpaceBetween = SpaceBetween;
			return this;
		}

		public ContainerGumpElement WithBackground(int Background)
		{
			this.Background = Background;
			return this;
		}

		protected override int GetContentSize(GumpSizeDimension Dimension)
		{
			bool UseMax = ContainerDisposition == GumpContainerDisposition.Stacked
				|| (ContainerDisposition == GumpContainerDisposition.Horizontal && Dimension == GumpSizeDimension.Height)
				|| (ContainerDisposition == GumpContainerDisposition.Vertical && Dimension == GumpSizeDimension.Width);

			if (UseMax)
			{
				return Children.Max(Child => Child.ComputeSize(Dimension, true));
			}
			else
			{
				return Children.Aggregate(0, (Total, Child) => Total + Child.ComputeSize(Dimension, true))
					+ (Math.Max(0, Children.Count - 1) * SpaceBetween);
			}
		}

		public override void AddToGump(Gumps.Gump Gump, int X, int Y, int Width, int Height)
		{
			if (Background >= 0)
			{
				Gump.AddBackground(X, Y, Width, Height, Background);
			}
			
			foreach (GumpElement Child in Children)
			{
				int ChildWidth = Child.ComputeSize(GumpSizeDimension.Width);
				int ChildHeight = Child.ComputeSize(GumpSizeDimension.Height);

				int ChildX = 0;
				int ChildY = 0;

				switch (ContainerDisposition)
				{
					case GumpContainerDisposition.Stacked:
						ChildX = (X + Width / 2) - ChildWidth / 2;
						ChildY = (Y + Height / 2) - ChildHeight / 2;
						break;
					case GumpContainerDisposition.Horizontal:
						ChildX = X + Child.GetPadding(GumpPaddingDimension.Left);
						if (Child != Children.First())
						{
							ChildX += SpaceBetween;
						}
						X = ChildX + ChildWidth + Child.GetPadding(GumpPaddingDimension.Right);

						ChildY = Y + Child.GetPadding(GumpPaddingDimension.Top);
						break;
					case GumpContainerDisposition.Vertical:
						ChildX = X + Child.GetPadding(GumpPaddingDimension.Left);

						ChildY = Y + Child.GetPadding(GumpPaddingDimension.Top);
						if (Child != Children.First())
						{
							ChildY += SpaceBetween;
						}
						Y = ChildY + ChildHeight + Child.GetPadding(GumpPaddingDimension.Bottom);
						break;
				}

				Child.AddToGump(Gump, ChildX, ChildY, ChildWidth, ChildHeight);
			}
		}
	}

	public class TextGumpElement : GumpElement<TextGumpElement>
	{
		private string Text = "";
		private string Color = "0xffffff";
		private bool Centered = false;

		public TextGumpElement()
		{
			WithSize(GUMP_SIZE_WRAP_CONTENT, 20);
		}

		public TextGumpElement WithText(string Text)
		{
			this.Text = Text;
			return this;
		}

		public TextGumpElement WithColor(string Color)
		{
			this.Color = Color;
			return this;
		}

		public TextGumpElement WithCentered(bool Centered)
		{
			this.Centered = Centered;
			return this;
		}

		protected override int GetContentSize(GumpSizeDimension Dimension)
		{
			if (Dimension == GumpSizeDimension.Height)
			{
				return GetSize(Dimension);
			}

			return Text.Length * 20;
		}

		public override void AddToGump(Gumps.Gump Gump, int X, int Y, int Width, int Height)
		{
			string Html = String.Format("<h3><basefont color={0}>{1}</basefont></h3>", Color, Text);
			if (Centered)
			{
				Html = String.Format("<center>{0}</center>", Html);
			}

			Gump.AddHtml(X, Y, Width, Height, Html, false, false);
		}
	}

	public class ImageGumpElement : GumpElement<ImageGumpElement>
	{
		private int GumpID;
		private bool Tiled = false;
		public int Hue = 0;

		public ImageGumpElement(int GumpID)
		{
			this.GumpID = GumpID;
		}

		public ImageGumpElement WithTiled(bool Tiled)
		{
			this.Tiled = Tiled;
			return this;
		}

		public ImageGumpElement WithHue(int Hue)
		{
			this.Hue = Hue;
			return this;
		}

		public override void AddToGump(Gumps.Gump Gump, int X, int Y, int Width, int Height)
		{
			if (Tiled)
			{
				Gump.AddImageTiled(X, Y, Width, Height, GumpID);
			}
			else
			{
				Gump.AddImage(X, Y, GumpID, Hue);
			}
		}

		protected override int GetContentSize(GumpSizeDimension Dimension)
		{
			throw new NotImplementedException();
		}
	}

	public class ButtonGumpElement : GumpElement<ButtonGumpElement>
	{
		private int ButtonID;
		private int[] StateIDs = new int[2];
		private GumpButtonType ButtonType = GumpButtonType.Reply;

		public ButtonGumpElement(int ID)
		{
			ButtonID = ID;
		}

		public ButtonGumpElement WithStateIDs(int Normal, int Pressed)
		{
			StateIDs[0] = Normal;
			StateIDs[1] = Pressed;
			return this;
		}

		public ButtonGumpElement WithStateID(int ID)
		{
			return WithStateIDs(ID, ID);
		}

		public ButtonGumpElement WithButtonType(GumpButtonType ButtonType)
		{
			this.ButtonType = ButtonType;
			return this;
		}

		public override void AddToGump(Gumps.Gump Gump, int X, int Y, int Width, int Height)
		{
			Gump.AddButton(X, Y, StateIDs[0], StateIDs[1], ButtonID, ButtonType, 0);
		}

		protected override int GetContentSize(GumpSizeDimension Dimension)
		{
			return 20;
		}
	}

	public class CustomGumpElement : GumpElement<CustomGumpElement>
	{
		private Action<Gumps.Gump, int, int, int, int> CustomBuild;

		public CustomGumpElement(Action<Gumps.Gump, int, int, int, int> CustomBuild)
		{
			this.CustomBuild = CustomBuild;
		}

		public override void AddToGump(Gumps.Gump Gump, int X, int Y, int Width, int Height)
		{
			CustomBuild(Gump, X, Y, Width, Height);
		}

		protected override int GetContentSize(GumpSizeDimension Dimension)
		{
			throw new NotImplementedException();
		}
	}

	public class GumpBuilder : ContainerGumpElement
	{
		int X, Y;

		public GumpBuilder(int X = 0, int Y = 0)
		{
			this.X = X;
			this.Y = Y;

			WithSize(GUMP_SIZE_WRAP_CONTENT, GUMP_SIZE_WRAP_CONTENT)
				.WithPadding(10);
		}

		public void Build(Gumps.Gump Gump)
		{
			AddToGump(Gump, X, Y, ComputeSize(GumpSizeDimension.Width), ComputeSize(GumpSizeDimension.Height));
		}
	}
}
