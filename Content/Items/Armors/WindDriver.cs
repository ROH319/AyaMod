using AyaMod.Core;
using Terraria.Localization;
using Terraria;
using AyaMod.Core.ModPlayers;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class WindDriverHeadgear : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public static LocalizedText WindDriverBonus { get; set; }

        public static int DamageBonus = 8;

        public override void SetStaticDefaults()
        {
            WindDriverBonus = this.GetLocalization("WindDriverBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 26;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Yellow8, Item.sellPrice(gold: 3));
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetModPlayer<CameraPlayer>().ChaseSpeedModifier += 0.15f;
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class WindDriverHaori : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);

        public static int DamageBonus = 8;
        public static int CritBonus = 8;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Yellow8, Item.sellPrice(gold: 3));
            Item.defense = 18;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
            player.hasMagiluminescence = true;
        }

    }
    [AutoloadEquip(EquipType.Legs)]
    public class WindDriverLeggings : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus, MoveSpeedBonus);

        public static int CritBonus = 8;
        public static int MoveSpeedBonus = 15;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Yellow8, Item.sellPrice(gold: 3));
            Item.defense = 14;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
            player.moveSpeed += MoveSpeedBonus / 100f;
        }
    }
}
