using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveCopperFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenceBonus);
        public override int DyeID => 3553;
        public override void Load()
        {
            AyaPlayer.OnHitByBothHook += OnHit;
        }
        public static void OnHit(Player player, ref Player.HurtInfo hurtInfo)
        {
            player.ClearBuff(BuffType<ReflectiveCopperBuff>());
        }
        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            if (capturecount > 0)
            {
                projectile.player.AddBuff(BuffType<ReflectiveCopperBuff>(), 2);
            }
        }

        public static int DefenceBonus = 10;
        public static int DefenceBonusDev = 15;
        public static int EnduranceBonus = 15;
    }
}
