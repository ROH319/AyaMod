using AyaMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class RumorBreakerHelmet : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public static LocalizedText RumorBreakerBonus { get; set; }

        public static int DamageBonus = 10;
        public static int CritBonus = 10;

        public override void SetStaticDefaults()
        {
            RumorBreakerBonus = this.GetLocalization("RumorBreakerBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(gold: 3));
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<RumorBreakerChestplate>() && legs.type == ItemType<RumorBreakerGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = RumorBreakerBonus.Value;
            RumorBreakerSetEffect(player);
        }
        public static void RumorBreakerSetEffect(Player player)
        {

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddIngredient(ItemID.CrystalShard, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class RumorBreakerChestplate : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);

        public static int DamageBonus = 10;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(gold: 3));
            Item.defense = 13;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.CrystalShard, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class RumorBreakerGreaves : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus, MoveSpeedBonus);

        public static int CritBonus = 6;
        public static int MoveSpeedBonus = 8;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(gold: 3));
            Item.defense = 12;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
            player.moveSpeed += MoveSpeedBonus / 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddIngredient(ItemID.CrystalShard, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
