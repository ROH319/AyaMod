using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using System;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Evolutive() : ExtraCameraPrefix()
    {
        public override void GlobalProjectile_OnHit(Projectile projectile, NPC target, NPC.HitInfo info, int damageDone)
        {
            if (target.life > 0) return;
            Player player = Main.player[projectile.owner];
            var mplr = player.GetModPlayer<EvolutivePlayer>();
            mplr.SetEvolutiveDamage(target.damage / 2);
        }
    }
    public class EvolutivePlayer : ModPlayer
    {
        public int EvolutiveDamage;
        public int EvolutiveTimeleft;
        public static int EvolutiveMaxTime = 5 * 60;

        public void SetEvolutiveDamage(int damage)
        {
            if(damage > EvolutiveDamage)
            {
                EvolutiveDamage = damage;
                EvolutiveTimeleft = EvolutiveMaxTime;
            }
        }
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (Player.HeldItem.prefix != PrefixType<Evolutive>()) return;
            damage.Flat += EvolutiveDamage;
        }

        public override void PostUpdateMiscEffects()
        {
            EvolutiveTimeleft--;
            if (EvolutiveTimeleft <= 0)
            {
                EvolutiveDamage = 0;
            }
        }
    }
}
