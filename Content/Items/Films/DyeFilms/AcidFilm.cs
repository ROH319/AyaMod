using AyaMod.Common.Easer;
using AyaMod.Content.Buffs.Films;
using AyaMod.Content.Projectiles.Auras;
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
        public override void OnSnap(BaseCameraProj projectile)
        {
            int bufftime = Main.player[projectile.Projectile.owner].DevEffect() ? AcidDotTimeDev : AcidDotTime;
            Color innerColor = new Color(62, 224, 95) * 0.15f;
            Color edgeColor = new Color(0, 165, 35);
            float radius = projectile.size * 2f;
            int auratime = 60;
            var aura = BaseBuffAura.Spawn<AuraFriendly>(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, auratime, BuffType<AcidBuff>(), bufftime, radius, innerColor, edgeColor, projectile.Projectile.owner);
            aura.DisableRadiusFadeout();
            aura.SetRadiusFadein(0.4f, Ease.OutCubic);
            aura.SetAlphaFadeout(0.5f, Ease.OutSine);
            aura.SetDust(163, 4, 16, 1f, 0.8f, 1f);
            aura.DistortIntensity = 24f;
        }

        public static int AcidDotDmg = 16;
        public static int AcidDotDmgDev = 24;
        public static int AcidDotTime = 60;
        public static int AcidDotTimeDev = 100;
    }
}
