using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AyaMod.Core.Globals
{
    public class AyaGlobalProjectile : GlobalProjectile
    {

        public StatModifier SpeedModifier;

        public int MaxTimeleft = 0;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            MaxTimeleft = projectile.timeLeft;
            base.OnSpawn(projectile, source);
        }

        public override void PostAI(Projectile projectile)
        {

            if (projectile.hostile)
            {
                var speedModifier = SpeedModifier.ApplyTo(1f);
                projectile.position += projectile.velocity * (speedModifier - 1f);
            }
        }

        public override bool InstancePerEntity => true;
    }
}
