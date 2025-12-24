using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Loaders;
using AyaMod.Helpers;
using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Items.Lens
{
    public class CircleLens : ILens
    {
        public bool Colliding(Vector2 center, float size, float rot, Rectangle targetRect)
        {
            return AyaUtils.CheckCircleRectangleCollision(center, size * 0.7f, targetRect);
        }
        public List<Rectangle> GetRectanglesAgainstEntity(Vector2 center, float size, float rot)
        {
            float gridSize = 20f;

            List<Rectangle> resultRects = new List<Rectangle>();
            Vector2 min = center + new Vector2(-size, -size);
            Vector2 max = center + new Vector2(size, size);

            // 计算网格数量和实际网格大小
            int gridCountX = (int)Math.Ceiling((max.X - min.X) / gridSize);
            int gridCountY = (int)Math.Ceiling((max.Y - min.Y) / gridSize);

            // 遍历每个网格单元
            for (int dx = 0; dx < gridCountX; dx++)
            {
                for (int dy = 0; dy < gridCountY; dy++)
                {
                    // 计算当前网格单元的矩形
                    Vector2 gridPos = min + new Vector2(dx * gridSize, dy * gridSize);
                    Vector2 nextPos = min + new Vector2((dx + 1) * gridSize, (dy + 1) * gridSize);

                    // 确保网格不超出最大范围
                    if (nextPos.X > max.X) nextPos.X = max.X;
                    if (nextPos.Y > max.Y) nextPos.Y = max.Y;

                    Rectangle gridRect = new Rectangle(
                        (int)gridPos.X, (int)gridPos.Y,
                        (int)(nextPos.X - gridPos.X),
                        (int)(nextPos.Y - gridPos.Y));

                    // 检查网格单元是否与旋转矩形相交
                    if (AyaUtils.CheckCircleRectangleCollision(center, size, gridRect))
                    {
                        resultRects.Add(gridRect);
                    }
                }
            }
            return Helper.MergeAdjacentRects(resultRects);
        }
        public void DrawCamera(SpriteBatch spriteBatch, Player player, Vector2 center, float rot, float size, float focusdScale, float maxFocusScale, Color outerFrameColor, Color innerFrameColor, Color focusCenterColor)
        {
            float focusFactor = (focusdScale - 1f) / (maxFocusScale - 1f);

            float scaledSize = size * 0.7f;
            float focusedSize = scaledSize * focusdScale;

            Vector2 dir = rot.ToRotationVector2();
            Vector2 ndir = dir.RotatedBy(MathHelper.PiOver2);
            float borderWidth = 2f;

            Texture2D texture = Request<Texture2D>(AssetDirectory.Extras + "Ball8").Value;

            var shader = ShaderLoader.GetShader("CircleEffect");
            shader.Parameters["PixelWidth"].SetValue(16f);
            shader.Parameters["WidthInPixel"].SetValue(1.5f);
            shader.Parameters["ScreenWidth"].SetValue(Main.screenWidth);
            shader.Parameters["segment"].SetValue(4);
            shader.Parameters["threshold"].SetValue(0.4f);
            shader.Parameters["rad"].SetValue(scaledSize);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

            shader.CurrentTechnique.Passes[0].Apply();

            Color innerColor = innerFrameColor * (1 - focusFactor);
            Color borderColor = outerFrameColor * focusFactor;
            //内框
            spriteBatch.Draw(texture, center - Main.screenPosition, null, innerColor, rot, texture.Size() / 2, scaledSize / 256f, 0, 0);

            shader.Parameters["rad"].SetValue(focusedSize);
            shader.CurrentTechnique.Passes[0].Apply();
            //外框
            spriteBatch.Draw(texture, center - Main.screenPosition, null, borderColor, rot, texture.Size() / 2, focusedSize / 256f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //焦点
            Utils.DrawLine(Main.spriteBatch, center - dir * size / 8f, center + dir * size / 8f, focusCenterColor, focusCenterColor, borderWidth);
            Utils.DrawLine(Main.spriteBatch, center - ndir * size / 8f, center + ndir * size / 8f, focusCenterColor, focusCenterColor, borderWidth);


        }
        public void SpawnFlash(Vector2 center, Color color, float size, float rot, int flashTime)
        {
            //TODO：填写正确的生成源
            CameraFlashCircle.Spawn(null, center, color, size * 0.6f, flashTime);
        }
    }
}
