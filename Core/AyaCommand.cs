using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace AyaMod.Core
{
    public class AyaCommand : ModCommand
    {
        public override string Command => "aya";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            switch (args.Length)
            {
                default:break;
                case 1:
                    switch (args[0].ToLower())
                    {
                        case "unlockall":
                            //for(int i = 0; i < ContentSamples.NpcBestiaryCreditIdsByNpcNetIds.Count; i++)
                            //{
                            //    var id = ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[i];
                            //    Main.BestiaryTracker.Kills.SetKillCountDirectly(id, 200);
                            //    Main.BestiaryTracker.Sights.SetWasSeenDirectly(id);
                            //    Main.BestiaryTracker.Chats.SetWasChatWithDirectly(id);
                            //}
                            //Main.BestiaryUI.Recalculate();
                            //Main.NewText($"finished");
                            break;
                        default:break;
                    }
                    break;
            }
        }
    }
}
