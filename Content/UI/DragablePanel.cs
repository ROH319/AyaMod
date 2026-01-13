using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace AyaMod.Content.UI
{
    public class DragablePanel : UIPanel
    {
        private Vector2 offset;
        public bool dragging;
        public bool CanDrag = true;
        public List<UIElement> ExtraChildren = [];
        public bool PanelsDraggable = true;
        public DragablePanel() { }
        public DragablePanel(params UIElement[] countMeAsChildren)
        {
            ExtraChildren.AddRange(countMeAsChildren);
        }
        public void AddExtraChild(params UIElement[] countMeAsChildren)
        {
            ExtraChildren.AddRange(countMeAsChildren);
        }

        public void DragStart(Vector2 pos)
        {
            offset = new Vector2(pos.X - Left.Pixels, pos.Y - Top.Pixels);
            dragging = true;
        }
        public void DragEnd(Vector2 pos)
        {
            Vector2 end = pos;
            dragging = false;

            Left.Set(end.X - offset.X, 0);
            Top.Set(end.Y - offset.Y, 0);

            Recalculate();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (CanDrag)
            {
                if (!dragging && ContainsPoint(Main.MouseScreen) && Main.mouseLeft && PlayerInput.MouseInfoOld.LeftButton == ButtonState.Released)
                {
                    bool upperMost = true;
                    IEnumerable<UIElement> children = Elements;

                    if (ExtraChildren != null)
                        children = children.Concat(ExtraChildren);

                    foreach(UIElement c in children)
                    {
                        if (CheckUpper(c))
                        {
                            upperMost = false;break;
                        }
                    } 

                    bool CheckUpper(UIElement element)
                    {
                        bool value = element.ContainsPoint(Main.MouseScreen);
                        if(!value)
                        {
                            foreach (UIElement c in element.Children.Where(x => x is not UIGrid.UIInnerList))
                                value = value |= CheckUpper(c);
                        }
                        return value;
                    }

                    if (upperMost)
                        DragStart(Main.MouseScreen);
                }
                else if (dragging && !Main.mouseLeft)
                {
                    DragEnd(Main.MouseScreen);
                }
            }
            
            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0); // Main.MouseScreen.X and Main.mouseX are the same.
                Top.Set(Main.mouseY - offset.Y, 0);
                Recalculate();
            }

            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
        }
    }
}
