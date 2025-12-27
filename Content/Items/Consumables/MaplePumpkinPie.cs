using AyaMod.Content.Items.Materials;
using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AyaMod.Content.Items.Consumables
{
    public class MaplePumpkinPie : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Consumables + Name;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.IsFood[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        }
        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 22, BuffID.WellFed3, 6 * 60 * 60);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.buyPrice(0, 1));
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MapleLeaf>()
                .AddIngredient(ItemID.PumpkinPie)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
