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
    public class CrimsonPupil2 : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease, SizeDecrease);

        public static int SpeedIncrease = 16;
        public static int SingleTargetIncrease = 16;
        public static int SizeDecrease = 8;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(gold: 1));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += SpeedIncrease / 100f;
            player.Camera().SizeModifier -= SizeDecrease / 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CthulhuLens>()
                .AddIngredient<CrimsonPupil>()
                .AddIngredient(ItemID.Bone, 15)
                .Register();
        }
    }
}
