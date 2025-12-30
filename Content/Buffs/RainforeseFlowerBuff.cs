using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs
{
    public class RainforeseFlowerBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(Player player, ref int buffIndex)
        {
            
        }
    }
}
