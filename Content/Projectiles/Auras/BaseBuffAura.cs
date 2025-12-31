using AyaMod.Common.Easer;
using AyaMod.Core;
using AyaMod.Core.Loaders;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Projectiles.Auras
{

    public class BaseBuffAura : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Timeleft => ref Projectile.ai[0];
        public ref float BuffType => ref Projectile.ai[1];
        public ref float Radius => ref Projectile.ai[2];
        public ref float BuffDuration => ref Projectile.localAI[0];

        public float DistortIntensity = 4f;

        public Color innerColor;
        public Color edgeColor;

        public float RadiusFadeinThreshold = 0.1f;
        public Ease RadiusFadeinEase = Ease.Linear;
        public float RadiusFadeoutThreshold = 0.9f;
        public Ease RadiusFadeoutEase = Ease.Linear;

        private float radiusFadeinProgress;
        private float radiusFadeoutProgress;
        public bool radiusFadeinEnabled = true;
        public bool radiusFadeoutEnabled = true;

        public float AlphaFadeinThreshold = 0.1f;
        public Ease AlphaFadeinEase = Ease.Linear;
        public float AlphaFadeoutThreshold = 0.9f;
        public Ease AlphaFadeoutEase = Ease.Linear;

        private float alphaFadeinProgress;
        private float alphaFadeoutProgress;

        public int dustType = -1;
        public int dustRate = 0;
        public int dustAmount = 0;
        public float dustScale = 1f;
        public float dustRangeMin = 0.9f;
        public float dustRangeMax = 1f;
        public static BaseBuffAura Spawn(IEntitySource source, Vector2 pos, int timeleft, int buffType, int buffTime, float radius, Color innerColor, Color edgeColor, int owner)
        {
            var projectile = Projectile.NewProjectileDirect(source, pos, Vector2.Zero, ProjectileType<BaseBuffAura>(), 0, 0, owner, timeleft, buffType, radius);
            BaseBuffAura aura = projectile.ModProjectile as BaseBuffAura;
            aura.BuffDuration = buffTime;
            aura.innerColor = innerColor;
            aura.edgeColor = edgeColor;
            return aura;
        }
        public static T Spawn<T>(IEntitySource source, Vector2 pos, int timeleft, int buffType, int buffTime, float radius, Color innerColor, Color edgeColor, int owner) where T : BaseBuffAura
        {
            var projectile = Projectile.NewProjectileDirect(source, pos, Vector2.Zero, ProjectileType<T>(), 0, 0, owner, timeleft, buffType, radius);
            T aura = projectile.ModProjectile as T;
            aura.BuffDuration = buffTime;
            aura.innerColor = innerColor;
            aura.edgeColor = edgeColor;
            return aura;
        }
        /// <summary>
        /// 设置半径淡入的阈值与缓动类型
        /// 阈值为生命周期因子（从0到1）的值：在生命周期因子从 0 到该阈值的区间内，半径按指定的缓动函数从 0 过渡到目标半径
        /// </summary>
        /// <param name="threshold">淡入结束点的生命周期因子，通常在0到1之间，默认为0.1</param>
        /// <param name="easeType">用于控制淡入插值曲线的缓动类型</param>
        public void SetRadiusFadein(float threshold, Ease easeType)
        {
            RadiusFadeinThreshold = threshold;
            RadiusFadeinEase = easeType;
        }
        /// <summary>
        /// 设置半径淡出的阈值与缓动类型
        /// 阈值为生命周期因子（从0到1）的值：当生命周期因子从该阈值到 1 的区间内，半径按指定的缓动函数从目标半径过渡到 0
        /// </summary>
        /// <param name="threshold">淡出开始点的生命周期因子，通常在0到1之间，默认为0.9</param>
        /// <param name="easeType">用于控制淡出插值曲线的缓动类型</param>
        public void SetRadiusFadeout(float threshold, Ease easeType)
        {
            RadiusFadeoutThreshold = threshold;
            RadiusFadeoutEase = easeType;
        }
        public void SetAlphaFadein(float threshold, Ease easeType)
        {
            AlphaFadeinThreshold = threshold;
            AlphaFadeinEase = easeType;
        }
        public void SetAlphaFadeout(float threshold, Ease easeType)
        {
            AlphaFadeoutThreshold = threshold;
            AlphaFadeoutEase = easeType;
        }
        public void SetDust(int type, int rate, int amount, float scale = 1f, float rangeMin = 0.9f, float rangeMax = 1f)
        {
            dustType = type;
            dustRate = rate;
            dustAmount = amount;
            dustScale = scale;
            dustRangeMin = rangeMin;
            dustRangeMax = rangeMax;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Timeleft;
            Projectile.Scale(Radius / Projectile.width, false);
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactorPositive();

            if (radiusFadeinEnabled)
                radiusFadeinProgress = EaseManager.Evaluate(RadiusFadeinEase, Utils.Remap(factor, 0f, RadiusFadeinThreshold, 0f, 1f), 1f);
            else radiusFadeinProgress = 1f;
            if (radiusFadeoutEnabled)
                radiusFadeoutProgress = EaseManager.Evaluate(RadiusFadeoutEase, Utils.Remap(factor, RadiusFadeoutThreshold, 1f, 1f, 0f), 1f);
            else radiusFadeoutProgress = 1f;

            //Main.NewText($"{radiusFadeoutProgress}");
            alphaFadeinProgress = EaseManager.Evaluate(AlphaFadeinEase, Utils.Remap(factor, 0f, AlphaFadeinThreshold, 0f, 1f), 1f);
            alphaFadeoutProgress = EaseManager.Evaluate(AlphaFadeoutEase, Utils.Remap(factor, AlphaFadeoutThreshold, 1f, 1f, 0f), 1f);

            ApplyBuff();
            SpawnDust();
        }
        public virtual void ApplyBuff() { }
        public virtual void SpawnDust()
        {
            if (dustType < 0) return;
            if (Main.GameUpdateCount % dustRate == 0)
            {
                for (int i = 0; i < dustAmount; i++)
                {
                    float radius = Radius * 0.45f * radiusFadeinProgress * radiusFadeoutProgress;
                    Vector2 pos = Main.rand.NextVector2Unit() * Main.rand.NextFloat(dustRangeMin, dustRangeMax) * radius + Projectile.Center;
                    Dust d = Dust.NewDustPerfect(pos, dustType, Scale: dustScale);
                    d.noGravity = true;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var shader = ShaderLoader.GetShader("CircleEffect2");
            if (shader == null) return false;

            Texture2D noiseTex = Request<Texture2D>(AssetDirectory.Extras + "GenFX_PlagueofMurlocs_Water_BW", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            bool floatingRadius = true;
            float radius = Radius * 2f;
            Color inColor = innerColor * alphaFadeinProgress * alphaFadeoutProgress;
            Color edColor = edgeColor * alphaFadeinProgress * alphaFadeoutProgress;
            float innerRadius = radius * 0.45f * radiusFadeinProgress * radiusFadeoutProgress;
            if (floatingRadius) innerRadius *= (MathF.Cos((float)(Main.timeForVisualEffects * 0.02f)) * 0.05f + 1f);
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 0.2f + Projectile.whoAmI * 3.8f);
            shader.Parameters["innerRadius"].SetValue(innerRadius);
            shader.Parameters["Radius"].SetValue(radius);
            shader.Parameters["edgeColor"].SetValue(edColor.ToVector4());
            shader.Parameters["innerColor"].SetValue(inColor.ToVector4());
            shader.Parameters["twistIntensity"].SetValue(DistortIntensity);
            shader.Parameters["maskFactor"].SetValue(0.05f);
            //shader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

            float scale = Radius / noiseTex.Width;

            Main.spriteBatch.Draw(noiseTex, Projectile.Center - Main.screenPosition
                , null, Color.White, 0, noiseTex.Size() / 2, scale, 0, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, Main.spriteBatch.GraphicsDevice.BlendState, Main.spriteBatch.GraphicsDevice.SamplerStates[0],
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, Main.spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, Main.spriteBatch.GraphicsDevice.BlendState, Main.spriteBatch.GraphicsDevice.SamplerStates[0],
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, Main.spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public void DisableRadiusFadein()
        {
            radiusFadeinEnabled = false;
        }
        public void DisableRadiusFadeout()
        {
            radiusFadeoutEnabled = false;
        }
    }
}
