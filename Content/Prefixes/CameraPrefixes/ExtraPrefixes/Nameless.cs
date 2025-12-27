using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    [ProjectileEffect]
    public class Nameless() : ExtraCameraPrefix(damageMult:1.35f,critBonus:15)
    {
        public override void GlobalProjectile_Spawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.hostile) return;
            projectile.AddEffect<Nameless>();
        }
        public override void GlobalProjectile_ModifyHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if(projectile.hostile || !projectile.HasEffect<Nameless>()) return;
            modifiers.FinalDamage *= 0.5f;
        }
    }
}
