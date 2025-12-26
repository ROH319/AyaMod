using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Buffs
{
    public class UninhibitedBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(ChaseSpeedBonus);
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CameraPlayer>().ChaseSpeedModifier += ChaseSpeedBonus / 100f;
        }
        public static int ChaseSpeedBonus = 15;

    }
}
