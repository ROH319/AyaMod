using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using static Terraria.ID.ArmorIDs;
using Terraria.DataStructures;
using AyaMod.Helpers;
using Terraria.ID;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [AutoloadEquip(EquipType.Wings)]
    public class TenguWings : BaseAccessories
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;
        public override void SetStaticDefaults()
        {
            //flySpeedOverride最大水平速度：7.5f->38mph
            //accelerationMultiplier水平加速：120%
            Wing.Sets.Stats[Item.wingSlot] = new WingStats(150,7.5f,1.2f);
        }
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 1));

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HeldCamera() && player.wingTime == player.wingTimeMax)
                player.Aya().FreeFlyFrame += 10;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 7.5f;
            acceleration *= 1.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HarpyWings)
                .AddIngredient(ItemID.SoulofFlight, 25)
                .AddIngredient(ItemID.SoulofMight,10)
                .AddIngredient(ItemID.Feather, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
