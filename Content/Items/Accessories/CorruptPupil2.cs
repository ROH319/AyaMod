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
    public class CorruptPupil2 : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease, SizeIncrease);

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(gold: 1));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += (float)SpeedIncrease / 100f;
            player.Camera().SizeModifier += (float)SizeIncrease / 100f;
        }
        public static int SpeedIncrease = 12;
        public static int SizeIncrease = 12;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CthulhuLens>()
                .AddIngredient<CorruptPupil>()
                .AddIngredient(ItemID.Bone, 15)
                .Register();
        }
    }
}
