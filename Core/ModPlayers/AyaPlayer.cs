using AyaMod.Content.Buffs.Films;
using AyaMod.Content.Items.Accessories;
using AyaMod.Content.Items.Accessories.Movements;
using AyaMod.Content.Items.Films.DyeFilms;
using AyaMod.Core.Loaders;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace AyaMod.Core.ModPlayers
{
    public partial class AyaPlayer : ModPlayer
    {
        public int itemTimeLastFrame;
        public int noHitTimer;
        public int freeFlyFrame = 0;
        public int FreeFlyFrame = 0;
        public float AttackSpeed;
        public StatModifier HealLifeModifier = StatModifier.Default;
        public StatModifier WingTimeModifier = StatModifier.Default;
        public StatModifier AccSpeedModifier = StatModifier.Default;

        public int ItemTimer = 0;
        public int NotItemTimer = 0;
        public int NotUsingCameraTimer = 0;

        public bool UltraMoveEnabled = false;

        public int DamageReduceFlat = 0;
        public float SourceDamageReduce = 0f;
        public int ManicStack = 0;
        public float WispDmg = 0;
        public float InfernalWispDmg = 0;

        public static event ModPlayerEvents.PlayerDrawEffectDelegate PlayerDrawEffectHook;
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            PlayerDrawEffectHook?.Invoke(Player, ref drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }

        public static event ModPlayerEvents.ModifyHitByNPCDelegate ModifyHitByNPCHook;
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            ModifyHitByNPCHook?.Invoke(Player, npc, ref modifiers);
            ModifyHitByBoth(ref modifiers);
        }
        public static event ModPlayerEvents.ModifyHitByProjectileDelegate ModifyHitByProjectileHook;
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            ModifyHitByProjectileHook?.Invoke(Player, proj, ref modifiers);
            ModifyHitByBoth(ref modifiers);
        }

        public static event ModPlayerEvents.ModifyHitByBothDelegate ModifyHitByBothHook;
        public void ModifyHitByBoth(ref Player.HurtModifiers modifiers)
        {
            ModifyHitByBothHook?.Invoke(Player, ref modifiers);
            if (DamageReduceFlat > 0)
            {
                modifiers.FinalDamage.Flat -= (float)DamageReduceFlat;
                DamageReduceFlat = 0;
            }
            if (SourceDamageReduce > 0)
            {
                modifiers.SourceDamage -= SourceDamageReduce;
            }
            noHitTimer = 0;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHitByBoth(ref hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHitByBoth(ref hurtInfo);
        }

        public static event ModPlayerEvents.OnHitByBothDelegate OnHitByBothHook;
        public void OnHitByBoth(ref Player.HurtInfo hurtInfo)
        {
            OnHitByBothHook?.Invoke(Player, ref hurtInfo);
        }

        public override bool CanUseItem(Item item)
        {
            if (IsUltraDashing) return false;
            return base.CanUseItem(item);
        }
        public override float UseSpeedMultiplier(Item item)
        {
            int useTime = item.useTime;
            int useAnimate = item.useAnimation;

            if (useTime <= 0 || useAnimate <= 0 || item.damage <= 0)
                return base.UseSpeedMultiplier(item);

            if (Player.HasEffect<ManicPupil>())
            {
                ManicPupil.CalManicAttackSpeed(Player, item);
            }

            return AttackSpeed;
        }

        public static event ModPlayerEvents.ModifyWeaponDamageDelegate ModifyWeaponDamageHook;
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            ModifyWeaponDamageHook?.Invoke(Player, item, ref damage);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (AyaKeybindLoader.UltraMove.Current && UltraMoveEnabled)
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

        public static event ModPlayerEvents.PlayerDelegate DoubleTapHook = (p) => { };
        public override void ArmorSetBonusActivated()
        {
            foreach (ModPlayerEvents.PlayerDelegate g in DoubleTapHook.GetInvocationList())
            {
                g(Player);
            }
        }

        public override void PreUpdate()
        {

        }
        public override void ResetEffects()
        {
            FreeFlyFrame = 0;
            AttackSpeed = 1f;
            HealLifeModifier = StatModifier.Default;
            AccSpeedModifier = StatModifier.Default;
            WingTimeModifier = StatModifier.Default;
            HasDash = false;
            AyaDash = DashType.None;

            DamageReduceFlat = 0;
            SourceDamageReduce = 0;
            UltraMoveEnabled = false;

            ResetAyaEffects();

            ResetDashDir();
        }

        public static event ModPlayerEvents.PlayerDelegate PostUpdateBuffsHook;
        public override void PostUpdateBuffs()
        {
            bool devEffect = Player.DevEffect();
            WispDmg += (int)(devEffect ? WispFilm.WispDmgRegenDev : WispFilm.WispDmgRegen);
            WispDmg = MathHelper.Clamp(WispDmg, 0, devEffect ? WispFilm.WispDmgMaxDev : WispFilm.WispDmgMax);

            InfernalWispDmg += InfernalWispDmg;
            InfernalWispDmg = MathHelper.Clamp(InfernalWispDmg, 0, InfernalWispFilm.WispDmgMax);

            PostUpdateBuffsHook?.Invoke(Player);
        }
        public override void PostUpdateRunSpeeds()
        {
            
            if(!Player.pulley && Player.grappling[0] == -1 && !Player.tongued)
            {
                Player.accRunSpeed = AccSpeedModifier.ApplyTo(Player.accRunSpeed);
            }
        }
        public static event ModPlayerEvents.PlayerDelegate PreUpdateMovementHook = (p) => { };
        public override void PreUpdateMovement()
        {
            foreach (ModPlayerEvents.PlayerDelegate p in PreUpdateMovementHook.GetInvocationList())
                p(Player);

            if(UltraMoveEnabled && IsUltraDashing)
            {
                Player.velocity = UltraDashDir.ToRotationVector2() * 30f;
                if (MathF.Abs(Player.velocity.Y) < 0.1f) Player.velocity.Y = 0.000001f;
            }
        }

        public override void PostUpdateEquips()
        {
            Player.wingTimeMax = (int)WingTimeModifier.ApplyTo(Player.wingTimeMax);

            if (UltraMoveEnabled)
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

        public static event ModPlayerEvents.PlayerDelegate PostUpdateMiscEffectsHook = (p) => { };
        public override void PostUpdateMiscEffects()
        {
            AddDashes(Player);
            UpdateDash();

            if(Player.wingTime == Player.wingTimeMax - 1)
            {
                freeFlyFrame = FreeFlyFrame;
            }

            if(Player.itemTime > 0)
            {
                ItemTimer++;
                NotItemTimer = 0;
            }
            else
            {
                NotItemTimer++;
                ItemTimer = 0;
            }
            NotUsingCameraTimer++;
            noHitTimer++;

            foreach(ModPlayerEvents.PlayerDelegate g in PostUpdateMiscEffectsHook.GetInvocationList())
            {
                g(Player);
            }
            //Main.NewText($"{Player.velocity.Y} {Player.wingTime} {Player.wingTimeMax} {Main.time}");
            //Main.NewText($"{Player.dashDelay}");
            //Console.WriteLine($"{Player.dashDelay} {DashDelay}");
            //Main.NewText($"{Player.itemTime} {Player.itemTimeMax} {Main.time}");
        }


        public static event ModPlayerEvents.NaturalLifeRegenDelegate NaturalLifeRegenHook = (ModPlayer m, ref float regen) => { };
        public override void NaturalLifeRegen(ref float regen)
        {
            foreach(ModPlayerEvents.NaturalLifeRegenDelegate g in NaturalLifeRegenHook.GetInvocationList())
                g(this, ref regen);
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            healValue = (int)HealLifeModifier.ApplyTo(healValue);
        }

        public override void PostUpdate()
        {
            if (!IsUltraDashing) UltraDashDir = Utils.AngleLerp(UltraDashDir,-MathHelper.PiOver2,0.2f);

            //Main.NewText($"{Main.time} {Player.bodyRotation} {UltraDashDir} {Player.direction}");
        }

        public override void UpdateDead()
        {
            WingTimeModifier = StatModifier.Default;
        }

        public override void Unload()
        {
            PlayerDrawEffectHook = null;
            ModifyHitByBothHook = null;
            ModifyWeaponDamageHook = null;
            PostUpdateBuffsHook = null;
            OnHitByBothHook = null;
        }
    }
}
