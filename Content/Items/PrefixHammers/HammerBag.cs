using AyaMod.Content.UI;
using AyaMod.Core;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace AyaMod.Content.Items.PrefixHammers
{
    public class HammerBag : ModItem, IItemContainer, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.PrefixHammers + Name;

        string IItemContainer.Name => Item.Name;
        public List<Item> ItemContainer { get; private set; } = [];

        public bool CanIntoContainer(Item item) => item.ModItem != null && item.ModItem is BasePrefixHammer;

        public void ItemIntoContainer(Item item)
        {
            //清除air和堆叠物品
            for(int i = 0; i < ItemContainer.Count; i++)
            {
                if (ItemContainer[i].IsAir)
                {
                    ItemContainer.RemoveAt(i);
                    i--;
                    continue;
                }
                if (ItemContainer[i].type == item.type && ItemContainer[i].stack < ItemContainer[i].maxStack && ItemLoader.TryStackItems(ItemContainer[i],item,out _))
                {
                    SoundEngine.PlaySound(SoundID.Grab);
                    if (item.stack <= 0)
                        item.TurnToAir();
                }
            }

            if(!item.IsAir && ItemContainer.Count < 60)
            {
                ItemContainer.Add(item.Clone());
                item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
            }

            SortContainer();
        }

        public void SortContainer()
        {
            ItemContainer.Sort((a, b) => a.type.CompareTo(b.type) * a.IsAir.CompareTo(b.IsAir));
        }

        public override ModItem Clone(Item newEntity)
        {
            var clone = (HammerBag)base.Clone(newEntity);
            clone.ItemContainer = new List<Item>(ItemContainer);
            return clone;
        }
        public override bool CanRightClick() => ItemContainer != null;
        public override void RightClick(Player player)
        {
            if (ItemContainerUI.Instance.Visible && ItemContainerUI.Instance.Container == this &&
                ItemContainerUI.Instance.OpenTimer.AnyPositive)
                ItemContainerUI.Instance.Close();
            else
                ItemContainerUI.Instance.Open(this);
        }
        public override bool ConsumeItem(Player player) => false;


        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.consumable = false;
            Item.maxStack = 1;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(silver: 30));
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<List<Item>>("hammers", out var hammers))
                ItemContainer = hammers;
        }
        public override void SaveData(TagCompound tag)
        {
            tag["hammers"] = ItemContainer;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk,16)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
