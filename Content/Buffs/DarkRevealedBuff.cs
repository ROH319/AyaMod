using AyaMod.Content.Items.Armors;
using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs
{
    public class DarkRevealedBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.TryGetGlobalNPC<DarkRevealNPC>(out var darkRevealNPC))
            {
                darkRevealNPC.darkRevealed = true;
            }
        }
    }
}
