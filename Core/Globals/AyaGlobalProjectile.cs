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
        public delegate void ProjectileModifyHitNPCDelegate(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers);
        public delegate void ProjectileHitNPCDelegate(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone);
        public delegate bool? ProjectileCanDamageChecker(Projectile projectile);
        public delegate void ProjectileDelegate(Projectile projectile);

        public StatModifier SpeedModifier;

        public int MaxTimeleft = 0;

        public IEntitySource SpawnSource;

        public delegate void ProjectileSpawnDelegate(Projectile projectile, IEntitySource source);
        public static event ProjectileSpawnDelegate OnProjectileSpawn = (p, s) => { };
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            SpawnSource = source;
            MaxTimeleft = projectile.timeLeft;
            foreach(ProjectileSpawnDelegate del in OnProjectileSpawn.GetInvocationList())
            {
                del.Invoke(projectile, source);
            }
        }

        public static event ProjectileDelegate OnProjectilePostAI = (p) => { };
        public override void PostAI(Projectile projectile)
        {
            foreach(ProjectileDelegate del in OnProjectilePostAI.GetInvocationList())
            {
                del.Invoke(projectile);
            }

            var speedModifier = SpeedModifier.ApplyTo(1f);
            projectile.position += projectile.velocity * (speedModifier - 1f);
        }

        public static event ProjectileModifyHitNPCDelegate ModifyProjectileHitNPC = (Projectile p, NPC target, ref NPC.HitModifiers modifiers) => { };
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach(ProjectileModifyHitNPCDelegate m in ModifyProjectileHitNPC.GetInvocationList())
            {
                m.Invoke(projectile, target, ref modifiers);
            }
        }

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
