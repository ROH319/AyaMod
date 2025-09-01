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
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using AyaMod.Content.Particles;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class ColdSnap : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 36;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<ColdSnapProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 80, 2f);
            SetCaptureStats(100, 5);
        }
    }

    public class ColdSnapProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(0, 255, 255);
        public override Color innerFrameColor => new Color(137, 255, 255) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(203, 255, 255).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {


            if (++EffectCounter >= 4)
            {
                EffectCounter = 0;

                if (!Projectile.MyClient()) return;

                for (int i = 0; i < 4; i ++) 
                {
                    float rot = (player.AngleToSafe(Projectile.Center) + i * MathHelper.PiOver2 + MathHelper.PiOver4);
                    Vector2 dir = rot.ToRotationVector2();
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + dir * 20, dir * Main.rand.NextFloat(1, 3), ModContent.ProjectileType<ColdMist>(), (int)(Projectile.damage * 0.8f), 0, Projectile.owner, 7);
                }
            }
        }
    }

    public class ColdMist : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Mist";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.timeLeft = 120;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.7f;
            Projectile.localAI[0] = Projectile.velocity.ToRotation();
            Projectile.localAI[1] = Projectile.velocity.Length();
            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            base.OnSpawn(source);
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            Projectile.Opacity = Utils.Remap(factor, 0.8f, 1f, 1f, 0);

            float vel = Utils.Remap(factor, 0.5f, 1f, 0, 1f);
            Projectile.velocity = new Vector2(Projectile.localAI[1] * vel, 0).RotatedBy(Projectile.localAI[0]);
            if (factor < 0.5f && Projectile.ai[2] <= 0)
            {
                //if (Projectile.ai[0] > 13)
                {
                    SoundEngine.PlaySound(SoundID.Item27 with 
                    {
                        MaxInstances = 30 ,
                        Volume = 0.7f,
                         Pitch = -0.8f
                    }, Projectile.Center);
                }
                Projectile.ai[2] = 2;
            }
            

            if (Projectile.localAI[2] > 0 && --Projectile.ai[0] > 0)
            {
                Vector2 dir = Projectile.velocity.Length(1);
                Vector2 ndir = dir.RotatedBy(MathHelper.PiOver2);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + dir * 60 * Projectile.scale + ndir * Main.rand.NextFloat(-4,4), Projectile.velocity, Projectile.type, Projectile.damage, 0, Projectile.owner, Projectile.ai[0]);
                Projectile.localAI[2] = int.MinValue;
            }
            Projectile.localAI[2]++;
            base.AI();
        }

        public override void OnKill(int timeLeft)
        {
            int dustamount = 30;
            for(int i = 0;i<dustamount;i++)
            {
                Vector2 pos = Projectile.Center + AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(0,60 * Projectile.scale);
                Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(1, 6);

                Dust d = Dust.NewDustPerfect(pos,DustID.IceGolem, vel,Scale:1.5f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item27 with 
            { 
                MaxInstances = 30,
                Volume = 0.5f
            }, Projectile.Center);

            for(int i = -1; i < 2; i+=2)
            {
                Vector2 dir = Projectile.localAI[0].ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i);
                Vector2 pos = Projectile.Center + dir.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(10, 80) * Projectile.scale;
                Vector2 vel = Projectile.localAI[0].ToRotationVector2().RotatedBy(MathHelper.Pi) * Main.rand.NextFloat(0.5f,1.8f) * 4;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<ColdBlade>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner, Main.rand.Next(5));
            }

            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D ball = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball", AssetRequestMode.ImmediateLoad).Value;

            float factor = Projectile.TimeleftFactor();
            float extraRot = Utils.Remap(factor, 0.5f, 1f, 0, MathHelper.PiOver2);
            float scale = Projectile.scale * 0.5f;
            {
                float Rot = Projectile.localAI[1] + extraRot;
                Color color = Color.White.AdditiveColor() * 0.4f * Projectile.Opacity;
                int drawcount = 20;
                float radius = 40 * Projectile.scale;
                for (int i = 0; i < drawcount; i++)
                {
                    Vector2 offset = (MathHelper.TwoPi / drawcount * i).ToRotationVector2() * radius;
                    Color color1 = color * (4f / drawcount);
                    if (factor < 0.5f) color1 *= 0.7f;
                    Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, color1, Projectile.rotation + Rot, texture.Size() / 2, scale, 0, 0);

                }
                for (int i = 0; i < drawcount / 5; i++)
                {
                    Vector2 offset = (MathHelper.TwoPi / drawcount * 5 * i).ToRotationVector2() * radius / 3;
                    Color color1 = color * (2f / drawcount * 5);

                    Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, color1, Projectile.rotation + Rot, texture.Size() / 2, scale, 0, 0);
                }
            }
            if (factor < 0.5f)
            {
                int trail = 5;
                for(int k = 0;k<trail;k++)
                {
                    float trailFactor = (float)k / trail;
                    Color color = Color.White.AdditiveColor() * 0.9f * Utils.Remap(trailFactor,0,1f,1f,0f);
                    float scale2 = scale * 0.7f;
                    int drawcount = 36;
                    float radius = (60 - k * 8) * Projectile.scale;

                    for (int j = 0; j < 1; j++)
                    {
                        for (int i = 0; i < drawcount; i++)
                        {
                            Vector2 offset = (MathHelper.TwoPi / drawcount * i).ToRotationVector2() * radius;
                            Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, color * (4f / drawcount), Projectile.rotation, texture.Size() / 2, scale2, 0, 0);
                        }
                    }
                    for (int i = 0; i < drawcount; i++)
                    {
                        Vector2 offset = (MathHelper.TwoPi / drawcount * i).ToRotationVector2() * 60 * Projectile.scale;
                        Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, color * (4f / drawcount), Projectile.rotation, texture.Size() / 2, scale2 * 0.6f, 0, 0);

                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.ReverseSubtract, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

                { 
                    Color color = Color.Red * 1f;
                    float scale2 = scale;

                    int drawcount = 5;
                    float radius = 10;

                    for (int i = 0; i < drawcount; i++)
                    {
                        Vector2 offset = (MathHelper.TwoPi / drawcount * i).ToRotationVector2() * radius;
                        Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, color * (5f / drawcount), Projectile.rotation, texture.Size() / 2, scale2, 0, 0);
                    }
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);
            }


            return false;
        }
    }

    public class ColdBlade : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaProjPath(349);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            base.OnSpawn(source);
        }

        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[0];
            //Projectile.velocity.Y += 0.2f;
            float factor = Projectile.TimeleftFactor();
            Projectile.Opacity = factor;

            float length = Projectile.velocity.Length();
            float consequent = Utils.Remap(length, 2f, 7.2f, 70, 25);
            if (Main.rand.NextBool((int)consequent))
            {
                Vector2 pos = Projectile.Center + AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(0, 18);
                StarParticle.Spawn(pos, Projectile.velocity * 0.6f * Projectile.MaxUpdates, Color.White.AdditiveColor(), Projectile.scale, 0.3f, 1.3f, 0.8f, 1f, Projectile.rotation, Projectile.Opacity);
                //int num464 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 185);
                //Main.dust[num464].noGravity = true;
                ////Main.dust[num464].noLight = true;
                //Main.dust[num464].scale = 0.7f;
            }
            //Projectile.velocity = Projectile.velocity.Length(length * 1.01f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            base.AI();
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Color color = new Color(200, 200, 200, Projectile.alpha) * Projectile.Opacity;
            Rectangle rect = new Rectangle(0, 30 * Projectile.frame, 12, 30);
            Vector2 origin = new Vector2(6, 15);

            int bloom = 6;
            float length = 5;
            for (int i = 0; i < bloom; i++)
            {
                float rev = 1f / bloom;
                Vector2 offset = (MathHelper.TwoPi / bloom * i).ToRotationVector2() * length;
                Color bloomColor = color * rev * 1.4f;
                Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, rect, bloomColor, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, 0, 0);
            }
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, rect, color, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
