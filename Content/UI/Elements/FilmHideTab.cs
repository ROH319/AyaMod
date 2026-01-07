using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class FilmHideTab : UIElement
    {
        public FilmContainerUI parent;

        public FilmHideTab(FilmContainerUI parent)
        {
            this.parent = parent;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}
