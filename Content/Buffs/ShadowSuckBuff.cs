﻿using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs
{
    public class ShadowSuckBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Aya().ShadowSuckBuff = true;
        }
    }
}
