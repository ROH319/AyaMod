using AyaMod.Core;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Materials
{
    public class StrangePlantEssence : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Materials + Name;
        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.AmberMinus11, Item.sellPrice(copper: 50));
        }
        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient(ItemID.StrangePlant1)
                .Register();
            CreateRecipe(3)
                .AddIngredient(ItemID.StrangePlant2)
                .Register();
            CreateRecipe(3)
                .AddIngredient(ItemID.StrangePlant3)
                .Register();
            CreateRecipe(3)
                .AddIngredient(ItemID.StrangePlant4)
                .Register();
        }
    }
}
