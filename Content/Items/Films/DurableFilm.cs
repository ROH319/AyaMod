using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria.ID;
using Terraria;

namespace AyaMod.Content.Items.Films
{
    public class DurableFilm : BaseFilm
    {
        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            int chance = player.DevEffect() ? 100 : 40;
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
