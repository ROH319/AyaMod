using AyaMod.Core;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.PlayerDrawLayers
{
    public class UltraDashDrawLayer : PlayerDrawLayer
    {
        public override bool IsHeadLayer => false;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => 
            drawInfo.drawPlayer.whoAmI == Main.myPlayer &&
            drawInfo.drawPlayer.active &&
            !drawInfo.drawPlayer.dead &&
            !drawInfo.drawPlayer.ghost &&
            drawInfo.shadow == 0 &&
            drawInfo.drawPlayer.Aya().IsUltraDashing;


        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            Texture2D cone = Request<Texture2D>(AssetDirectory.Extras + "UltraDashCone", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //Rectangle rectangle = cone.Bounds;

            Color color = new Color(255, 150, 150, 0);
            float opacity = Main.mouseTextColor / 255f;
            opacity *= opacity;
            float extraRot = 0f;
            //if (player.velocity.X > 0) extraRot = -MathHelper.PiOver2;
            SpriteEffects se = drawInfo.playerEffect;
            if(player.Aya().UltraDashDir.ToRotationVector2().X == 0)se = SpriteEffects.None;
            DrawData data = new DrawData(
                cone,
                player.Center - Main.screenPosition,
                null,
                color * opacity,
                player.bodyRotation - MathHelper.PiOver2,
                cone.Size()/2,
                0.75f,
                SpriteEffects.None,
                0
                );
            drawInfo.DrawDataCache.Add(data);
        }
    }
}
