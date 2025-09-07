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
            //Main.NewText($"{Player.controlRight} {Player.releaseRight}");
            //if (Player.dashType != 0) HasDash = true;
            //if (Player.dashDelay > 0 && DashDelay > 0)
            //    Player.dashDelay = Math.Max(Player.dashDelay, DashDelay);
            //AddDashes(Player);
            //HandleDashes(Player);

            //if (DashDelay > 0)
            //    DashDelay--;

            //if(DashTimer > 0)
            //    DashTimer--;
        }

        public override void PostUpdateEquips()
        {
            Player.wingTimeMax = (int)WingTimeModifier.ApplyTo(Player.wingTimeMax);

        }

        public override void PostUpdateMiscEffects()
        {
            AddDashes(Player);
            UpdateDash();

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
