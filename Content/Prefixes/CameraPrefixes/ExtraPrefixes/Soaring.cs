using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Soaring() : ExtraCameraPrefix()
    {
        public override LocalizedText PrefixExtraTooltip => base.PrefixExtraTooltip.WithFormatArgs(SoaringBuff.DamageBonus, SoaringBuff.CritBonus, SoaringBuff.AttackSpeedBonus, SoaringBuff.ChaseSpeedBonus);
    }
    public class SoaringBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(DamageBonus, CritBonus, AttackSpeedBonus, ChaseSpeedBonus);
        public static int DamageBonus = 20;
        public static int CritBonus = 12;
        public static int AttackSpeedBonus = 12;
        public static int ChaseSpeedBonus = 30;
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
            player.GetAttackSpeed<ReporterDamage>() += AttackSpeedBonus / 100f;
            player.Camera().ChaseSpeedModifier += ChaseSpeedBonus / 100f;
        }
    }
    public class SoaringPlayer : ModPlayer
    {
        public int AirborneTimer;
        public override void PreUpdateBuffs()
        {
            if (AirborneTimer > 3 * 60 && Player.TryGetHeldModItem(out ModItem moditem) && moditem is BaseCamera camera && camera.Item.prefix == PrefixType<Soaring>())
                Player.AddBuff(BuffType<SoaringBuff>(), 2);
        }
        public override void PostUpdate()
        {

            AirborneTimer++;
            if (Player.mount.Active) AirborneTimer = 0;
            if (Player.grappling[0] != -1) AirborneTimer = 0;
            if(Player.velocity.Y == 0) AirborneTimer = 0; 
        }
    }
}
