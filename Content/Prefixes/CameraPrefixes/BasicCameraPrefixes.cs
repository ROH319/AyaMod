using AyaMod.Core;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Peerless() : BaseCameraPrefix(damageMult: 1.15f, focusSpeedMult: 0.9f, 5, sizeMult: 1.1f, stunMult: 1.15f, 1.75f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Fashionable() : BaseCameraPrefix(damageMult: 1.05f, focusSpeedMult: 0.9f, critBonus: 15, sizeMult: 1.1f, stunMult: 1.2f, valueMult: 1.75f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Dazzling() : BaseCameraPrefix(damageMult: 1.20f, focusSpeedMult: 1.1f, critBonus: 15, stunMult: 2f, valueMult: 1.65f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Global() : BaseCameraPrefix(damageMult: 1.05f, focusSpeedMult: 1.05f, critBonus: 5, sizeMult: 1.3f, valueMult: 1.65f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Realistic() : BaseCameraPrefix(critBonus: 30, sizeMult: 1.1f, stunMult: 1.1f, valueMult: 1.65f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Ephemeral() : BaseCameraPrefix(focusSpeedMult: 0.7f, sizeMult: 1.05f, valueMult: 1.65f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Focused() : BaseCameraPrefix(damageMult: 1.45f, focusSpeedMult: 1.1f, sizeMult: 1.15f, valueMult: 1.65f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Precise() : BaseCameraPrefix(damageMult: 1.4f, sizeMult: 0.8f, valueMult: 1.65f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Sensitive() : BaseCameraPrefix(damageMult: 1.1f, sizeMult: 1.05f, stunMult: 1.1f, valueMult: 1.17f)
    {
        public override float RollChance(Item item) => 0.66f;
    }
    public class Wideangle() : BaseCameraPrefix(sizeMult: 1.1f, stunMult: 1.05f, valueMult: 1.06f)
    {
    }
    public class Jammed() : BaseCameraPrefix(focusSpeedMult: 1.15f, stunMult: 0.9f, valueMult: 0.9f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Blurred() : BaseCameraPrefix(damageMult: 0.85f, critBonus: -10, valueMult: 0.85f)
    {
        public override float RollChance(Item item) => 0.9f;
    }
    public class Overexposed() : BaseCameraPrefix(critBonus: -10, sizeMult: 0.9f, valueMult: 0.76f)
    {
        public override float RollChance(Item item) => 0.66f;
    }
    public class Deceptive() : BaseCameraPrefix(damageMult: 0.85f, sizeMult: 0.9f, stunMult: 0.85f, valueMult: 0.7f)
    {
        public override float RollChance(Item item) => 0.66f;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damageMult">伤害乘数，大于1为增加伤害</param>
    /// <param name="focusSpeedMult">聚焦时间乘数，小于1为增加攻速</param>
    /// <param name="critBonus">暴击率加成</param>
    /// <param name="sizeMult">大小乘数，大于1为增加大小</param>
    /// <param name="stunMult">眩晕时间乘数，大于1为增加时长</param>
    /// <param name="valueMult">价值乘数</param>
    public abstract class BaseCameraPrefix(float damageMult = 1f, float focusSpeedMult = 1f, int critBonus = 0, 
        float sizeMult = 1f, float stunMult = 1f, float valueMult = 1f) : ModPrefix
    {
        public float damageMult = damageMult;
        public float focusSpeedMult = focusSpeedMult;
        public int critBonus = critBonus;
        public float sizeMult = sizeMult;
        public float stunMult = stunMult;
        public float valueMult = valueMult;

        public override PrefixCategory Category => PrefixCategory.Custom;

        public static LocalizedText SizeTooltip {  get; private set; }
        public static LocalizedText StunTooltip { get; private set; }
        public static LocalizedText PrefixFocusSpeed {  get; private set; }

        public override void SetStaticDefaults()
        {
            SizeTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(SizeTooltip)}");
            StunTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(StunTooltip)}");
            PrefixFocusSpeed = Mod.GetLocalization($"{LocalizationCategory}.{nameof(PrefixFocusSpeed)}");
        }
        public override void Load()
        {
            AyaGlobalProjectile.OnProjectileSpawn += OnProjectileSpawnHook;
            AyaGlobalProjectile.ModifyProjectileHitNPC += GlobalModifyHitNPCHook;
            AyaGlobalProjectile.OnProjectileHitNPC += OnProjectileHitNPCHook;
            AyaGlobalProjectile.OnProjectilePostAI += OnProjectilePostAIHook;
            GlobalCamera.PreAIHook += PreAIHook;
            GlobalCamera.PostAIHook += PostAIHook;
            GlobalCamera.ModifyHitNPCHook += ModifyHitNPCHook;
            GlobalCamera.OnHitNPCHook += OnHitNPCHook;
            GlobalCamera.SnapHook += SnapHook;
            GlobalCamera.SnapInSightHook += SnapInSightHook;
            AyaPlayer.PostUpdateMiscEffectsHook += PostUpdateMiscEffectsHook;
        }

        public static void OnProjectileSpawnHook(Projectile projectile, IEntitySource source)
        {
            if (Main.player[projectile.owner].HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.GlobalProjectile_Spawn(projectile, source);
        }
        public static void GlobalModifyHitNPCHook(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.GlobalProjectile_ModifyHit(projectile, target, ref modifiers);
        }
        public static void OnProjectileHitNPCHook(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[projectile.owner].HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.GlobalProjectile_OnHit(projectile, target, hit, damageDone);
        }
        public static void OnProjectilePostAIHook(Projectile projectile)
        {
            if (Main.player[projectile.owner].HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.GlobalProjectile_PostAI(projectile);
        }

        public static bool PreAIHook(Player player, BaseCameraProj projectile)
        {
            if (player.HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.Camera_PreAI(player, projectile);
            return true;
        }
        public static void PostAIHook(Player player, BaseCameraProj projectile)
        {
            if(player.HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.Camera_PostAI(player, projectile);
        }
        public static void ModifyHitNPCHook(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.Camera_ModifyHitNPC(projectile, target, ref modifiers);
        }
        public static void OnHitNPCHook(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[projectile.owner].HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.Camera_OnHitNPC(projectile, target, hit, damageDone);
        }
        public static void SnapHook(BaseCameraProj projectile)
        {
            if (projectile.player.HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.Camera_OnSnap(projectile);
        }
        public static void SnapInSightHook(BaseCameraProj projectile)
        {
            if (projectile.player.HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.Camera_OnSnapInSight(projectile);
        }
        public static void PostUpdateMiscEffectsHook(Player player)
        {
            if (player.HeldItemCameraPrefix(out BaseCameraPrefix prefix))
                prefix.Player_PostUpdateMiscEffects(player);
        }


        public override void Apply(Item item)
        {
            if (item.TryGetGlobalItem(out CameraGlobalItem cameraGlobalItem))
            {
                cameraGlobalItem.SizeMult = sizeMult;
                cameraGlobalItem.StunTimeMult = stunMult;
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = this.valueMult;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (focusSpeedMult != 1)
            {
                bool isbad = focusSpeedMult > 1;
                string sign = isbad ? "" : "+";
                string modify = string.Concat(sign, ((int)((1 - focusSpeedMult) * 100)).ToString());
                yield return new TooltipLine(Mod, "PrefixFocusSpeed", PrefixFocusSpeed.Format(modify))
                {
                    IsModifier = true,
                    IsModifierBad = isbad
                };
            }

            if (sizeMult != 1)
            {
                bool isbad = sizeMult < 1;
                string sign = isbad ? "" : "+";
                string modify = string.Concat(sign, ((int)((sizeMult - 1) * 100)).ToString());
                yield return new TooltipLine(Mod, "PrefixSizeMult", SizeTooltip.Format(modify))
                {
                    IsModifier = true,
                    IsModifierBad = isbad
                };
            }

            if (stunMult != 1)
            {
                bool isbad = stunMult < 1;
                string sign = isbad ? "" : "+";
                string modify = string.Concat(sign, ((int)((stunMult - 1) * 100)).ToString());
                yield return new TooltipLine(Mod, "PrefixStunMult", StunTooltip.Format(modify))
                {
                    IsModifier = true,
                    IsModifierBad = isbad
                };
            }

        }

        public override bool CanRoll(Item item)
        {
            return item.DamageType == ReporterDamage.Instance && RollChance(item) > 0;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            useTimeMult = this.focusSpeedMult;
            critBonus = this.critBonus;
        }
        public virtual void GlobalProjectile_Spawn(Projectile projectile, IEntitySource source) { }
        public virtual void GlobalProjectile_ModifyHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void GlobalProjectile_OnHit(Projectile projectile, NPC target, NPC.HitInfo info, int damageDone) { }
        public virtual void GlobalProjectile_PostAI(Projectile projectile) { }
        public virtual void Camera_PreAI(Player player, BaseCameraProj projectile) { }
        public virtual void Camera_PostAI(Player player, BaseCameraProj projectile) { }
        public virtual void Camera_ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void Camera_OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void Camera_OnSnap(BaseCameraProj projectile) { }
        public virtual void Camera_OnSnapInSight(BaseCameraProj projectile) { }
        public virtual void Player_PostUpdateMiscEffects(Player player) { }
    }
}
