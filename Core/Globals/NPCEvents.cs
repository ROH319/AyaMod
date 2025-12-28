using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Globals
{
    public class NPCEvents
    {
        public delegate void NPCDelegate(NPC npc);
        public delegate void ModifyIncomingHitDelegate(NPC npc, ref NPC.HitModifiers modifiers);
        public delegate void HitByProjectileModifierDelegate(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers);
        public delegate void CameraNPCDelegate(BaseCameraProj projectile, NPC npc);

    }
}
