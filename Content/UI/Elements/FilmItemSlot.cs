using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class FilmItemSlot : GenericItemSlot
    {
        public bool Using;
        public FilmItemSlot(IList<Item> items, int index) : base(items, index)
        {
        }

        public bool CanIntoSlot(Item item)
        {
            return item.ModItem != null && item.ModItem is BaseFilm;
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            Player p = Main.LocalPlayer;
            var dp = p.GetModPlayer<DataPlayer>();
            if (dp == null) return;

            if (Using)
            {
                Item inv = dp.UsingFilm[_index];
                if (!Main.mouseItem.IsAir && !p.ItemAnimationActive && CanIntoSlot(Main.mouseItem))
                    ItemSlot.LeftClick(ref inv, ItemSlot.Context.VoidItem);

                ItemSlot.RightClick(ref inv, ItemSlot.Context.InventoryItem);

                dp.UsingFilm[_index] = inv;
            }
            else
            {
                Item inv = dp.FilmVault[_index];
                if (!Main.mouseItem.IsAir && !p.ItemAnimationActive && CanIntoSlot(Main.mouseItem))
                    ItemSlot.LeftClick(ref inv, ItemSlot.Context.VoidItem);

                ItemSlot.RightClick(ref inv, ItemSlot.Context.InventoryItem);

                dp.FilmVault[_index] = inv;
            }
        }
        public override int CompareTo(object obj)
        {
            if (obj is GenericItemSlot slot)
                return _index.CompareTo(slot._index);
            return base.CompareTo(obj);
        }
    }
}
