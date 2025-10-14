using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using AyaMod.Helpers;
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    public class IntelligentGyroscope : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ChaseSpeedBonus);
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(gold: 1));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Camera().ChaseSpeedModifier += ChaseSpeedBonus / 100f;
        }

        public static int ChaseSpeedBonus = 15;

    }
}
