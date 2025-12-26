using AyaMod.Content.Tiles;
using AyaMod.Core;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Materials
{
    public class IzanagiObject : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Materials + Name;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(silver: 40));
            Item.DefaultToPlaceableTile(TileType<IzanagiObjectOre>());
        }

        public static void RegisterIzanagiRecipe(int resultID, int itemID, int resultAmount = 1, int itemAmount = 1)
        {
            Recipe.Create(resultID, resultAmount)
                .AddIngredient(itemID, itemAmount)
                .AddIngredient<IzanagiObject>()
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();
        }
    }
}
