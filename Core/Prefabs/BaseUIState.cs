using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace AyaMod.Core.Prefabs
{
    public abstract class BaseUIState : UIState
    {
        /// <summary>
        /// 是否可见
        /// </summary>
        public virtual bool Visible { get; set; } = false;

        public abstract int UILayer(List<GameInterfaceLayer> layers);
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //bool displayHitbox = true;
            //if (displayHitbox)
            //{
            //    DrawHitbox(this);
            //}
            //void DrawHitbox(UIElement element)
            //{
            //    var dimensions = element.GetDimensions();
            //    var rect = dimensions.ToRectangle();
            //    Utils.DrawRect(spriteBatch, rect, Color.Red);
            //    foreach (var c in element.Children)
            //        DrawHitbox(c);
            //}
            //Utils.DrawBorderString(spriteBatch, "????", Main.MouseScreen, Color.Blue);
            //Utils.DrawBorderString(spriteBatch, "???????", 487,493, Color.Red, Color.Pink, new Vector2(487,493));
        }
    }
}
