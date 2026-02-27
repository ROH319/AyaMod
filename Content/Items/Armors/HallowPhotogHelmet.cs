using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class HallowPhotogHelmet : ModItem, IPlaceholderItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public override string Texture => AssetDirectory.Armors + Name;
        public override void SetDefaults()
        {
            Item.height = Item.height = 18;
            Item.defense = 5;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(gold: 25));
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus * 0.01f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ItemType<HallowPhotogHelmet>() && body.type == ItemID.HallowedPlateMail && legs.type == ItemID.HallowedGreaves;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("ArmorSetBonus.Hallowed");
            player.onHitDodge = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public static int DamageBonus = 12;
        public static int CritBonus = 10;
    }
}
