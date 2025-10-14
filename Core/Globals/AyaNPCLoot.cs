using AyaMod.Common.ItemDropRules;
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
                #region Boss
                case NPCID.EyeofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CthulhuLens>(), 4));
                    break;
                case NPCID.EaterofWorldsBody or NPCID.EaterofWorldsHead or NPCID.EaterofWorldsTail:
                    {
                        LeadingConditionRule lastEater = new(new Conditions.LegacyHack_IsABoss());
                        lastEater.OnSuccess(ItemDropRule.Common(ItemType<Luminvore>(), 4));
                        npcLoot.Add(lastEater);
                    }
                    break;
                case NPCID.BrainofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(ItemType<Legilimency>(), 4));
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
                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<YukaCamera>(), 4));
                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DukeCamera>(), 4));
                    break;
                case NPCID.HallowBoss:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EmpressCamera>(), 4));
                    break;
                #endregion

                case NPCID.Ghost:
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedSkeletron(), ItemType<SpiritHeart>(), 6));
                    break;
                case NPCID.PirateGhost:
                    npcLoot.Add(new CommonDrop(ItemType<CaptainCamera>(),4,1,1,10));
                    break;
                default: break;
            }
        }

    }
}
