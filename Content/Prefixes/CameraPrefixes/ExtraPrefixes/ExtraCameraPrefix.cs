using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damageMult">伤害乘数，大于1为增加伤害</param>
    /// <param name="focusSpeedMult">聚焦时间乘数，小于1为增加攻速</param>
    /// <param name="critBonus">暴击率加成</param>
    /// <param name="sizeMult">大小乘数，大于1为增加大小</param>
    /// <param name="stunMult">眩晕时间乘数，大于1为增加时长</param>
    /// <param name="valueMult">价值乘数</param>
    public abstract class ExtraCameraPrefix(float damageMult = 1f, float focusSpeedMult = 1f, int critBonus = 0,
        float sizeMult = 1f, float stunMult = 1f, float valueMult = 1f) : BaseCameraPrefix(damageMult, focusSpeedMult, critBonus, sizeMult, stunMult, valueMult)
    {
        public virtual LocalizedText PrefixExtraTooltip => Mod.GetLocalization($"{LocalizationCategory}.{Name}.PrefixExtraTooltip");

        public override float RollChance(Item item) => 0;
        public override bool CanRoll(Item item) => true;

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            var baseTooltips = base.GetTooltipLines(item);
            return baseTooltips.Append(new TooltipLine(Mod, "ExtraPrefixTooltip", PrefixExtraTooltip.Value));
        }
    }
}
