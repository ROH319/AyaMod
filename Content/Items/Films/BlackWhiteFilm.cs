using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria.ID;
using Terraria;

namespace AyaMod.Content.Items.Films
{
    public class BlackWhiteFilm : BaseFilm
    {
        public static int ArmorPenBonus;
        public static int ArmorPenBonusDev;
        public override void ModifyHitNPCFilm(BaseCameraProj projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += projectile.player.DevEffect() ? ArmorPenBonusDev : ArmorPenBonus;
        }
        public override void AddRecipes()
        {
            CreateRecipe(450)
                .AddIngredient<CameraFilm>(450)
                .AddIngredient(ItemID.SilverDye)
                .AddIngredient(ItemID.BlackDye)
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}
