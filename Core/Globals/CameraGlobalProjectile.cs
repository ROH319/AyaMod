using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Core.Globals
{
    public class CameraGlobalProjectile : GlobalProjectile
    {

        public int ShadowTimer;

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (ShadowTimer > 0)
            {
                float factor = ShadowTimer / 10f;
                float damageMult = Utils.Remap(factor, 0, 1f, 1f, 0.85f);
                modifiers.FinalDamage *= damageMult;
            }
        }



        public override bool InstancePerEntity => true;
    }
}
