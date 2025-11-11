using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Phantom() : ExtraCameraPrefix(damageMult:1.05f,critBonus:5,sizeMult:0.9f)
    {
        public override void Load()
        {
            CameraPlayer.CheckSnapThrouthWallEvent += PhantomSnap;
            GlobalCamera.OnProjectileCanDamage += PhantomCanDamage;
            AyaGlobalProjectile.OnProjectileSpawn += PhantomSpawn;
        }

        public static void PhantomSpawn(Projectile projectile, Terraria.DataStructures.IEntitySource source)
        {
            if (projectile.hostile) return;
            Player player = Main.player[projectile.owner];
            if (player.HeldItem.prefix != PrefixType<Phantom>()) return;
            BaseCameraProj camera = null;
            if (!projectile.CameraSourcedProj(out camera)) return;
            projectile.tileCollide = false;
        }

        public static bool? PhantomCanDamage(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (player.HeldItem.prefix != PrefixType<Phantom>()) return null;
            bool colli = AyaUtils.CheckLineCollisionTile(projectile.Center, player.Center, 8);
            if (!colli) return false;
            return null;
        }

        public static bool PhantomSnap(Player player, BaseCameraProj proj) => player.HeldItem.prefix == PrefixType<Phantom>();
    }
}
