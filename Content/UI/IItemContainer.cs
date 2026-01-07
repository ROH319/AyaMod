using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.UI
{
    public interface IItemContainer
    {
        string Name { get; }

        List<Item> ItemContainer { get; }

        /// <summary>
        /// 将物品放入容器中
        /// </summary>
        /// <param name="item"></param>
        void ItemIntoContainer(Item item);
        
        /// <summary>
        /// 符合进入标准
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CanIntoContainer(Item item);
    }
}
