using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems.Trails;
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
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static AyaMod.Core.ModPlayers.AyaPlayer;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [PlayerEffect]
    public class GaleGeta1 : BaseAccessories
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(gold: 2));

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var ayaPlayer = player.Aya();

            player.accRunSpeed = 6.75f;
            player.rocketBoots = (player.vanityRocketBoots = 2);
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += 1.4f;
            Player.jumpHeight += 8;
            player.autoJump = true;
            ayaPlayer.AccSpeedModifier += 0.5f;

            if (player.HeldCamera())
            {
                var a = player.AddEffect<GaleGeta1>();
                player.moveSpeed += 0.05f;
                ayaPlayer.AccSpeedModifier += 0.25f;
                ayaPlayer.WingTimeModifier.Flat += 10;//增加10帧飞行时间

            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<GaleGeta>())
                .AddIngredient(ItemID.Magiluminescence)
                .AddIngredient(ItemID.SoulofMight, 3)
                .AddIngredient(ItemID.SoulofSight, 3)
                .AddIngredient(ItemID.SoulofFright, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public static void AddDash(Player player)
        {
            var ayaPlayer = player.Aya();
            ayaPlayer.HasDash = true;
            ayaPlayer.AyaDash = DashType.GaleGeta1;
        }

        public static void GetaDash1(Player player, int direction)
        {
            AyaPlayer ayaPlayer = player.Aya();


            float speed = 16f;
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
            ayaPlayer.DashDelay = GaleGetaDashDelay;
            ayaPlayer.DashTimer = 10;

            int damage = GaleGetaDamage;
            damage = (int)(player.GetTotalDamage<ReporterDamage>().ApplyTo(damage));
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, player.velocity, ModContent.ProjectileType<GaleGetaDash1>(), damage, 0, player.whoAmI);

        }

        public static void WhileDashing1(Player player, int direction)
        {
            var modPlayer = player.Aya();
            if (modPlayer.DashDelay < 20) return;

            //运动部分
            if (!player.controlRight && !player.controlLeft)
            {
                float factor = Utils.Remap(modPlayer.DashDelay, 0, 20, 0.9f, 0.96f);
                player.velocity.X *= factor;
            }
            player.dashDelay = -1;
            player.velocity.Y = 0.000001f;
            player.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(12 * 3);

            //对NPC造成伤害并获得无敌帧
            Rectangle rectangle = new Rectangle((int)(player.Center.X - DamageBoxSize / 2), (int)(player.Center.Y - DamageBoxSize / 2), DamageBoxSize, DamageBoxSize);
            foreach(var npc in Main.ActiveNPCs)
            {
                if (npc.dontTakeDamage || npc.friendly || !player.CanNPCBeHitByPlayerOrPlayerProjectile(npc)) 
                    continue;
                Rectangle targetRect = npc.getRect();

                if(rectangle.Intersects(targetRect) && (npc.noTileCollide || player.CanHit(npc)))
                {
                    float damage = player.GetTotalDamage<ReporterDamage>().ApplyTo(GaleGetaDamage);
                    float knockBack = player.GetTotalKnockback<ReporterDamage>().ApplyTo(9.5f);
                    bool crit = Main.rand.NextBool((int)player.GetTotalCritChance<ReporterDamage>(), 100);

                    if (player.whoAmI == Main.myPlayer)
                        player.ApplyDamageToNPC(npc, (int)damage, knockBack, direction, crit, ReporterDamage.Instance, false);

                    player.GiveImmuneTimeForCollisionAttack(12);
                }

            }
        }

        public static int DamageBoxSize = 72;
        public static int GaleGetaDamage = 100;
        public static int GaleGetaDashDelay = 50;
    }
    public class GaleGetaDash1 : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public static Color DrawColor = new Color(73, 93, 115);

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
            Projectile.timeLeft = GaleGeta1.GaleGetaDashDelay * 2;
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
            if (Projectile.timeLeft < GaleGeta1.GaleGetaDashDelay * 2f * 5f / 8f)
                Projectile.Opacity -= Projectile.Opacity > 0.5f ? 0.07f : 0.03f;
            if (Projectile.Opacity < 0.05f) Projectile.Kill();
            for (int i = 0; i < 2; i++)
            {
                float velFactor = MathF.Abs(Projectile.velocity.X) / 12f;
                Vector2 dir = (Projectile.rotation * (8 + 8 * i)).ToRotationVector2().RotatedByRandom(0.5f);
                dir.X *= 0.5f;
                Vector2 pos = Projectile.Center + dir * Main.rand.NextFloat(8, 16);
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

                StarParticle.Spawn(Projectile.GetSource_FromAI(), dustPos, dustVel, Color.White.AdditiveColor(), Projectile.scale, 0.1f, 0.35f, 0.7f, 1f, dustVel.ToRotation(), Projectile.Opacity);

            }
            Projectile.rotation += 0.1f;


        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "DarkBlue-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D sampler = TextureAssets.Extra[189].Value;
            Texture2D star = TextureAssets.Extra[98].Value;

            Effect effect = AssetDirectory.Trail;

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
                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                float radius = MathHelper.Lerp(maxRadius, minRadius, EaseManager.Evaluate(Ease.InCirc, factor, 1f));
                radius *= 1f;
                Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
                oldpos += Projectile.Size / 2;
                Vector2 dir = (innerRot + Projectile.rotation).ToRotationVector2();
                dir.X *= 0.5f;
                //dir = dir.RotatedBy(Projectile.rotation);
                Vector2 trailPos = oldpos + dir * radius;

                var color = Color.Lerp(Color.Black, Color.White, factor) * Projectile.Opacity;
                var alpha = EaseManager.Evaluate(Ease.InOutSine, factor, 1f) * 0.5f * Projectile.Opacity;
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

                bars.Add(new CustomVertexInfo(top, color, new Vector3(factor, 1, alpha)));
                bars.Add(new CustomVertexInfo(bottom, color, new Vector3(factor, 0, alpha)));

            }

            if (bars.Count > 2)
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
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor, Color.White, 0.3f).AdditiveColor() * Projectile.Opacity * 0.5f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.8f, 0, 0);
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor, Color.White, 0.3f).AdditiveColor() * Projectile.Opacity * 0.4f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.6f, 0, 0);
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor, Color.White, 0.3f).AdditiveColor() * Projectile.Opacity * 0.3f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.4f, 0, 0);

            return true;
        }
    }

    public class UltraDash : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float IsDashing => ref Projectile.ai[0];

        public static MultedTrail strip = new MultedTrail();

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 40);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
                IsDashing = 1;

            if (IsDashing == 0)
            {
                Projectile.timeLeft++;
                Projectile.Center = player.Center;
                Projectile.rotation = player.Aya().UltraDashDir + MathHelper.PiOver2;

                {
                    Vector2 projVel = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();

                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(8, 12);
                    Vector2 vel = -projVel.RotatedByRandom(0.2f);

                    var particle = StarParticle.Spawn(Projectile.GetSource_FromAI(), pos, projVel.RotatedByRandom(0.1f) * Main.rand.NextFloat(10,40), 
                        new Color(255,150,150).AdditiveColor(), 1f, 0.1f, 0.3f, 0.8f, 1f, vel.ToRotation(), 1f);
                }
            }
            else
            {
                Projectile.Opacity -= 0.05f;
            }
        }
        public Color ColorFunction(float progress)
        {
            Color drawColor = new Color(255, 90, 90);
            return Color.Lerp(drawColor, Color.Black, progress).AdditiveColor() * Projectile.Opacity;
        }
        public float WidthFunction(float progress) => 35f;
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0f, 0.05f, 0f, 1f);
            return EaseManager.Evaluate(Ease.InOutSine, 1f - progress, 1f) * Projectile.Opacity * 2f * fadeinFactor;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D shape = TextureAssets.Extra[197].Value;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            Color drawcolor = Color.Red with { G = 90,B = 90};
            float width = 70f;
            float mult =4;
            int total = (int)(Projectile.oldPos.Length * mult - mult);
            Vector2 lastTrailPos = Vector2.Zero;

            strip.PrepareStrip(Projectile.oldPos, mult, ColorFunction, WidthFunction,
                Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            Main.graphics.GraphicsDevice.Textures[0] = shape;
            strip.DrawTrail();
            #region 原写法
            //for (int i = 0; i < total - 1; i++)
            //{
            //    if (Projectile.oldPos[(int)(i / mult)] == Vector2.Zero || Projectile.oldPos[(int)(i / mult) + 1] == Vector2.Zero) continue;
            //    float factor = 1f - (float)i / total;//factor 1为轨迹头部， 0为轨迹尾部
            //    float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
            //    Vector2 trailPos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
            //    trailPos += Projectile.Size / 2;

            //    Color color = Color.Lerp(Color.Black, drawcolor, factor) * Projectile.Opacity;
            //    var alpha = EaseManager.Evaluate(Ease.InOutSine, factor, 1f) * Projectile.Opacity * 0.9f;
            //    float fadeinFactor = Utils.Remap(factor, 0.95f, 1, 1, 0f);
            //    alpha *= fadeinFactor;
            //    var normalDir = lastTrailPos - trailPos;
            //    normalDir = normalDir.RotatedBy(MathHelper.PiOver2);
            //    normalDir = normalDir.SafeNormalize(Vector2.Zero);

            //    lastTrailPos = trailPos;

            //    if (i == 0) continue;

            //    Vector2 top = trailPos - Main.screenPosition + normalDir * width * 0.5f;
            //    Vector2 bottom = trailPos - Main.screenPosition - normalDir * width * 0.5f;

            //    bars.Add(new CustomVertexInfo(top, color * alpha, new Vector3(factor, 1, alpha)));
            //    bars.Add(new CustomVertexInfo(bottom, color * alpha, new Vector3(factor, 0, alpha)));

            //}

            //for (int i = 0; i < 2; i++)
            //{
            //    if (bars.Count > 2)
            //    {
            //        Main.spriteBatch.End();
            //        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
            //        Main.graphics.GraphicsDevice.Textures[0] = shape;
            //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            //        Main.spriteBatch.End();
            //        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //    }
            //}
            #endregion
            var vel = Projectile.position - Projectile.oldPosition;
            var ballColor = new Color(255, 90, 90).AdditiveColor();
            Texture2D ball4 = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball4", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball1 = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball1", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(ball4, Projectile.Center - -(Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 18 - Main.screenPosition, null, ballColor * Projectile.Opacity * 0.4f, Projectile.rotation, ball4.Size() / 2, Projectile.scale * new Vector2(0.2f,0.4f), 0, 0);
                Main.spriteBatch.Draw(ball1, Projectile.Center - -(Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 30 - Main.screenPosition, null, ballColor * Projectile.Opacity * 0.5f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * new Vector2(0.15f,0.25f), 0, 0);
                Main.spriteBatch.Draw(ball1, Projectile.Center - -(Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 30 - Main.screenPosition, null, ballColor * Projectile.Opacity * 0.4f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * new Vector2(0.15f,0.25f), 0, 0);
            }

            return false;
        }
    }
}
