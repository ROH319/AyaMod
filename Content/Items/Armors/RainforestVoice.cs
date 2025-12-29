using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
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

        public override void Load()
        {
            AyaPlayer.DoubleTapHook += RainforestKeyEffect;
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
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, type, 0, 0f, player.whoAmI);
        }

        public static void RainforestKeyEffect(Player player)
        {
            if (!player.HasEffect(RainforestVoiceSet)) return;


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
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.Alive()) return;
            if (!player.HasEffect(RainforestVoiceHelmet.RainforestVoiceSet)) Projectile.Kill();

            Vector2 idlePos = player.Center + new Vector2(0, -60f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePos, 0.5f);

            switch (State)
            {
                case 0:
                    {

                    }
                    break;
                case 1:
                    {

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
