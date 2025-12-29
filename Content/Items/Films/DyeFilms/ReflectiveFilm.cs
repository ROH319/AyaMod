using AyaMod.Content.Buffs.Films;
using AyaMod.Content.Items.Materials;
using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DmgReduceFlat);

        public override int DyeID => 3190;

        public static int DmgReduceFlat = 6;
        public static int DmgReduceFlatDev = 18;
        public override void Load()
        {
            AyaPlayer.OnHitByBothHook += OnHit;
        }

        public static void OnHit(Player player, ref Player.HurtInfo hurtInfo)
        {
            player.ClearBuff(BuffType<ReflectiveBuff>());
        }
        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            if (capturecount > 0)
            {
                projectile.player.AddBuff(BuffType<ReflectiveBuff>(), 60);
            }
        }
    }
}
