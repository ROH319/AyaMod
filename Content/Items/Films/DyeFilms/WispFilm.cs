using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class WispFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";

        public override void OnSnapInSight(BaseCameraProj projectile)
        {
            if(projectile.player.Aya().WispDmg > projectile.Projectile.damage)
            {

                projectile.player.Aya().WispDmg -= projectile.Projectile.damage;
            }
        }

        public static float WispDmgRegen = 80f / 60;
        public static float WispDmgRegenDev = 125 / 60;
        public static int WispDmgMax = 320;
        public static int WispDmgMaxDev = 500;
    }
}
