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

        public override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            Texture2D cone = Request<Texture2D>(AssetDirectory.Extras + "bulletBa004", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //Rectangle rectangle = cone.Bounds;

            Color color = new Color(255, 150, 150, 0);
            float opacity = Main.mouseTextColor / 255f;
            //opacity *= opacity*1.2f;
            float extraRot = 0f;
            Vector2 scale = new Vector2(1.4f, 1.1f);
            //if (player.velocity.X > 0) extraRot = -MathHelper.PiOver2;
            float rot = player.bodyRotation - MathHelper.PiOver2;
            Vector2 dir = rot.ToRotationVector2();
            SpriteEffects se = drawInfo.playerEffect;
            if (player.Aya().UltraDashDir.ToRotationVector2().X == 0) se = SpriteEffects.None;
            DrawData data = new DrawData(
                cone,
                player.Center - Main.screenPosition - dir * 12,
                null,
                color * opacity,
                rot,
                cone.Size()/2,
                scale,
                SpriteEffects.None,
                0
                );
            DrawData data2 = new DrawData(cone,
                player.Center - Main.screenPosition + rot.ToRotationVector2() * 15,null,color * opacity, rot,
                cone.Size() / 2,
                /*new Vector2(1.2f,1.2f)*/scale *0.65f,
                SpriteEffects.None,
                0
                );
            drawInfo.DrawDataCache.Add(data);
            drawInfo.DrawDataCache.Add(data2);
        }
    }
}
