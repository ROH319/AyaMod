using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class AcidFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AcidDotDmg / 2);
        public override int DyeID => 3040;
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AcidBuff>(), Main.player[projectile.Projectile.owner].DevEffect() ? AcidDotTimeDev : AcidDotTime);
        }

        public static int AcidDotDmg = 16;
        public static int AcidDotDmgDev = 24;
        public static int AcidDotTime = 60;
        public static int AcidDotTimeDev = 100;
    }
}
