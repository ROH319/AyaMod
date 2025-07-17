using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Common.Easer
{
    public static class EaseManager
    {
        public static float Evaluate(this Ease easeType, float time, float duration, float overShootOrAmplitude = 0.5f, float period = 0f)
        {
            switch (easeType)
            {
                case Ease.Linear:
                    return time / duration;
                case Ease.InSine:
                    return -(float)Math.Cos((double)(time / duration * MathHelper.PiOver2)) + 1f;
                case Ease.OutSine:
                    return (float)Math.Sin((double)(time / duration * MathHelper.PiOver2));
                case Ease.InOutSine:
                    return -0.5f * ((float)Math.Cos((double)(MathHelper.Pi * time / duration)) - 1f);
                case Ease.InQuad:
                    return (time /= duration) / time;
                case Ease.OutQuad:
                    return -(time /= duration) / (time - 2f);
                case Ease.InOutQuad:
                    if ((time /= duration / 2) < 1f)
                    {
                        return time * time / 2;
                    }
                    return -0.5f * ((time -= 1f) * (time - 2f) - 1f);
                case Ease.InCubic:
                    return (time /= duration) * time * time;
                case Ease.OutCubic:
                    return (time = time / duration - 1f) * time * time + 1f;
                case Ease.InOutCubic:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time;
                    }
                    return 0.5f * ((time -= 2f) * time * time + 2f);
                case Ease.InQuart:
                    return (time /= duration) * time * time * time;
                case Ease.OutQuart:
                    return -((time = time / duration - 1f) * time * time * time - 1f);
                case Ease.InOutQuart:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time * time;
                    }
                    return -0.5f * ((time -= 2f) * time * time * time - 2f);
                case Ease.InQuint:
                    return (time /= duration) * time * time * time * time;
                case Ease.OutQuint:
                    return (time = time / duration - 1f) * time * time * time * time + 1f;
                case Ease.InOutQuint:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time * time * time;
                    }
                    return 0.5f * ((time -= 2f) * time * time * time * time + 2f);
                case Ease.InExpo:
                    if (time != 0f)
                    {
                        return (float)Math.Pow(2.0, (double)(10f * (time / duration - 1f)));
                    }
                    return 0f;
                case Ease.OutExpo:
                    if (time == duration)
                    {
                        return 1f;
                    }
                    return -(float)Math.Pow(2.0, (double)(-10f * time / duration)) + 1f;
                case Ease.InOutExpo:
                    if (time == 0f)
                    {
                        return 0f;
                    }
                    if (time == duration)
                    {
                        return 1f;
                    }
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * (float)Math.Pow(2.0, (double)(10f * (time - 1f)));
                    }
                    return 0.5f * (-(float)Math.Pow(2.0, (double)(-10f * (time -= 1f))) + 2f);
                case Ease.InCirc:
                    return -((float)Math.Sqrt((double)(1f - (time /= duration) * time)) - 1f);
                case Ease.OutCirc:
                    return (float)Math.Sqrt((double)(1f - (time = time / duration - 1f) * time));
                case Ease.InOutCirc:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return -0.5f * ((float)Math.Sqrt((double)(1f - time * time)) - 1f);
                    }
                    return 0.5f * ((float)Math.Sqrt((double)(1f - (time -= 2f) * time)) + 1f);
                case Ease.InElastic:
                    {
                        if (time == 0f)
                        {
                            return 0f;
                        }
                        if ((time /= duration) == 1f)
                        {
                            return 1f;
                        }
                        if (period == 0f)
                        {
                            period = duration * 0.3f;
                        }
                        float num;
                        if (overShootOrAmplitude < 1f)
                        {
                            overShootOrAmplitude = 1f;
                            num = period / 4f;
                        }
                        else
                        {
                            num = period / 6.28318548f * (float)Math.Asin((double)(1f / overShootOrAmplitude));
                        }
                        return -(overShootOrAmplitude * (float)Math.Pow(2.0, (double)(10f * (time -= 1f))) * (float)Math.Sin((double)((time * duration - num) * 6.28318548f / period)));
                    }
                case Ease.OutElastic:
                    {
                        if (time == 0f)
                        {
                            return 0f;
                        }
                        if ((time /= duration) == 1f)
                        {
                            return 1f;
                        }
                        if (period == 0f)
                        {
                            period = duration * 0.3f;
                        }
                        float num2;
                        if (overShootOrAmplitude < 1f)
                        {
                            overShootOrAmplitude = 1f;
                            num2 = period / 4f;
                        }
                        else
                        {
                            num2 = period / 6.28318548f * (float)Math.Asin((double)(1f / overShootOrAmplitude));
                        }
                        return overShootOrAmplitude * (float)Math.Pow(2.0, (double)(-10f * time)) * (float)Math.Sin((double)((time * duration - num2) * 6.28318548f / period)) + 1f;
                    }
                case Ease.InOutElastic:
                    {
                        if (time == 0f)
                        {
                            return 0f;
                        }
                        if ((time /= duration * 0.5f) == 2f)
                        {
                            return 1f;
                        }
                        if (period == 0f)
                        {
                            period = duration * 0.450000018f;
                        }
                        float num3;
                        if (overShootOrAmplitude < 1f)
                        {
                            overShootOrAmplitude = 1f;
                            num3 = period / 4f;
                        }
                        else
                        {
                            num3 = period / 6.28318548f * (float)Math.Asin((double)(1f / overShootOrAmplitude));
                        }
                        if (time < 1f)
                        {
                            return -0.5f * (overShootOrAmplitude * (float)Math.Pow(2.0, (double)(10f * (time -= 1f))) * (float)Math.Sin((double)((time * duration - num3) * 6.28318548f / period)));
                        }
                        return overShootOrAmplitude * (float)Math.Pow(2.0, (double)(-10f * (time -= 1f))) * (float)Math.Sin((double)((time * duration - num3) * 6.28318548f / period)) * 0.5f + 1f;
                    }
                case Ease.InBack:
                    return (time /= duration) * time * ((overShootOrAmplitude + 1f) * time - overShootOrAmplitude);
                case Ease.OutBack:
                    return (time = time / duration - 1f) * time * ((overShootOrAmplitude + 1f) * time + overShootOrAmplitude) + 1f;
                case Ease.InOutBack:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * (time * time * (((overShootOrAmplitude *= 1.525f) + 1f) * time - overShootOrAmplitude));
                    }
                    return 0.5f * ((time -= 2f) * time * (((overShootOrAmplitude *= 1.525f) + 1f) * time + overShootOrAmplitude) + 2f);
                //case Ease.InBounce:
                //    return Bounce.EaseIn(time, duration, overShootOrAmplitude, period);
                //case Ease.OutBounce:
                //    return Bounce.EaseOut(time, duration, overShootOrAmplitude, period);
                //case Ease.InOutBounce:
                //    return Bounce.EaseInOut(time, duration, overShootOrAmplitude, period);
                //case Ease.Flash:
                //    return Flash.Ease(time, duration, overShootOrAmplitude, period);
                //case Ease.InFlash:
                //    return Flash.EaseIn(time, duration, overShootOrAmplitude, period);
                //case Ease.OutFlash:
                //    return Flash.EaseOut(time, duration, overShootOrAmplitude, period);
                //case Ease.InOutFlash:
                //    return Flash.EaseInOut(time, duration, overShootOrAmplitude, period);
                case Ease.INTERNAL_Zero:
                    return 1f;
                default:
                    return -(time /= duration) * (time - 2f);
            }
        }
    }
}
