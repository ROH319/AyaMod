using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public abstract class ExtraCameraPrefix(float damageMult = 1f, float focusSpeedMult = 1f, int critBonus = 0,
        float sizeMult = 1f, float stunMult = 1f, float valueMult = 1f) : BaseCameraPrefix(damageMult,focusSpeedMult,critBonus,sizeMult,stunMult,valueMult)
    {
        public virtual LocalizedText PrefixExtraTooltip => Mod.GetLocalization($"{LocalizationCategory}.{this.GetType().Name}.{nameof(PrefixExtraTooltip)}", () => "");

        public override float RollChance(Item item) => 0;
        public override bool CanRoll(Item item) => true;

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            var baseTooltips = base.GetTooltipLines(item);
            return baseTooltips.Append(new TooltipLine(Mod, "ExtraPrefixTooltip", PrefixExtraTooltip.Value));
        }
    }
}
