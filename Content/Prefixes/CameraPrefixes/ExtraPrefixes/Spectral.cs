using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    [ProjectileEffect]
    public class Spectral() : ExtraCameraPrefix(damageMult:1.15f,focusSpeedMult:0.9f,sizeMult:1.1f)
    {
        public override void Load()
        {
            AyaGlobalProjectile.OnProjectileSpawn += SpectralSpawn;
            AyaGlobalProjectile.OnProjectilePostAI += SpectralAI;
        }

        public static void SpectralSpawn(Projectile projectile, IEntitySource source)
        {
            if (!ProjectileID.Sets.CultistIsResistantTo[projectile.type]) return;
            if (projectile.hostile) return;
            Player player = Main.player[projectile.owner];
            if (player.HeldItem.prefix != PrefixType<Spectral>()) return;
            BaseCameraProj camera = null;
            if (!projectile.CameraSourcedProj(out camera)) return;
            projectile.tileCollide = false;
            projectile.AddEffect<Spectral>();
        }

        public static void SpectralAI(Projectile projectile)
        {
            if (projectile.HasEffect<Spectral>())
                projectile.GetGlobalProjectile<AyaGlobalProjectile>().SpeedModifier += 0.2f;
        }
    }
}
