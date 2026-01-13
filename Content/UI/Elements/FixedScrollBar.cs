using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class FixedScrollBar : UIScrollbar
    {
        public FixedScrollBar()
        {
            OnScrollWheel += HotbarScrollFix;
        }
        private void HotbarScrollFix(UIScrollWheelEvent evt, UIElement listeningElement) => Main.LocalPlayer.ScrollHotbar(PlayerInput.ScrollWheelDelta / 60);

    }
}
