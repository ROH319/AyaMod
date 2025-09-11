using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs.Films
{
    public class RedAcidBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs_Films + Name;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Aya().RedAcid = true;
        }
    }
}
