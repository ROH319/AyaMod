using AyaMod.Content.Items.Materials;
using AyaMod.Core;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class LilyWhiteHat : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public static LocalizedText LilyBonus {  get; set; }

        public override void SetStaticDefaults()
        {
            LilyBonus = this.GetLocalization("LilyBonus");
        }

        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(silver: 2));
            Item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<SakuraPetal>(), 10)
                .AddIngredient(ItemID.Silk, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<LilyWhiteDress>() && legs.type == ItemType<LilyWhiteBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LilyBonus.Value;
            LilySetEffect(player);
        }

        public static void LilySetEffect(Player player)
        {
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), TorchID.Pink, 0.5f);

            if (Main.GameUpdateCount % 4 == 0 && Main.rand.NextBool(6))
            {
                int petalDamage = 30;

                float x = Main.rand.NextFloat(75) * (Main.rand.NextBool() ? 1 : -1);
                float y = -Main.rand.NextFloat(30,80);
                Vector2 pos = player.Center + new Vector2(x, y);
                Vector2 vel = new Vector2(0, -1.5f).RotateRandom(0.5f) + new Vector2(player.velocity.X * 0.7f,0);
                //Vector2 vel = (player.Center - pos).RotateRandom(0.2f).Length(Main.rand.NextFloat(0.5f,2f));
                //vel.Y *= 1.5f;
                //vel += Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 2f);
                //vel.X = MathHelper.Clamp(vel.X, -3f, 3f);
                //if (vel.Y > 3f) vel.Y = 3;
                int type = ProjectileType<LilyPetal>();
                var petal = Projectile.NewProjectileDirect(player.GetSource_FromThis(), pos, vel, type, petalDamage, 2f,player.whoAmI);
            }
        }

        public static int DamageBonus = 8;
    }

    [AutoloadEquip(EquipType.Body)]
    public class LilyWhiteDress : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus);

        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(silver: 4));
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public static int CritBonus = 6;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<SakuraPetal>(), 15)
                .AddIngredient(ItemID.Silk, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class LilyWhiteBoots : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus);
        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(silver: 3));
            Item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeedBonus / 100f;
        }
        public static int MoveSpeedBonus = 5;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<SakuraPetal>(), 12)
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LilyPetal : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaProjPath(221);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 9 * 60;
            Projectile.scale = 1f + (float)Main.rand.Next(30) * 0.01f;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(3);
            Projectile.localAI[1] = Main.rand.Next(100);
            //Projectile.Opacity = 0f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.2f;
            Projectile.localAI[0] = 1;
            //if (Projectile.timeLeft > 60) Projectile.timeLeft = 60;
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 3; height = 3;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), TorchID.Pink, 0.5f * Projectile.Opacity);

            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                var vector = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                //vector = new Vector2(vector.X, MathF.Abs(vector.Y));
                //Projectile.position += vector * 0.5f;
            }
            float factor = Projectile.TimeleftFactor();

            float alphafadein = Utils.Remap(factor, 0.9f, 1f, 1f, 0f);
            float alphafadeout = Utils.Remap(factor, 0, 0.1f, 0, 1f);
            Projectile.Opacity = alphafadein * alphafadeout;

            if (Projectile.localAI[0] < 1)
            {
                Projectile.localAI[2] = (MathF.Sin(Main.GameUpdateCount * 0.05f + Projectile.localAI[1])) * 0.8f;
                Projectile.localAI[2] += MathF.Sign(Projectile.localAI[2]) * 0.2f;
            }
            float grav = 0.035f;
            if (factor > 0.9f) grav *= 3f;
            float gravMult = MathF.Abs(MathF.Sin(Main.GameUpdateCount * 0.02f + Projectile.localAI[1] - MathHelper.PiOver2)) * 0.4f;
            Projectile.UseGravity(0.998f, grav * gravMult, 10);
            if (Projectile.localAI[0] < 1) Projectile.position.X += MathF.Sin(Main.GameUpdateCount * 0.02f + Projectile.localAI[1]) * 1.7f;
            //Main.NewText($"{Projectile.localAI[2]}");
            if (Main.rand.NextBool(5))
            {

                Vector2 vel = Projectile.velocity;
                //Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 297,
                //    vel.X,vel.Y);
                
            }

            if (Projectile.localAI[0] > 0)
            {
                //Projectile.localAI[0] = 0;
                //Projectile.localAI[2] = 1;
            }
            else
                Projectile.rotation += 0.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Type];
            int y = frameHeight * Projectile.frame;

            SpriteEffects se = SpriteEffects.None;
            if (Projectile.localAI[2] < 0) se = SpriteEffects.FlipVertically;

            Rectangle rect = new Rectangle(0, y, 20, 20);
            Color color = Color.White * Projectile.Opacity;
            Vector2 scale = new Vector2(1, MathF.Abs(Projectile.localAI[2])) * Projectile.scale;

            RenderHelper.DrawBloom(8, 3, texture, Projectile.Center - Main.screenPosition, rect, color.AdditiveColor() * 0.2f, Projectile.rotation, new Vector2(10, 10), scale, se);


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, 
                color, Projectile.rotation, new Vector2(10,10), scale, se);

            return false;
        }
    }
}
