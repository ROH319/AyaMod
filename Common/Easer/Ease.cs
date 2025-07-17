using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Common.Easer
{
    public enum Ease
    {
        Flash,
        InBack,
        InBounce,
        InCirc,
        InCubic,
        InElastic,
        InExpo,
        InFlash,
        InOutBack,
        InOutBounce,
        InOutCirc,
        InOutCubic,
        InOutElastic,
        InOutExpo,
        InOutFlash,
        InOutQuad,
        InOutQuart,
        InOutQuint,
        /// <summary>
        /// 先加速后减速
        /// </summary>
        InOutSine,
        InQuad,
        InQuart,
        InQuint,
        InSine,
        INTERNAL_Zero,
        /// <summary>
        /// 匀速
        /// </summary>
        Linear,
        OutBack,
        OutBounce,
        OutCirc,
        OutCubic,
        OutElastic,
        OutExpo,
        OutFlash,
        OutQuad,
        OutQuart,
        OutQuint,
        OutSine,
        Unset,
    }
}
