﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace AyaMod.Helpers
{
    public static class RenderHelper
    {
        public static RenderTarget2D render;

        public static void CreateRender()
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            int width = gd.PresentationParameters.BackBufferWidth;
            int height = gd.PresentationParameters.BackBufferHeight;
            SurfaceFormat format = gd.PresentationParameters.BackBufferFormat;
            render = new RenderTarget2D(gd, width, height, false, format, 0);
        }

        public static BlendState ReverseSubtract = new BlendState()
        {
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            
        };

        public static BlendState MaxAdditive = new BlendState()
        {
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Max
        };

        public static void DrawRing(int pointCount, Vector2 center, float radius, Color drawcolor, float rot, Vector2 scale)
        {
            var star = TextureAssets.Extra[98].Value;
            for (int i = 0; i < pointCount; i++)
            {
                float dir = MathHelper.TwoPi / pointCount * i + rot;
                Vector2 drawpos = center + dir.ToRotationVector2() * radius;
                Main.spriteBatch.Draw(star, drawpos - Main.screenPosition, null, drawcolor, dir, new Vector2(36, 36), scale, 0, 0);
            }
        }

        public static void DrawCameraFrame(SpriteBatch spriteBatch, Vector2[] boundries, Color borderColor, float borderWidth, float lengthPercent)
        {
            Vector2 dirx = boundries[0].DirectionToSafe(boundries[2]);
            Vector2 diry = dirx.RotatedBy(MathHelper.PiOver2);

            float sizex = boundries[0].Distance(boundries[2]);
            float sizey = boundries[0].Distance(boundries[1]);

            float lengthX = sizex * lengthPercent;
            float lengthY = sizey * lengthPercent;

            Utils.DrawLine(spriteBatch, boundries[0], boundries[0] + dirx * lengthX, borderColor, borderColor, borderWidth);
            Utils.DrawLine(spriteBatch, boundries[2], boundries[2] - dirx * lengthX, borderColor, borderColor, borderWidth);
            Utils.DrawLine(spriteBatch, boundries[1], boundries[1] + dirx * lengthX, borderColor, borderColor, borderWidth);
            Utils.DrawLine(spriteBatch, boundries[3], boundries[3] - dirx * lengthX, borderColor, borderColor, borderWidth);
            Utils.DrawLine(spriteBatch, boundries[0], boundries[0] + diry * lengthY, borderColor, borderColor, borderWidth);
            Utils.DrawLine(spriteBatch, boundries[1], boundries[1] - diry * lengthY, borderColor, borderColor, borderWidth);
            Utils.DrawLine(spriteBatch, boundries[2], boundries[2] + diry * lengthY, borderColor, borderColor, borderWidth);
            Utils.DrawLine(spriteBatch, boundries[3], boundries[3] - diry * lengthY, borderColor, borderColor, borderWidth);
        }

        public static void DrawBloom(int repeat, float offset, Texture2D texture, Vector2 center, Rectangle? source, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect = SpriteEffects.None)
        {
            for (int i = 0; i < repeat; i++)
            {
                float factor = (float)i / repeat;
                Vector2 posmove = (MathHelper.TwoPi / repeat * i).ToRotationVector2() * offset;
                Main.EntitySpriteDraw(texture, center + posmove, source, color, rotation, origin, scale, effect);
            }
        }

        public static void DrawBloom(int repeat, float offset, Texture2D texture, Vector2 center, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect = SpriteEffects.None)
        {
            for (int i = 0; i < repeat; i++)
            {
                float factor = (float)i / repeat;
                Vector2 posmove = (MathHelper.TwoPi / repeat * i).ToRotationVector2() * offset;
                Main.EntitySpriteDraw(texture, center + posmove, source, color, rotation, origin, scale, effect);
            }
        }

        //public static void DrawBloomScaled(int repeat)


        public static Color AdditiveColor(this Color color) => color with { A = 0 };
        public static Color WithAlpha(this Color color, byte alpha) => color with { A = alpha };
    }
}
