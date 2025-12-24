using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs
{
    public class OrionBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
        }
    }

    public class CancerBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 10;
        }
    }
    public class HerculesBuff : ModBuff 
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 15;
            player.endurance += 0.05f;
        }
    }
    public class PegasusBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.25f;
            player.GetModPlayer<AyaPlayer>().AccSpeedModifier += 0.25f;
            player.GetModPlayer<AyaPlayer>().WingTimeModifier *= 2f;
        }
    }
    public class LyraBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.15f;

            player.GetModPlayer<CameraPlayer>().ChaseSpeedModifier += 0.5f;
            player.GetModPlayer<CameraPlayer>().SizeModifier += 0.5f;

            

            player.manaCost -= 0.15f;
        }
    }
}
