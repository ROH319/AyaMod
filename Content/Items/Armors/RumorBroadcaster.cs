using AyaMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class RumorBroadcasterHelmet : ModItem, IPlaceholderItem 
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public static LocalizedText RumorBroadcasterBonus { get; set; }

        public static int DamageBonus = 15;
        public static int CritBonus = 10;

        public override void SetStaticDefaults()
        {
            RumorBroadcasterBonus = this.GetLocalization("RumorBroadcasterBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 26;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(silver: 3));
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<RumorBroadcasterChestplate>() && legs.type == ItemType<RumorBroadcasterGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = RumorBroadcasterBonus.Value;
            RumorBroadcasterSetEffect(player);
        }
        public static void RumorBroadcasterSetEffect(Player player)
        {

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class RumorBroadcasterChestplate : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus);

        public static int CritBonus = 5;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(silver: 3));
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class RumorBroadcasterGreaves : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, MoveSpeedBonus);

        public static int DamageBonus = 10;
        public static int MoveSpeedBonus = 8;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(silver: 3));
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.moveSpeed += MoveSpeedBonus / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
