using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class ItemContainerItemSlot : GenericItemSlot
    {
        public ItemContainerItemSlot(IList<Item> items, int index) : base(items, index)
        {   
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            bool moustItemIsAir = Main.mouseItem.IsAir;
            base.LeftMouseDown(evt);

            //把栏位里的物品拿到鼠标上
            if (!Item.IsAir && moustItemIsAir)
            {
                LeftClick_ItemSlot();
            }
        }
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch); 
            bool hovering = IsMouseHovering;
            var style = GetDimensions();
            //if (hovering)
            //{
            //    Item.scale = 2f;
            //}
            //else Item.scale = 1f;

            //if (hovering)
            //{
            //    Main.HoverItem = Item.Clone();
            //    Main.hoverItemName = Item.Name;
            //    Utils.DrawRect(spriteBatch, style.ToRectangle(), Color.Red);
            //}
            //Utils.DrawBorderString(spriteBatch, $"{_index}", style.ToRectangle().TopLeft(), Color.Red);
        }
        public void LeftClick_ItemSlot()
        {
            if (Main.LocalPlayer.ItemAnimationActive)
                return;

            if(!Main.mouseItem.IsAir)
            {
                return;
            }

            Main.mouseItem = Item;
            Item = new Item();
            SoundEngine.PlaySound(SoundID.Grab);
            //Parent.Recalculate();
        }

        public override int CompareTo(object obj)
        {
            if (obj is ItemContainerItemSlot slot)
            {
                var result = -(this.Item.type.CompareTo(slot.Item.type));
                var isair = (this.Item.IsAir.CompareTo(slot.Item.IsAir));
                
                if (isair == 0) return result;
                if (isair == 0 && result == 0) return _index.CompareTo(slot._index);
                return isair;
            }
            return base.CompareTo(obj);
        }
    }
}
