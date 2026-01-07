using AyaMod.Common.Easer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Common
{
    /// <summary>
    /// 简易 Tween 定时器
    /// </summary>
    public class TweenTimer
    {
        /// <summary>
        /// 起始值
        /// </summary>
        public float StartValue;
        /// <summary>
        /// 结束值
        /// </summary>
        public float EndValue;
        /// <summary>
        /// 每次 Update 移动的基础速度（与 updateSpeed 相乘）
        /// 。</summary>
        public float Speed;
        /// <summary>
        /// 当前计时器值（0 到 Duration 之间）
        /// </summary>
        public float Timer;
        /// <summary>
        /// 插值总持续时间。
        /// </summary>
        public float Duration;
        /// <summary>使用的缓动类型。</summary>
        public Ease ease;

        private bool isPlaying;
        private bool isCompleted;
        private bool isReversed;

        /// <summary>
        /// 返回当前进度（0 到 1），经过缓动函数处理的值。
        /// </summary>
        public float Progress
        {
            get
            {
                float p = Timer / Duration;
                return ease.Evaluate(p, 1f);
            }
        }

        /// <summary>
        /// 返回当前进度没经过缓动函数处理的值
        /// </summary>
        public float ProgressLinear => Timer / Duration;

        /// <summary>
        /// 创建一个 TweenTimer 实例。
        /// </summary>
        /// <param name="startValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="speed">基础速度，控制 Timer 变化量</param>
        /// <param name="duration">插值的总时长</param>
        /// <param name="ease">缓动类型</param>
        /// <param name="startPlay">是否在构造时开始播放</param>
        /// <param name="reverse">是否以反向初始（从末尾向开始）</param>
        public TweenTimer(float startValue, float endValue, float speed = 0.05f, float duration = 1f, Ease ease = Ease.Linear, bool startPlay = true, bool reverse = false)
        {
            StartValue = startValue;
            EndValue = endValue;
            Speed = speed;
            Duration = duration;
            this.ease = ease;
            isPlaying = startPlay;
            isCompleted = false;
            isReversed = reverse;
        }

        /// <summary>
        /// 更新计时器。按 Speed * updateSpeed 推进或回退 Timer，并在到达边界时标记完成
        /// </summary>
        /// <param name="updateSpeed">倍率，用于缩放当前帧或逻辑步长</param>
        public virtual void Update(float updateSpeed = 1f)
        {
            //if (!isPlaying || isCompleted) return;

            float deltaTime = Speed * updateSpeed;
            Timer += isReversed ? -deltaTime : deltaTime;

            Timer = MathHelper.Clamp(Timer, 0f, Duration);

            bool isBoundary = isReversed ? Timer <= 0.0001f : Timer >= Duration - 0.0001f;
            if (isBoundary)
            {
                isCompleted = true;
                isPlaying = false;
            }
        }

        /// <summary>
        /// 开始播放，positive=true 表示正向播放
        /// </summary>
        public void Play(bool positive = true)
        {
            isPlaying = true;
            isReversed = !positive;
        }
        /// <summary>
        /// 开始播放并重置到起点或终点（取决于 positive）
        /// </summary>
        public void PlayAndReset(bool positive = true)
        {
            isPlaying = true;
            isCompleted = false;
            isReversed = !positive;
            Timer = positive ? 0f : Duration;
        }
        /// <summary>
        /// 直接结束动画并将 Timer 设置到对应边界（正向为末尾，反向为起点）
        /// </summary>
        public void Finish(bool positive = true)
        {
            isPlaying = false;
            isCompleted = true;
            isReversed = !positive;
            Timer = positive ? Duration : 0f;
        }

        /// <summary>
        /// 切换播放方向并开始播放（用于反向播放）
        /// </summary>
        public void ReversePlay()
        {
            isReversed = !isReversed;
            isCompleted = false;
            isPlaying = true;
        }

        /// <summary>是否已完成到达边界</summary>
        public bool IsCompleted => isCompleted;

        /// <summary>当前是否处于反向播放状态</summary>
        public bool IsReversed => isReversed;

        /// <summary>是否正在播放（任意方向）</summary>
        public bool IsPlaying => isPlaying;

        /// <summary>是否位于起点（Timer ≈ 0）</summary>
        public bool IsAtStart => MathF.Abs(Timer - 0f) < 0.001f;

        /// <summary>是否位于终点（Timer ≈ Duration）</summary>
        public bool IsAtEnd => MathF.Abs(Duration - Timer) < 0.001f;

        /// <summary>是否正在正向播放</summary>
        public bool PositivePlaying => isPlaying && !isReversed;

        /// <summary>是否正在反向播放</summary>
        public bool NegativePlaying => isPlaying && isReversed;

        /// <summary>正向相关状态：正在正向播放或已到达末尾</summary>
        public bool AnyPositive => PositivePlaying || IsAtEnd;

        /// <summary>反向相关状态：正在反向播放或已到达起点</summary>
        public bool AnyNegative => NegativePlaying || IsAtStart;

        public Color Lerp(Color color1, Color color2) => Color.Lerp(color1, color2, Progress);

        public Vector2 Lerp(Vector2 start, Vector2 end) => Vector2.Lerp(start, end, Progress);

        public float Lerp(float start, float end) => start + (end - start) * Progress;

    }
}
