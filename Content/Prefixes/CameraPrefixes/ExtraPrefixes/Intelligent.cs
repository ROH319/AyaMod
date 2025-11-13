using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Intelligent() : ExtraCameraPrefix(critBonus:10,sizeMult:0.9f)
    {
        public override void Load()
        {
            GlobalCamera.PostAIHook += IntelligentChase;
        }

        public static void IntelligentChase(Player player, BaseCameraProj projectile)
        {
            if (player.HeldItem.prefix != PrefixType<Intelligent>()) return;

            NPC target = null;
            float minDist = player.Camera().SizeModifier.ApplyTo(ChaseRange);
            foreach(var npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                if (npc.Distance(projectile.Projectile.Center) > minDist) continue;
                if (!Collision.CanHit(player.Center, 1, 1, npc.position, npc.width, npc.height)) continue;
                target = npc;
                minDist = npc.Distance(projectile.Projectile.Center);
            }
            if (target != null)
            {
                float chaseFactor = player.GetModPlayer<CameraPlayer>().ChaseSpeedModifier.ApplyTo(projectile.CameraStats.ChaseFactor);
                projectile.Projectile.Center = Vector2.Lerp(projectile.Projectile.Center, target.Center, chaseFactor * 2f);
            }
        }

        public static int ChaseRange = 16 * 15;
    }
}
