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
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease, SizeDecrease, SingleTargetIncrease);

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 3));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += (float)SpeedIncrease / 100f;
            player.Camera().SizeBonus -= (float)SizeDecrease / 100f;
            player.Camera().SingleTargetMultiplier += (float)SingleTargetIncrease / 100f;
        }

        public static int SpeedIncrease = 15;
        public static int SizeDecrease = 20;
        public static int SingleTargetIncrease = 18;
    }
}
