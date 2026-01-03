using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class TwilightFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3039;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffTimeBase / 60);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(BuffTimeBaseDev / 60 - BuffTimeBase / 60);
        public static int BuffTimeBase = 5 * 60;
        public static int BuffTimeBaseDev = 8 * 60;
        public static int BuffTimeStep = 1 * 60;
        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            int buffTime = projectile.player.DevEffect() ? BuffTimeBaseDev : BuffTimeBase;
            buffTime += capturecount * BuffTimeStep;
            projectile.player.AddBuff(BuffType<TwilightBuff>(), buffTime);
        }
    }
}
