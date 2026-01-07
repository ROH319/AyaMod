using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.UI.Elements;

namespace AyaMod.Content.UI.Elements
{
    public class FilmContainerGrid : UIGrid
    {

        public bool Using;

        public int MaxSlot;

        public int AvailibleSlot;

        public IList<Item> Items;

        public float Opacity;

        public FilmContainerGrid(bool Using = false)
        {
            this.Using = Using;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var item in _innerList.Children)
            {
                if (item is not BaseItemSlot) continue;
                var slot = (item as BaseItemSlot);
                slot.Opacity = Opacity;
            }
        }

        public void Open(IList<Item> items)
        {

            Items = items;
            Clear();
            var slotNumber = items.Count;

            var itemlist = new List<FilmItemSlot>();
            for (int i = 0; i < slotNumber; i++)
            {
                FilmItemSlot itemslot = new(items, i)
                {
                    Using = Using
                };
                itemlist.Add(itemslot);
            }
            AddRange(itemlist);
        }
    }
}
