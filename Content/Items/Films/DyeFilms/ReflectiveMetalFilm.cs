using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveMetalFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenseBonus, DRBonus, thornBonus);
        public override int DyeID => 3555;

        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            projectile.player.AddBuff(BuffType<ReflectiveMetalBuff>(),2);
        }

        public static int DefenseBonus = 16;
        public static int DefenseBonusDev = 20;
        public static int DRBonus = 16;
        public static int DRBonusDev = 20;
        public static int thornBonus = 150;
        public static int thornBonusDev = 250;
    }
}
