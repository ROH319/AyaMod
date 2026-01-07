using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.UI.Elements
{
    public class GenericItemSlot(IList<Item> items, int index) : BaseItemSlot
    {
        protected readonly IList<Item> _items = items;
        internal readonly int _index = index;

        public override Item Item 
        {
            get => (_items.Count > 0 && _index < _items.Count) ? _items[_index] : AirItem;
            set
            {
                if (_items.Count > 0 && _index < _items.Count)
                    _items[_index] = value;
            } 
        }
    }
}
