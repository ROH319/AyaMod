using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static AyaMod.Core.Globals.AyaGlobalProjectile;

namespace AyaMod.Core.Globals
{
    public class GlobalCamera : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public delegate void CameraProjDelegate(BaseCameraProj projectile);

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            if(entity.ModProjectile == null || entity.ModProjectile is not BaseCameraProj) return false;
            return base.AppliesToEntity(entity, lateInstantiation);
        }

        public delegate bool PreAIDelegate(Player player, BaseCameraProj projectile);
        public static event PreAIDelegate PreAIHook = (plr, p) => true;
        public override bool PreAI(Projectile projectile)
        {
            bool result = true;
            foreach (PreAIDelegate g in PreAIHook.GetInvocationList())
                result &= g.Invoke(Main.player[projectile.owner], projectile.ModProjectile as BaseCameraProj);
            return result;
        }

        public static event ProjectileModifyHitNPCDelegate ModifyHitNPCHook = (Projectile p, NPC n, ref NPC.HitModifiers m) => { };
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach(ProjectileModifyHitNPCDelegate p in ModifyHitNPCHook.GetInvocationList())
            {
                p(projectile, target, ref modifiers);
            }
        }

        public static event ProjectileHitNPCDelegate OnHitNPCHook = (p, n, h, d) => { };
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (ProjectileHitNPCDelegate g in OnHitNPCHook.GetInvocationList())
                g.Invoke(projectile, target, hit, damageDone);
        }

        public static event CameraNPCEvents.CameraNPCDelegate HoverNPCHook = (p, n) => { };
        public static void HoverNPC(BaseCameraProj projectile, NPC npc)
        {
            if (HoverNPCHook == null) return;
            foreach (CameraNPCEvents.CameraNPCDelegate g in HoverNPCHook.GetInvocationList())
                g.Invoke(projectile, npc);
        }

        public static event CameraNPCEvents.CameraNPCDelegate NotHoverNPCHook = (p, n) => { };
        public static void NotHoverNPC(BaseCameraProj projectile, NPC npc)
        {
            if (NotHoverNPCHook == null) return;
            foreach (CameraNPCEvents.CameraNPCDelegate g in NotHoverNPCHook.GetInvocationList())
                g.Invoke(projectile, npc);
        }

        public static event CameraProjDelegate PreClearHook = (p) => { };
        public static void PreClear(BaseCameraProj projectile)
        {
            foreach(CameraProjDelegate g in PreClearHook.GetInvocationList())
                g.Invoke(projectile);
        }

        public delegate void PostClearDelegate(BaseCameraProj projectile, int captureCount);
        public static event PostClearDelegate PostClearHook = (p, c) => { };
        public static void PostClear(BaseCameraProj projectile, int captureCount)
        {
            if (PostClearHook == null) return;
            foreach (PostClearDelegate g in PostClearHook.GetInvocationList())
                g.Invoke(projectile, captureCount);
        }

        public static event CameraProjDelegate SnapHook = (p) => { };
        public static void OnSnap(BaseCameraProj projectile)
        {
            foreach(CameraProjDelegate g in SnapHook.GetInvocationList())
                g.Invoke(projectile);
        }

        public static event CameraProjDelegate SnapInSightHook = (p) => { };
        public static void OnSnapInSight(BaseCameraProj projectile)
        {
            foreach (CameraProjDelegate g in SnapInSightHook.GetInvocationList())
                g.Invoke(projectile);
        }

        public override void Unload()
        {
            PreAIHook = null;
            HoverNPCHook = null;
            NotHoverNPCHook = null;
            PostClearHook = null;
            SnapHook = null;
        }
    }
}
