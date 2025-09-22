using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveSilverFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3026;

        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            projectile.player.AddBuff(BuffType<ReflectiveSilverBuff>(), capturecount);
        }

        public static int DefenceBonusMin = 8;
        public static int DefenceBonusMax = 16;
        public static int DefenceBonusDevMax = 24;
        public static int DRBonusMin = 10;
        public static int DRBonusMax = 20;
    }
}
