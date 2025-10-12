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
