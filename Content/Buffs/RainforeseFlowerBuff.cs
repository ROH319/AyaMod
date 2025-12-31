using AyaMod.Content.Items.Armors;
using AyaMod.Core;
using Humanizer;
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
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int overheal = Main.LocalPlayer.GetModPlayer<RainforestPlayer>().Overheal;
            tip = tip.FormatWith(overheal);
        }
    }
}
