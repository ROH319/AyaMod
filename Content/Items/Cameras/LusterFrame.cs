using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.Enums;
using AyaMod.Core.Prefabs;
using Terraria.ModLoader;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AyaMod.Content.Items.Cameras
{
    public class LusterFrame : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = Item.height = 32;

            Item.damage = 28;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<LusterFrameProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 86, 2f);
            SetCaptureStats(100, 5);

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 8)
                .AddIngredient(ItemID.Sapphire, 2)
                .AddIngredient(ItemID.Ruby, 2)
                .AddIngredient(ItemID.Emerald, 2)
                .AddIngredient(ItemID.Topaz, 2)
                .AddIngredient(ItemID.Amethyst, 2)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumBar, 8)
                .AddIngredient(ItemID.Sapphire, 2)
                .AddIngredient(ItemID.Ruby, 2)
                .AddIngredient(ItemID.Emerald, 2)
                .AddIngredient(ItemID.Topaz, 2)
                .AddIngredient(ItemID.Amethyst, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LusterFrameProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(127,247,202);
        public override Color innerFrameColor => new Color(255,240,189);
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(255,229,255);
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            int amount = 6;
            float rot = AyaUtils.RandAngle;
            Vector2 dir = rot.ToRotationVector2();
            float rotdir = Main.rand.NextBool() ? -1 : 1;
            for(int i = 0; i < amount; i++)
            {
                //Vector2 vel = dir.RotatedBy(MathHelper.TwoPi / amount * i) * 3;
                float velrot = Main.rand.NextFloat(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2);
                Vector2 vel = new Vector2(0, -Main.rand.NextFloat(6, 10)).RotatedBy(velrot);
                int offset = Main.rand.Next(2, 5)* i;
                Vector2 pos = Projectile.Center + dir.RotatedBy(MathHelper.TwoPi / amount * i) * Main.rand.NextFloat(20,40);
                float rotSpeed = 0.03f;
                rotSpeed = 0f;
                int gemtype = Main.rand.Next(5);
                var gem = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<LusterFrameGem>(), Projectile.damage / 5, Projectile.knockBack, Projectile.owner, offset, rotSpeed, gemtype);
            }
        }

        public override void OnClearProjectile(Projectile projectile)
        {
            if (Main.rand.NextBool(10))
            {

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LusterFrameGem : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaItemPath(ItemID.Sapphire);


        public Color gemcolor => Projectile.ai[2] switch
        {
            0 => Color.Blue,
            1 => Color.Red,
            2 => Color.Green,
            3 => Color.Yellow,
            4 => Color.Purple,
            _ => Color.White
        };

        public Color sparkleColor => Projectile.ai[2] switch
        {
            0 => new Color(20, 20, 255),
            1 => new Color(255, 20, 20),
            2 => new Color(20, 255, 20),
            3 => new Color(255, 255, 20),
            4 => new Color(255, 20, 255),
            _ => Color.White
        };

        public int dustType => Projectile.ai[2] switch
        {
            0 => DustID.GemSapphire,
            1 => DustID.GemRuby,
            2 => DustID.GemEmerald,
            3 => DustID.GemTopaz,
            4 => DustID.GemAmethyst,
            _ => DustID.GemDiamond
        };

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 15);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ArmorPenetration = 30;
            Projectile.Scale(1.5f);
            Projectile.localAI[2] = 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnDust();
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnDust();
            return base.OnTileCollide(oldVelocity);
        }

        public void SpawnDust()
        {
            int dustamount = 8;
            for (int i = 0; i < dustamount; i++)
            {
                Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(2f, 5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, vel);
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] < 15 + Projectile.ai[0])
            {
                float factor = Projectile.localAI[0] / (15 + Projectile.ai[0]);
                Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1]);
                Projectile.UseGravity(1f, 0.2f, 30);
                Projectile.localAI[1] = Utils.Remap(factor, 0, 1f, 0f, 1.3f);
            }
            else
            {
                
                Projectile.localAI[1] -= 0.04f;

                Projectile.UseGravity(0.97f, 0.6f, 30);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dir = Projectile.whoAmI %2 == 0 ? 1 : -1;
            Projectile.localAI[2] += 0.01f * dir;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int type = (int)(177 + Projectile.ai[2]);
            Texture2D texture = TextureAssets.Item[type].Value;

            Texture2D star = TextureAssets.Extra[98].Value;


            int length = Projectile.oldPos.Length;
            for(int i = 7; i < length - 1; i++)
            {
                float totalFactor = (float)i / length;

                var dir = Projectile.oldPos[i + 1] - Projectile.oldPos[i];
                dir = dir.SafeNormalize(Vector2.Zero);
                var ndir = dir.RotatedBy(MathHelper.PiOver2);
                if (dir != Vector2.Zero)
                {
                    Vector2 offset = ndir * 2;
                    float oldrot = Projectile.oldRot[i];

                    float alphaFactor = Utils.Remap(totalFactor, 0, 1f, 1f, 0);

                    Color color = gemcolor.AdditiveColor() * 0.2f * alphaFactor;
                    for(int j = -1; j < 2; j += 2)
                    {
                        Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(0, 6f * j).RotatedBy(oldrot) * Projectile.scale;
                        Rectangle sourceRect = new Rectangle(18 - 18 * j, 0, 36, 72);
                        Vector2 origin = new Vector2(18 + 18 * j, 36);
                        Main.spriteBatch.Draw(star, pos, sourceRect, color, oldrot + MathHelper.PiOver2, origin, Projectile.scale * new Vector2(0.5f,1f), 0, 0);
                    }
                }
            }


            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale, 0, 0);

            {
                Vector2 offset = (MathHelper.Pi + MathHelper.PiOver4).ToRotationVector2() * 8;
                Color sparklecolor = sparkleColor.AdditiveColor() * 0.7f;
                float sparkleRot = -Projectile.localAI[2];
                Main.spriteBatch.Draw(star, Projectile.Center + offset - Main.screenPosition, null, sparklecolor, sparkleRot, star.Size() / 2, new Vector2(0.5f, 1.2f) * Projectile.localAI[1], 0, 0);
                Main.spriteBatch.Draw(star, Projectile.Center + offset - Main.screenPosition, null, sparklecolor, sparkleRot + MathHelper.PiOver2, star.Size() / 2, new Vector2(0.5f,1.2f) * Projectile.localAI[1] * 0.7f, 0, 0);
            }

            return false;
        }
    }
}
