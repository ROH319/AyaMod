using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    public class SpazmatismLens : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease, SizeIncrease);
        public static int SpeedIncrease = 15;
        public static int SizeIncrease = 20;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 3));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += SpeedIncrease / 100f;
            player.Camera().SizeModifier += SizeIncrease / 100f;
        }
    }
}
