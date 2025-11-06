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
    public partial class AyaGlobalProjectile : GlobalProjectile
    {

        public StatModifier SpeedModifier;

        public int MaxTimeleft = 0;

        public delegate void ProjectileSpawnDelegate(Projectile projectile, IEntitySource source);
        public static event ProjectileSpawnDelegate OnProjectileSpawn = (p, s) => { };
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            MaxTimeleft = projectile.timeLeft;
            foreach(ProjectileSpawnDelegate del in OnProjectileSpawn.GetInvocationList())
            {
                del.Invoke(projectile, source);
            }
        }

        public override void PostAI(Projectile projectile)
        {

            if (projectile.hostile)
            {
                var speedModifier = SpeedModifier.ApplyTo(1f);
                projectile.position += projectile.velocity * (speedModifier - 1f);
            }
        }

        public delegate void ProjectileHitNPCDelegate(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone);
        public static event ProjectileHitNPCDelegate OnProjectileHitNPC = (p, n, h, d) => { };
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach(ProjectileHitNPCDelegate del in OnProjectileHitNPC.GetInvocationList())
            {
                del.Invoke(projectile, target, hit, damageDone);
            }
        }

        public override bool InstancePerEntity => true;
    }
}
