using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    [ProjectileEffect]
    public class Abundant() : ExtraCameraPrefix(focusSpeedMult: 0.9f)
    {
        public override void Load()
        {
            AyaPlayer.NaturalLifeRegenHook += AbundantHook;
        }
        public static void AbundantHook(ModPlayer modPlayer, ref float regen)
        {
            if (modPlayer.Player.HeldItem.DamageType != ReporterDamage.Instance || modPlayer.Player.HeldItem.prefix != PrefixType<Abundant>())
                return;
            int liferegen = (int)(modPlayer.Player.lifeRegen + regen);
            if (liferegen < 0) return;
            //因为禁止了自然回血的机制，所以这里直接把回血时间设为负数
            modPlayer.Player.lifeRegenTime = -liferegen;
            regen = 0;
            modPlayer.Player.lifeRegen = 0;
        }
        public override void GlobalProjectile_Spawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.hostile) return; 
            BaseCameraProj camera = null;
            if (!projectile.CameraSourcedProj(out camera)) return;
            projectile.AddEffect<Abundant>();
        }
        public override void GlobalProjectile_OnHit(Projectile projectile, NPC target, NPC.HitInfo info, int damageDone)
        {
            if (projectile.HasEffect<Abundant>())
            {
                Player player = Main.player[projectile.owner];
                if (player == null || !player.Alive()) return;
                bool canheal = Regen2Heal((int)-player.lifeRegenTime);
                if (!canheal) return;
                player.Heal(1);
            }
        }
        public static bool Regen2Heal(int regen)
        {
            float healchance = Utils.Remap(regen, 0, 40, 0f, 1f);
            return Main.rand.NextFloat() < healchance;
        }
    }
}
