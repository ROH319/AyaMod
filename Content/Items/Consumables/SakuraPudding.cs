using AyaMod.Content.Items.Materials;
using AyaMod.Core;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Consumables
{
    public class SakuraPudding : ModItem
    {
        public override string Texture => AssetDirectory.Consumables + Name;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsFood[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 22, BuffID.WellFed2, 6 * 60 * 60);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.buyPrice(0, 1));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SakuraPetal>()
                .AddIngredient(ItemID.BottledHoney)
                .AddCondition(Terraria.Condition.NearWater)
                .Register();
        }
    }
}
