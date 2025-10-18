using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
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
    public class StealthBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(SpeedBonus, AccSpeedBonus, DamageBonus, CritBonus, HurtModifier);
        public static void StealthEffect(Player player)
        {
            player.moveSpeed += SpeedBonus / 100f;
            player.Aya().AccSpeedModifier += AccSpeedBonus / 100f;
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public static void StealthDR(Player player, ref Player.HurtModifiers modifier)
        {
            modifier.FinalDamage *= (1f - HurtModifier / 100f);
        }

        public static void StealthDrawEffect(Player player, ref PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            r *= 0.5f;
            g *= 0.5f;
            b *= 0.5f;
        }

        public static int SpeedBonus = 10;
        public static int AccSpeedBonus = 10;
        public static int DamageBonus = 10;
        public static int CritBonus = 5;
        public static int HurtModifier = 50;
    }
}
