using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Buffs
{
    public class TripodBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(ChaseSpeedBonus, AutoBonus, ManualBonus);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            CameraPlayer cp = player.GetModPlayer<CameraPlayer>();
            cp.ChaseSpeedModifier += ChaseSpeedBonus / 100f;
            cp.AutoSnapDamageModifier.Base += AutoBonus / 100f;
            cp.ManualSnapDamageModifier.Base += ManualBonus / 100f;
        }
        public static int ChaseSpeedBonus = 20;
        public static int ManualBonus = 5;
        public static int AutoBonus = 5;
    }
}
