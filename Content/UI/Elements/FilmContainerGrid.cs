using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace AyaMod.Content.UI.Elements
{
    public class FilmContainerGrid : UIGrid
    {
        public int MaxSlot;

        public IList<Item> Items;

        public float Opacity;
        public FixedScrollBar Scrollbar;
        public FilmContainerGrid(bool withScrollbar = true)
        {
            if (!withScrollbar) return;
            Scrollbar = new();
            Scrollbar.SetView(400, 1000);
            Scrollbar.Width.Set(20, 0);
            Scrollbar.OverflowHidden = true;
            SetScrollbar(Scrollbar);
            Append(Scrollbar);
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
            Scrollbar?.SetView(400,1000);
            Scrollbar?.MaxHeight.Set(this.Height.Pixels,this.Height.Percent);
        }

        public void Open(IList<Item> items)
        {

            Items = items;
            Clear();
            var slotNumber = items.Count;

            var itemlist = new List<FilmItemSlot>();
            for (int i = 0; i < slotNumber; i++)
            {
                var itemslot = new FilmItemSlot(items, i);
                itemlist.Add(itemslot);
            }
            AddRange(itemlist);
        }
    }
    public class UsingFilmGrid : FilmContainerGrid
    {
        
        public int AvailableSlot;
        public UsingFilmGrid() : base(false) { }
        public void Open(IList<Item> items, int slot)
        {
            AvailableSlot = slot;
            Items = items;
            Clear();
            var slotNumber = items.Count;

            var itemlist = new List<FilmItemSlot>();
            for (int i = 0; i < slotNumber; i++)
            {
                var itemslot = new UsingFilmItemSlot(items, i)
                { 
                    Available = i < slot
                };
                itemlist.Add(itemslot);
            }
            AddRange(itemlist);
        }
    }
}
