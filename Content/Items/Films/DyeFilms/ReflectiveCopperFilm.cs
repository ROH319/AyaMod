using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveCopperFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3553;
        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            if (capturecount > 0)
            {
                projectile.player.AddBuff(BuffType<ReflectiveCopperBuff>(), 60);
            }
        }

        public static int DefenceBonus = 10;
        public static int DefenceBonusDev = 15;
        public static int EnduranceBonus = 15;
    }
}
