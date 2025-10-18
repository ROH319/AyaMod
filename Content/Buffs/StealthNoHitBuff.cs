using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Buffs
{
    public class StealthNoHitBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(StealthBuff.SpeedBonus, StealthBuff.AccSpeedBonus, StealthBuff.DamageBonus, StealthBuff.CritBonus, StealthBuff.HurtModifier);

        public override void SetStaticDefaults()
        {
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void Load()
        {
            AyaPlayer.ModifyHitByNPCHook += StealthHurtDR;
            AyaPlayer.OnHitByBothHook += StealthHurt;
            AyaPlayer.PostUpdateBuffsHook += StealthUpdate;
            AyaPlayer.PlayerDrawEffectHook += StealthDraw;
        }

        public static void StealthUpdate(Player player)
        {
            if(player.HasBuff<StealthBuff>() || player.HasBuff<StealthNoHitBuff>())
            {
                StealthBuff.StealthEffect(player);
            }
        }
        public static void StealthHurtDR(Player player, NPC npc, ref Player.HurtModifiers modifier)
        {
            if (player.HasBuff<StealthBuff>() || player.HasBuff<StealthNoHitBuff>())
            {
                StealthBuff.StealthDR(player, ref modifier);
            }
        }
        public static void StealthHurt(Player player, ref Player.HurtInfo hurtInfo)
        {
            player.ClearBuff(ModContent.BuffType<StealthNoHitBuff>());
        }
        public static void StealthDraw(Player player, ref PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (player.HasBuff<StealthBuff>() || player.HasBuff<StealthNoHitBuff>())
            {
                StealthBuff.StealthDrawEffect(player, ref drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            }
        }
    }
}
