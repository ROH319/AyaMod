using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class SimpleCross : TimerElement
    {
        public float Opacity = 0f;
        public Color BackColor, HoverStartColor, HoverEndColor;
        public float CrossLength, CrossWidth;

        public SimpleCross(float width = 50f, float height = 50f)
        {
            Width.Set(width, 0f);
            Height.Set(height, 0f);
            CrossLength = 30;
            CrossWidth = 4;
            BackColor = new Color(73, 94, 171);
            HoverStartColor = Color.OrangeRed;
            HoverEndColor = Color.Gold;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer.Update(1f);
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            var rect = dimensions.ToRectangle();
            Vector2 center = dimensions.Center();

            Color crossColor = HoverTimer.Lerp(HoverStartColor, HoverEndColor) * Opacity;

            var tex = TextureAssets.MagicPixel.Value;
            Color backColor = BackColor * HoverTimer.Lerp(0,0.3f) * Opacity;
            Main.spriteBatch.Draw(tex, rect, null, backColor, 0, Vector2.Zero, 0, 0);

            Vector2 scale = new(CrossLength, CrossWidth / 1000f);
            for(int i = -1; i < 2; i += 2)
            {
                float rot = i * MathHelper.PiOver4;
                Main.spriteBatch.Draw(tex, center, null, crossColor, rot, tex.Size() / 2, scale, 0, 0);
                //Utils.DrawLine(spriteBatch, center + dir * length + Main.screenPosition, center - dir * length + Main.screenPosition, crossColor, crossColor, width);
            }
            base.DrawSelf(spriteBatch);

        }

    }
}
