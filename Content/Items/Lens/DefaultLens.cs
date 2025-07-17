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

namespace AyaMod.Content.Items.Lens
{
    public class DefaultLens : ILens
    {
        public bool Colliding(Vector2 center, float size, float rot, Rectangle targetRect)
        {
            var offset = rot.ToRotationVector2() * size * 0.5f;
            float point = 0;
            float height = size * 1.4f;
            return Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), center - offset, center + offset, height, ref point);
        }

        public List<Rectangle> GetRectanglesAgainstEntity(Vector2 center, float size, float rot)
        {
            float gridSize = 20f;
            float sizex = size;
            float sizey = size * 1.4f;
            var pos = AyaUtils.GetCameraRect(center, rot, sizex, sizey);
            // 计算旋转矩形的四个顶点
            List<Vector2> vertices = pos.ToList();

            // 计算旋转矩形的包围盒
            (Vector2 min, Vector2 max) = Helper.GetBoundingBox(pos);

            // 计算网格数量和实际网格大小
            int gridCountX = (int)Math.Ceiling((max.X - min.X) / gridSize);
            int gridCountY = (int)Math.Ceiling((max.Y - min.Y) / gridSize);

            List<Rectangle> resultRects = new List<Rectangle>();

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
                    if (Helper.IsRectInRotatedRect(gridRect, center, new Vector2(sizex,sizey), rot))
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

            float sizex = size;
            float sizey = size * 1.4f;
            var pos = AyaUtils.GetCameraRect(center, rot, sizex, sizey);
            var mplr = player.GetModPlayer<CameraPlayer>();

            //Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.06f, rot, TextureAssets.BlackTile.Value.Size() / 2, new Vector2(sizex / 16f,sizey / 16f), 0, 0);

            //显示消弹矩形
            //var rects = GetRectanglesAgainstProjectile(center, size * focusdScale, rot);

            //foreach (var r in rects)
            //{
            //    var dest = new Rectangle((int)(r.X - Main.screenPosition.X), (int)(r.Y - Main.screenPosition.Y), r.Width, r.Height);
            //    Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, dest, null, Color.Green);
            //}

            Vector2 dir = pos[0].DirectionToSafe(pos[2]);
            Vector2 ndir = pos[0].DirectionToSafe(pos[1]);

            float borderWidth = 2f;

            float focusdSizeX = sizex * focusdScale;
            float focusdSizeY = sizey * focusdScale;
            var pos2 = AyaUtils.GetCameraRect(center, rot, focusdSizeX, focusdSizeY);

            Color borderColor = outerFrameColor * focusFactor;
            float frameBoundriesFactor = 0.3f;

            //内框
            RenderHelper.DrawCameraFrame(Main.spriteBatch, pos, innerFrameColor * (1 - focusFactor), 2f, frameBoundriesFactor);
            //外框
            RenderHelper.DrawCameraFrame(Main.spriteBatch, pos2, borderColor, 2f, frameBoundriesFactor - 0.1f);

            //焦点
            Utils.DrawLine(Main.spriteBatch, center - dir * sizex / 8f, center + dir * sizex / 8f, focusCenterColor, focusCenterColor, borderWidth);
            Utils.DrawLine(Main.spriteBatch, center - ndir * sizey / 8f, center + ndir * sizey / 8f, focusCenterColor, focusCenterColor, borderWidth);

            //draw flash light
            float flashFactor = mplr.FlashTimer / mplr.FlashTimerMax;
            float extraScale = Utils.Remap(flashFactor, 0, 1f, 0.9f, 1.3f);
            //Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.5f * flashFactor, dir.ToRotation(), TextureAssets.BlackTile.Value.Size() / 2, new Vector2(sizex / 16f, sizey / 16f) * extraScale, 0, 0);

        }
    }
}
