using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Content.Buffs.Films
{
    public class ScaredBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs_Films + Name;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.buffTime[buffIndex]++;

            if (npc.buffTime[buffIndex] > 3 * 60)
                npc.Aya().ScaredDev = true;
            else
                npc.Aya().Scared = true;
        }
    }
}
