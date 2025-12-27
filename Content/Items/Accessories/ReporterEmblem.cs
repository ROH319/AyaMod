using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using Terraria.Localization;
namespace AyaMod.Content.Items.Accessories
{
    public class ReporterEmblem : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageIncrease);
        public static int DamageIncrease = 15;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 2));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ReporterDamage>() += DamageIncrease / 100f;
        }
    }
}
