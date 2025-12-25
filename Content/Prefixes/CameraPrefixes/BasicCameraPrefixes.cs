using AyaMod.Core;
using AyaMod.Core.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
    }
}
