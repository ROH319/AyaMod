using AyaMod.Core;
using Terraria;

namespace AyaMod.Content.Items.Materials
{
    public class WindyStarFragment : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Materials + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Cyan9, Item.sellPrice(silver: 20));
        }
    }
}
