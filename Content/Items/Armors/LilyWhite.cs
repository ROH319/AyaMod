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
    public class LilyWhiteHat : ModItem
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
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1));
            Item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += (float)DamageBonus / 100f;
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

            if (Main.GameUpdateCount % 4 == 0 && Main.rand.NextBool(4))
            {
                int petalDamage = 30;

                float x = Main.rand.NextFloat(75) * (Main.rand.NextBool() ? 1 : -1);
                float y = -Main.rand.NextFloat(50,100);
                Vector2 pos = player.Center + new Vector2(x, y);
                Vector2 vel = new Vector2(0, -3).RotateRandom(0.5f);
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
    public class LilyWhiteDress : ModItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus);

        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1));
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public static int CritBonus = 6;
    }

    [AutoloadEquip(EquipType.Legs)]
    public class LilyWhiteBoots : ModItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1));
            Item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
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
            Projectile.timeLeft = 8 * 60;
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
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), TorchID.Pink, 0.5f * Projectile.Opacity);

            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                var vector = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                //vector = new Vector2(vector.X, MathF.Abs(vector.Y));
                Projectile.position += vector * 0.5f;
            }
            float factor = Projectile.TimeleftFactor();

            float alphafadein = Utils.Remap(factor, 0.9f, 1f, 1f, 0f);
            Projectile.Opacity = alphafadein;

            Projectile.localAI[2] = MathF.Sin(Main.GameUpdateCount * 0.05f + Projectile.localAI[1]);
            Projectile.UseGravity(0.998f, 0.035f, 10);

            if(Main.rand.NextBool(5))
            {

                Vector2 vel = Projectile.velocity;
                //Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 297,
                //    vel.X,vel.Y);
                
            }

            if (Projectile.localAI[0] > 0)
            {
                Projectile.localAI[0] = 0;
                Projectile.localAI[2] = 1;
            }
            else
                Projectile.rotation += 0.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Type];
            int y = frameHeight * Projectile.frame;

            Rectangle rect = new Rectangle(0, y, Projectile.width, Projectile.height);
            Color color = Color.White * Projectile.Opacity;
            Vector2 scale = new Vector2(1, Projectile.localAI[2]) * Projectile.scale;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, 
                color, Projectile.rotation, new Vector2(10,10), scale, 0);

            return false;
        }
    }
}
