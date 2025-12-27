using AyaMod.Core.Globals;
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
    public class Devouring() : ExtraCameraPrefix(damageMult: 1.1f, critBonus:5)
    {
        public override void Camera_PreAI(Player player, BaseCameraProj projectile)
        {
            if (projectile.player.ItemAnimationActive)
            {
                float devourRange = projectile.size * 1.5f;
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.friendly || npc.lifeMax <= 5 || npc.life <= 0 || npc.Distance(projectile.Projectile.Center) > devourRange) continue;

                    npc.position += npc.DirectionToSafe(projectile.Projectile.Center) * 1.2f;
                    //npc.velocity += npc.DirectionToSafe(projectile.Projectile.Center) * 0.05f;
                }
            }
        }
    }
}
