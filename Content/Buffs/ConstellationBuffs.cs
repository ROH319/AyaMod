using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Globals;
using Terraria;
using Terraria.Localization;
using AyaMod.Helpers;

namespace AyaMod.Content.Buffs
{
    public class OrionBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(DamageBonus);
        public static int DamageBonus = 15;
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += DamageBonus / 100f;
        }
    }

    public class CancerBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(RegenBonus);
        public static int RegenBonus = 10;
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += RegenBonus;
        }
    }
    public class HerculesBuff : ModBuff 
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(DefenseBonus, DRBonus);
        public static int DefenseBonus = 15;
        public static int DRBonus = 5;
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += DefenseBonus;
            player.endurance += DRBonus / 100f;
        }
    }
    public class PegasusBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(MovementBonus);
        public static int MovementBonus = 25;
        /// <summary>
        /// 钩爪牵引速度加成参见<see cref="AyaGlobalProjectile.GrapplePullSpeed(Projectile, Player, ref float)"/>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="buffIndex"></param>
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += MovementBonus / 100f;
            player.GetModPlayer<AyaPlayer>().AccSpeedModifier += MovementBonus / 100f;
            player.GetModPlayer<AyaPlayer>().WingTimeModifier *= 2f;
        }
    }
    public class LyraBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description =>
            base.Description.WithFormatArgs(MeleeSpeedBonus, ArrowSpeedBonus, WhipRangeBonus, AmmoCostBonus, ManaCostBonus);
        public static int MeleeSpeedBonus = 15;
        public static int ArrowSpeedBonus = 50;
        public static int WhipRangeBonus = 30;
        public static int AmmoCostBonus = 50;
        public static int ManaCostBonus = 15;
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeedBonus / 100f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += MeleeSpeedBonus / 100f;

            var cp = player.Camera();

            cp.ChaseSpeedModifier += ArrowSpeedBonus / 100f;

            player.whipRangeMultiplier += WhipRangeBonus / 100f;
            cp.SizeModifier += WhipRangeBonus / 100f;

            cp.AmmoCostModifier -= AmmoCostBonus / 100f;

            player.manaCost -= ManaCostBonus / 100f;
        }
    }
}
