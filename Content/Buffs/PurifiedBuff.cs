using AyaMod.Content.Items.Armors;
using AyaMod.Core;
using AyaMod.Core.Globals;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs
{
    public class PurifiedBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;

        public static int DefenseDecrease = 25;

        public static int BuffTimeMin = 2;
        public static int BuffTimeMax = 60;
        public static int RegenBonusMin = 2;
        public static int RegenBonusMax = 8;
        public override void Load()
        {
            AyaGlobalNPC.ModifyHitHook += PurifiedHit;
        }

        public static void PurifiedHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!npc.HasBuff<PurifiedBuff>()) return;
            modifiers.Defense.Flat -= DefenseDecrease;
        }

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            int time = player.buffTime[buffIndex];
            float regen = Utils.Remap(time, BuffTimeMin, BuffTimeMax, RegenBonusMin, RegenBonusMax);
            player.lifeRegen += (int)regen;
            if (player.HasEffect(RumorBreakerHelmet.RumorBreakerSet))
                player.buffTime[buffIndex] = 2;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Aya().SpeedModifier -= 0.4f;
        }
        //public override bool ReApply(Player player, int time, int buffIndex)
        //{
        //    player.buffTime[buffIndex] += time;
        //    return false;
        //}
    }
}
