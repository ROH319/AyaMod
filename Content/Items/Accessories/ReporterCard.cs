using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using Terraria.Localization;
using AyaMod.Core;
using Terraria.ID;

namespace AyaMod.Content.Items.Accessories
{
    public class ReporterCard : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageIncrease);

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(gold: 2));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ReporterDamage>() += (float)DamageIncrease / 100f;
        }

        public static int DamageIncrease = 8;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BambooBlock, 10)
                //TODO：枫叶
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.BambooBlock, 10)
                //TODO：枫叶
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
