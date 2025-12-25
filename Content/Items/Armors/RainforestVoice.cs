using AyaMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class RainforestVoiceHelmet : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public static LocalizedText RainforestVoiceBonus { get; set; }

        public static int DamageBonus = 16;
        public static int CritBonus = 8;

        public override void SetStaticDefaults()
        {
            RainforestVoiceBonus = this.GetLocalization("RainforestVoiceBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 20;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Lime7, Item.sellPrice(gold: 6));
            Item.defense = 9;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
            player.detectCreature = true;
            player.dangerSense = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class RainforestVoiceChestplate : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);

        public static int DamageBonus = 10;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Lime7, Item.sellPrice(gold: 4, silver: 80));
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 18)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class RainforestVoicePants : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus, MoveSpeedBonus);

        public static int CritBonus = 10;
        public static int MoveSpeedBonus = 10;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Lime7, Item.sellPrice(gold: 4, silver: 80));
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
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
