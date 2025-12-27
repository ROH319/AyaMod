using AyaMod.Core.Globals;
using AyaMod.Core.ItemDropRules;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Necromantic() : ExtraCameraPrefix(damageMult:1.16f,critBonus:16)
    {
        public override void Camera_OnSnap(BaseCameraProj projectile)
        {
            if (Main.rand.NextFloat(100) >= SpawnGhostChance) return;
            var n = NPC.NewNPC(projectile.Projectile.GetSource_FromThis(), (int)projectile.Projectile.Center.X, (int)projectile.Projectile.Center.Y, NPCID.DungeonSpirit);
            NPC npc = Main.npc[n];
            npc.GetGlobalNPC<GhostNerfed>().nerfed = true;
        }
        public static int SpawnGhostChance = 20;
    }

    public class GhostNerfed : GlobalNPC
    {
        public bool nerfed = false;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == NPCID.DungeonSpirit;
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source is not EntitySource_Parent parent || parent.Entity is not Projectile parentProj
                || !parentProj.TryGetCameraProj(out BaseCameraProj camera)
                || camera.player.HeldItem.prefix != PrefixType<Necromantic>()) return;

            npc.lifeMax /= 3;
            npc.life = npc.lifeMax;
            npc.damage /= 2;
            nerfed = true;
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.RemoveWhere(rule => rule is CommonDrop commonDrop && commonDrop.itemId == ItemID.Ectoplasm && LockSpiritDrop(npcLoot, rule));
        }
        public static bool LockSpiritDrop(NPCLoot npcLoot, IItemDropRule rule)
        {
            NecromanticGhostDropCondition necroCondition = new ();
            IItemDropRule conditionalRule = new LeadingConditionRule(necroCondition);
            conditionalRule.OnSuccess(rule);
            npcLoot.Add(conditionalRule);
            return true;
        }
        public override bool InstancePerEntity => true;
    }
}
