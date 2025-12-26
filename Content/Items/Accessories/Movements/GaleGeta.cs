using AyaMod.Common.Easer;
using AyaMod.Content.Items.Materials;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Loaders;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
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
using static AyaMod.Core.ModPlayers.AyaPlayer;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [PlayerEffect]
    public class GaleGeta : BaseAccessories, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 1));

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 6.75f;
            player.rocketBoots = (player.vanityRocketBoots = 2);
            player.moveSpeed += 0.08f;
            player.jumpSpeedBoost += 1.4f;
            Player.jumpHeight += 8;
            player.autoJump = true;

            if (player.HeldCamera())
            {
                player.moveSpeed += 0.04f;
                player.AddEffect<GaleGeta>();
                player.Aya().WingTimeModifier.Flat += 10;//增加10帧飞行时间
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LightningBoots)
                .AddIngredient(ItemID.AmphibianBoots)
                .AddIngredient<MapleLeaf>(30)
                .AddIngredient(ItemID.SoulofFlight, 7)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public static void AddDash(Player player)
        {
            var modPlayer = player.Aya();
            modPlayer.AyaDash = DashType.GaleGeta0;
            modPlayer.HasDash = true;
        }

        public static void GetaDash0(Player player, int direction)
        {
            AyaPlayer modPlayer = player.Aya();

            float speed = 15f;
            float dashDirection;
            switch (direction)
            {
                case AyaPlayer.DashLeft:
                case AyaPlayer.DashRight:
                    {
                        dashDirection = direction == AyaPlayer.DashRight ? 1 : -1;
                        speed *= dashDirection;
                        break;
                    }
                default:
                    return;
            }
            player.velocity.X = speed;
            player.velocity.Y = 0.000001f;

            player.direction = (int)dashDirection;
            modPlayer.DashDelay = GaleGetaDashDelay;
            modPlayer.DashTimer = 10;

            Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, player.velocity, ModContent.ProjectileType<GaleGetaDash>(), 0, 0, player.whoAmI);
        }

        public static void WhileDashing0(Player player, int direction)
        {
            var modPlayer = player.Aya();
            if (modPlayer.DashDelay < 30) return;
            if (!player.controlRight && !player.controlLeft)
            {
                float factor = Utils.Remap(modPlayer.DashDelay, 0, 20, 0.9f, 0.96f);
                player.velocity.X *= factor;
            }
            player.dashDelay = -1;
            player.velocity.Y = 0.000001f;
            player.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(12 * 3);

        }

        public static int GaleGetaDashDelay = 60;
    }

    public class GaleGetaDash : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public static Color DrawColor = new Color(158, 65, 20);

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 25);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(15);
            Projectile.penetrate = -1;
            Projectile.timeLeft = GaleGeta.GaleGetaDashDelay * 2;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
                Projectile.Kill();
            //if (player.Aya().DashDelay > 0)
            //{
            //    Projectile.velocity = player.Center - Projectile.Center;
            //}
            if (!player.controlRight && !player.controlLeft)
            {
                float factor = Utils.Remap(player.Aya().DashDelay, 0, 10, 0.9f, 0.96f);
                Projectile.velocity *= factor;
                Projectile.Opacity -= 0.03f;
            }
            else
            {
                float factor = Utils.Remap(player.Aya().DashDelay, 0, 20, 0.985f, 0.995f);
                Projectile.velocity *= factor;
            }
            if (Projectile.timeLeft < GaleGeta.GaleGetaDashDelay * 2f * 5f / 8f)
                Projectile.Opacity -= Projectile.Opacity > 0.5f ? 0.08f : 0.04f;
            if (Projectile.Opacity < 0.05f) Projectile.Kill();
            for(int i = 0; i < 2; i++)
            {
                float velFactor = MathF.Abs(Projectile.velocity.X) / 12f;
                Vector2 dir = (Projectile.rotation * (8 + 8 * i)).ToRotationVector2().RotatedByRandom(0.5f);
                dir.X *= 0.5f;
                Vector2 pos = Projectile.Center + dir * Main.rand.NextFloat(8,16);
                Vector2 vel = Projectile.Center.DirectionToSafe(pos) * Main.rand.NextFloat(2, 4) * 0.5f;

                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                   pos,
                    vel, ModContent.ProjectileType<GaleGetaMist>(), 0, 0, Projectile.owner, 25 * velFactor * Projectile.Opacity);
                p.scale = 0.75f;
                p.Opacity *= velFactor * Projectile.Opacity;

                Vector2 dustPos = (Projectile.Center - Projectile.velocity * 10f * i) + dir * Main.rand.NextFloat(12, 16) * (2 + i);
                var toPos = Projectile.Center.DirectionToSafe(dustPos);
                Vector2 dustVel = toPos.RotatedBy(MathHelper.PiOver2 * MathF.Sign(Projectile.velocity.X))
                     * Main.rand.NextFloat(2, 4) * 1.5f + toPos * 2.5f;

                StarParticle.Spawn(Projectile.GetSource_FromAI(), dustPos,dustVel, Color.White.AdditiveColor(), Projectile.scale, 0.1f, 0.35f, 0.7f, 1f, dustVel.ToRotation(), Projectile.Opacity);

            }
            Projectile.rotation += 0.65f;


        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Maple-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D sampler = TextureAssets.Extra[189].Value;
            Texture2D star = TextureAssets.Extra[98].Value;

            Effect effect = ShaderLoader.GetShader("Trail");

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float width = 60f;
            float maxRadius = 64f;
            float minRadius = 12f;

            float mult = 64;
            int total = (int)(Projectile.oldPos.Length * mult - mult);
            float innerRot = 0;
            Vector2 lastTrailPos = Vector2.Zero;
            
            for (int i = 0; i < total - 1; i++)
            {
                if (Projectile.oldPos[(int)(i / mult)] == Vector2.Zero || Projectile.oldPos[(int)(i / mult) + 1] == Vector2.Zero) continue;
                float factor = 1f - (float)i / total;//factor 1为轨迹头部， 0为轨迹尾部
                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1/mult, 1f);
                float radius = MathHelper.Lerp(maxRadius, minRadius, EaseManager.Evaluate(Ease.InCirc, factor, 1f));
                radius *= 1f;
                Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
                oldpos += Projectile.Size / 2;
                Vector2 dir = (innerRot + Projectile.rotation).ToRotationVector2();
                dir.X *= 0.5f;
                //dir = dir.RotatedBy(Projectile.rotation);
                Vector2 trailPos = oldpos + dir * radius;

                var color = Color.Lerp(Color.Black, Color.White, factor) * Projectile.Opacity;
                var alpha = EaseManager.Evaluate(Ease.InOutSine, factor, 1f) * Projectile.Opacity;
                float fadeinFactor = Utils.Remap(factor, 0.93f, 1, 1, 0);
                alpha *= fadeinFactor;
                if (dir.X > 0 && Projectile.velocity.X > 0) alpha *= Utils.Remap(dir.X, 0, 0.5f, 1f, 0f);
                if (dir.X < 0 && Projectile.velocity.X < 0) alpha *= Utils.Remap(dir.X, 0, -0.5f, 1f, 0f);
                var normalDir = lastTrailPos - trailPos;
                normalDir = normalDir.RotatedBy(MathHelper.PiOver2);
                normalDir = normalDir.SafeNormalize(Vector2.Zero);

                lastTrailPos = trailPos;

                innerRot -= 0.035f;
                if (i == 0/* || i % 3 != 0*/) continue;

                Vector2 top = trailPos + normalDir * width * 0.5f;
                Vector2 bottom = trailPos - normalDir * width * 0.5f;

                bars.Add(new CustomVertexInfo(top, color * alpha, new Vector3(factor, 1, alpha)));
                bars.Add(new CustomVertexInfo(bottom, color * alpha, new Vector3(factor, 0, alpha)));

            }

            if(bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                // 干掉注释掉就可以只显示三角形栅格
                //RasterizerState rasterizerState = new RasterizerState();
                //rasterizerState.CullMode = CullMode.None;
                //rasterizerState.FillMode = FillMode.WireFrame;
                //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * projection);
                effect.Parameters["timer"].SetValue((float)Main.timeForVisualEffects * 0.02f);
                Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
                Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
                Main.graphics.GraphicsDevice.Textures[2] = sampler;//蒙版
                //Main.graphics.GraphicsDevice.Textures[0] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[1] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[2] = (Texture)TextureAssets.MagicPixel;

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            Texture2D ball4 = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball4", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball1 = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball1", AssetRequestMode.ImmediateLoad).Value;
            Color ballColor = DrawColor;
            Main.spriteBatch.Draw(ball4, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, ballColor * Projectile.Opacity * 0.15f, Projectile.rotation, ball4.Size() / 2, Projectile.scale * 0.6f, 0, 0);
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor,Color.White,0.3f).AdditiveColor() * Projectile.Opacity * 0.5f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.8f, 0, 0);
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor,Color.White,0.3f).AdditiveColor() * Projectile.Opacity * 0.4f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.6f, 0, 0);
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor,Color.White,0.3f).AdditiveColor() * Projectile.Opacity * 0.3f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.4f, 0, 0);

            return true;
        }
    }

    public class GaleGetaMist : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Cloud";
        public ref float MaxTimeleft => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.ignoreWater = true;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)MaxTimeleft;
        }
        public override bool? CanDamage() => false;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.9f;
            return false;
        }
        public override void AI()
        {
            var factor = Projectile.TimeleftFactor();
            Projectile.velocity *= 0.95f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Projectile.type].Value;
            float factor = Projectile.TimeleftFactor();
            Color baseColor = Color.Lerp(Color.Black, new Color(67, 86, 112),  factor);
            baseColor *= lightColor.A / 255f * 0.5f;
            Main.spriteBatch.Draw(tex, 
                Projectile.Center - Main.screenPosition, null,
                baseColor.AdditiveColor() * Projectile.Opacity, Projectile.rotation,
                tex.Size() / 2, 
                Projectile.scale, 0, 0);
            return false;
        }
    }
}
