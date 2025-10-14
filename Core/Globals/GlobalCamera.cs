using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Globals
{
    public class GlobalCamera : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            if(entity.ModProjectile == null || entity.ModProjectile is not BaseCameraProj) return false;
            return base.AppliesToEntity(entity, lateInstantiation);
        }

        public delegate bool PreAIDelegate(Player player, BaseCameraProj projectile);
        public static event PreAIDelegate PreAIHook;
        public override bool PreAI(Projectile projectile)
        {
            bool result = true;
            foreach (PreAIDelegate g in PreAIHook.GetInvocationList())
                result &= g.Invoke(Main.player[projectile.owner], projectile.ModProjectile as BaseCameraProj);
            return result;
        }

        public override void Unload()
        {
            PreAIHook = null;
        }
    }
}
