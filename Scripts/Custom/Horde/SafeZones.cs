using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml;
using System.Xml.Linq;
using Server.Commands;

namespace Server.Custom.Horde
{
	public class SafeZones
	{
		private struct SafeZone
		{
			List<Rectangle2D> Rects;
			List<Point2D> Perimeter;

			public SafeZone(List<Rectangle2D> Rects)
			{
				this.Rects = Rects;

				Perimeter = new List<Point2D>();

				BuildPerimeter();
			}

			private void BuildPerimeter()
			{
				var PerimeterSet = new HashSet<Point2D>();
				foreach (var Rect in Rects)
				{
					for (var x = Rect.Start.X - 1; x <= Rect.End.X + 1; x++)
					{
						var Point = new Point2D(x, Rect.Start.Y - 1);
						if (!IsInSafeZone(Point))
						{
							PerimeterSet.Add(Point);
						}

						Point = new Point2D(x, Rect.End.Y + 1);
						if (!IsInSafeZone(Point))
						{
							PerimeterSet.Add(Point);
						}
					}

					for (var y = Rect.Start.Y - 1; y <= Rect.End.Y + 1; y++)
					{
						var Point = new Point2D(Rect.Start.X - 1, y);
						if (!IsInSafeZone(Point))
						{
							PerimeterSet.Add(Point);
						}

						Point = new Point2D(Rect.End.X + 1, y);
						if (!IsInSafeZone(Point))
						{
							PerimeterSet.Add(Point);
						}
					}
				}

				Perimeter.AddRange(PerimeterSet);
			}

			public bool IsInSafeZone(Point2D Location)
			{
				return Rects.Any(Rect => Rect.Contains(Location));
			}

			public Point2D GetLocationOnPerimeter()
			{
				return Perimeter[Utility.Random(Perimeter.Count)];
			}
		};

		private static readonly string ConfigFilePath = Path.Combine("Config", "SafeZones.xml");

		private static Dictionary<Map, List<SafeZone>> Zones = new Dictionary<Map, List<SafeZone>>();

		public static void Configure()
		{
			LoadSafeZones();

			CommandSystem.Register("AddSafeZone", AccessLevel.Administrator, AddSafeZone);
		}

		private static void LoadSafeZones()
		{
			var XmlDocument = XDocument.Load(ConfigFilePath);

			foreach (var Node in XmlDocument.Root.Descendants("map"))
			{
				Map Map = Map.Parse(Node.Attribute("name").Value);
				if (Map != null)
				{
					var RectGroups = new List<List<Rectangle2D>>();
					foreach (var ZoneNode in Node.Descendants())
					{
						var Rect = new Rectangle2D(
							new Point2D(int.Parse(ZoneNode.Attribute("startx").Value), int.Parse(ZoneNode.Attribute("starty").Value)),
							new Point2D(int.Parse(ZoneNode.Attribute("endx").Value), int.Parse(ZoneNode.Attribute("endy").Value))
						);

						var RectGroup = RectGroups.FirstOrDefault(Group => Group.Any(GroupedRect => Intersect(GroupedRect, Rect)));
						if (RectGroup == null)
						{
							RectGroup = new List<Rectangle2D>();
							RectGroups.Add(RectGroup);
						}
						RectGroup.Add(Rect);
					}

					Zones[Map] = RectGroups.Select(RectGroup => new SafeZone(RectGroup)).ToList();
				}
			}
		}

		private static bool Intersect(Rectangle2D Rect1, Rectangle2D Rect2)
		{
			var Rect1Start = Rect1.Start;
			var Rect2Start = Rect2.Start;
			var Rect1End = Rect1.End;
			var Rect2End = Rect2.End;

			return Math.Max(Rect1Start.X, Rect2Start.X) < Math.Min(Rect1End.X, Rect2End.X)
				&& Math.Max(Rect1Start.Y, Rect2Start.Y) < Math.Min(Rect1End.Y, Rect2End.Y);
		}

		public static bool IsInSafeZone(Map Map, Point2D Location)
		{
			return GetSafeZone(Map, Location) != null;
		}

		public static bool IsInSafeZone(Mobile Mobile)
		{
			return IsInSafeZone(Mobile.Map, Mobile.Location);
		}

		public static Point2D GetLocationOutsideOfSafeZone(Map Map, Point2D Location, int Min, int Max)
		{
			var SafeZone = GetSafeZone(Map, Location);

			if (SafeZone == null)
			{
				return Location;
			}

			var PerimeterLocation = SafeZone.Value.GetLocationOnPerimeter();

			var Direction = new Vector2(PerimeterLocation.X - Location.X, PerimeterLocation.Y - Location.Y);
			Direction /= Direction.Length();
			Direction *= Utility.Random(Min, Max);

			return new Point2D(PerimeterLocation.X + (int)Direction.X, PerimeterLocation.Y + (int)Direction.Y);
		}

		public static Point2D GetLocationOutsideOfSafeZone(Mobile Mobile, int Min, int Max)
		{
			return GetLocationOutsideOfSafeZone(Mobile.Map, Mobile.Location, Min, Max);
		}

		private static SafeZone? GetSafeZone(Map Map, Point2D Location)
		{
			if (!Zones.ContainsKey(Map))
			{
				return null;
			}

			return Zones[Map]
				.Where(Zone => Zone.IsInSafeZone(Location))
				.Select(Zone => (SafeZone?)Zone)
				.FirstOrDefault();
		}

		[Usage("AddSafeZone")]
		private static void AddSafeZone(CommandEventArgs e)
		{
			BoundingBoxPicker.Begin(e.Mobile, OnSafeZonePicked, null);
		}

		private static void OnSafeZonePicked(Mobile From, Map Map, Point3D Start, Point3D End, object State)
		{
			var XmlDocument = new XmlDocument();
			XmlDocument.Load(ConfigFilePath);

			XmlNode MapNode = null;
			foreach (XmlNode Node in XmlDocument.GetElementsByTagName("maps")[0].ChildNodes)
			{
				var ZoneMap = Map.Parse(Node.Attributes["name"].Value);
				if (ZoneMap == Map)
				{
					MapNode = Node;
					break;
				}
			}

			if (MapNode == null)
			{
				MapNode = XmlDocument.GetElementsByTagName("maps")[0].AppendChild(XmlDocument.CreateElement("map"));
				MapNode.Attributes.Append(XmlDocument.CreateAttribute("name")).Value = Map.Name;
			}

			var ZoneNode = MapNode.AppendChild(XmlDocument.CreateElement("zone"));
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("startx")).Value = Start.X.ToString();
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("starty")).Value = Start.Y.ToString();
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("endx")).Value = End.X.ToString();
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("endy")).Value = End.Y.ToString();

			XmlDocument.Save(ConfigFilePath);
		}
	}
}
