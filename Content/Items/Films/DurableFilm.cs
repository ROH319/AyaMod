using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films
{
    public class DurableFilm : BaseFilm
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, AmmoCostDec);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(AmmoCostDecDev);
        public static int DamageBonus = 10;
        public static int AmmoCostDec = 30;
        public static int AmmoCostDecDev = 50;
        public override StatModifier DamageModifier => base.DamageModifier + DamageBonus / 100f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(copper:3));
            FilmArgs = [("ammocost", AmmoCostDec.ToString(), AmmoCostDecDev.ToString())];
        }
        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            int chance = player.DevEffect() ? AmmoCostDecDev : AmmoCostDec;
            if (Main.rand.Next(100) < chance)
                return false;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(200)
                .AddIngredient<CameraFilm>(200)
                .AddIngredient(ItemID.SilverBar)
                .AddTile(TileID.Furnaces)
                .Register();
            CreateRecipe(200)
                .AddIngredient<CameraFilm>(200)
                .AddIngredient(ItemID.TungstenBar)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
