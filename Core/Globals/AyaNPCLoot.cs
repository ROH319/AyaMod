using AyaMod.Content.Items.Accessories;
using AyaMod.Content.Items.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace AyaMod.Core.Globals
{
    public partial class AyaGlobalNPC : GlobalNPC
    {


        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CthulhuLens>(), 4));
                    break;
                case NPCID.QueenBee:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BeeCamera>(), 4));
                    break;
                case NPCID.Deerclops:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ColdSnap>(), 4));
                    break;
                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<YukariCamera>(), 4));
                    break;
                case NPCID.BloodNautilus:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScarletCamera>(), 1));
                    break;
                case NPCID.Spazmatism:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpazmatismLens>(), 4));
                    break;
                case NPCID.Retinazer:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RetinazerLens>(), 4));
                    break;
                default: break;
            }
        }

    }
}
