using AyaMod.Core;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Projectiles
{
    public class MapleLeafProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + "MapleLeafProjectile";

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 60);
        }
        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            SetOtherDefaults();
        }
        public virtual void SetOtherDefaults()
        {
        }
        public override void AI()
        {

            if (Collision.LavaCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.Kill();
                return;
            }
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                    Projectile.frame = 0;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // 计算光照颜色
            Vector2 positionCenter = new Vector2(
                Projectile.position.X + Projectile.width * 0.5f,
                Projectile.position.Y + Projectile.height * 0.5f);
            Color baseColor = Color.White;

            // 绘制配置
            const float MAX_SCALE = 1.3f;
            const int SEGMENTS = 18;
            const int TRAIL_STEP = -3;
            const float SEGMENT_INTERPOLATION = 15f;
            const int ANIMATION_CYCLE_HEIGHT = 20 * 7; // 7 frames height

            Rectangle baseFrame = new Rectangle(0, 20 * Projectile.frame, 32, 20);
            Vector2 spriteOrigin = baseFrame.Size() / 2f;
            SpriteEffects drawDirection = SpriteEffects.None;

            float lifetimeFactor = Projectile.TimeleftFactor();
            float lifetimefactor2 = Utils.Remap(lifetimeFactor, 0, 0.2f, 0f, 1f);
            // 绘制拖尾
            for (int segmentIndex = SEGMENTS; segmentIndex >= 0; segmentIndex += TRAIL_STEP)
            {

                float baseRotation = Projectile.oldRot[segmentIndex];
                Vector2 segmentPosition = Projectile.oldPos[segmentIndex];

                // 处理帧动画
                baseFrame.Y += baseFrame.Height;
                baseFrame.Y %= ANIMATION_CYCLE_HEIGHT;

                // 计算每个拖尾段的颜色衰减
                Vector2 lightPosition = segmentPosition + Projectile.Size / 2f;
                Color segmentColor = baseColor;
                float alphafactor = 18 - segmentIndex;
                segmentColor *= alphafactor / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1f);
                segmentColor *= lifetimefactor2;
                // 计算拖尾位置
                Vector2 drawPosition = segmentPosition + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                // 计算拖尾缩放
                float scaleInterpolation = MathHelper.Lerp(Projectile.scale, MAX_SCALE, segmentIndex / SEGMENT_INTERPOLATION);

                // 绘制拖尾
                Main.EntitySpriteDraw(
                    texture,
                    drawPosition,
                    baseFrame,
                    segmentColor,
                    baseRotation,
                    spriteOrigin,
                    scaleInterpolation,
                    drawDirection,
                    0);
            }

            Color bloomColor = Color.Orange.AdditiveColor() * lifetimeFactor * 0.3f;
            RenderHelper.DrawBloom(6, 4, texture, Projectile.Center - Main.screenPosition, baseFrame, bloomColor, Projectile.rotation, spriteOrigin, Projectile.scale);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, baseFrame, baseColor * lifetimeFactor, Projectile.rotation, spriteOrigin, Projectile.scale, drawDirection, 0);


            return false;
        }
    }
}
