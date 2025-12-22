using AyaMod.Core;
using AyaMod.Core.Prefabs;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Dusts
{
    public class YellowStarDust : BaseDust
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.White;
            dust.frame.X = 10 * 292;
            dust.frame.Y = 10 * Main.rand.Next(3);
            int type = 292;
            while (type >= 100)
            {
                type -= 100;
                dust.frame.X -= 1000;
                dust.frame.Y += 30;
            }
            dust.frame.Width = 8;
            dust.frame.Height = 8;
            //dust.frame = Texture2D.Frame(1, 3, 0, 0);
            //dust.frame.Y = Main.rand.Next(3);

        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
        public override bool Update(Dust dust)
        {
            dust.rotation += 0.02f * MathF.Sin(dust.dustIndex);
            dust.velocity.Y *= 0.98f;
            dust.velocity.X *= 0.99f;

            dust.velocity *= 0.965f;

            dust.scale -= 0.017f;
            if (dust.scale < 0.1f) dust.active = false;

            float gravity = 0.12f;

            //if (dust.velocity.Y < 5) dust.velocity.Y += gravity;
            dust.position += dust.velocity;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = TextureAssets.Dust.Value;
            Color color = dust.GetAlpha(Color.White);


            Main.spriteBatch.Draw(texture, dust.position - Main.screenPosition, dust.frame, color, dust.GetVisualRotation(), new Vector2(4f, 4f), dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
