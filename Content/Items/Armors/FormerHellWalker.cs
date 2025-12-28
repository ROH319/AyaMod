using AyaMod.Content.Buffs;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(OverrideEffectName ="FormerHellSet")]
    public class FormerHellWalkerHelmet : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public static LocalizedText HellWalkerBonus { get; set; }

        public static int DamageBonus = 5;
        public static int CritBonus = 5;
        public static int AttackSpeedBonus = 12;
        public override void Load()
        {
            AyaPlayer.DoubleTapHook += HellWalkerKeyEffect;
        }
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
            player.setBonus = HellWalkerBonus.WithFormatArgs(AttackSpeedBonus, EnergeticBuff.RegenBonus, EnergeticBuff.DefenseBonus, EnergeticBuff.MovementBonus).Value;
            player.AddEffect("FormerHellSet");
            HellWalkerSetEffect(player);
        }
        public static void HellWalkerSetEffect(Player player)
        {
            if (player.HasBuff(BuffID.Tipsy))
            {
                player.statDefense += 4;
                player.GetAttackSpeed<ReporterDamage>() += AttackSpeedBonus / 100f;
            }
        }
        public static void HellWalkerKeyEffect(Player player)
        {
            if (!player.HasEffect("FormerHellSet")) return;
            if (player.ownedProjectileCounts[ProjectileType<HotSpringCircle>()] > 0 || player.HasBuff<HotSpringCDBuff>())
                return;

            float springDuration = 20 * 60;
            float debuffDuration = 10 * 60;
            Projectile spring = Projectile.NewProjectileDirect(player.GetSource_Misc(""), player.Center, Vector2.Zero, ProjectileType<HotSpringCircle>(), 0, 0f, player.whoAmI, springDuration, debuffDuration);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class HotSpringCircle : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Duration => ref Projectile.ai[0];
        public ref float DebuffDuration => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 300;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20 * 60;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Duration;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            float radius = Projectile.width;

            foreach(Player player in Main.ActivePlayers)
            {
                if (player.Distance(Projectile.Center) > radius) continue;
                player.AddBuff(BuffType<EnergeticBuff>(), 8 * 60);
            }

            int dustamount = 50;
            for(int i = 0; i < dustamount; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * radius;
                Vector2 vel = -Vector2.UnitY.RotateRandom(0.3f) * 2f;
                Dust d = Dust.NewDustPerfect(pos, DustID.SolarFlare, vel, Scale: 2f);
                d.noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.Alive()) return;
            player.AddBuff(BuffType<HotSpringCDBuff>(), (int)DebuffDuration);
        }
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
