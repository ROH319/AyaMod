using AyaMod.Common.Easer;
using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AyaMod.Content.Particles
{
    /// <summary>
    /// 表示一个圆环状的粒子效果。
    /// </summary>
    /// <remarks>
    /// 圆环由多个点组成，通过可配置的起始/结束半径与透明度以及缓动函数在生命周期内插值变化。
    /// </remarks>
    public class RingParticle : Particle
    {
        /// <summary>
        /// 使用的纹理资源标识（此处为空占位纹理路径）。
        /// </summary>
        public override string Texture => AssetDirectory.EmptyTexturePass;

        /// <summary>
        /// 生命周期开始时的圆环半径（像素单位，乘以 <see cref="Scale"/> 使用）。
        /// </summary>
        public float StartRadius;
        /// <summary>
        /// 生命周期结束时的圆环半径（像素单位，乘以 <see cref="Scale"/> 使用）。
        /// </summary>
        public float EndRadius;
        /// <summary>
        /// 生命周期开始时的透明度（0 到 1）。
        /// </summary>
        public float StartAlpha;
        /// <summary>
        /// 生命周期结束时的透明度（0 到 1）。
        /// </summary>
        public float EndAlpha;
        /// <summary>
        /// 用于半径插值的缓动函数类型。
        /// </summary>
        public Ease RadiusEaser;
        /// <summary>
        /// 用于透明度插值的缓动函数类型。
        /// </summary>
        public Ease AlphaEaser;
        /// <summary>
        /// 环上点的数量（用于绘制圆环的离散点）。
        /// 较大的值会使圆环更平滑但增加绘制开销。
        /// </summary>
        public int pointCount;
        /// <summary>
        /// 绘制时使用的缩放向量（X 为水平缩放，Y 为垂直缩放）。
        /// </summary>
        public Vector2 ScaleV;

        /// <summary>
        /// 每帧更新逻辑。增加内部计时器并在超过 <see cref="TotalTime"/> 时将粒子置为非活动。
        /// </summary>
        public override void AI()
        {
            float extrascale = 2f;
        }

        /// <summary>
        /// 绘制圆环粒子。根据当前计时器通过缓动函数在起始与结束半径、透明度之间插值，
        /// 并在圆周上按 <see cref="pointCount"/> 分布点进行绘制，附带泛光（bloom）副本以增强光晕效果。
        /// </summary>
        /// <param name="spriteBatch">用于绘制的 <see cref="SpriteBatch"/>（未直接使用，使用 <see cref="Main.spriteBatch"/>/ <see cref="Main.EntitySpriteDraw"/>）。</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            float radius = MathHelper.Lerp(StartRadius, EndRadius, EaseManager.Evaluate(RadiusEaser, timer, maxtime));
            float alpha = MathHelper.Lerp(StartAlpha, EndAlpha, EaseManager.Evaluate(AlphaEaser, timer, maxtime));

            Texture2D texture = TextureAssets.Extra[98].Value;

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            for (int i = 0; i < pointCount; i++)
            {
                float factor = (float)i / pointCount;
                float dir = factor * MathHelper.TwoPi;
                Vector2 drawPos = Center + dir.ToRotationVector2() * radius * Scale - Main.screenPosition;

                Color drawColor = color * alpha * this.alpha;

                Main.spriteBatch.Draw(texture, drawPos, null, drawColor, dir, texture.Size() / 2, ScaleV, 0, 0);
                int repeat = 8;
                float offset = 1.5f * ScaleV.X;
                Color bloomcolor = drawColor * 0.3f;
                for (int j = 0; j < repeat; j++)
                {
                    for (int k = -1; k < 2; k += 2)//正向和反向
                    {
                        float fact = (float)j / repeat;
                        Vector2 posmove = dir.ToRotationVector2() * j * offset * k;
                        Main.EntitySpriteDraw(texture, drawPos + posmove, null, bloomcolor, dir, texture.Size() / 2, ScaleV, 0, 0);
                    }
                }
            }
            //Main.spriteBatch.End();
        }

        /// <summary>
        /// 创建并初始化一个新的 <see cref="RingParticle"/> 实例。
        /// </summary>
        /// <param name="source">实体源（用于粒子系统的来源跟踪）。</param>
        /// <param name="center">圆环的中心世界坐标。</param>
        /// <param name="color">圆环点和光晕的基础颜色。</param>
        /// <param name="startRadius">起始半径。</param>
        /// <param name="endRadius">结束半径。</param>
        /// <param name="startAlpha">起始透明度（0-1）。</param>
        /// <param name="endAlpha">结束透明度（0-1）。</param>
        /// <param name="xScale">纹理在 X 轴上的缩放（通常控制点大小）。</param>
        /// <param name="yScale">纹理在 Y 轴上的缩放。</param>
        /// <param name="totaltime">粒子总生命周期（帧）。</param>
        /// <param name="pointcount">圆周上的点数，默认为 180。</param>
        /// <param name="radiusEaser">半径插值使用的缓动类型，默认 <see cref="Ease.OutSine"/>。</param>
        /// <param name="alphaEaser">透明度插值使用的缓动类型，默认 <see cref="Ease.InSine"/>。</param>
        /// <returns>已初始化且激活的 <see cref="RingParticle"/> 实例。</returns>
        public static RingParticle Spawn(IEntitySource source, Vector2 center, Color color, float startRadius, float endRadius, float startAlpha, float endAlpha, float xScale, float yScale, int totaltime, int pointcount = 180, Ease radiusEaser = Ease.OutSine, Ease alphaEaser = Ease.InSine)
        {
            RingParticle particle = NewParticle<RingParticle>(source, center, Vector2.Zero, color, 1f,maxtime: totaltime);
            particle.alpha = 1f;
            particle.StartRadius = startRadius;
            particle.EndRadius = endRadius;
            particle.StartAlpha = startAlpha;
            particle.EndAlpha = endAlpha;
            particle.RadiusEaser = radiusEaser;
            particle.AlphaEaser = alphaEaser;
            particle.ScaleV = new Vector2(xScale, yScale);
            particle.pointCount = pointcount;
            return particle;
        }
    }
}
