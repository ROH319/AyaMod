using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.Localization;
using Terraria;

namespace AyaMod.Content.Items.Accessories
{
    public class FalseReporterEmblem : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageIncrease, DefenceDecrease);
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 2));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ReporterDamage>() += (float)DamageIncrease / 100f;
            player.statDefense -= DefenceDecrease;
        }

        public static int DamageIncrease = 15;
        public static int DefenceDecrease = 6;
    }
}
