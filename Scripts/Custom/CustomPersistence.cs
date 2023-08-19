using Server.Mobiles;
using System.Collections.Generic;
using System.IO;
using System;

namespace Server.Custom
{
    public static class CustomPersistence
    {
        public static string FilePath = Path.Combine("Saves/", "CustomPersistence.bin");

        public static DateTime Ouverture { get; set; }
		
	/*	public static Dictionary<string, double> SellItems = new Dictionary<string, double>();

		public static void AddSellItem(string items, double value)
		{
			if (SellItems.ContainsKey(items))
			{
				SellItems[items] += value;
			}
			else
			{
				SellItems.Add(items, value);
			}


		}

		public static void SellingLog(CustomPlayerMobile player,bool contrebandier, string item, int amount, int pricebyitem)
		{
		

			if (player != null && player.Account != null)
			{
				string path = "Logs/SellLog/";
				string fileName = path + "SellItem.csv";

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);

					using (StreamWriter sw = new StreamWriter(fileName, true))
						sw.WriteLine("Date;Nom;Account;Contrebandier;Item;Prix;Qte;Total");  // CSV fIle type..
				}
					

				using (StreamWriter sw = new StreamWriter(fileName, true))
					sw.WriteLine(DateTime.Now.ToString() + ";" + player.Name + ";" + player.Account.Username + ";" + contrebandier.ToString() + ";" + item + ";" + pricebyitem.ToString() + ";" + amount.ToString() + ";" + (amount * pricebyitem).ToString());  // CSV fIle type..
			}
		}


*/



		public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

			Ouverture = DateTime.Now;
		

		}

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

				
	/*				writer.Write(SellItems.Count);

					foreach (KeyValuePair<string, double> item in SellItems)
					{
						writer.Write(item.Key);
						writer.Write(item.Value);
					}	*/				

					writer.Write(Ouverture);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

					switch (version)
					{
						case 0:
							{

							/*	int count = reader.ReadInt();

								for (int i = 0; i < count; i++)
								{
									SellItems.Add(reader.ReadString(), reader.ReadDouble());
								}
							*/

								Ouverture = reader.ReadDateTime();
								break;
							}
					}
				});
        }
    }
}
