using AyaMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class TimerElement : UIElement
    {
        public TweenTimer HoverTimer = new(0, 1f, 0.03f,startPlay:false);

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)
            {
                if (!HoverTimer.AnyPositive)
                    HoverTimer.PlayAndReset();
            }
            else
            {
                if (!HoverTimer.AnyNegative)
                    HoverTimer.PlayAndReset(false);
            }

            HoverTimer.Update();
            base.Update(gameTime);
        }
    }
}
