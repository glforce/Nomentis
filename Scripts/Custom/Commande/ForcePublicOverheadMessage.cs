using System;
using Server.Network;
using Server.Targeting;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class ForcePublicOverheadMessage
    {
        public static void Initialize()
        {
            CommandSystem.Register("ForceEmote", AccessLevel.GameMaster, ForceEmote);
            CommandSystem.Register("ForceSay", AccessLevel.GameMaster, ForceSay);
        }

		private static void ForceEmote(CommandEventArgs e)
		{
			TargetSpeakCommand(e, MessageType.Emote);
		}

		private static void ForceSay(CommandEventArgs e)
		{
			TargetSpeakCommand(e, MessageType.Regular);
		}

		private static void TargetSpeakCommand(CommandEventArgs e, MessageType MessageType)
		{
			e.Mobile.Target = new SpeakTarget(e.ArgString.Trim(), MessageType);
		}

        private class SpeakTarget : Target
        {
            private string Message;
			private MessageType MessageType;

			public SpeakTarget(string Message, MessageType MessageType) : base(12, false, TargetFlags.None)
            {
                this.Message = Message;
				this.MessageType = MessageType;
            }

            protected override void OnTarget(Mobile From, object Target)
            {
				string Format = "{0}";
				if (MessageType == MessageType.Emote)
				{
					Format = "*{0}*";
				}

				if (Target is Mobile)
                {
					Mobile TargetMobile = (Mobile)Target;

                    if (TargetMobile.AccessLevel > From.AccessLevel)
                    {
						From.SendMessage("Vous ne pouvez faire le parler un personnage avec plus de droits que vous.");
                    }
                    else
                    {
						int Hue = 0XFFFFFF;

						switch(MessageType)
						{
							case MessageType.Emote:
								Hue = TargetMobile.EmoteHue;
								break;
							case MessageType.Regular:
								Hue = TargetMobile.SpeechHue;
								break;
						}

						TargetMobile.PublicOverheadMessage(MessageType, Hue, false, String.Format(Format, Message), false);
                    }
                }
                else if (Target is Item)
                {
                    (Target as Item).PublicOverheadMessage(MessageType, 0, false, String.Format(Format, Message));
                }
                else
                {
					From.SendMessage("Vous devez choisir un mobile ou un item.");
                }
            }
        }
    }
}