using AyaMod.Content.Buffs;
using AyaMod.Content.Items.Materials;
using AyaMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Consumables
{
    public class UninhibitedDrink : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Consumables + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(UninhibitedBuff.ChaseSpeedBonus);
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = [
                new Color(240, 240, 240),
                new Color(200, 200, 200),
                new Color(140, 140, 140)
            ];
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = Item.useAnimation = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(silver: 2));
            Item.buffType = BuffType<UninhibitedBuff>();
            Item.buffTime = 8 * 60 * 60;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<MapleLeaf>()
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
