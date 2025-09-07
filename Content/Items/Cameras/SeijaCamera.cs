using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class SeijaCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 70;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Rapier;

            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.05f, 132, 1.4f, 0.5f);
            SetCaptureStats(100, 5);
        }

        public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            float totalY = 0;
            foreach (var line in lines)
            {
                totalY += FontAssets.MouseText.Value.MeasureString(line.Text).Y;
            }
            if (!Item.favorited)
            {
                var yAxis = (totalY * 0.5f + Main.mouseY + 26) * Main.UIScale;

                // 创建一个缩放矩阵，该矩阵沿 Y 轴缩放 -1，从而实现镜像翻转。
                Matrix scaleMatrix = Matrix.CreateScale(1, -1, 1);

                // 创建一个平移矩阵，该矩阵将坐标系向上移动 yAxis。
                Matrix translateToOrigin = Matrix.CreateTranslation(0, -yAxis, 0);

                // 创建一个平移矩阵，该矩阵将坐标系向下移动 yAxis。
                Matrix translateBack = Matrix.CreateTranslation(0, yAxis, 0);

                // 将这些矩阵组合起来，以先平移到原点，然后缩放（镜像翻转），然后平移回来的顺序进行。
                var mirrorY = translateToOrigin * scaleMatrix * translateBack;

                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                    Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, null, Main.UIScaleMatrix * mirrorY);
            }
            return base.PreDrawTooltip(lines, ref x, ref y);
        }

        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            if (!Item.favorited)
            {
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
                    Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            }
        }
    }
}
