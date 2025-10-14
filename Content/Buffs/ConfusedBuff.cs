using AyaMod.Content.Items.Cameras;
using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Content.Buffs
{
    public class ConfusedBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<LegilimencyNPC>().Confused = true;
            if (!npc.boss)
            {
                npc.confused = true;
            }
        }
    }
}
