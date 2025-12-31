using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Projectiles.Auras
{
    public class AuraHostile : BaseBuffAura
    {
        public override void ApplyBuff()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (player.Distance(Projectile.Center) > Radius * 0.45f) continue;
                player.AddBuff((int)BuffType, (int)BuffDuration);
            }
        }
    }
}
