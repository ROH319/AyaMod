using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Tetrad() : ExtraCameraPrefix(damageMult:1.08f,focusSpeedMult:0.96f,critBonus:4)
    {
        public override void Player_PostUpdateMiscEffects(Player player)
        {
            var melee = player.GetTotalDamage(DamageClass.Melee).Additive - 1f;
            var ranged = player.GetTotalDamage(DamageClass.Ranged).Additive - 1f;
            var magic = player.GetTotalDamage(DamageClass.Magic).Additive - 1f;
            var summon = player.GetTotalDamage(DamageClass.Summon).Additive - 1f;
            player.GetDamage(ReporterDamage.Instance) += (melee + ranged + magic + summon) * 0.25f;
        }
    }
}
