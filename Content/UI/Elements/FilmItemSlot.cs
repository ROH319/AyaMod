using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
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
    public class FilmItemSlot : GenericItemSlot
    {
        public override Item Item { get => base.Item; set => base.Item = value; }
        public FilmItemSlot(IList<Item> items, int index) : base(items, index)
        {
        }

        public static bool CanIntoSlot(Item item)
        {
            return item.ModItem != null && item.ModItem is BaseFilm;
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            //Main.NewText($"love you left mouse down{Main.GameUpdateCount}");

            if (!Main.mouseItem.IsAir && !CanIntoSlot(Main.mouseItem)) return;

            if (Item.IsAir)
            {
                if (Main.mouseItem.IsAir) return;

                Item = Main.mouseItem;
                Main.mouseItem = new Item();
                SoundEngine.PlaySound(SoundID.Grab);
            }
            else
            {
                if (Main.mouseItem.IsAir)
                {
                    Main.mouseItem = Item;
                    Item = new Item();
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                else
                {
                    // 同种物品
                    if (Main.mouseItem.type == Item.type)
                    {
                        if (Item.stack < Item.maxStack)
                        {
                            if (Item.stack + Main.mouseItem.stack <= Item.maxStack)
                            {
                                Item.stack += Main.mouseItem.stack;
                                Main.mouseItem.SetDefaults();
                            }
                            else
                            {
                                Main.mouseItem.stack -= Item.maxStack - Item.stack;
                                Item.stack = Item.maxStack;
                            }
                        }
                        else if (Main.mouseItem.stack < Main.mouseItem.maxStack)
                        {
                            if (Main.mouseItem.stack + Item.stack <= Main.mouseItem.maxStack)
                            {
                                Main.mouseItem.stack += Item.stack;
                                Item.SetDefaults();
                            }
                            else
                            {
                                Item.stack -= Main.mouseItem.maxStack - Main.mouseItem.stack;
                                Main.mouseItem.stack = Main.mouseItem.maxStack;
                            }
                        }
                        else
                        {
                            (Main.mouseItem, Item) = (Item, Main.mouseItem);
                        }
                    }
                    else
                    {
                        (Item, Main.mouseItem) = (Main.mouseItem, Item);
                    }

                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            //Recalculate();
            if (IsMouseHovering)
            {
                Main.HoverItem = Item.Clone();
                Main.hoverItemName = Item.Name;
                //Utils.DrawRect(spriteBatch, style.ToRectangle(), Color.Red);
            }
            //var style = GetDimensions();
            //var l = Main.LocalPlayer.GetModPlayer<DataPlayer>().FilmVault.Count(x => !x.IsAir);
            //Utils.DrawBorderString(spriteBatch, $"????????{l.ToString()}", Main.MouseScreen, Color.Red);
            //Utils.DrawBorderString(spriteBatch, $"{_index}", style.ToRectangle().TopLeft(), Color.Red);
        }
        public override int CompareTo(object obj)
        {
            if (obj is GenericItemSlot slot)
                return _index.CompareTo(slot._index);
            return base.CompareTo(obj);
        }
    }

    public class UsingFilmItemSlot : FilmItemSlot
    {
        public bool Available = false;
        public UsingFilmItemSlot(IList<Item> items, int index) : base(items, index)
        {
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (!Available)
            {
                var dimension = GetDimensions();
                Rectangle rect = new((int)dimension.X, (int)dimension.Y, (int)(52 * Main.inventoryScale), (int)(52 * Main.inventoryScale));
                var tex = TextureAssets.MagicPixel.Value;
                float CrossLength = 32;
                float CrossWidth = 6;
                var crossColor = (Color.OrangeRed * 0.5f).WithAlpha(180);
                Vector2 scale = new(CrossLength, CrossWidth / 1000f);
                for (int i = -1; i < 2; i += 2)
                {
                    float rot = i * MathHelper.PiOver4;
                    Main.spriteBatch.Draw(tex, rect.Center(), null, crossColor, rot, tex.Size() / 2, scale, 0, 0);
                }
            }
        }
    }
}
