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
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class RedAcidFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3560;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RedAcidDotDmg / 2);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(RedAcidDotDmgDev / 2);

        public static int RedAcidDotDmg = 120;
        public static int RedAcidDotDmgDev = 180;
        public static int RedAcidDotTime = 100;
        public static int RedAcidDotTimeDev = 160;
        public static int RedAcidRange = 350;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FilmArgs = [("dot", (RedAcidDotDmg / 2).ToString(), (RedAcidDotDmgDev / 2).ToString())];
        }
        public override void OnSnap(BaseCameraProj projectile)
        {
            int bufftime = Main.player[projectile.Projectile.owner].DevEffect() ? RedAcidDotTimeDev : RedAcidDotTime;
            //Color innerColor = new Color(255, 75, 75) * 0.15f;
            //Color edgeColor = new Color(200, 0, 0);
            Color innerColor = new Color(255, 75, 75) * 0.15f;
            Color edgeColor = new Color(216, 58, 47);
            float radius = projectile.size * 2.5f;
            int auratime = 60;
            var aura = BaseBuffAura.Spawn<AuraFriendly>(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, auratime, BuffType<RedAcidBuff>(), bufftime, radius, innerColor, edgeColor, projectile.Projectile.owner);
            aura.DisableRadiusFadeout();
            aura.SetRadiusFadein(0.4f, Common.Easer.Ease.OutCubic);
            aura.SetAlphaFadeout(0.5f, Common.Easer.Ease.OutSine);
            aura.SetDust(DustID.GemRuby, 4, 14, 1f, 0.8f, 1f);
            aura.DistortIntensity = 24f;
        }
    }
}
