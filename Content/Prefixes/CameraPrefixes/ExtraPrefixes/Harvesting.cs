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
        public override void GlobalProjectile_Spawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.hostile) return;
            if (!ProjectileID.Sets.CultistIsResistantTo[projectile.type] && projectile.penetrate == 1)
            {
                projectile.penetrate++;
            }
            projectile.AddEffect<Harvesting>();
        }
        public override void GlobalProjectile_ModifyHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.hostile || projectile.penetrate == 1 || !projectile.HasEffect<Harvesting>()) return;
            modifiers.FinalDamage += 0.2f;
            projectile.CritChance += 10;
        }
        public override void GlobalProjectile_OnHit(Projectile projectile, NPC target, NPC.HitInfo info, int damageDone)
        {
            //减穿透在OnHit后
            if (projectile.hostile || projectile.penetrate == 1 || !projectile.HasEffect<Harvesting>()) return;

            projectile.CritChance -= 10;
        }
    }
}
