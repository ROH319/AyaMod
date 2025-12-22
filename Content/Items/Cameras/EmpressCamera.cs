using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using static AyaMod.Core.ModPlayers.AyaPlayer;

namespace AyaMod.Content.Items.Cameras
{
    public class EmpressCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {

            Item.width = 52;
            Item.height = 48;

            Item.damage = 200;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<EmpressCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(0, 5, 0, 0));
            SetCameraStats(0.07f, 182, 1.5f);
            SetCaptureStats(1000, 60);
        }
    }

    public class EmpressCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => Main.DiscoColor;
        public override Color innerFrameColor => Main.hslToRgb((float)(Main.timeForVisualEffects * 0.003f) % 1f, 1f, 0.6f);
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(243, 209, 255).AdditiveColor() * 0.5f;
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;


            float extraRot = Main.rand.NextFloat(0, MathHelper.TwoPi);

            float start = Main.rand.NextFloat(0, 1f);

            EffectCounter++;

            float speed = 8;
            int starDmg = (int)(Projectile.damage * 0.16f);
            int starCount = 5;
            int hasLightBall = 1;
            float rotSpeed = MathHelper.PiOver2 / 60f * (Main.rand.NextBool() ? -1 : 1);
            if (EffectCounter >= 5) 
            {
                starCount *= 2;
                hasLightBall = 0;
                //starDmg = (int)(starDmg * 1.5f);
            }
            for (int i = 0; i < starCount; i++)
            {
                float color = start + 1f / starCount * i;
                float rot = extraRot + MathHelper.TwoPi / starCount * i;
                float speedMult = 1f;
                if (hasLightBall == 0 && i % 2 == 0) speedMult = 0.8f;
                Vector2 vel = rot.ToRotationVector2() * speed * speedMult;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<EmpressStar>(),
                    starDmg, Projectile.knockBack, Projectile.owner, rotSpeed, color);
            }


            if (EffectCounter >= 5)
            {
                int laserDmg = (int)(Projectile.damage * 0.25f);
                int maxcount = 5;
                for (int i = 0; i < 5; i++)
                {
                    float color = start + 0.2f * i;
                    float rot = Projectile.rotation + MathHelper.TwoPi / 5 * i + extraRot;
                    Vector2 pos = Projectile.Center + rot.ToRotationVector2() * 90;

                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<EmpressLaser>(), laserDmg, Projectile.knockBack, Projectile.owner,
                        rot + MathHelper.Pi + MathHelper.PiOver4 / 2, color, maxcount);
                }
                EffectCounter = 0;
            }

        }
    }

    public class EmpressLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_197";

        public ref float Rot => ref Projectile.ai[0];
        public ref float Hue => ref Projectile.ai[1];
        public ref float Count => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.SetImmune(6);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            float width = 10;
            float height = 2400;
            Vector2 endPoint = Projectile.Center;
            Vector2 startPoint = Projectile.Center + Rot.ToRotationVector2() * height;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, endPoint, width, ref point);
        }

        public override void AI()
        {
            if (Timer == 0)
            {
                Helper.PlayPitched("se_lazer01", 0.07f, position: Projectile.Center);
            }
            Timer++;

            float factor = Projectile.TimeleftFactor();
            Rot -= 0.03f * factor;
            Projectile.velocity = (Rot + MathHelper.Pi).ToRotationVector2() * 2f;

            int spawnNextTime = 10;

            float distNext = 60;
            float rotAdd = MathHelper.PiOver4 / 6;
            if (Timer == spawnNextTime && Count > 0)
            {
                Vector2 pos = Projectile.Center + (Rot + MathHelper.Pi - rotAdd).ToRotationVector2() * distNext;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, Projectile.type,
                    Projectile.damage, Projectile.knockBack, Projectile.owner, Rot - rotAdd, Hue, Count - 1);


                RingParticle.Spawn(Projectile.GetSource_FromAI(), pos, (Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.6f) * Projectile.Opacity).AdditiveColor(), 10, 40, 0.8f, 0f,
                            0.15f, 0.5f, 30, 120, Ease.OutCirc, Ease.OutCubic);

            }
            Projectile.rotation = Rot;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            Color drawColor = (Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.6f) * Projectile.Opacity).AdditiveColor();
            float timefactor = Projectile.TimeleftFactor();

            float timeFadein = Utils.Remap(timefactor, 0, 0.5f, 0, 1f);
            float timeFadeout = Utils.Remap(timefactor, 0.5f, 1f, 1f, 0f);
            float width = 15 * timeFadein * timeFadeout;

            int length = 2200;
            int count = length / 22;
            for(int i = 0; i < count; i++)
            {
                float factor = (float)i / count;//factor 0为轨迹头部， 1为轨迹尾部
                float x = length * factor;
                Vector2 pos = Projectile.Center + Rot.ToRotationVector2() * x;

                Color color = Color.Lerp(drawColor, Color.Black, 0);
                var alpha = /*Utils.Remap(factor, 0, 1f, 1f, 0)*/1f * Projectile.Opacity;
                float fadeinFactor = Utils.Remap(factor, 0.001f, 0, 1, 0f);
                alpha *= fadeinFactor;

                var normalDir = (Rot + MathHelper.PiOver2).ToRotationVector2();

                Vector2 top = pos - Main.screenPosition + normalDir * width;
                Vector2 bottom = pos - Main.screenPosition - normalDir * width;
                bars.Add(new CustomVertexInfo(top, color * alpha, new Vector3(factor, 1, alpha)));
                bars.Add(new CustomVertexInfo(bottom, color * alpha, new Vector3(factor, 0, alpha)));
            }
            for (int i = 0; i < 2; i++)
            {
                if (bars.Count > 2)
                {
                    Main.graphics.GraphicsDevice.Textures[0] = texture;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }
            }
            {
                Texture2D ball1 = Request<Texture2D>(AssetDirectory.Extras + "Ball", AssetRequestMode.ImmediateLoad).Value;
                Color ballColor = (Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.8f) * Projectile.Opacity).AdditiveColor();
                Vector2 pos = Projectile.Center - Main.screenPosition;
                for (int j = 0; j < 5; j++)
                {
                    Main.spriteBatch.Draw(ball1, pos, null, ballColor * timeFadein * timeFadeout, Main.GameUpdateCount * 0.1f, ball1.Size() / 2, Projectile.scale * (0.4f + j * 0.1f) * 0.6f, 0, 0);

                }

                Texture2D star = TextureAssets.Projectile[98].Value;
                float starrot1 = Main.GameUpdateCount * 0.07f + 0.5f;
                float starrot2 = -Main.GameUpdateCount * 0.02f + 0.1f;
                float scaleFactor = Utils.Remap(timefactor, 0, 0.5f, 0.5f, 1f) * Utils.Remap(timefactor,0.5f,1f,1f,0.5f);
                Vector2 extraScale = new Vector2(0.5f, 1) * scaleFactor * 1.4f;
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, ballColor * timeFadein * timeFadeout * 0.3f, starrot1, star.Size() / 2, Projectile.scale * extraScale, 0, 0);
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, ballColor * timeFadein * timeFadeout * 0.3f, starrot2, star.Size() / 2, Projectile.scale * extraScale, 0, 0);

                Main.spriteBatch.Draw(texture, Projectile.Center + Rot.ToRotationVector2() * 0.5f * length - Main.screenPosition, null, Color.White.AdditiveColor(), Rot + MathHelper.PiOver2, new Vector2(0, 128), new Vector2(width / 256f * 0.2f, length / 256f) * Projectile.scale, 0, 0);
                Main.spriteBatch.Draw(texture, Projectile.Center + Rot.ToRotationVector2() * 0.5f * length - Main.screenPosition, null, Color.White.AdditiveColor(), Rot + MathHelper.PiOver2, new Vector2(0, 128), new Vector2(width / 256f * 0.1f, length / 256f) * Projectile.scale, 0, 0);
            }
            return false;
        }
    }

    public class EmpressStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "bulletBa005";
        public ref float RotSpeed => ref Projectile.ai[0];
        public ref float Hue => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 5);
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.SetImmune(-1);
            Projectile.timeLeft = 60 * 5;
            Projectile.penetrate = 1;
        }
        public override bool? CanDamage()
        {
            return Projectile.TimeleftFactor() < 0.8f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //for(int i = 0; i < 15; i++)
            //{
            //    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(60);
            //    float speedFactor = 0.3f;
            //    var particle = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, Projectile.velocity.RotatedByRandom(0.3f) * speedFactor, Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.8f), 20);
            //    particle.VelMult = 0.9f;
            //}
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            int offset = (int)MathF.Sin(Projectile.whoAmI / 10);

            if(Projectile.timeLeft < 60 * 4 - offset * 160)
            {
                Projectile.Chase(2000, 28, 0.04f);
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(RotSpeed);

            }

            if (Main.GameUpdateCount % 1 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(40);
                float speedFactor = 0.4f;
                if (Projectile.timeLeft < 60 * 4 + offset) speedFactor /= 2f;
                LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, Projectile.velocity * speedFactor, Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.8f), 30);
            }

            //if(HasLightBall > 0 && (Projectile.timeLeft + Projectile.whoAmI) % 16 == 0)
            //{
            //    Vector2 vel = Projectile.velocity.RotateRandom(0.1f) * 0.1f;
            //    int damage = (int)(Projectile.damage * 0.5f);
            //    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel,
            //        ProjectileType<EmpressLight>(), damage, Projectile.knockBack, Projectile.owner, 0, Hue);
            //}

            Projectile.rotation += 0.04f;

        }

        public override void OnKill(int timeLeft)
        {

            int dustamount = 60;
            float startRot = Projectile.rotation + MathHelper.TwoPi / 10 + AyaUtils.RandAngle;
            float length = 20;
            for (int i = 0; i < dustamount; i++)
            {
                float factor = (float)i / dustamount;
                float rot = MathHelper.TwoPi * factor + startRot;
                Vector2 dir = rot.ToRotationVector2();
                float radius = length * 1f + MathF.Sin(factor * MathHelper.TwoPi * 5) * length * 0.4f;
                Vector2 pos = Projectile.Center + dir * radius;
                Vector2 vel = (pos - Projectile.Center).Length(1.3f).RotatedByRandom(0.1f);

                var particle = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel + Projectile.velocity * 0.15f, Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.8f), Main.rand.Next(15,25));
                particle.Scale = Main.rand.NextFloat(1,1.5f);
                particle.SetVelMult(.92f);
                //Dust d = Dust.NewDustPerfect(pos, dusttype, vel, Scale: 1.5f);
                //d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D ball1 = Request<Texture2D>(AssetDirectory.Extras + "Ball", AssetRequestMode.ImmediateLoad).Value;

            float timeleftFactor = Projectile.TimeleftFactor();


            Color color = (Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.7f) * Projectile.Opacity).AdditiveColor() * timeleftFactor * 0.8f;

            Vector2 scale = new Vector2(0.8f, 1.2f) * 0.3f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - (float)i / Projectile.oldPos.Length;

                DrawStar(Projectile, texture, Projectile.oldPos[i] + Projectile.Size / 2, color, factor * 0.4f, scale);

            }
            DrawStar(Projectile, texture, Projectile.Center, color, 0.8f, scale);


            DrawStar(Projectile, texture, Projectile.Center, color, 0.8f, scale);

            Color ballColor = (Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.8f) * Projectile.Opacity).AdditiveColor() * timeleftFactor * 1.2f;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            for (int j = 0; j < 5; j++)
            {
                Main.spriteBatch.Draw(ball1, pos, null, ballColor, Main.GameUpdateCount * 0.1f, ball1.Size() / 2, Projectile.scale * (0.4f + j * 0.2f), 0, 0);

            }
            return false;
        }
        public static void DrawStar(Projectile projectile, Texture2D texture, Vector2 center, Color color, float alpha, Vector2 scale, float dist = 18, float extraRot = 0)
        {
            color = color.AdditiveColor() * alpha * projectile.Opacity;
            Vector2 drawScale = scale * projectile.scale;
            for (int i = 0; i < 5; i++)
            {
                float rot = MathHelper.TwoPi * (float)i / 5 + projectile.rotation + extraRot;
                Vector2 pos = center + rot.ToRotationVector2() * dist - Main.screenPosition;
                Main.spriteBatch.Draw(texture, pos, null, color, rot, texture.Size() / 2, drawScale, 0, 0);

            }
        }
    }

    public class EmpressLight : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Ball";
        public ref float Hue => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(8);
            Projectile.timeLeft = 48;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 45;
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            Projectile.rotation += 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float factor = Projectile.TimeleftFactor();
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color color = (Main.hslToRgb((Hue + 0.5f) % 1f, 1f, 0.8f) * Projectile.Opacity).AdditiveColor() * factor;
            for (int i = 1; i < 6; i++)
            {
                float scalefac = Utils.Remap(i, 1, 6f, 1.5f, 0f);
                float colorfac = 1 / 5f * i;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * colorfac * 0.4f, Projectile.rotation, texture.Size() / 2, Projectile.scale * (scalefac) * 0.9f, 0, 0);

            }
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.1f, 0, 0);
            return false;
        }
    }
}
