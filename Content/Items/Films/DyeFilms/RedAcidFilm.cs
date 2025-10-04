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

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class RedAcidFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RedAcidDotDmg / 2);
        public override int DyeID => 3560;
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool devEffect = Main.player[projectile.Projectile.owner].DevEffect();
            if (devEffect)
            {
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (!npc.CanBeChasedBy(projectile, true)) continue;
                    if (npc.Distance(target.Center) < RedAcidRange)
                    {
                        npc.AddBuff(ModContent.BuffType<RedAcidBuff>(), RedAcidDotTimeDev);

                    }
                }
            }
            else
            {
                target.AddBuff(ModContent.BuffType<RedAcidBuff>(), RedAcidDotTime);
            }
        }

        public static int RedAcidDotDmg = 120;
        public static int RedAcidDotDmgDev = 180;
        public static int RedAcidDotTime = 100;
        public static int RedAcidDotTimeDev = 150;
        public static int RedAcidRange = 350;
    }
}
