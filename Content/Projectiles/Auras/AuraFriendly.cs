using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Projectiles.Auras
{
    public class AuraFriendly : BaseBuffAura
    {
        public override void ApplyBuff()
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.Distance(Projectile.Center) > Radius * 0.45f) continue;
                npc.AddBuff((int)BuffType, (int)BuffDuration);
            }
        }
    }
}
