using AyaMod.Core;
using AyaMod.Core.Prefabs;
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
    public class SquareLens : BaseLens, ILens
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
            //return base.IsLoadingEnabled(mod);
        }
        public bool Colliding(Vector2 center, float size, float rot, Rectangle targetRect)
        {
            var floatScale = 1f + MathF.Sin(Main.GameUpdateCount * 0.05f) * 0.1f;
            var offset = rot.ToRotationVector2() * size * 0.5f * floatScale;
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), center - offset, center + offset, size, ref point);
        }


        public List<Rectangle> GetRectanglesAgainstEntity(Vector2 center, float size, float rot)
        {
            return null;
        }

        public void DrawCamera(SpriteBatch spriteBatch, Player player, Vector2 center, float rot, float size, float focusdScale, float maxFocusScale, Color outerFrameColor, Color innerFrameColor, Color focusCenterColor)
        {
            float focusFactor = (focusdScale - 1f) / (maxFocusScale - 1f);

            var floatScale = 1f + MathF.Sin(Main.GameUpdateCount * 0.05f) * 0.1f;
            size *= floatScale;
            var pos = AyaUtils.GetCameraRect(center, rot, size);
            var mplr = player.GetModPlayer<CameraPlayer>();

            Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.06f, rot, TextureAssets.BlackTile.Value.Size() / 2, size / 16f, 0, 0);


            Vector2 dir = pos[0].DirectionToSafe(pos[2]);
            Vector2 ndir = pos[0].DirectionToSafe(pos[1]);

            Color baseColor = Color.White * 0.3f;
            float borderWidth = 2f;

            Utils.DrawLine(Main.spriteBatch, pos[0], pos[1], baseColor, baseColor, borderWidth);
            Utils.DrawLine(Main.spriteBatch, pos[1], pos[3], baseColor, baseColor, borderWidth);
            Utils.DrawLine(Main.spriteBatch, pos[3], pos[2], baseColor, baseColor, borderWidth);
            Utils.DrawLine(Main.spriteBatch, pos[2], pos[0], baseColor, baseColor, borderWidth);

            float focusdSize = size * focusdScale;
            var pos2 = AyaUtils.GetCameraRect(center, rot, focusdSize);

            Color borderColor = new Color(255, 66, 66, 255) * 0.6f;
            var possets = pos2;
            for (int i = 0; i < 2; i++)
            {
                float extraoffset = 0;
                if (i == 1)
                {
                    possets = pos;
                    extraoffset = 0;
                    borderColor = Color.White * 0.6f;
                }
                Utils.DrawLine(Main.spriteBatch, possets[0] + ndir * extraoffset, possets[0] + dir * size * 0.3f + ndir * extraoffset, borderColor, borderColor, borderWidth);
                Utils.DrawLine(Main.spriteBatch, possets[2] + ndir * extraoffset, possets[2] - dir * size * 0.3f + ndir * extraoffset, borderColor, borderColor, borderWidth);
                Utils.DrawLine(Main.spriteBatch, possets[1] - ndir * extraoffset, possets[1] + dir * size * 0.3f - ndir * extraoffset, borderColor, borderColor, borderWidth);
                Utils.DrawLine(Main.spriteBatch, possets[3] - ndir * extraoffset, possets[3] - dir * size * 0.3f - ndir * extraoffset, borderColor, borderColor, borderWidth);
                Utils.DrawLine(Main.spriteBatch, possets[0] + dir * extraoffset, possets[0] + ndir * size * 0.3f + dir * extraoffset, borderColor, borderColor, borderWidth);
                Utils.DrawLine(Main.spriteBatch, possets[1] + dir * extraoffset, possets[1] - ndir * size * 0.3f + dir * extraoffset, borderColor, borderColor, borderWidth);
                Utils.DrawLine(Main.spriteBatch, possets[2] - dir * extraoffset, possets[2] + ndir * size * 0.3f - dir * extraoffset, borderColor, borderColor, borderWidth);
                Utils.DrawLine(Main.spriteBatch, possets[3] - dir * extraoffset, possets[3] - ndir * size * 0.3f - dir * extraoffset, borderColor, borderColor, borderWidth);
            }
            borderColor = Color.White;
            Utils.DrawLine(Main.spriteBatch, center - dir * size / 8f, center + dir * size / 8f, borderColor, borderColor, borderWidth);
            Utils.DrawLine(Main.spriteBatch, center - ndir * size / 8f, center + ndir * size / 8f, borderColor, borderColor, borderWidth);

            //draw flash light
            float flashFactor = mplr.FlashTimer / mplr.FlashTimerMax;
            float extraScale = Utils.Remap(flashFactor, 0, 1f, 0.9f, 1.3f);
            Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.5f * flashFactor, dir.ToRotation(), TextureAssets.BlackTile.Value.Size() / 2, size / 16f * extraScale, 0, 0);

        }

    }
}
