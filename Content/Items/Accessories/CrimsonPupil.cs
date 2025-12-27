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
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    public class CrimsonPupil : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease, SizeDecrease);

        public static int SingleTargetIncrease = 16;
        public static int SpeedIncrease = 12;
        public static int SizeDecrease = 5;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += SpeedIncrease / 100f;
            player.Camera().SizeModifier -= SizeDecrease / 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TissueSample, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
