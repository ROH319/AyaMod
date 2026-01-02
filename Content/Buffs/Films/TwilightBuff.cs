using AyaMod.Core;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs.Films
{
    public class TwilightBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs_Films + Name;
        public override void Load()
        {
            AyaGlobalProjectile.ModifyProjectileHitNPC += AyaGlobalProjectile_ModifyProjectileHitNPC;
        }

        public static void AyaGlobalProjectile_ModifyProjectileHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            if (player == null || !player.Alive() || !player.HasBuff<TwilightBuff>()) return;
            modifiers.SetCrit();
        }
    }
}
