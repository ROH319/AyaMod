using AyaMod.Common.Easer;
using AyaMod.Content.Buffs;
using AyaMod.Content.Items.Lens;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Loaders;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems.Trails;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class DoremyCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 50;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<DoremyCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 152, 1.6f, 0.5f);
            SetCaptureStats(1000, 60);
        }
    }

    public class DoremyCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(179, 0, 255);
        public override Color innerFrameColor => new Color(102,0,255) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(196, 243, 240).AdditiveColor() * 0.5f;
        public override ILens Lens => LensLoader.GetLens<CircleLens>();
        public override void OnSnap()
        {
            if (!Projectile.MyClient()) return;

            int type = ProjectileType<DreamZoneSmall>();
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, type, Projectile.damage, 0f, player.whoAmI, 2 * 60);
        }
    }

    public class DreamZoneSmall : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public static Texture2D DreamTexture;
        public static Texture2D NoiseTexture;

        public override void Load()
        {
            DreamTexture = ModContent.Request<Texture2D>($"{AssetDirectory.VanillaTexturePath("SplashScreens/Splash_6_0")}",AssetRequestMode.ImmediateLoad).Value;
            NoiseTexture = ModContent.Request<Texture2D>(AssetDirectory.Extras + "T_Amitus_GlowNoise", AssetRequestMode.ImmediateLoad).Value;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 384;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2 * 60;
        }

        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Projectile.ai[0];
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            float baseFactor = 1f;
            float fadeinFactor = Utils.Remap(factor, 0.8f, 1f, 1f, 0f);
            baseFactor *= fadeinFactor;
            
            float fadeonFactor = MathF.Cos(Utils.Remap(factor,0.2f, 0.8f, MathHelper.TwoPi, 0f)) / 5 + 0.8f;
            if(factor < 0.5f)
            {
                fadeonFactor = 0.6f + (fadeonFactor - 0.6f) * 0.5f;
            }
            /*if (factor > 0.2f && factor < 0.8f) */baseFactor *= fadeonFactor;

            float fadeoutFactor = Utils.Remap(factor,0f,0.2f,0f,1f);
            baseFactor *= fadeoutFactor;

            Projectile.scale = baseFactor;

            Projectile.rotation += 0.02f;
            Projectile.Opacity = 0.6f;

            float radius = Projectile.width * Projectile.scale * 0.7f;
            foreach(var npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.damage <= 0 || npc.Distance(Projectile.Center) > radius) continue;
                npc.AddBuff(BuffType<TiredBuff>(), 2);
            }

            for(int i = 0; i < 10; i++)
            {
                Vector2 pos = Projectile.Center + 0.7f* Main.rand.NextFloat(0.9f,1.1f) * Main.rand.NextVector2Unit() * Projectile.width / 2 * Projectile.scale;
                Vector2 vel = Projectile.DirectionToSafe(pos).RotatedBy(-MathHelper.PiOver2) * 4;
                //dustid:21,
                Dust d = Dust.NewDustPerfect(pos, 27, vel, 120, Scale: 0.75f);
                d.noGravity = true;
            }

            float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
            if (Projectile.timeLeft % 3 == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 dir = (startRot + MathHelper.TwoPi / 3 * i + Main.rand.NextFloat(0.6f)).ToRotationVector2();
                    Vector2 pos = Projectile.Center + dir * 0.8f * Main.rand.NextFloat(0.9f, 1.1f) * Projectile.width / 2 * Projectile.scale;
                    Vector2 vel = dir * Main.rand.NextFloat(3, 3) * 2f;
                    StarParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, new Color(255, 20, 255).AdditiveColor(), Projectile.scale * 0.6f, 0.4f, 1.4f, 0.82f, 0.9f, vel.ToRotation(), Projectile.Opacity);
                }
            }
            if(factor < 0.2f)
            {

                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Projectile.Center + 0.9f * Main.rand.NextFloat(0.9f, 1.1f) * Main.rand.NextVector2Unit() * Projectile.width / 2 * Projectile.scale;
                    Vector2 vel = pos.DirectionToSafe(Projectile.Center) * Main.rand.NextFloat(1,3);
                    Dust d = Dust.NewDustPerfect(pos,  DustID.PurpleTorch, vel, Scale: 2f);
                    d.noGravity = true;
                }
            }

        }
        public override void OnKill(int timeLeft)
        {
            //int type = ProjectileType<DreamShot>();
            int type = ProjectileType<DreamEssence>();
            int damage = (int)(Projectile.damage * 0.4f);
            float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
            for(int i = 0; i < 3; i++)
            {
                Vector2 dir = (startRot + MathHelper.TwoPi / 3 * i + Main.rand.NextFloat(1f)).ToRotationVector2();
                Vector2 vel = dir * Main.rand.NextFloat(3,4) * 2f;
                Vector2 pos = Projectile.Center + dir * Main.rand.NextFloat(0, 50);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, type, damage, Projectile.knockBack, Projectile.owner);
            }

            for (int i = 0; i < 40; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 3) * 2f;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, vel, Scale: 2f);
                d.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var shader = ShaderLoader.GetShader("ScreenMapper");
            if (shader == null) return false;

            Texture2D spin = ModContent.Request<Texture2D>(AssetDirectory.Extras + "GenFX_SpinRing1_1", AssetRequestMode.ImmediateLoad).Value;
            
            float spinScale = Projectile.width / (float)spin.Width * Projectile.scale * 1.1f;
            Main.spriteBatch.Draw(spin, Projectile.Center - Main.screenPosition, null, Color.DarkViolet * Projectile.Opacity, -Projectile.rotation, spin.Size() / 2, spinScale, 0, 0);


            Texture2D texture = TextureAssets.MagicPixel.Value;
            Texture2D mask = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball6", AssetRequestMode.ImmediateLoad).Value;


            shader.Parameters["threshold"].SetValue(1f);
            shader.Parameters["uScreenResolution"].SetValue(new Vector4(Main.screenWidth, Main.screenHeight, 0, 0));
            Main.graphics.GraphicsDevice.Textures[1] = DreamTexture;
            Main.graphics.GraphicsDevice.Textures[2] = NoiseTexture;
            Main.graphics.GraphicsDevice.Textures[3] = mask;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            shader.CurrentTechnique.Passes[0].Apply();
            Rectangle rect = new Rectangle((int)(Projectile.position.X - Main.screenPosition.X), (int)(Projectile.position.Y - Main.screenPosition.Y), Projectile.width,Projectile.height);
            //Main.spriteBatch.Draw(texture, rect, null, Color.White, 0, texture.Size() / 2, 0, 0);
            Vector2 scale = new Vector2(Projectile.width / (float)texture.Width, Projectile.height / (float)texture.Height) * Projectile.scale;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class DreamShot : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public static MultedTrail strip = new MultedTrail();

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 50);
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.SetImmune(100);
            Projectile.timeLeft = 10 * 60;
        }
        public override bool? CanDamage() => Projectile.ai[2] >= 0 && Projectile.TimeleftFactor() < 0.95f;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            EndMove();
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EndMove();
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            if (Projectile.ai[2] < 0)
            {
                Projectile.velocity *= 0;
                Projectile.extraUpdates = 0;
                return;
            }

            if(factor > 0.95f)
            {
                Projectile.velocity = Projectile.velocity * 0.97f;
                float collisionDistance = AyaUtils.CheckCollisionDistance(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX), maxdistance: 300);
                float extraFactor = Utils.Remap(collisionDistance, 0, 200, 0.7f, 1f);
                Projectile.velocity = Projectile.velocity * extraFactor;
            }
            Projectile.localAI[0] = Utils.Remap(factor, 0.83f, 0.97f, 0.1f, 0f);
            Projectile.Chase(2000, 22, Projectile.localAI[0]);
            Projectile.Opacity += 0.03f;
            if (Projectile.Opacity > 1f) Projectile.Opacity = 1f;

            //Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Projectile.velocity * 0.5f, Scale: 2f);
            //d.noGravity = true;

        }
        public void EndMove()
        {
            Projectile.penetrate++;
            Projectile.timeLeft = 50;
            Projectile.ai[2] = -1;

            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 3) * 3;
                Dust d = Dust.NewDustDirect(Projectile.position - new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.width * 2, Projectile.height * 2, DustID.PurpleTorch, vel.X, vel.Y, Scale: 2f);
                d.noGravity = true;
            }
            float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (startRot + MathHelper.TwoPi / 4 * i + Main.rand.NextFloat(0.4f)).ToRotationVector2();
                Vector2 pos = Projectile.Center + dir * 20;
                Vector2 vel = dir * Main.rand.NextFloat(3, 3) * 1.5f;
                StarParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.White, Projectile.scale * 0.5f, 0.3f, 1.3f, 0.8f, 0.96f, vel.ToRotation(), Projectile.Opacity);
            }
        }
        public override void OnKill(int timeLeft)
        {
        }
        public Color ColorFunction(float progress)
        {
            Color drawColor = new Color(138, 43, 226);
            return Color.Lerp(drawColor, Color.Black, progress).AdditiveColor() * Projectile.Opacity;
        }
        public float WidthFunction(float progress)
        {
            return 2f;
        }
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0f, 0.05f, 0f, 1f);
            return EaseManager.Evaluate(Ease.InOutSine, 1f - progress, 1f) * Projectile.Opacity * fadeinFactor;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            Vector2 lastTrailPos = Vector2.Zero;

            int mult = 1;
            int total = (int)(Projectile.oldPos.Length * mult - mult);
            //strip.PrepareStrip(Projectile.oldPos, 2, ColorFunction, WidthFunction,
            //    Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            //strip.DrawTrail();

            for (int i = 0; i < total - 1; i++)
            {
                if (Projectile.oldPos[(int)(i / mult)] == Vector2.Zero || Projectile.oldPos[(int)(i / mult) + 1] == Vector2.Zero) continue;
                float factor = (float)i / total;//factor 1为轨迹尾部， 0为轨迹头部
                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                if (float.IsNaN(lerpFactor)) lerpFactor = 0f;
                Vector2 trailPos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
                trailPos += Projectile.Size / 2;

                Color color = ColorFunction(factor);
                float alpha = AlphaFunction(factor);
                var normalDir = lastTrailPos - trailPos;
                normalDir = normalDir.RotatedBy(MathHelper.PiOver2);
                normalDir = normalDir.SafeNormalize(Vector2.Zero);

                lastTrailPos = trailPos;

                if (i == 0) continue;

                float width = WidthFunction(factor);
                Vector2 top = trailPos - Main.screenPosition + normalDir * width * 0.5f;
                Vector2 bottom = trailPos - Main.screenPosition - normalDir * width * 0.5f;

                bars.Add(new CustomVertexInfo(top, color * alpha, new Vector3(factor, 1, alpha)));
                bars.Add(new CustomVertexInfo(bottom, color * alpha, new Vector3(factor, 0, alpha)));

            }

            //for (int i = 0; i < 2; i++)
            {
                if (bars.Count > 2)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
                    Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            if (Projectile.ai[2] < 0) return false;

            Texture2D ball7 = Request<Texture2D>(AssetDirectory.Extras + "Ball7_1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D star = TextureAssets.Extra[98].Value;
            Texture2D ball4 = Request<Texture2D>(AssetDirectory.Extras + "Ball4_1", AssetRequestMode.ImmediateLoad).Value;

            float ballScale = Projectile.width / (float)ball7.Width * Projectile.scale * 6;
            Color backColor = Color.Lerp(Color.DarkViolet, Color.White, 0f);
            Color darkvio = Color.DarkViolet;
            //打底紫光
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, backColor * Projectile.Opacity * 0.2f, Projectile.rotation, ball7.Size() / 2, ballScale * 1.5f, 0, 0);
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, darkvio * Projectile.Opacity * 0.5f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.65f, 0, 0);

            //十字
            Vector2 starScale = new Vector2(0.06f, 1f) * 0.2f;
            for (int i = 0; i < 2; i++) {
                Main.spriteBatch.Draw(ball4, Projectile.Center - Main.screenPosition, null, Color.Violet * Projectile.Opacity * 0.7f, i * MathHelper.PiOver2, ball4.Size() / 2, starScale* Projectile.scale * 1.5f, 0, 0);
            }
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(ball4, Projectile.Center - Main.screenPosition, null, Color.Violet * Projectile.Opacity * 0.5f, MathHelper.PiOver4 + i * MathHelper.PiOver2, ball4.Size() / 2, starScale * Projectile.scale * 1f, 0, 0);
            }

            //高光
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.3f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.4f, 0, 0);
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.5f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.3f, 0, 0);
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.8f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.2f, 0, 0);
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 1f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.1f, 0, 0);

            RenderHelper.DrawRing(60, Projectile.Center, 0.4f * 32f * Projectile.scale, Color.Violet, Projectile.rotation, new Vector2(0.2f, 0.8f) * 0.3f);

            return false;
        }
    }

    public class DreamEssence : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            Projectile.SetTrail(4, 40);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.SetImmune(100);
            Projectile.timeLeft = 10 * 60;
            Projectile.hide = true;
        }
        public override bool? CanDamage() => Projectile.ai[2] >= 0 && Projectile.TimeleftFactor() < 0.95f;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0f;
            Projectile.scale = 0f;
            Projectile.localAI[1] = 0.6f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EndMove();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            EndMove();
            return base.OnTileCollide(oldVelocity);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 24; height = 24;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override void AI()
        {

            float factor = Projectile.TimeleftFactor();

            if (Projectile.ai[2] < 0)
            {
                Projectile.velocity *= 0.83f;
                Projectile.extraUpdates = 0;
                float deathFactor = Projectile.timeLeft / 50f;

                Projectile.scale = Utils.Remap(deathFactor, 0, 1f, 1.5f, 1f);
                Projectile.Opacity = deathFactor;
                Projectile.localAI[1] = Utils.Remap(deathFactor, 0f, 1f, 1f, 0.6f);
                return;
            }

            if (factor > 0.95f)
            {
                Projectile.velocity = Projectile.velocity * 0.97f;
                float collisionDistance = AyaUtils.CheckCollisionDistance(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX), maxdistance: 300);
                float extraFactor = Utils.Remap(collisionDistance, 0, 200, 0.7f, 1f);
                Projectile.velocity = Projectile.velocity * extraFactor;
            }
            Projectile.localAI[0] = Utils.Remap(factor, 0.7f, 0.97f, 0.05f, 0f);
            Projectile.Chase(1400, 18, Projectile.localAI[0]);
            Projectile.Opacity += 0.03f;
            if (Projectile.Opacity > 1f) Projectile.Opacity = 1f;
            Projectile.scale += 0.03f;
            if (Projectile.scale > 1f) Projectile.scale = 1f;

            {
                Vector2 vel = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitX) * MathF.Sin(Main.GameUpdateCount * 0.1f + Projectile.whoAmI) * 10f;
                Vector2 dustpos = Projectile.Center;
                Dust d = Dust.NewDustPerfect(dustpos, DustID.PurpleTorch, Projectile.velocity * 0.2f /*+ vel * 0.2f*/, Scale: 2f);
                d.noGravity = true;
            }
        }
        public void EndMove()
        {
            Projectile.penetrate++;
            Projectile.timeLeft = 20;
            Projectile.ai[2] = -1;

            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 3) * 3;
                Dust d = Dust.NewDustDirect(Projectile.position - new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.width * 2, Projectile.height * 2, DustID.PurpleTorch, vel.X, vel.Y, Scale: 2f);
                d.noGravity = true;
            }
        }
        public static void DrawRing(int pointCount, Vector2 center, Vector2 distCenter, Color drawcolor, float radius, float rot, Vector2 scale, float distance)
        {

            var star = TextureAssets.Extra[98].Value;
            Vector2 origin = new Vector2(36, 36);
            for (int i = 0; i < pointCount; i++)
            {
                float dir = MathHelper.TwoPi / pointCount * i + rot;
                Vector2 drawpos = center + dir.ToRotationVector2() * radius;
                if (drawpos.Distance(distCenter) > distance) continue;
                Main.spriteBatch.Draw(star, drawpos - Main.screenPosition, null, drawcolor, dir, origin, scale, 0, 0);
            }
        }
        public Color ColorFunction(float progress)
        {
            Color drawColor = new Color(138, 43, 226);
            return Color.Lerp(drawColor, Color.Black, progress).AdditiveColor() * Projectile.Opacity;
        }
        public float WidthFunction(float progress)
        {
            return 2f;
        }
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0f, 0.05f, 0f, 1f);
            return EaseManager.Evaluate(Ease.InOutSine, 1f - progress, 1f) * Projectile.Opacity * fadeinFactor;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            #region 拖尾
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            Vector2 lastTrailPos = Vector2.Zero;

            int mult = 1;
            int total = (int)(Projectile.oldPos.Length * mult - mult);
            //strip.PrepareStrip(Projectile.oldPos, 2, ColorFunction, WidthFunction,
            //    Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            //strip.DrawTrail();

            for (int i = 0; i < total - 1; i++)
            {
                if (Projectile.oldPos[(int)(i / mult)] == Vector2.Zero || Projectile.oldPos[(int)(i / mult) + 1] == Vector2.Zero) continue;
                float factor = (float)i / total;//factor 1为轨迹尾部， 0为轨迹头部
                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                if (float.IsNaN(lerpFactor)) lerpFactor = 0f;
                Vector2 trailPos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
                trailPos += Projectile.Size / 2;

                Color color = ColorFunction(factor);
                float alpha = AlphaFunction(factor);
                var normalDir = lastTrailPos - trailPos;
                normalDir = normalDir.RotatedBy(MathHelper.PiOver2);
                normalDir = normalDir.SafeNormalize(Vector2.Zero);

                lastTrailPos = trailPos;

                if (i == 0) continue;

                float width = WidthFunction(factor);
                Vector2 top = trailPos - Main.screenPosition + normalDir * width * 0.5f;
                Vector2 bottom = trailPos - Main.screenPosition - normalDir * width * 0.5f;

                bars.Add(new CustomVertexInfo(top, color * alpha, new Vector3(factor, 1, alpha)));
                bars.Add(new CustomVertexInfo(bottom, color * alpha, new Vector3(factor, 0, alpha)));

            }

            //for (int i = 0; i < 2; i++)
            {
                if (bars.Count > 2)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
                    Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }
            #endregion

            float radius = 26 * Projectile.scale;

            Color pink = new Color(255,105,180).AdditiveColor();
            Color blue = new Color(65, 105, 225).AdditiveColor();
            Color gradientColor = Color.Lerp(pink, blue, MathF.Sin((float)(Main.timeForVisualEffects * 0.02f + Projectile.whoAmI)) * 0.5f + 0.5f);
            Vector2 baseScale = new Vector2(0.25f, 0.8f);
            int drawcount = 12;
            float rad = radius * Projectile.localAI[1] * (MathF.Sin((float)(Main.timeForVisualEffects * 0.03f + Projectile.whoAmI)) * 0.2f + 1f);
            for(int i = 0; i < drawcount; i++)
            {
                float sign = i % 2 == 0 ? -1 : 1;
                float dir = MathHelper.TwoPi / drawcount * i + (float)(Main.timeForVisualEffects * 0.01f) * sign;
                Vector2 drawpos = Projectile.Center + dir.ToRotationVector2() * rad;
                DrawRing(40, drawpos, Projectile.Center, (i % 2 == 0 ? pink : blue) * Projectile.Opacity, rad, Projectile.rotation, new Vector2(0.25f, 0.8f) * 0.2f, radius);
            }

            RenderHelper.DrawRing(80, Projectile.Center, radius, pink * Projectile.Opacity, Projectile.rotation, baseScale * 0.4f);


            Texture2D ball7 = Request<Texture2D>(AssetDirectory.Extras + "Ball7_1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball4 = Request<Texture2D>(AssetDirectory.Extras + "Ball4_1", AssetRequestMode.ImmediateLoad).Value;

            float ballScale = Projectile.width / (float)ball7.Width * Projectile.scale * 4;
            Color backColor = Color.Lerp(Color.DarkViolet, Color.White, 0f);
            Color darkvio = Color.DarkViolet;
            //打底紫光
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, backColor * Projectile.Opacity * 0.15f, Projectile.rotation, ball7.Size() / 2, ballScale * 1.5f, 0, 0);
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, darkvio * Projectile.Opacity * 0.5f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.65f, 0, 0);


            //高光
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.3f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.4f, 0, 0);
            //Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.5f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.3f, 0, 0);
            //Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.8f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.2f, 0, 0);
            Main.spriteBatch.Draw(ball7, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 1f, Projectile.rotation, ball7.Size() / 2, ballScale * 0.1f, 0, 0);

            return false;
        }
    }

    public class DormantNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int DormantCounter = 0;
        public static int DormantThreshold = 2 * 60;
        public bool Dormanting = false;
        public static int DormantTimeleft = 0;

        public override bool PreAI(NPC npc)
        {
            if (Dormanting)
            {
                npc.chaseable = false;
                return false;
            }
            else npc.chaseable = true;
            return base.PreAI(npc);
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if(Dormanting) return false;
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (Dormanting) return false;
            return base.CanBeHitByItem(npc, player, item);
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (Dormanting) return false;
            return base.CanBeHitByProjectile(npc, projectile);
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (npc.HasBuff<TiredBuff>())
                modifiers.FinalDamage -= 0.1f;
        }
        public override bool CheckActive(NPC npc)
        {
            //if(npc.type == 695) Main.NewText($"{Dormanting} {DormantCounter} {DormantTimeleft} {Main.time}");
            if(Dormanting)
            {
                npc.position = npc.oldPosition;
                if(DormantTimeleft % 120 == 0)
                {
                    Vector2 vel = new Vector2(0, -5);
                    //int type = ProjectileType<DreamShot>();
                    int type = ProjectileType<DreamEssence>();
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, vel, type, npc.damage / 3, 0, Main.myPlayer);

                }
                if(DormantTimeleft % 60 == 0)
                {
                    CombatText.NewText(npc.getRect(), new Color(255, 0, 255), "zzz");
                }


                if (DormantTimeleft > 0)
                    DormantTimeleft--;
                if (DormantTimeleft <= 0) Dormanting = false;
            }
            else
            {
                if(npc.HasBuff<TiredBuff>() && DormantCounter < DormantThreshold)
                    DormantCounter++;
                if (DormantCounter >= DormantThreshold)
                {
                    Dormanting = true;
                    DormantTimeleft = 5 * 60;
                    DormantCounter = 0;
                }
            }

            return base.CheckActive(npc);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!Dormanting) return;

            float radius = MathF.Min(npc.width, npc.height);

            Color color = new Color(255, 0, 255);
            RenderHelper.DrawRing((int)(radius * MathHelper.Pi), npc.Center, radius / 1.5f, color, 0, new Vector2(0.25f, 0.8f) *radius / 60f * 0.6f);

            Texture2D zzz = Request<Texture2D>(AssetDirectory.Extras + "zzz_1", AssetRequestMode.ImmediateLoad).Value;

            float scale = radius / 256f * 1.2f;

            spriteBatch.Draw(zzz, npc.Center - Main.screenPosition, null, color, 0, zzz.Size() / 2, scale, 0, 0);

            //for(int i = -1; i < 2; i++)
            //{
            //    Vector2 drawpos = npc.Center + new Vector2(-radius, radius * 0.7f) * 0.2f * i;
            //    RenderHelper.DrawZ(drawpos, color, 0, radius * 0.15f, radius / 10f * 0.4f,  radius / 40f * 0.3f);
            //}
        }
    }
}
