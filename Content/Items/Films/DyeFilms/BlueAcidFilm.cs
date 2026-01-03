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
    public class BlueAcidFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3028;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BlueAcidDotDmg / 2);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(BlueAcidDotDmgDev / 2);
        public static int BlueAcidDotDmg = 54;
        public static int BlueAcidDotDmgDev = 72;
        public static int BlueAcidDotTime = 80;
        public static int BlueAcidDotTimeDev = 120;
        public static int BlueAcidRange = 250;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FilmArgs = [("dot", (BlueAcidDotDmg / 2).ToString(), (BlueAcidDotDmgDev / 2).ToString())];
        }
        public override void OnSnap(BaseCameraProj projectile)
        {
            int bufftime = Main.player[projectile.Projectile.owner].DevEffect() ? BlueAcidDotTimeDev : BlueAcidDotTime;
            Color innerColor = new Color(131, 150, 224) * 0.15f;
            Color edgeColor = new Color(34, 45, 137);
            float radius = projectile.size * 2f;
            int auratime = 60;
            var aura = BaseBuffAura.Spawn<AuraFriendly>(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, auratime, BuffType<BlueAcidBuff>(), bufftime, radius, innerColor, edgeColor, projectile.Projectile.owner);
            aura.DisableRadiusFadeout();
            aura.SetRadiusFadein(0.4f, Common.Easer.Ease.OutCubic);
            aura.SetAlphaFadeout(0.5f, Common.Easer.Ease.OutSine);
            aura.SetDust(DustID.MushroomSpray, 4, 15, 0.7f, 0.8f, 1f);
            aura.DistortIntensity = 24f;
        }
    }
}
