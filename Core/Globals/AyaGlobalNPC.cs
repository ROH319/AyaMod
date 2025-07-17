using AyaMod.Content.Items.Cameras;
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
    public class AyaGlobalNPC : GlobalNPC
    {

        public StatModifier SpeedModifier;

        public bool ElectrifiedBuff;
        public bool ShadowSuckBuff;

        public bool Acid;
        public bool Confused;
        public bool Scared;

        public override void ResetEffects(NPC npc)
        {

            SpeedModifier = StatModifier.Default;

            ElectrifiedBuff = false;
            ShadowSuckBuff = false;

            Acid = false;
            Confused = false;
            Scared = false;

        }
        public override bool PreAI(NPC npc)
        {
            if (Confused)
            {
                bool legilimencyEffect = Main.player.Any((player => player.AliveCheck(npc.Center, 3000) && player.HeldItem.type == ModContent.ItemType<Legilimency>()));

                if (legilimencyEffect)
                {
                    //降低20%速度
                    SpeedModifier *= 0.8f;
                }

                if (Main.rand.NextBool(3))
                {
                    foreach (var player in Main.ActivePlayers)
                    {
                        if (!player.AliveCheck(npc.Center, 2000) || !(player.HeldItem.type == ModContent.ItemType<Legilimency>())) continue;


                        Vector2 vel = npc.DirectionToSafe(player.Center) * 2f;
                        Dust d = Dust.NewDustPerfect(npc.Center, 112, vel, 128, Scale: 1f);
                        d.noGravity = true;
                    }
                }
            }
            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (Scared && !npc.boss && (npc.life / (float)npc.lifeMax) < 0.15f)
            {
                npc.StrikeInstantKill();
            }

            if (!npc.boss)
            {
                var speedModifier = SpeedModifier.ApplyTo(1f);
                npc.position += npc.velocity * (speedModifier - 1f);
            }
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (ElectrifiedBuff)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height,DustID.Electric ,0,0,128, Scale: 0.6f);
                    d.noGravity = true;
                }

                npc.lifeRegen -= 10;
                if (damage < 5)
                    damage = 5;
            }

            if (Acid)
            {
                npc.lifeRegen -= 20;
                if (damage < 2)
                    damage = 2;
            }
            if (Confused)
            {

                bool legilimencyEffect = Main.player.Any((player => player.AliveCheck(npc.Center, 3000)));

                if (legilimencyEffect)
                {
                    //15dps
                    npc.lifeRegen -= 30;
                }
                if (damage < 5)
                    damage = 5;
            }
            if (ShadowSuckBuff)
            {
                npc.lifeRegen -= 60;
                if (damage < 4) 
                    damage = 4;
            }
        }


        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.QueenBee:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BeeCamera>(), 4));
                    break;
                case NPCID.Deerclops:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ColdSnap>(), 4));
                    break;

                default: break;
            }
        }

        public override bool InstancePerEntity => true;
    }
}
