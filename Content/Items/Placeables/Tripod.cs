using AyaMod.Content.Tiles;
using AyaMod.Core;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Placeables
{
    public class Tripod : ModItem
    {
        public override string Texture => AssetDirectory.Placeables + Name;
        public override void SetDefaults()
        {
            Item.width = Item.height = 30;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(gold: 4));
            Item.DefaultToPlaceableTile(TileType<TripodTile>());
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 15)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 15)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
