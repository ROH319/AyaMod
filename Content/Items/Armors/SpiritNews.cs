using AyaMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class SpiritNewsHood : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public static LocalizedText SpiritNewsBonus { get; set; }
        public static int DamageBonus = 10;

        public override void SetStaticDefaults()
        {
            SpiritNewsBonus = this.GetLocalization("SpiritNewsBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(silver: 90));
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<SpiritNewsCloak>() && legs.type == ItemType<SpiritNewsBoots>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SpiritNewsBonus.Value;
            SpiritNewsSetEffect(player);
        }
        public static void SpiritNewsSetEffect(Player player)
        {

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class SpiritNewsCloak : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);

        public static int DamageBonus = 5;
        public static int CritBonus = 3;

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
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 40)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class SpiritNewsBoots : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus, MoveSpeedBonus);

        public static int CritBonus = 6;
        public static int MoveSpeedBonus = 8;

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
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 35)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
