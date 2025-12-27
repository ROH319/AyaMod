using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Hallowed() : ExtraCameraPrefix(damageMult:1.1f,focusSpeedMult:0.9f,critBonus:5,sizeMult:0.9f,stunMult:3f)
    {
        public override void Camera_OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist > 0f) return;

            Player player = Main.player[projectile.owner];
            if (player.TryGetHeldModItem(out ModItem moditem) && moditem is BaseCamera camera)
            {
                var stuntime = player.Camera().GetStunTime(camera);
                var hallowedNPC = target.GetGlobalNPC<HallowedNPC>();
                hallowedNPC.DizzyCounter += (int)stuntime;

                int dizzyRequired = target.lifeMax / 100;
                if (hallowedNPC.DizzyCounter > dizzyRequired)
                {
                    target.Camera().StunTimer += (int)(target.boss ? stuntime * 2.5f : stuntime * 5);

                    hallowedNPC.DizzyCounter = 0;
                }
            }
        }
    }
    public class HallowedNPC : GlobalNPC
    {
        public int DizzyCounter;
        public override bool InstancePerEntity => true;
    }
}
