using AyaMod.Content.Items.Accessories;
using AyaMod.Core.Loaders;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.ModPlayers
{
    public partial class AyaPlayer : ModPlayer
    {
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            ModifyHitByBoth(ref modifiers);
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            ModifyHitByBoth(ref modifiers);

        }

        public void ModifyHitByBoth(ref Player.HurtModifiers modifiers)
        {
            if (Player.HasEffect<FalsePHDJ>())
            {
                modifiers.FinalDamage *= 1f + (float)FalsePHDJ.HurtIncrease / 100f;
            }
        }
    }
}
