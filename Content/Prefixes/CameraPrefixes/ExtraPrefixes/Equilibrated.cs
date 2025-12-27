using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Equilibrated() : ExtraCameraPrefix(focusSpeedMult:0.9f,critBonus:5,sizeMult:1.05f)
    {
        public override void Player_PostUpdateMiscEffects(Player player)
        {
            int itemtime = player.GetModPlayer<AyaPlayer>().ItemTimer;
            if (itemtime > 0)
            {
                float bonus = Utils.Remap(itemtime, 0, 3 * 60, 0f, DamageBonus / 100f);
                player.GetDamage(ReporterDamage.Instance) += bonus;
            }
            else
            {
                int time = player.GetModPlayer<AyaPlayer>().NotItemTimer;
                float bonus = Utils.Remap(time, 0, 2 * 60, 0f, DRBonus / 100f);
                player.endurance += bonus;
            }
        }
        public static int DamageBonus = 20;
        public static int DRBonus = 20;
    }
}
