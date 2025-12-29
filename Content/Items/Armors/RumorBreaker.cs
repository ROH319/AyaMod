using AyaMod.Content.Buffs;
using AyaMod.Content.Projectiles.Auras;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
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
    [PlayerEffect(OverrideEffectName = RumorBreakerSet)]
    public class RumorBreakerHelmet : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public const string RumorBreakerSet = "RumorBreakerSet";
        public static LocalizedText RumorBreakerBonus { get; set; }

        public static int DamageBonus = 10;
        public static int CritBonus = 10;

        public override void Load()
        {
            AyaPlayer.DoubleTapHook += RumorBreakerKeyEffect;
            GlobalCamera.SnapHook += RumorBreakerSnap;
        }
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
            player.AddEffect(RumorBreakerSet);
            RumorBreakerSetEffect(player);
        }
        public static void RumorBreakerSetEffect(Player player)
        {
            int type = ProjectileType<PurifyAura>();
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, type, 0, 0f, player.whoAmI, 6 * 60);
        }
        public static void RumorBreakerKeyEffect(Player player)
        {
            if (!player.HasEffect(RumorBreakerSet)) return;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type != ProjectileType<PurifyAura>()) continue;
                (p.ModProjectile as PurifyAura).Release();
            }
        }

        public static void RumorBreakerSnap(BaseCameraProj projectile)
        {
            if (!projectile.player.HasEffect(RumorBreakerSet)) return;
            //每5次拍摄触发
            if (projectile.player.Camera().GlobalSnapCounter % 5 != 0) return;

            var aura = AuraFriendly.Spawn(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, 2 * 60, BuffType<PurifiedBuff>(), 
                3 * 60, 300f, new Color(255,247,170,128), new Color(255,255,81,156), projectile.player.whoAmI);
            aura.SetRadiusFadeout(1f, Common.Easer.Ease.Linear);
            aura.SetAlphaFadeout(0.6f, Common.Easer.Ease.OutSine);
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
    public class PurifyAura : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public ref float ChargeTime => ref Projectile.ai[0];
        public ref float Released => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10 * 60;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)ChargeTime;
        }
        public void Release() { Released = 2; }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.Alive()) return;
            if (!player.HasEffect(RumorBreakerHelmet.RumorBreakerSet)) Projectile.Kill();

            Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, 0.2f);

            float factor = Projectile.TimeleftFactor();

            Projectile.Opacity = Utils.Remap(factor, 0, 1f, 1f, 0f);
            if (Released < 1)
            {
                ApplySlow(factor);
                ApplyRegen(factor);
            }
            else
            {
                ReleaseEffect(player);
            }

            if (Projectile.timeLeft < 3 && Released < 1) Projectile.timeLeft++;
            Projectile.rotation += 0.02f;

        }
        public void ApplySlow(float factor)
        {
            float slowFactor = Utils.Remap(factor, 0.1f, 1f, 0.2f, 0f);
            foreach(var npc in Main.ActiveNPCs)
            {
                if (npc.Distance(Projectile.Center) > Projectile.width / 2f) continue;
                npc.Aya().SpeedModifier -= slowFactor;
            }
        }
        public void ApplyRegen(float factor)
        {
            float time = Utils.Remap(factor, 0.1f, 1f, PurifiedBuff.BuffTimeMax, PurifiedBuff.BuffTimeMin);
            foreach(var player in Main.ActivePlayers)
            {
                if (player.Distance(Projectile.Center) > Projectile.width / 2f) continue;
                player.AddBuff(BuffType<PurifiedBuff>(), (int)time);
            }
        }
        public void ReleaseEffect(Player player)
        {
            TryCleanDebuff(player);
            SpreadPurify();
            Projectile.Kill();
        }
        public void SpreadPurify()
        {
            float range = 800f;
            int duration = 6 * 60;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.Distance(Projectile.Center) > range) continue;

                npc.AddBuff(BuffType<PurifiedBuff>(), duration);
                int dustamount = 32;
                for(int i = 0; i < dustamount; i++)
                {
                    Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.YellowTorch, 0, Main.rand.NextFloat(-1, -4), Scale: 1.5f);
                    d.noGravity = true;
                }
            }
        }
        public static void TryCleanDebuff(Player player)
        {
            //TODO：代码来自Fargo魂（MIT），记得写README里
            int max = player.buffType.Length;

            for (int i = 0; i < max; i++)
            {
                int timeleft = player.buffTime[i];
                if (timeleft <= 0) continue;

                int buffType = player.buffType[i];
                if (buffType <= 0) continue;

                if(timeleft > 5 && Main.debuff[buffType] && 
                    !Main.buffNoTimeDisplay[buffType] && 
                    !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                {
                    player.DelBuff(i);
                    i--;
                    max--;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float scale = texture.Width / (float)Projectile.width * Projectile.scale;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, scale, 0);

            return false;
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
