using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    public class CthulhuLens : BaseAccessories, IPlaceholderItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease);
        public static int SpeedIncrease = 10;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 1));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += SpeedIncrease / 100f;
        }
    }
}
