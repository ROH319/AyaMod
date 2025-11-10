using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Core.ModPlayers
{
    public class ModPlayerEvents
    {
        public delegate void PlayerDelegate(Player player);

        public delegate void PlayerItemDelegate(Player player, Item item);

        public delegate void PlayerDrawEffectDelegate(Player player, ref PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright);

        public delegate void ModifyWeaponDamageDelegate(Player player, Item item, ref StatModifier modifier);

        public delegate void ModifyHitByNPCDelegate(Player player, NPC npc, ref Player.HurtModifiers modifier);

        public delegate void ModifyHitByProjectileDelegate(Player player, Projectile proj, ref Player.HurtModifiers modifier);

        public delegate void ModifyHitByBothDelegate(Player player, ref Player.HurtModifiers modifier);

        public delegate void OnHitByBothDelegate(Player player, ref Player.HurtInfo hurtInfo);

        public delegate void ModPlayerDelegate(ModPlayer modPlayer);

        public delegate void NaturalLifeRegenDelegate(ModPlayer modPlayer, ref float regen);
    }
}
