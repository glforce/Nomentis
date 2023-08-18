using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml;
using Server.Commands;

namespace Server.Services.Horde
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
				HashSet<Point2D> PerimeterSet = new HashSet<Point2D>();
				foreach (Rectangle2D Rect in Rects)
				{
					for (int x = Rect.Start.X - 1; x <= Rect.End.X + 1; x++)
					{
						Point2D Point = new Point2D(x, Rect.Start.Y - 1);
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

					for (int y = Rect.Start.Y - 1; y <= Rect.End.Y + 1; y++)
					{
						Point2D Point = new Point2D(Rect.Start.X - 1, y);
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
			XmlDocument XmlDocument = new XmlDocument();
			XmlDocument.Load(ConfigFilePath);
			foreach (XmlNode Node in XmlDocument.GetElementsByTagName("maps")[0].ChildNodes)
			{
				Map Map = Map.Parse(Node.Attributes["name"].Value);
				if (Map != null)
				{
					List<List<Rectangle2D>> RectGroups = new List<List<Rectangle2D>>();
					foreach (XmlNode ZoneNode in Node.ChildNodes)
					{
						Rectangle2D Rect = new Rectangle2D(
							new Point2D(int.Parse(ZoneNode.Attributes["startx"].Value), int.Parse(ZoneNode.Attributes["starty"].Value)),
							new Point2D(int.Parse(ZoneNode.Attributes["endx"].Value), int.Parse(ZoneNode.Attributes["endy"].Value))
						);

						List<Rectangle2D> RectGroup = RectGroups.FirstOrDefault(Group => Group.Any(GroupedRect => Intersect(GroupedRect, Rect)));
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
			Point2D Rect1Start = Rect1.Start;
			Point2D Rect2Start = Rect2.Start;
			Point2D Rect1End = Rect1.End;
			Point2D Rect2End = Rect2.End;

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
			SafeZone? SafeZone = GetSafeZone(Map, Location);
			
			if (SafeZone == null)
			{
				return Location;
			}

			Point2D PerimeterLocation = SafeZone.Value.GetLocationOnPerimeter();

			Vector2 Direction = new Vector2(PerimeterLocation.X - Location.X, PerimeterLocation.Y - Location.Y);
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

			return Zones[Map].Where(Zone => Zone.IsInSafeZone(Location)).Select(Zone => (SafeZone?)Zone).FirstOrDefault();
		}

		private static void AddSafeZone(CommandEventArgs e)
		{
			BoundingBoxPicker.Begin(e.Mobile, OnSafeZonePicked, null);
		}

		private static void OnSafeZonePicked(Mobile From, Map Map, Point3D Start, Point3D End, object State)
		{
			Rectangle2D SafeZone = new Rectangle2D(Start.X, Start.Y, End.X, End.Y);

			XmlDocument XmlDocument = new XmlDocument();
			XmlDocument.Load(ConfigFilePath);

			XmlNode MapNode = null;
			foreach (XmlNode Node in XmlDocument.GetElementsByTagName("maps")[0].ChildNodes)
			{
				Map ZoneMap = Map.Parse(Node.Attributes["name"].Value);
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

			XmlNode ZoneNode = MapNode.AppendChild(XmlDocument.CreateElement("zone"));
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("startx")).Value = Start.X.ToString();
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("starty")).Value = Start.Y.ToString();
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("endx")).Value = End.X.ToString();
			ZoneNode.Attributes.Append(XmlDocument.CreateAttribute("endy")).Value = End.Y.ToString();

			XmlDocument.Save(ConfigFilePath);
		}
	}
}
