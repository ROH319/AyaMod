using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveSilverFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3026;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenceBonusMin, DefenceBonusMax);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(DefenceBonusDevMax, DRBonusMin, DRBonusMax);

        public static int DefenceBonusMin = 8;
        public static int DefenceBonusMax = 16;
        public static int DefenceBonusDevMax = 24;
        public static int DRBonusMin = 10;
        public static int DRBonusMax = 20;
        public override void Load()
        {
            AyaPlayer.OnHitByBothHook += OnHit;
        }
        public static void OnHit(Player player, ref Player.HurtInfo hurtInfo)
        {
            player.ClearBuff(BuffType<ReflectiveSilverBuff>());
        }
        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            projectile.player.AddBuff(BuffType<ReflectiveSilverBuff>(), capturecount);
        }

    }
}
