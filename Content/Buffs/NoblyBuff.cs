using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Buffs
{
    public class NoblyBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(HealBonus);
        public static int HealBonus = 10;
        public override void Update(Player player, ref int buffIndex)
        {
            player.Aya().HealLifeModifier += HealBonus / 100f;
        }
    }
}
