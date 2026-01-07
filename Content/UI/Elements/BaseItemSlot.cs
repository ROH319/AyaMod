using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class BaseItemSlot : UIElement
    {
        public readonly Item AirItem = new Item();

        public virtual Item Item { get => AirItem; set { } }

        public Color ItemColor = Color.White;

        public float Opacity = 0f;

        public BaseItemSlot()
        {
            Width.Set(52, 0);
            Height.Set(52, 0);
            Recalculate();
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (!Item.IsAir)
            {
                Item inv = Item;
                ItemSlot.OverrideHover(ref inv, ItemSlot.Context.VoidItem);
                ItemSlot.MouseHover(ref inv, ItemSlot.Context.VoidItem);
            }
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Item inv = Item;
            var dimension = GetDimensions();
            var rect = dimension.ToRectangle();
            //Utils.DrawBorderString(spriteBatch, "?", rect.TopLeft(), Color.Red);
            //(int)(Main.GameUpdateCount * 0.01f % 32)

            //ItemSlot.Draw(spriteBatch, ref inv, ItemSlot.Context.ChestItem, dimension.Position(), ItemColor * Opacity);
            DrawItemSlot(spriteBatch, Item, dimension.Position(), ItemColor * Opacity);
        }

        public static void DrawItemSlot(SpriteBatch spriteBatch, Item item, Vector2 position, Color lightColor)
        {
            //Player player = Main.player[Main.myPlayer];
            float inventoryScale = Main.inventoryScale;
            Color color = lightColor != Color.Transparent ? lightColor : Color.White;

            Texture2D invBack = TextureAssets.InventoryBack5.Value;
            Color backColor = Main.inventoryBack.MultiplyRGBA(lightColor);


            spriteBatch.Draw(invBack, position, null, backColor, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);

            if(!item.IsAir)
            {
                float scale = ItemSlot.DrawItemIcon(item, 3, spriteBatch, position + invBack.Size() * inventoryScale / 2f, inventoryScale, 32f, color);
            }
        }
    }
}
