using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria.ID;

namespace AyaMod.Content.Items.Films
{
    public class FlashyFilm : BaseFilm
    {
        public static int StunTimeBonus = 30;
        public static int StunTimeBonusDev = 45;
        public override void PreAI(BaseCameraProj projectile)
        {
            projectile.player.GetModPlayer<CameraPlayer>().StunTimeModifier += projectile.player.DevEffect() ? StunTimeBonusDev : StunTimeBonus;
        }
        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient<CameraFilm>(100)
                .AddIngredient(ItemID.FallenStar)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
