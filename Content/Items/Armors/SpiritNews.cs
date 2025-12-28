using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(OverrideEffectName = "SpiritNewsSet")]
    public class SpiritNewsHood : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public static LocalizedText SpiritNewsBonus { get; set; }
        public static int DamageBonus = 10;

        public static int SurroundMax = 12;
        public override void Load()
        {
            GlobalCamera.OnHitNPCHook += GlobalCamera_OnHitNPCHook;
        }

        public static void GlobalCamera_OnHitNPCHook(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            if (player == null || !player.HasEffect("SpiritNewsSet") || target.life > 0) return;

            Vector2 vel = Main.rand.NextVector2Unit() * 6f;
            int type = ProjectileType<EarthSpirits>();
            Projectile.NewProjectileDirect(target.GetSource_Death(), target.Center, vel, type, target.damage / 2, 0f, player.whoAmI);
        }

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
            player.AddEffect("SpiritNewsSet");
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class EarthSpirits : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 1 * 60;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.Alive()) return;

            int totalSpirits = player.ownedProjectileCounts[Type];
            int index = Projectile.GetMyGroupIndex();

            if (totalSpirits > SpiritNewsHood.SurroundMax && Projectile.ai[0] < 1)
            {
                int value = totalSpirits - SpiritNewsHood.SurroundMax;
                if (index < value)
                {
                    TransformToShot();
                }
            }

            int radius = 200;
            Vector2 offset = (MathHelper.TwoPi / totalSpirits * index + Main.GameUpdateCount * 0.01f).ToRotationVector2() * radius;
            Vector2 targetPos = player.Center + offset;

            float chaseFactor = Utils.Remap(Projectile.timeLeft, 2, 1 * 60, 0.15f, 0f);

            Projectile.velocity = Projectile.velocity * 0.99f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, chaseFactor);

            if (Projectile.timeLeft < 3 && Projectile.ai[0] < 1) Projectile.timeLeft++;

            int dustamount = 1;
            for(int i = 0; i < dustamount; i++)
            {
                Vector2 vel = -Vector2.UnitY.RotatedByRandom(0.3f) * 2;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenFairy, vel, Scale:1.5f);
                d.noGravity = true;
            }
        }
        public void TransformToShot()
        {
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ProjectileType<SpiritShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            Projectile.ai[0] = 2;
        }
    }
    public class SpiritShot : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 6 * 60;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Helper.GenDustRand(32, DustID.HallowSpray, Projectile.Center, 20, 40, 2, 5, scale: 1.5f, noGravity: true);
        }
        public override void AI()
        {
            Projectile.Chase(2500, 32, 0.06f);

            int dustamount = 1;
            for (int i = 0; i < dustamount; i++)
            {
                Vector2 vel = Projectile.velocity.Length(3);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.HallowSpray, vel, Scale: 1.5f);
                d.noGravity = true;
            }
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
