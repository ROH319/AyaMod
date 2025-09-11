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
    public class BlueAcidFilm : BaseDyeFilm
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
                    if (npc.Distance(target.Center) < BlueAcidRange)
                    {
                        npc.AddBuff(ModContent.BuffType<BlueAcidBuff>(), BlueAcidDotTimeDev);

                    }
                }
            }
            else
            {
                target.AddBuff(ModContent.BuffType<BlueAcidBuff>(), BlueAcidDotTime);
            }
        }

        public static int BlueAcidDotDmg = 54;
        public static int BlueAcidDotTime = 80;
        public static int BlueAcidDotTimeDev = 120;
        public static int BlueAcidRange = 250;
    }
}
