using AyaMod.Content.Buffs;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(OverrideEffectName = RainforestVoiceSet)]
    public class RainforestVoiceHelmet : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public const string RainforestVoiceSet = "RainforestVoiceSet";
        public static LocalizedText RainforestVoiceBonus { get; set; }

        public static int DamageBonus = 16;
        public static int CritBonus = 8;

        public static int ExtraHeartDropChance = 10;
        public static int BossHeartDropChance = 1;

        public override void Load()
        {
            AyaPlayer.DoubleTapHook += RainforestKeyEffect;
            AyaGlobalProjectile.OnProjectileHitNPC += DropExtraHeart;
            GlobalCamera.SnapHook += SnapPickupHeart;
        }


        public static void DropExtraHeart(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            if (!player.HasBuff<RainforeseFlowerBuff>()) return;

            if (target.boss)
            {
                if (Main.rand.Next(100) < BossHeartDropChance)
                    Item.NewItem(player.GetSource_FromThis(), target.getRect(), ItemID.Heart);
            }
            else if (target.life <= 0)
            {
                if (Main.rand.Next(100) < ExtraHeartDropChance)
                    Item.NewItem(player.GetSource_FromThis(), target.getRect(), ItemID.Heart);
            }

        }
        public static void SnapPickupHeart(BaseCameraProj projectile)
        {
            Player player = projectile.player;
            if (!player.HasEffect(RainforestVoiceChestplate.SnapPickupHeartEffect)) return;
            foreach(var item in Main.ActiveItems)
            {
                if (item.type != ItemID.Heart || !(bool)projectile.Colliding(projectile.Projectile.getRect(), item.getRect())) 
                    continue;

                item.TurnToAir();
                player.Heal(20);
            }
        }
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
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<RainforestVoiceChestplate>() && legs.type == ItemType<RainforestVoicePants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = RainforestVoiceBonus.Value;
            player.AddEffect(RainforestVoiceSet);
            RainforestVoiceSetEffect(player);
        }
        public static void RainforestVoiceSetEffect(Player player)
        {
            int type = ProjectileType<RainforestHeart>();
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, type, 200, 6f, player.whoAmI);
        }

        public static void RainforestKeyEffect(Player player)
        {
            if (!player.HasEffect(RainforestVoiceSet)) return;

            player.GetModPlayer<RainforestPlayer>().ReverseState();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class RainforestHeart : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public ref float State => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.Alive()) return;
            if (!player.HasEffect(RainforestVoiceHelmet.RainforestVoiceSet)) Projectile.Kill();

            Vector2 idlePos = player.Center + new Vector2(0, -60f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePos, 0.5f);

            State = player.GetModPlayer<RainforestPlayer>().RainforestFlowerState ? 0 : 1;
            Projectile.timeLeft = 2;
            switch (State)
            {
                case 0:
                    {
                        player.AddBuff(BuffType<RainforeseFlowerBuff>(), 2);
                    }
                    break;
                case 1:
                    {
                        Lighting.AddLight(Projectile.Center, 0,1f,55/ 255f);
                        if (player.ownedProjectileCounts[ProjectileType<RainforestCameraProj>()] < 1)
                        {
                            Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<RainforestCameraProj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                        }
                    }
                    break;
                default:break;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            var spriteEffect = State == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition,
                null, Color.White, Projectile.rotation,
                texture.Size() / 2, Projectile.scale, spriteEffect, 0);

            return false;
        }
    }
    public class RainforestCameraProj : BaseCameraProjAuto
    {

        public override Color outerFrameColor => new Color(165, 228, 138);
        public override Color innerFrameColor => new Color(182, 196, 28) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(107, 203, 0).AdditiveColor() * 0.5f;

        public override void SetOtherDefault()
        {
            base.SetOtherDefault();
            SetCameraStats(160, 40, 400f, 1.3f);
        }
        public override bool PreAI()
        {
            if (player.dead || !player.active)
            {
                return false;
            }
            if (player.HasEffect(RainforestVoiceHelmet.RainforestVoiceSet) && !player.GetModPlayer<RainforestPlayer>().RainforestFlowerState)
                Projectile.timeLeft = 2;

            return base.PreAI();
        }
        public override NPC GetTargetIndex()
        {

            return player.FindCloestNPC(sightRange, true, false);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            return true;
        }
    }
    public class RainforestPlayer : ModPlayer
    {
        public int Overheal;
        public bool RainforestFlowerState;
        public static int MaxOverheal = 200;
        public override void Load()
        {
            On_Player.Heal += HackHeal;
        }
        public override void Unload()
        {
            On_Player.Heal -= HackHeal;
        }   
        public static void HackHeal(On_Player.orig_Heal orig, Player self, int amount)
        {
            if (self.HasBuff<RainforeseFlowerBuff>())
            {
                if(self.statLife + amount > self.statLifeMax2)
                {
                    self.GetModPlayer<RainforestPlayer>().Overheal += self.statLife + amount - self.statLifeMax2;
                }
            }
            orig(self, amount);
        }
        public void ReverseState()
        {
            RainforestFlowerState = !RainforestFlowerState;
        }
        public override void UpdateLifeRegen()
        {
            if(Overheal > 0 && Player.statLife < Player.statLifeMax2)
            {
                int regen = 10;
                Player.lifeRegen += regen;
                float regenEfficiency = 2;
                int consumeRate = (int)(120 / regen * regenEfficiency);
                if (Main.GameUpdateCount % consumeRate == 0)
                    Overheal--;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            //Main.NewText($"{Overheal} {Main.GameUpdateCount}");
            if(Overheal > MaxOverheal)
            {
                Overheal = MaxOverheal;
            }
        }
    }
    [AutoloadEquip(EquipType.Body)]
    [PlayerEffect(OverrideEffectName = SnapPickupHeartEffect)]
    public class RainforestVoiceChestplate : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public const string SnapPickupHeartEffect = "SnapPickupHeartEffect";

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
