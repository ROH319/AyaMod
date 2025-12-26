using AyaMod.Common.ItemDropRules;
using AyaMod.Content.Items.Accessories;
using AyaMod.Content.Items.Cameras;
using AyaMod.Content.Items.Films;
using AyaMod.Content.Items.Materials;
using AyaMod.Content.Items.PrefixHammers;
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
                    npcLoot.Add(ItemDropRule.Common(ItemType<CthulhuLens>(), 4));
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
                    npcLoot.Add(ItemDropRule.Common(ItemType<BeeCamera>(), 4));
                    break;
                case NPCID.Deerclops:
                    npcLoot.Add(ItemDropRule.Common(ItemType<ColdSnap>(), 4));
                    break;
                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.Common(ItemType<YukariCamera>(), 4));
                    break;
                case NPCID.BloodNautilus:
                    npcLoot.Add(ItemDropRule.Common(ItemType<ScarletCamera>(), 1));
                    break;
                case NPCID.Spazmatism:
                    npcLoot.Add(ItemDropRule.Common(ItemType<SpazmatismLens>(), 4));
                    break;
                case NPCID.Retinazer:
                    npcLoot.Add(ItemDropRule.Common(ItemType<RetinazerLens>(), 4));
                    break;
                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.Common(ItemType<YukaCamera>(), 4));
                    break;
                case NPCID.Everscream:
                    npcLoot.Add(ItemDropRule.Common(ItemType<FestiveHammer>(), 4));
                    break;
                case NPCID.Pumpking:
                    npcLoot.Add(ItemDropRule.Common(ItemType<HarvestingHammer>(), 4));
                    break;
                case NPCID.DD2Betsy:

                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.Common(ItemType<DukeCamera>(), 4));
                    break;
                case NPCID.HallowBoss:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EmpressCamera>(), 4));
                    break;
                #endregion

                case NPCID.Ghost:
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedSkeletron(), ItemType<SpiritHeart>(), 6));
                    break;
                case NPCID.PirateGhost:
                    npcLoot.Add(new CommonDrop(ItemType<CaptainCamera>(), 10, 1, 1, 4));
                    break;
                case NPCID.Drippler:
                case NPCID.BloodZombie:
                    npcLoot.Add(new CommonDrop(ItemType<BloodyFilm>(), 10, 1, 1, 1));
                    npcLoot.Add(new CommonDrop(ItemType<BloodthirstyHammer>(), 10, 1, 1, 1));
                    break;
                case NPCID.LunarTowerSolar:
                case NPCID.LunarTowerVortex:
                case NPCID.LunarTowerNebula:
                case NPCID.LunarTowerStardust:
                    {
                        int windyStarFragment = ItemType<WindyStarFragment>();
                        var parameters = new DropOneByOne.Parameters()
                        {
                            ChanceNumerator = 1,
                            ChanceDenominator = 1,
                            MinimumStackPerChunkBase = 1,
                            MaximumStackPerChunkBase = 2,
                            MinimumItemDropsCount = 12,
                            MaximumItemDropsCount = 15
                        };
                        npcLoot.Add(new DropOneByOne(windyStarFragment, parameters));
                    }
                    break;
                default: break;
            }
        }

    }
}
