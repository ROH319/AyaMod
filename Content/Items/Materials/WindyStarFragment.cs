using AyaMod.Core;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Materials
{
    public class WindyStarFragment : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Materials + Name;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemIconPulse[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Cyan9, Item.sellPrice(silver: 20));
        }
    }
}
