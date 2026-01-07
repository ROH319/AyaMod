using AyaMod.Content.Items.Consumables;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using System;
using Terraria;

namespace AyaMod.Content.Buffs
{
    [PlayerEffect]
    public class TenguBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Load()
        {
            AyaPlayer.PostUpdateMiscEffectsHook += DashBonus;
        }

        public static void DashBonus(Player player)
        {
            if (player.HasEffect<TenguBuff>() && (player.dashDelay != 0 || player.Aya().DashDelay != 0))
                player.position += player.velocity * (TenguTango.SpeedBonus / 100f);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.velocity.Y != 0)
                player.moveSpeed += TenguTango.SpeedBonus / 100f;
            else
                player.moveSpeed -= TenguTango.SpeedDeduction / 100f;
            player.AddEffect<TenguBuff>();
        }
    }
}
