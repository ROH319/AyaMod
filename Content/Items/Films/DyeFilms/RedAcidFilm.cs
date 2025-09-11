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

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class RedAcidFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
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
                        npc.AddBuff(ModContent.BuffType<BlueAcidBuff>(), RedAcidDotTimeDev);

                    }
                }
            }
            else
            {
                target.AddBuff(ModContent.BuffType<BlueAcidBuff>(), RedAcidDotTime);
            }
        }

        public static int RedAcidDotDmg = 120;
        public static int RedAcidDotDmgDev = 180;
        public static int RedAcidDotTime = 100;
        public static int RedAcidDotTimeDev = 150;
        public static int RedAcidRange = 350;
    }
}
