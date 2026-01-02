using AyaMod.Content.Items.Materials;
using AyaMod.Content.Projectiles;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(OverrideEffectName = WindDriverSet)]
    public class WindDriverHeadgear : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public const string WindDriverSet = "WindDriverSet";
        public static LocalizedText WindDriverBonus { get; set; }

        public static int DamageBonus = 8;
        public static int AttackDamageBonus = 16;
        public static int EnduranceBonus = 15;
        public static int RegenBonus = 6;

        public override void Load()
        {
            AyaPlayer.DoubleTapHook += WindDriverKeyEffect;
            AyaPlayer.OnHitByBothHook += OnHitFeather;
            GlobalCamera.PostAIHook += MapleLeafShower;
            GlobalCamera.SnapHook += SnapMapleLeaf;
        }

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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddIngredient<MapleLeaf>(20)
                .Register();
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetModPlayer<CameraPlayer>().ChaseSpeedModifier += 0.15f;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<WindDriverHaori>() && legs.type == ItemType<WindDriverLeggings>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = WindDriverBonus.Value;
            player.AddEffect(WindDriverSet);
            WindDriverSetEffect(player);
        }
        public static void WindDriverSetEffect(Player player)
        {
            if (!player.HasEffect(WindDriverSet)) return;
            WindDriverPlayer modPlayer = player.GetModPlayer<WindDriverPlayer>();
            if (modPlayer.AttackState)
            {
                player.GetDamage<ReporterDamage>() += AttackDamageBonus / 100f;
            }
            else
            {
                player.Aya().SourceDamageReduce += EnduranceBonus / 100f;
                player.lifeRegen += RegenBonus;
            }
        }
        public static void WindDriverKeyEffect(Player player)
        {
            if (!player.HasEffect(WindDriverSet)) return;
            player.GetModPlayer<WindDriverPlayer>().ReverseState();
        }
        public static void SpawnMapleLeafFromHeaven(Player player, Vector2 projectileCenter, IEntitySource source, int damage, float randOffset = 70, float speed = 32f, float chaseFactor = 0.1f)
        {

            float dir = (projectileCenter.X < player.Center.X).ToDirectionInt();
            Vector2 pos = player.Center + new Vector2(Main.screenWidth * 0.5f * 0.75f * dir, -Main.screenHeight - 200f)
                + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, randOffset);
            Vector2 vel = pos.DirectionToSafe(player.Center).RotatedBy(MathHelper.PiOver4 * -dir) * speed;
            int type = ProjectileType<WindDriverMapleLeaf>();
            float targetPosX = projectileCenter.X;
            float targetPosY = projectileCenter.Y;
            var p = Projectile.NewProjectileDirect(source, pos, vel, type, damage, 0f, player.whoAmI, targetPosX, targetPosY, chaseFactor);
        }
        public static void MapleLeafShower(Player player, Core.Prefabs.BaseCameraProj projectile)
        {
            if (!player.HasEffect(WindDriverSet)) return;
            if (!player.ItemTimeIsZero && Main.GameUpdateCount % 15 == 0)
            {
                SpawnMapleLeafFromHeaven(player, projectile.Projectile.Center, projectile.Projectile.GetSource_FromAI(), 60, 70, 32, 0.1f);
            }
        }

        public static void OnHitFeather(Player player, ref Player.HurtInfo hurtInfo)
        {
            if (!player.HasEffect(WindDriverSet) || player.GetModPlayer<WindDriverPlayer>().AttackState) return;
            if (hurtInfo.Damage < 50) return;

            int maxFeather = 12;
            float speed = 16;
            int type = ProjectileType<WindDriverFeather>();
            int damage = hurtInfo.Damage;
            float knockBack = hurtInfo.Knockback;
            for (int i = 0; i < maxFeather; i++)
            {
                Vector2 vel = (Main.GameUpdateCount + MathHelper.TwoPi / maxFeather * i).ToRotationVector2() * speed;
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, vel, type, damage, knockBack, player.whoAmI);
            }
        }
        public static void SnapMapleLeaf(Core.Prefabs.BaseCameraProj projectile)
        {
            var player = projectile.player;
            if (!player.HasEffect(WindDriverSet) || !player.GetModPlayer<WindDriverPlayer>().AttackState) return;
            SpawnMapleLeafFromHeaven(player, projectile.Projectile.Center, projectile.Projectile.GetSource_FromAI(), 60);
        }
        public static void DashMapleLeaf(Player player)
        {
            if (!player.HasEffect(WindDriverSet)) return;
            if (!player.GetModPlayer<WindDriverPlayer>().AttackState) return;
            int type = ProjectileType<WindDriverMapleHoming>();
            int dmg = 40;
            for (int i = -1;i < 2; i++)
            {
                Vector2 vel = player.velocity.RotatedBy(MathHelper.PiOver4 * i + Main.rand.NextFloat(-0.5f,0.5f));
                vel *= Main.rand.NextFloat(0.8f, 1.5f);
                Projectile p = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, vel, type, dmg, 0f, player.whoAmI);
            }
        }
    }

    public class WindDriverPlayer: ModPlayer
    {
        public bool AttackState;
        public int DashDelayLaseFrame;
        public void ReverseState()
        {
            AttackState = !AttackState;
        }
        public override void PostUpdate()
        {
            if(Player.dashDelay == -1 && DashDelayLaseFrame != -1)
            {

                WindDriverHeadgear.DashMapleLeaf(Player);
            }
            //Main.NewText($"{Player.dash} {Player.dashDelay} {DashDelayLaseFrame}");
            DashDelayLaseFrame = Player.dashDelay;
        }
    }
    /// <summary>
    /// ai0：目标点X<br></br>
    /// ai1：目标点Y<br></br>
    /// ai2：追击系数
    /// </summary>
    public class WindDriverMapleLeaf : MapleLeafProjectile
    {
        public ref float TargetPosX => ref Projectile.ai[0];
        public ref float TargetPosY => ref Projectile.ai[1];
        public ref float ChaseFactor => ref Projectile.ai[2];
        public override void SetOtherDefaults()
        {
            Projectile.penetrate = 2;
            Projectile.SetImmune(20);
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.ArmorPenetration = 15;
        }
        public override void AI()
        {


            Vector2 targetPos = new Vector2(TargetPosX, TargetPosY);
            float targetR = (targetPos - Projectile.Center).ToRotation();
            float selfR = Projectile.velocity.ToRotation();
            float dif = MathHelper.WrapAngle(targetR - selfR);
            float chaseFactor = ChaseFactor;
            if (Projectile.timeLeft > 240)
                chaseFactor /= 3f;
            float r = selfR + dif * chaseFactor;

            if (Projectile.Distance(targetPos) < 200f && Projectile.velocity.Length() > 8f)
                Projectile.velocity *= 0.99f;

            Projectile.velocity = Projectile.velocity.Length() * r.ToRotationVector2();

            base.AI();

        }
    }
    public class WindDriverMapleHoming : MapleLeafProjectile
    {
        public override void SetOtherDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.SetImmune(20);
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.ArmorPenetration = 15;
        }
        public override void AI()
        {
            Projectile.Chase(3000f, 27f, 0.03f);
            base.AI();
        }
    }
    public class WindDriverFeather : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float StickToNPC => ref Projectile.localAI[0];
        public ref float ExplodeTimer => ref Projectile.localAI[1];
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 18);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.timeLeft = 4 * 60;
        }
        public override void OnSpawn(IEntitySource source)
        {
            StickToNPC = -1;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 8;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            if (ExplodeTimer <= 0) ExplodeTimer = 60;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (StickToNPC < 0)
            {
                StickToNPC = target.whoAmI;
                if (ExplodeTimer <= 0) ExplodeTimer = 60;
            }
        }
        public override void AI()
        {
            foreach (var p in Main.ActiveProjectiles)
            {
                if (!p.hostile || (p.ModProjectile != null && p.ModProjectile.CanDamage() == false)) continue;
                if (!p.Colliding(p.getRect(), Projectile.getRect())) continue;
                p.Kill();
            }
            if (ExplodeTimer > 0) ExplodeTimer--;
            if (ExplodeTimer == 1)
            {
                Projectile.Scale(3f, false);
                Projectile.Damage();

                int dustamount = 32;
                for (int i = 0; i < dustamount; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Confetti_Blue);

                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[ItemID.GiantHarpyFeather].Value;

            int length = Projectile.oldPos.Length;
            for (int i = 0; i < length; i++)
            {
                float factor = (float)i / length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                float rot = Projectile.oldRot[i] + MathHelper.PiOver2;
                float alpha = Utils.Remap(factor, 0, 1, 1, 0);
                Color color = Color.White * alpha * Projectile.Opacity;
                Main.EntitySpriteDraw(texture, pos, null, color, rot, texture.Size() / 2, 1f, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2, 1f, 0);
            return false;
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ectoplasm, 14)
                .AddIngredient<MapleLeaf>(20)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    [PlayerEffect(OverrideEffectName = WindDriverBuffReduce)]
    public class WindDriverLeggings : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus, MoveSpeedBonus);
        public const string WindDriverBuffReduce = "WindDriverBuffReduce";
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
            player.AddEffect(WindDriverBuffReduce);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ectoplasm, 10)
                .AddIngredient<MapleLeaf>(20)
                .Register();
        }
    }
    public class WindDriverBuff : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (!player.HasEffect(WindDriverLeggings.WindDriverBuffReduce)) return;
            if (!Main.debuff[type]) return;
            if (player.velocity.X != 0f) return;

            if (Main.rand.NextBool())
                player.buffTime[buffIndex]--; 
        }
    }
}
