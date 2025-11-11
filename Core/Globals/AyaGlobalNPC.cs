using AyaMod.Content.Items.Accessories;
using AyaMod.Content.Items.Cameras;
using AyaMod.Content.Items.Films.DyeFilms;
using AyaMod.Content.Items.PrefixHammers;
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
    public partial class AyaGlobalNPC : GlobalNPC
    {

        public StatModifier SpeedModifier;

        public bool ElectrifiedBuff;
        public bool ShadowSuckBuff;

        public bool Acid;
        public bool BlueAcid;
        public bool RedAcid;
        public bool Scared;
        public bool FlourshingPoison;

        public override void ResetEffects(NPC npc)
        {

            SpeedModifier = StatModifier.Default;

            ElectrifiedBuff = false;
            ShadowSuckBuff = false;

            Acid = false;
            BlueAcid = false;
            RedAcid = false;
            Scared = false;
            FlourshingPoison = false;
        }
        public override bool PreAI(NPC npc)
        {
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

            if (FlourshingPoison)
            {
                ChlorophyteFilm.SpawnSpore(npc);
            }
        }

        public static event NPCEvents.NPCDelegate OnNPCKill = (n) => { };
        public override void OnKill(NPC npc)
        {
            foreach(NPCEvents.NPCDelegate del in OnNPCKill.GetInvocationList())
            {
                del.Invoke(npc);
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            bool devEffect = Main.player.Any(player => player.AliveCheck(npc.Center, 2000) && player.DevEffect());
            if (ElectrifiedBuff)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height,DustID.Electric ,0,0,128, Scale: 0.6f);
                    d.noGravity = true;
                }

                npc.lifeRegen -= VitricLightning.VitricDotDmg;
                if (damage < 5)
                    damage = 5;
            }

            if (Acid)
            {
                npc.lifeRegen -= devEffect ? AcidFilm.AcidDotDmgDev : AcidFilm.AcidDotDmg;
                if (damage < 2)
                    damage = 2;
            }
            if (BlueAcid)
            {
                npc.lifeRegen -= BlueAcidFilm.BlueAcidDotDmg;
                if (damage < 4)
                    damage = 4;
            }
            if (RedAcid)
            {
                npc.lifeRegen -= devEffect ? RedAcidFilm.RedAcidDotDmgDev : RedAcidFilm.RedAcidDotDmg;
                if (damage < 6)
                    damage = 6;
            }
            if (FlourshingPoison)
            {
                npc.lifeRegen -= ChlorophyteFilm.FlourshingDotDmg;
                if(damage < 8) damage = 8;
            }
            if (ShadowSuckBuff)
            {
                npc.lifeRegen -= ShadowCamera.ShadowSuckDotDmg;
                if (damage < 4) 
                    damage = 4;
            }
        }
        public override void ModifyShop(NPCShop shop)
        {

            switch (shop.NpcType)
            {
                case NPCID.Merchant:
                    shop.Add(new Item(ItemType<FrugalHammer>()) { shopCustomPrice = Item.buyPrice(gold: 5) });
                    break;
                //军火商
                case NPCID.ArmsDealer:
                    shop.Add(new Item(ItemType<TriggerShutter>()) { shopCustomPrice = Item.buyPrice(gold: 1) });
                    break;
                //巫医
                case NPCID.WitchDoctor:
                    shop.Add(new Item(ItemType<AbundantHammer>()) { shopCustomPrice = Item.buyPrice(gold: 7) });
                    break;
                //机械师
                case NPCID.Mechanic:
                    shop.Add(new Item(ItemType<DigitalCamera>()) { shopCustomPrice = Item.buyPrice(gold: 10) });
                    shop.Add(new Item(ItemType<IntelligentGyroscope>()) { shopCustomPrice = Item.buyPrice(gold: 3) });
                    break;
                default:
                    break;
            }
        }
        public override bool InstancePerEntity => true;
    }
}
