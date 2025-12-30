using AyaMod.Content.Items.Lens;
using AyaMod.Core.Configs;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Prefabs
{
    public class BaseCameraProjAuto : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public float size;
        public int itemTimeMax;
        public float sightRange;
        public float maxFocusScale;
        public ILens lens;
        public virtual ILens Lens { get; }

        public Player player => Main.player[Projectile.owner];

        public int itemTime;

        public float FocusFactor;
        public bool CanDamageNPC { get; set; }
        public float Size => size * FocusFactor;
        public virtual Color outerFrameColor => Color.Red;
        public virtual Color innerFrameColor => Color.White;
        public virtual Color focusCenterColor => Color.White;
        public virtual Color flashColor => Color.White.AdditiveColor() * 0.5f;

        public bool CanSpawnFlash = true;
        public bool CanAcceptAttackSpeedBonus;

        public sealed override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.netUpdate = true;
            Projectile.netImportant = true;

            Projectile.DamageType = ReporterDamage.Instance;

            Projectile.timeLeft = 60;

            SetOtherDefault();
        }

        public virtual void SetOtherDefault() { }
        public void SetCameraStats(float size, int focusTime, float sightRange, float maxFocusScale = 2f)
        {
            this.size = size;
            itemTimeMax = focusTime;
            this.sightRange = sightRange;
            this.maxFocusScale = maxFocusScale;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (lens != null)
            {
                return lens.Colliding(Projectile.Center, Size, Projectile.rotation, targetHitbox);
            }
            return false;
        }
        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyHitNPCAlt(target, ref modifiers);
            ApplyStun(target, ref modifiers);
            modifiers.DisableKnockback();
        }
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;
            OnHitNPCAlt(target, hit, damageDone);
        }
        public virtual void ModifyHitNPCAlt(NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void OnHitNPCAlt(NPC target, NPC.HitInfo hit, int damageDone) { }
        public override bool? CanDamage()
        {
            if (CanDamageNPC)
                return true;
            return false;
        }
        public virtual bool CheckCanDamage()
        {
            return itemTime == 0;
        }
        public override void AI()
        {

            lens = Lens ?? CameraPlayer.DefaultLens;
            var target = GetTargetIndex();

            if(target != null)
            {
                AttackMovement(target);
                UpdateFocusTime();
                CanDamageNPC = CheckCanDamage();
                if (CanDamageNPC)
                {
                    Snap();
                }
            }
            else
            {
                IdleMovement();
                itemTime = itemTimeMax;

            }
            FocusFactor = Utils.Remap(itemTime, 0, itemTimeMax, 1f, maxFocusScale);
            Projectile.rotation = Projectile.AngleFromSafe(GetIdlePos());
        }
        public virtual NPC GetTargetIndex()
        {
            NPC target = Projectile.FindCloestNPC(sightRange, true, false);
            return target;
        }
        public virtual void AttackMovement(NPC target)
        {
            Projectile.Chase(target, 10f, 0.04f);
        }
        public virtual Vector2 GetIdlePos() => player.Center;
        public virtual void IdleMovement()
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionToSafe(GetIdlePos()) * 12, 0.06f);
        }
        public void UpdateFocusTime()
        {
            if(itemTime == 0)
            {
                int itemtimemax = itemTimeMax;
                if (CanAcceptAttackSpeedBonus)
                {
                    float attackSpeed = player.GetTotalAttackSpeed(ReporterDamage.Instance);
                    itemtimemax = Math.Max(1, (int)(itemtimemax / attackSpeed));
                }
                itemTime = itemTimeMax;
            }
            itemTime--;
        }
        public void Snap()
        {
            OnSnap();

            if (ClientConfig.Instance.SnapFlash && CanSpawnFlash)
            {
                SpawnFlash();
            }
        }
        public virtual void OnSnap() { }

        public virtual void SpawnFlash()
        {
            lens?.SpawnFlash(Projectile.Center, flashColor * ClientConfig.Instance.SnapFlashAlpha, size, Projectile.rotation, (int)(Projectile.knockBack * 2));
        }
        public virtual void ApplyStun(NPC npc, ref NPC.HitModifiers modifiers)
        {
            int stunTime = (int)modifiers.GetKnockback(Projectile.knockBack);
            var globalNPC = npc.GetGlobalNPC<Globals.CameraGlobalNPC>();
            globalNPC.StunTimer += stunTime;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lens?.DrawCamera(Main.spriteBatch, Projectile.Center, Projectile.rotation, Size, FocusFactor, maxFocusScale, outerFrameColor, innerFrameColor, focusCenterColor);
            return true;
        }
    }
}
