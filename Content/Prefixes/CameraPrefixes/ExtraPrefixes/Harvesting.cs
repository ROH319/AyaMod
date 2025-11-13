using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    [ProjectileEffect]
    public class Harvesting() : ExtraCameraPrefix(focusSpeedMult:0.85f,sizeMult:1.1f)
    {
        public override void Load()
        {
            AyaGlobalProjectile.OnProjectileSpawn += HarvestingSpawn;
            AyaGlobalProjectile.ModifyProjectileHitNPC += HarvestingHit;
            AyaGlobalProjectile.OnProjectileHitNPC += HarvestingOnHit;
        }

        public static void HarvestingHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.hostile || projectile.penetrate == 1 || !projectile.HasEffect<Harvesting>()) return;
            modifiers.FinalDamage += 0.2f;
            projectile.CritChance += 10;
        }
        public static void HarvestingOnHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //减穿透在OnHit后
            if (projectile.hostile || projectile.penetrate == 1 || !projectile.HasEffect<Harvesting>()) return;

            projectile.CritChance -= 10;
        }


        public static void HarvestingSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.hostile) return;
            Player player = Main.player[projectile.owner];
            if (player.HeldItem.prefix != PrefixType<Harvesting>()) return;
            if (!ProjectileID.Sets.CultistIsResistantTo[projectile.type] && projectile.penetrate == 1)
            {
                projectile.penetrate++;
            }
            projectile.AddEffect<Harvesting>();
        }
    }
}
