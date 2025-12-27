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
    public class RetinazerLens : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease, SizeDecrease);
        public static int SpeedIncrease = 20;
        public static int SizeDecrease = 10;
        public static int SingleTargetIncrease = 18;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 3));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += SpeedIncrease / 100f;
            player.Camera().SizeModifier -= SizeDecrease / 100f;
        }
    }
}
