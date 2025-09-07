using AyaMod.Content.Items.Accessories;
using AyaMod.Content.Items.Accessories.Movements;
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
        public int itemTimeLastFrame;
        public int freeFlyFrame = 0;
        public int FreeFlyFrame = 0;
        public StatModifier WingTimeModifier = StatModifier.Default;
        public StatModifier AccSpeedModifier = StatModifier.Default;


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
        public override void ResetEffects()
        {
            FreeFlyFrame = 0;
            AccSpeedModifier = StatModifier.Default;
            WingTimeModifier = StatModifier.Default;
            HasDash = false;
            AyaDash = DashType.None;

            ResetAyaEffects();

            ResetDashDir();
        }

        public override void PostUpdateRunSpeeds()
        {
            
            if(!Player.pulley && Player.grappling[0] == -1 && !Player.tongued)
            {
                Player.accRunSpeed = AccSpeedModifier.ApplyTo(Player.accRunSpeed);
            }
        }

        public override void PreUpdateMovement()
        {
            if (Player.grappling[0] == -1 && !Player.tongued
                && !Player.CCed && !Player.mount.Active)
            {
                var tile = Main.tile[(int)(Player.position.X / 16f), (int)((Player.position.Y + Player.height + 4) / 16f)];
                if(Player.velocity.Y > 0 && Player.wingTime <= 0 && Player.HasEffect<TenguWings1>() && !tile.HasTile)
                {
                    Player.position -= Player.velocity * 0.5f;
                }
            }
        }

        public override void PostUpdateEquips()
        {
            Player.wingTimeMax = (int)WingTimeModifier.ApplyTo(Player.wingTimeMax);

        }

        public override void PostUpdateMiscEffects()
        {
            AddDashes(Player);
            UpdateDash();

            if(Player.wingTime == Player.wingTimeMax - 1)
            {
                freeFlyFrame = FreeFlyFrame;
            }

            //Main.NewText($"{Player.velocity.Y} {Player.wingTime} {Player.wingTimeMax} {Main.time}");
            //Main.NewText($"{Player.dashDelay}");
            //Console.WriteLine($"{Player.dashDelay} {DashDelay}");
        }

        public override void PostUpdate()
        {
            itemTimeLastFrame = Player.itemTime;
        }

        public override void UpdateDead()
        {
            WingTimeModifier = StatModifier.Default;
        }
    }
}
