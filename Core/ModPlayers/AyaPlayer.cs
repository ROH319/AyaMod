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
using Terraria.GameInput;

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
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (AyaKeybindLoader.UltraMove.Current && Player.HeldCamera())
            {
                Vector2 targetDir = Vector2.Zero;
                if (triggersSet.Left)
                    targetDir.X -= 1;
                if (triggersSet.Right)
                    targetDir.X += 1;
                if (triggersSet.Jump || triggersSet.Up)
                    targetDir.Y -= 1;
                if (triggersSet.Down)
                    targetDir.Y += 1;

                if (targetDir != Vector2.Zero) targetDir = targetDir.SafeNormalize(Vector2.Zero);
                else return;

                Player.direction = targetDir.X > 0 ? 1 : -1;
                if (!IsUltraDashing)
                {
                    UltraDashDir = targetDir.ToRotation();
                    var dashTrail = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<UltraDash>(), 100, 0, Player.whoAmI);
                }
                else
                {
                    UltraDashDir = Utils.AngleLerp(UltraDashDir, targetDir.ToRotation(), 0.2f);
                }


                IsUltraDashing = true;
            }
            else
            {
                if (IsUltraDashing)
                {
                    foreach(var projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.owner != Player.whoAmI || projectile.type != ProjectileType<UltraDash>()) continue;
                        projectile.ai[0] = 1f;
                    }
                }
                IsUltraDashing = false;
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
            if(IsUltraDashing && Player.HasEffect<GaleGeta1>())
            {
                Player.velocity = UltraDashDir.ToRotationVector2() * 30f;
                if (MathF.Abs(Player.velocity.Y) < 0.1f) Player.velocity.Y = 0.000001f;
            }
        }

        public override void PostUpdateEquips()
        {
            Player.wingTimeMax = (int)WingTimeModifier.ApplyTo(Player.wingTimeMax);

            if (Player.HasEffect<GaleGeta1>())
            {
                if (IsUltraDashing)
                {
                    Player.noKnockback = true;
                    Player.noFallDmg = true;
                }
                Player.fullRotation = UltraDashDir + MathHelper.PiOver2;
                Player.fullRotationOrigin = Player.Center - Player.position;

                //Main.NewText($"{Player.fullRotation}");
            }
            else Player.fullRotation = 0;

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
            if (!IsUltraDashing) UltraDashDir = Utils.AngleLerp(UltraDashDir,-MathHelper.PiOver2,0.2f);

            //Main.NewText($"{Main.time} {Player.bodyRotation} {UltraDashDir} {Player.direction}");
        }

        public override void UpdateDead()
        {
            WingTimeModifier = StatModifier.Default;
        }
    }
}
