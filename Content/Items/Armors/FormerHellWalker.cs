using AyaMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class FormerHellWalkerHelmet : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public static LocalizedText HellWalkerBonus { get; set; }

        public override void SetStaticDefaults()
        {
            HellWalkerBonus = this.GetLocalization("HellWalkerBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(silver: 90));
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<FormerHellWalkerLongcoat>() && legs.type == ItemType<FormerHellWalkerPants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = HellWalkerBonus.Value;
            HellWalkerSetEffect(player);
        }
        public static void HellWalkerSetEffect(Player player)
        {

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public static int DamageBonus = 5;
        public static int CritBonus = 5;
    }
    [AutoloadEquip(EquipType.Body)]
    public class FormerHellWalkerLongcoat : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(silver: 60));
            Item.defense = 7;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public static int DamageBonus = 5;

    }
    [AutoloadEquip(EquipType.Legs)]
    public class FormerHellWalkerPants : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus, MoveSpeedBonus);

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(silver: 60));
            Item.defense = 6;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
            player.moveSpeed += MoveSpeedBonus / 100f;
            player.fireWalk = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public static int CritBonus = 5;
        public static int MoveSpeedBonus = 8;

    }
}
