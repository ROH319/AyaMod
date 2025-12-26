using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using AyaMod.Helpers;
using Terraria.DataStructures;
using Terraria.Audio;
using AyaMod.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class SpiderCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 60;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<SpiderCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.05f, 132, 1.6f,0.5f);
            SetCaptureStats(1000, 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpiderFang, 12)
                .AddIngredient(ItemID.CursedFlame, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.SpiderFang, 12)
                .AddIngredient(ItemID.Ichor, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SpiderCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(82, 49, 44);
        public override Color innerFrameColor => new Color(96, 248, 2) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(239, 231, 184).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            EffectCounter++;
            bool specialShot = EffectCounter > 4;
            int type = ProjectileType<SpiderCursedFlame>();
            int damage = (int)(Projectile.damage * 0.4f);
            if (specialShot)
            {
                type = ProjectileType<SpiderWispShot>();
                damage = (int)(damage * 1.5f);
            }
            for (int i = -1; i < 2; i++)
            {
                float rot = MathHelper.PiOver2 + MathHelper.Pi / 3 / 2 * i;
                if (!specialShot) rot += Main.rand.NextFloat(-0.2f, 0.2f);

                Vector2 vel = rot.ToRotationVector2() * 6;
                if (specialShot)
                {
                    vel *= 5f;
                }

                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, type, damage, Projectile.knockBack / 4, Projectile.owner);
            }
            if (specialShot) EffectCounter = 0;
        }
    }

    public class SpiderCursedFlame : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 6);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 8 * 60;
            Projectile.SetImmune(15);
            //Projectile.aiStyle = 8;
            
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -1.2f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -0.9f;
            Projectile.penetrate -= 2;
            if (Projectile.penetrate <= 0) Projectile.Kill();
            return false;
        }

        public override void AI()
        {

            //Projectile.velocity.X *= 0.97f;
            float r = Projectile.light * 0.35f;
            float g = Projectile.light;
			Lighting.AddLight((int)((Projectile.position.X + (float)(Projectile.width / 2)) / 16f), (int)((Projectile.position.Y + (float)(Projectile.height / 2)) / 16f), r, g, 0f);

            Projectile.UseGravity(0.99f, 0.15f, 25);
            if(Projectile.timeLeft % 1 == 0)
            {
                Color color = new Color(96, 248, 2);

                //Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-Projectile.width / 2, Projectile.width / 2), Main.rand.NextFloat(-Projectile.height / 2, Projectile.height / 2));
                //var ball = SoulsParticle.Spawn(pos, Projectile.velocity, color, 0.5f, 1f);
                //ball.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.01f));
                //ball.SetScaleFadeout(new Core.FloatModifier().SetMultiplicative(0.9f));
                //ball.SetVelFadeout(new Core.FloatModifier().SetMultiplicative(0.8f));

                Vector2 pos = new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y);
                int d = Dust.NewDust(pos, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X, Projectile.velocity.Y, 100, default(Color), 2f * Projectile.scale);
                Main.dust[d].noGravity = true;
            }
            Projectile.rotation += 0.03f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Color color = new Color(179, 252, 0);

            for (int num704 = 0; num704 < 20; num704++)
            {
                //Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(4f);
                //Vector2 pos = Projectile.position + new Vector2(Main.rand.NextFloat(0, Projectile.width), Main.rand.NextFloat(0, Projectile.height));
                //var ball = SoulsParticle.Spawn(pos, vel, color, 0.5f, 1f);
                //ball.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.01f));
                //ball.SetScaleFadeout(new Core.FloatModifier().SetMultiplicative(0.9f));
                //ball.SetVelFadeout(new Core.FloatModifier().SetMultiplicative(0.9f));

                int num705 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 75, (0f - Projectile.velocity.X) * 0.2f, (0f - Projectile.velocity.Y) * 0.2f, 100, default(Color), 2f * Projectile.scale);
                Main.dust[num705].noGravity = true;
                Dust dust2 = Main.dust[num705];
                dust2.velocity *= 2f;
                num705 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 75, (0f - Projectile.velocity.X) * 0.2f, (0f - Projectile.velocity.Y) * 0.2f, 100, default(Color), 1f * Projectile.scale);
                dust2 = Main.dust[num705];
                dust2.velocity *= 2f;
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2;
            float alpha = Projectile.Opacity;
            Vector2 scale = new Vector2(0.8f) * Projectile.scale;

            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;

                float rot = i == 0 ? Projectile.rotation : Projectile.oldRot[i];
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color color = Color.White.AdditiveColor() * factor * 0.5f * Projectile.Opacity;
                Vector2 trailScale = scale * factor;
                Main.spriteBatch.Draw(texture, drawPos, null, color, rot, origin, trailScale, 0, 0);

            }
            RenderHelper.DrawBloom(6, 3f, texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.2f, Projectile.rotation, origin, scale);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, origin, scale, 0, 0);


            return false;
        }
    }

    public class SpiderWispShot : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(10);
            Projectile.timeLeft = 60;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            int width = (int)(Projectile.width * 1.2f);
            int length = (int)(80 * 1.2f);
            Vector2 top = Projectile.Center + Projectile.rotation.ToRotationVector2() * length / 2;
            Vector2 bottom = Projectile.Center - Projectile.rotation.ToRotationVector2() * length / 2;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), top, bottom, width, ref point);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            Projectile.velocity *= 0.75f;
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override void AI()
        {
            Projectile.velocity *= 0.93f;

            float factor = Projectile.TimeleftFactor();
            float fadein = Utils.Remap(factor, 0.85f, 1f, 1f, 0.1f);
            float opacityFactor = fadein * factor;
            if (factor > 0.1f)
            {
                //opacityFactor *= factor;
                Projectile.Opacity = opacityFactor;
            }
            else Projectile.Opacity -= 0.008f;
            if (Projectile.Opacity <= 0.008f) Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height);
            Vector2 baseScale = new Vector2(2f, 1.2f) * Projectile.scale;
            float alpha = Projectile.Opacity;
            for(int i = 0; i < 2; i++)
            {
                float rot = Projectile.rotation + i * MathHelper.Pi + MathHelper.PiOver2;


                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * alpha, rot, origin, baseScale, 0, 0);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * alpha * 0.5f, rot, origin, baseScale * 0.8f, 0, 0);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * alpha * 0.5f, rot, origin, baseScale * 0.7f, 0, 0);
            }

            return false;
        }
    }
}
