using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    [ProjectileEffect]
    public class Alchemistical() : ExtraCameraPrefix(damageMult:1.1f,focusSpeedMult:0.9f)
    {
        public override void Load()
        {
            AyaGlobalProjectile.OnProjectileSpawn += AlchemistSpawn;
            GlobalCamera.OnHitNPCHook += AlchemistHit;
        }

        public static void AlchemistSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.hostile) return;
            Player player = Main.player[projectile.owner];
            if (!player.TryGetHeldModItem(out var moditem) || moditem is not BaseCamera || moditem.Item.prefix != PrefixType<Alchemistical>()) return;
            if (projectile.ModProjectile is not BaseCameraProj) return;
            projectile.AddEffect<Alchemistical>();
        }

        public static void AlchemistHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!projectile.HasEffect<Alchemistical>()) return;

            if (target.life > 0)
            {
                for (int i = 0; i < NPC.maxBuffs; i++)
                {
                    if (!Main.debuff[target.buffType[i]] || target.buffTime[i] >= TimeleftThreshold) continue;
                    target.buffTime[i] += TimeToAdd;
                    var name = Lang.GetBuffName(target.buffType[i]);
                    CombatText.NewText(target.getRect(), Main.DiscoColor, $"+{TimeToAdd / 60}s{name}");
                }
            }

            Player player = Main.player[projectile.owner];
            if (player == null || !player.Alive()) return;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (!Main.debuff[player.buffType[i]]
                    || player.buffTime[i] >= FriendlyTimeleftMax || player.buffTime[i] <= FriendlyTimeleftMin) continue;

                player.buffTime[i] += FriendlyTimeToAdd;
                CombatText.NewText(player.getRect(), Main.DiscoColor, $"+{FriendlyTimeToAdd / 60}s");

            }
        }

        public static int TimeleftThreshold = 20 * 60;
        public static int TimeToAdd = 5 * 60;
        public static int FriendlyTimeleftMin = 20 * 60;
        public static int FriendlyTimeleftMax = 3 * 60 * 60;
        public static int FriendlyTimeToAdd = 10 * 60;
    }
}
