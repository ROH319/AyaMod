using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria.ID;
using Terraria;

namespace AyaMod.Content.Items.Films
{
    public class FlashyFilm : BaseFilm
    {
        public static int StunTimeBonus = 30;
        public static int StunTimeBonusDev = 45;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(copper:3));
            FilmArgs = [("flashtime", StunTimeBonus.ToString(), StunTimeBonusDev.ToString())];
        }
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
