using AyaMod.Common.Easer;
using AyaMod.Content.Buffs;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using static AyaMod.Core.ModPlayers.AyaPlayer;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [PlayerEffect]
    public class GaleGeta2 : BaseAccessories, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(gold: 5));
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
                player.AddEffect<GaleGeta2>();
                player.moveSpeed += 0.05f;
                ayaPlayer.UltraMoveEnabled = true;
                ayaPlayer.AccSpeedModifier += 0.3f;
                ayaPlayer.WingTimeModifier.Flat += 10;//增加10帧飞行时间
            }

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<GaleGeta1>())
                .AddIngredient(ItemID.BeetleHusk, 3)
                .AddIngredient(ItemID.BlackBelt)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public static void AddDash(Player player)
        {
            var ayaPlayer = player.Aya();
            ayaPlayer.HasDash = true;
            ayaPlayer.AyaDash = DashType.GaleGeta2;
        }


        public static void GetaDash2(Player player, int direction)
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
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, player.velocity, ModContent.ProjectileType<GaleGetaDash2>(), damage, 0, player.whoAmI);

        }

        public static void WhileDashing2(Player player, int direction)
        {
            var modPlayer = player.Aya();
            if (modPlayer.DashDelay < 10) return;

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
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.dontTakeDamage || npc.friendly || !player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
                    continue;
                Rectangle targetRect = npc.getRect();

                if (rectangle.Intersects(targetRect) && (npc.noTileCollide || player.CanHit(npc)))
                {
                    float damage = player.GetTotalDamage<ReporterDamage>().ApplyTo(GaleGetaDamage);
                    float knockBack = player.GetTotalKnockback<ReporterDamage>().ApplyTo(9.5f);
                    bool crit = Main.rand.NextBool((int)player.GetTotalCritChance<ReporterDamage>(), 100);

                    if (player.whoAmI == Main.myPlayer)
                        player.ApplyDamageToNPC(npc, (int)damage, knockBack, direction, crit, ReporterDamage.Instance, false);
                    npc.immune[player.whoAmI] = 12;
                    player.GiveImmuneTimeForCollisionAttack(12);
                    
                }

            }

            if (player.HasBuff<GaleGetaCDBuff>())
                return;
            //闪避弹幕
            foreach (var projectile in Main.ActiveProjectiles)
            {
                var candamage = ProjectileLoader.CanDamage(projectile);
                if (candamage == null) candamage = true;
                if (projectile.friendly || projectile.damage <= 0 || (bool)!candamage || !CombinedHooks.CanBeHitByProjectile(player,projectile)) 
                    continue;
                if(!projectile.Colliding(projectile.GetHitbox(), rectangle))
                    continue;

                player.SetImmuneTimeForAllTypes(player.longInvince ? 90 : 60);
                player.AddBuff(BuffType<GaleGetaCDBuff>(), 1 * 60);
                break;
                //projectile.Kill();
            }
        }

        public static int DamageBoxSize = 84;
        public static int GaleGetaDamage = 175;
        public static int GaleGetaDashDelay = 40;
    }
    public class GaleGetaDash2 : ModProjectile
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
            Projectile.SetImmune(15);
            Projectile.penetrate = -1;
            Projectile.timeLeft = GaleGeta2.GaleGetaDashDelay * 2;
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
            if (Projectile.timeLeft < GaleGeta2.GaleGetaDashDelay * 2f * 5f / 8f)
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
            Projectile.rotation += 0.7f;


        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D sampler = TextureAssets.Extra[189].Value;
            Texture2D star = TextureAssets.Extra[98].Value;

            Effect effect = ShaderLoader.GetShader("Trail");

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float width = 100f;
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
                radius *= 1.3f;
                Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
                oldpos += Projectile.Size / 2;
                Vector2 dir = (innerRot + Projectile.rotation).ToRotationVector2();
                dir.X *= 0.5f;
                //dir = dir.RotatedBy(Projectile.rotation);
                Vector2 trailPos = oldpos + dir * radius;

                var color = Color.Lerp(Color.Black, Color.White, factor) * Projectile.Opacity;
                var alpha = EaseManager.Evaluate(Ease.InOutSine, factor, 1f) * 1f * Projectile.Opacity;
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
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor, Color.White, 0.3f).AdditiveColor() * Projectile.Opacity * 0.5f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.6f, 0, 0);
            Main.spriteBatch.Draw(ball1, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 6 - Main.screenPosition, null, Color.Lerp(ballColor, Color.White, 0.3f).AdditiveColor() * Projectile.Opacity * 0.6f, Projectile.rotation, ball1.Size() / 2, Projectile.scale * 0.4f, 0, 0);

            return true;
        }
    }
}
