using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using Terraria;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3190;
        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            if (capturecount > 0)
            {
                projectile.player.AddBuff(BuffType<ReflectiveBuff>(), 60);
            }
        }

        public static int DmgReduceFlat = 6;
        public static int DmgReduceFlatDev = 18;
    }
}
