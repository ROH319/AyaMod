using AyaMod.Content.Items.Cameras;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Core.Globals
{
    public class CameraGlobalNPC : GlobalNPC
    {
        public int StunTimer;


        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is BaseCameraProj)
            {
                (projectile.ModProjectile as BaseCameraProj).ApplyStun(npc, ref modifiers);
                modifiers.DisableKnockback();
            }
            base.ModifyHitByProjectile(npc, projectile, ref modifiers);
        }


        public override void PostAI(NPC npc)
        {
            if (StunTimer > 0)
            {
                //npc.position -= npc.velocity;
                npc.velocity = Vector2.Zero;
            }
            UpdateStun();
        }

        public void UpdateStun()
        {
            if (StunTimer > 0) StunTimer--;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {

        }


        public override bool InstancePerEntity => true;
    }
}
