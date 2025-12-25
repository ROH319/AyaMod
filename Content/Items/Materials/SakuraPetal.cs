using AyaMod.Core;
using Terraria;

namespace AyaMod.Content.Items.Materials
{
    public class SakuraPetal : ModItem
    {
        public override string Texture => AssetDirectory.Materials + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(silver: 2));
        }
    }
}
