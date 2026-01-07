using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace AyaMod.Content.UI.Elements
{
    public class ItemContainerGrid : UIGrid
    {
        public int MaxSlot;

        public IList<Item> Items;

        public float Opacity;

        public override void Update(GameTime gameTime)
        {
            UpdateOrder();
            foreach (var item in _innerList.Children)
            {
                if (item is not BaseItemSlot) continue;
                var slot = (item as BaseItemSlot);
                slot.Opacity = Opacity;
            }
            //for (int i = 0; i < Items.Count; i++)
            //{
            //    Item item = Items[i];
            //    if (item.IsAir) continue;
            //    Items.RemoveAt(i--);
            //}
            //int slotNumber = GetSlotNumber(Items.Count);
            //if (Count != slotNumber)
            //    SetInventory(Items, slotNumber);

            base.Update(gameTime);
        }

        public void SetInventory(IList<Item> items, int slotNumber = -1)
        {
            Items = items;
            Clear();

            MaxSlot = slotNumber;

            var itemlist= new List<ItemContainerItemSlot>();
            for(int i = 0; i < slotNumber; i++)
            {
                var itemslot = new ItemContainerItemSlot(items, i);

                itemlist.Add(itemslot);
            }
            AddRange(itemlist);
        }
        public override void RecalculateChildren()
        {
            base.RecalculateChildren();
        }
    }
}
