using AyaMod.Common.Easer;
using AyaMod.Content.Buffs;
using AyaMod.Content.Items.Lens;
using AyaMod.Content.Particles;
using AyaMod.Content.ScreenEffects;
using AyaMod.Core;
using AyaMod.Core.Globals;
using AyaMod.Core.Loaders;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems;
using AyaMod.Core.Systems.Trails;
using AyaMod.Helpers;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace AyaMod.Content.Items.Cameras
{
    public class GalaxyProjector : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 200;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<GalaxyProjectorProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.07f, 188, 1.5f);
            SetCaptureStats(1000, 60);
        }
        public override void HoldItem(Player player)
        {
            int type = ProjectileType<CelestialMap>();
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, type, 0, 0, player.whoAmI);
            base.HoldItem(player);
        }
    }
    public class GalaxyProjectorProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(179, 0, 255);
        public override Color innerFrameColor => new Color(102, 0, 255) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(196, 243, 240).AdditiveColor() * 0.5f;
        public override ILens Lens => LensLoader.GetLens<CircleLens>();

        public override void OnHitNPCAlt(NPC target, NPC.HitInfo hit, int damageDone)
        {
            GalaxyPlayer galaxyPlayer = Main.player[Projectile.owner].GetModPlayer<GalaxyPlayer>();
            if (galaxyPlayer == null) return;

            int lightedCount = galaxyPlayer.ConstellationLevel;

            float progressIncrease = Main.rand.NextFloat(0.15f, 0.25f) * Utils.Remap(lightedCount, 0, 4, 2f, 1f) * 0.8f;
            int index = -1;
            while(index < 0 || (galaxyPlayer.ConstellationProgress[index] >= 1f && galaxyPlayer.ConstellationLevel < 5))
            {
                index = Main.rand.Next(5);
            }
            galaxyPlayer.ConstellationProgress[index] += progressIncrease;
        }
        public override void OnSnap()
        {
            bool canAttack = Main.rand.NextBool(5, 10);
            if (!canAttack) return;

            GalaxyPlayer galaxyPlayer = player.GetModPlayer<GalaxyPlayer>();
            int attackIndex = galaxyPlayer.ChooseToAttack();

            //Main.NewText($"{attackIndex} {Main.GameUpdateCount}");
            //attackIndex = 3;
            var source = Projectile.GetSource_FromAI();
            if (attackIndex >= 0)
            {
                //Console.WriteLine($"ConstellationAttack: {attackIndex}");
                galaxyPlayer.ConstellationAtkCD[attackIndex] = galaxyPlayer.ConstellationCDMax[attackIndex];
                switch (attackIndex)
                {
                    case (int)ConstellationType.Orion:
                        int orionDamage = Projectile.damage;
                        OrionAttack(5, source, galaxyPlayer.GetConstellationCenter(ConstellationType.Orion), orionDamage, Projectile.owner);
                        break;
                    case (int)ConstellationType.Cancer:
                        int cancerDamage = Projectile.damage;
                        CancerAttack(source, Projectile.Center, cancerDamage, Projectile.owner);
                        break;
                    case (int)ConstellationType.Hercules:
                        Vector2 herculesCenter = galaxyPlayer.GetConstellationCenter(ConstellationType.Hercules);
                        int herculesDamage = Projectile.damage;
                        HerculesAttack(source, herculesCenter, herculesDamage, Projectile.owner, herculesCenter.DirectionToSafe(Projectile.Center).ToRotation());
                        break;
                    case (int)ConstellationType.Pegasus:
                        int pegasusDamage = Projectile.damage;
                        PegasusAttack(source, galaxyPlayer.GetConstellationCenter(ConstellationType.Pegasus), pegasusDamage, Projectile.owner);
                        break;
                    case (int)ConstellationType.Lyra:
                        int lyraDamage = Projectile.damage;
                        LyraAttack(source, player.Center + GalaxyPlayer.lyraOffset, Projectile.Center, lyraDamage, Projectile.owner);
                        break;
                    default: break;

                }
            }

            #region 天马
            {
                for (int i = 0; i < 1; i++)
                {


                }
                //StarPerishParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, 60, 0.5f);
                //Vector2 pos = Projectile.Center;
                //Vector2 vel = new Vector2(0, -1f).RotatedByRandom(0.4f) * 8f;
                //Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ProjectileType<PegasusStarVisual>(), Projectile.damage, Projectile.knockBack, Projectile.owner);


                //Vector2 origin = Projectile.Center + new Vector2(Main.rand.Next(-200, 200), -1000);
                //Vector2 vel = origin.DirectionTo(Projectile.Center) * 12f;
                //Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), origin, vel, ProjectileType<PegasusMeteorStar>(), Projectile.damage, 0f, Projectile.owner, Projectile.Center.X, Projectile.Center.Y);
            }

            #endregion

            #region 天琴
            {


                //var flare = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, player.DirectionToSafe(Projectile.Center) * 1f, ProjectileType<LyraFlare>(), Projectile.damage, 0f, Projectile.owner);
            }

            #endregion

            int dustcount = 80;

            float range = 60;
            //for(int i = 0; i < dustcount; i++)
            //{
            //    float dist = Main.rand.NextFloat(range);
            //    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * dist;
            //    float speed = (dist / range * 7 + 3) * 0.3f;
            //    Vector2 vel = Projectile.Center.DirectionToSafe(pos) * speed;
            //    float alpha = Main.rand.NextFloat(0.5f, 1f) * 0.5f;
            //    float scale = Main.rand.NextFloat(1, 2f);
            //    Color baseColor = Color.Lerp(new Color(138, 43, 226), new Color(75, 0, 130), Main.rand.NextFloat());
            //    if (dist / range < 0.5f) baseColor = Color.Lerp(baseColor, Color.Black, 0.6f);
            //    Color color = baseColor * (alpha / scale);
            //    float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            //    Smoke_UnholyTwisting.Spawn(Projectile.GetSource_FromAI(), pos, vel, color, scale * 0.8f, rot, (int)(dist / range * 30 + 60), 0.98f);
            //}
        }

        public static void OrionAttack(int count, IEntitySource source, Vector2 pos, int dmg, int owner, float startSpeed = 5f)
        {
            int type = ProjectileType<OrionComet>();
            for (int i = 0; i < count; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 3) * startSpeed;
                Projectile.NewProjectileDirect(source, pos, vel, type, dmg, 0f, owner);
            }
        }
        public static void CancerAttack(IEntitySource source, Vector2 pos, int dmg, int owner)
        {
            float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
            float radius = 250;
            float speed = 7.2f;
            float rotSpeed = speed / radius;

            float a = 350;
            float b = 100;
            int type = ProjectileType<CancerBlackHole>();
            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = (startRot + MathHelper.Pi * i).ToRotationVector2();
                Vector2 vel = dir.RotatedBy(MathHelper.PiOver2) * speed;

                var p = Projectile.NewProjectileDirect(source, pos, vel, type, dmg, 0, owner, a, b);
                p.localAI[2] = 0.01f;
                (p.ModProjectile as CancerBlackHole).Phase = i * MathHelper.Pi;
                (p.ModProjectile as CancerBlackHole).Blue = i > 0;
            }
        }
        public static void HerculesAttack(IEntitySource source, Vector2 pos, int dmg, int owner, float rot)
        {
            int type = ProjectileType<HerculesLaser>();
            var p = Projectile.NewProjectileDirect(source, pos, Vector2.Zero, type, dmg, 0, owner);
            p.rotation = rot;
        }
        public static void PegasusAttack(IEntitySource source, Vector2 pos, int dmg, int owner)
        {
            PegasusRail.SpawnRail(source, pos, 60, 2 * 60, 80, dmg, owner, 0.8f, 3);
        }
        public static void LyraAttack(IEntitySource source, Vector2 lyrapos, Vector2 camerapos, int dmg, int owner, float radius = 800f)
        {
            var circles = Helper.FindCircleCenters(lyrapos, camerapos, radius);
            if (circles.Count > 0)
            {
                Vector2 start = Helper.GetClockwiseStartOfMinorArc(circles[0], lyrapos, camerapos);
                bool clockwise = start == lyrapos;
                Vector2 end = clockwise ? camerapos : lyrapos;
                var a1 = circles[0].AngleTo(start);
                var a2 = circles[0].AngleTo(end);
                var l = LyraMusicalStaff.Spawn(source, circles[0], 60, 3 * 60, radius, dmg, owner, (MathHelper.PiOver2) * clockwise.ToDirectionInt());
                l.Projectile.rotation = l.Projectile.Center.AngleTo(lyrapos);
            }
        }
    }
    public class CelestialMap : ModProjectile, ISinkProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Galaxy1";

        public ref float State => ref Projectile.ai[0];
        public ref float BorderRadius => ref Projectile.localAI[0];

        public float SinkDepth => 2;

        public float[] StarX;
        public float[] StarY;
        public static int starCount = 250;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3 * 60;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0f;
            Projectile.scale = 1f;
            StarX = new float[starCount];
            StarY = new float[starCount];
            for (int i = 0; i < starCount; i++)
            {
                StarX[i] = Main.rand.NextFloat(-Projectile.width / 2, Projectile.width / 2);
                StarY[i] = Main.rand.NextFloat(-Projectile.height / 2, Projectile.height / 2);
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player == null || !player.Alive()) return;

            if (player.HeldItem.type != ItemType<GalaxyProjector>() && State != 1)
            {
                State = 1;
                Projectile.timeLeft = 3 * 60;
            }

            float factor = Projectile.TimeleftFactor();

            switch (State)
            {
                case 0:

                    if (factor > 0.8f)
                    {
                        float sinFactor = (float)MathF.Sin(Utils.Remap(factor, 0.8f, 1f, 0f, -MathHelper.TwoPi / 3f)) * 0.5f;
                        BorderRadius = sinFactor;
                    }
                    else
                    {
                        float innerFactor = Utils.Remap(factor, 0f, 0.8f, 0f, 1f);
                        float sin = (float)Math.Sin((1 - innerFactor) * 2f * MathF.PI) * 0.5f;
                        float ease = EaseManager.Evaluate(Ease.OutQuart, 1f - innerFactor, 1f);
                        float value = MathHelper.Lerp(sin, ease, Utils.Remap(innerFactor, 0.5f, 0.7f, 1f, 0f));
                        BorderRadius = ease;
                    }
                    if (Projectile.timeLeft < 2) Projectile.timeLeft++;
                    Projectile.Center = player.Center + new Vector2(0, player.gfxOffY);
                    Projectile.Opacity += 0.008f;
                    if (Projectile.Opacity > 1f) Projectile.Opacity = 1f;
                    Projectile.scale += 0.03f;
                    if (Projectile.scale > 1f) Projectile.scale = 1f;

                    break;
                //淡出
                case 1:
                    float dFactor = Utils.Remap(factor, 0.6f, 1f, 0f, 1f);
                    BorderRadius = dFactor;
                    Projectile.Opacity = dFactor;
                    if (BorderRadius < 0.01f) Projectile.Kill();
                    break;
                default: break;
            }

            //Main.NewText($"{factor} {Projectile.timeLeft}");

            Projectile.rotation += 0.002f;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            var galaxyShader = ShaderLoader.GetShader("MaskEffect");
            if (galaxyShader == null)
                return false;
            Player player = Main.player[Projectile.owner];
            GalaxyPlayer galaxyPlayer = player.GetModPlayer<GalaxyPlayer>();


            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D block = TextureAssets.MagicPixel.Value;
            Texture2D star = Request<Texture2D>(AssetDirectory.Extras + "Ball", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = Request<Texture2D>(AssetDirectory.Extras + "Ball5", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball = Request<Texture2D>(AssetDirectory.Extras + "Ball5_1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "GFX_clouds1_sparseMotes", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D gradient = Request<Texture2D>(AssetDirectory.Extras + "GalaxyColorMap3", AssetRequestMode.ImmediateLoad).Value;
            Texture2D gradient = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;

            Color gold = new Color(255, 215, 140);

            float borderRadius = Utils.Remap(MathF.Abs(BorderRadius), 0, 1f, 0, 0.4f);
            float r = borderRadius * Projectile.width;

            Rectangle rect = new Rectangle((int)(Projectile.Center.X - Main.screenPosition.X), (int)(Projectile.Center.Y - Main.screenPosition.Y),
                Projectile.width, Projectile.height);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, RenderHelper.ReverseSubtract, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.5f, Projectile.rotation, ball.Size() / 2f, Projectile.width / (float)ball.Width * 1.3f * BorderRadius, 0, 0);

            Main.graphics.GraphicsDevice.Textures[1] = mask;
            Main.graphics.GraphicsDevice.Textures[3] = gradient;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);



            galaxyShader.Parameters["drawInner"].SetValue(BorderRadius > 0f);
            galaxyShader.Parameters["uShape"].SetValue(shape);
            galaxyShader.Parameters["extraU"].SetValue(StarX[0] * 2f);
            galaxyShader.Parameters["extraV"].SetValue(StarY[0] * 2f);
            galaxyShader.Parameters["scale"].SetValue(2f);
            galaxyShader.Parameters["borderWidth"].SetValue(0f);
            galaxyShader.Parameters["borderFeatherWidth"].SetValue(20f);
            galaxyShader.Parameters["borderRadius"].SetValue(borderRadius/*MathF.Sin(Main.GameUpdateCount * 0.02f) * 0.2f + 0.25f*/);
            galaxyShader.Parameters["size"].SetValue(Projectile.width);
            galaxyShader.Parameters["borderColor"].SetValue(gold.ToVector4());
            galaxyShader.Parameters["borderInnerColor"].SetValue(new Vector4(0.15f, 0.15f, 0.15f, 0.3f));
            galaxyShader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.6f, 0, texture.Size() / 2, Projectile.width / (float)1535, 0, 0);

            if (BorderRadius > 0f)
            {
                var distanceMask = ShaderLoader.GetShader("DistanceCutter");
                distanceMask.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                distanceMask.Parameters["uCenter"].SetValue(Projectile.Center - Main.screenPosition);
                distanceMask.Parameters["radius"].SetValue(r);

                distanceMask.CurrentTechnique.Passes[0].Apply();

                for (int i = 0;i < 5; i++)
                {
                    ConstellationType type = (ConstellationType)i;
                    Vector2 center = galaxyPlayer.GetConstellationCenter(type);
                    Vector2[] poses = GalaxyPlayer.GetConstellationPoses(type);
                    Vector2[] edges = GalaxyPlayer.GetConstellationEdges(type);
                    float scale = GalaxyPlayer.GetConstellationScale(type);
                    float rot = GalaxyPlayer.GetConstellationRotation(type);
                    float consAlpha = galaxyPlayer.ConstellationProgress[i];
                    Color baseColor = galaxyPlayer.ConstellationTimeleft[i] > 0 ? new Color(250, 250, 94) : Color.SkyBlue;
                    Color consColor = baseColor * Projectile.Opacity * 0.5f * consAlpha;
                    DrawEdges(center, poses, edges, 1f, scale, rot, consColor);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (BorderRadius > 0f)
            {
                for (int i = 0; i < 5; i++)
                {
                    ConstellationType type = (ConstellationType)i;
                    Vector2[] poses = GalaxyPlayer.GetConstellationPoses(type);
                    Vector2 offset = GalaxyPlayer.GetConstellationOffset(type) - Main.screenPosition;
                    float scale = GalaxyPlayer.GetConstellationScale(type);
                    float rot = GalaxyPlayer.GetConstellationRotation(type);
                    Color color = Color.LightSkyBlue * Projectile.Opacity;
                    DrawStars(poses, Projectile.Center, offset, scale, rot, color, r);
                }

                Color starColor = Color.White;
                for (int i = 0; i < starCount; i++)
                {
                    float x = StarX[i];
                    float y = StarY[i];
                    if (x * x + y * y < r * r)
                    {
                        float alpha = MathF.Sin((float)(Main.timeForVisualEffects * 0.06f + i * 24098f)) * 0.4f + 0.5f;
                        Main.spriteBatch.Draw(star, new Vector2(x, y) + Projectile.Center - Main.screenPosition, null, starColor.AdditiveColor() * Projectile.Opacity * alpha, (float)(Main.timeForVisualEffects * 0.02f), star.Size() / 2, 0.1f * (MathF.Sin(i * 41701) * 0.25f + 1f), 0, 0);
                    }
                }
            }
            return false;
        }
        public static void DrawStars(Vector2[] poses, Vector2 center, Vector2 offset, float scale, float rot, Color color, float r)
        {
            Texture2D texture = Request<Texture2D>(AssetDirectory.Extras + "Ball4_1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D bloom = Request<Texture2D>(AssetDirectory.Extras + "Ball7_1", AssetRequestMode.ImmediateLoad).Value;
            Vector2 origin = texture.Size() / 2;
            Vector2 bloomOrigin = bloom.Size() / 2;
            float baseScale = 0.15f;
            Vector2 drawScale = new Vector2(0.07f, 1f) * baseScale;
            float alpha = MathF.Sin((float)(Main.timeForVisualEffects * 0.05f)) * 0.25f + 0.75f;
            for (int i = 0; i < poses.Length; i++)
            {
                float starScale = MathF.Sin(i * 36521) * 0.4f + 0.6f;
                Vector2 drawpos = center + offset + poses[i].RotatedBy(rot) * scale;
                if (Vector2.Distance(center - Main.screenPosition, drawpos) >= r) continue;
                for (int j = 0; j < 2; j++)
                {
                    Main.spriteBatch.Draw(texture, drawpos, null, color * alpha, j * MathHelper.PiOver2, origin, drawScale * starScale, 0, 0);
                }
                Main.spriteBatch.Draw(bloom, drawpos, null, Color.White * 0.3f * alpha, 0, bloomOrigin, baseScale * 0.7f * starScale, 0, 0);
                Main.spriteBatch.Draw(bloom, drawpos, null, Color.White * 0.7f * alpha, 0, bloomOrigin, baseScale * 0.3f * starScale, 0, 0);
            }
        }
        public static void DrawEdges(Vector2 center, Vector2[] poses, Vector2[] edges, float width, float scale, float rot, Color color)
        {
            for (int i = 0; i < edges.Length; i++)
            {
                Vector2 drawpos = center + poses[(int)edges[i].X].RotatedBy(rot) * scale;
                Vector2 drawpos2 = center + poses[(int)edges[i].Y].RotatedBy(rot) * scale;

                Utils.DrawLine(Main.spriteBatch, drawpos, drawpos2, color, color, width);
            }
        }
    }
    public enum ConstellationType
    {
        //猎户座
        Orion,
        //巨蟹座
        Cancer,
        //武仙座
        Hercules,
        //天马座
        Pegasus,
        //天琴座
        Lyra
    }
    public class GalaxyPlayer : ModPlayer
    {

        public float[] ConstellationProgress = new float[5];
        /// <summary>
        /// 星座点亮剩余时间，每帧递减，到达0后熄灭
        /// </summary>
        public int[] ConstellationTimeleft = new int[5];
        /// <summary>
        /// 已点亮星座个数
        /// </summary>
        public int ConstellationLevel => ConstellationProgress.Count(p => p >= 1f);

        public int[] ConstellationAtkCD = new int[5];
        public int[] ConstellationCDMax = [0, 10 * 60, 6 * 60, 1 * 60, 3 * 60];

        public static Vector2 pegasusOffset = new(-270, 100);
        public static float pegasusScale = 1.2f, pegasusRot = 0.3f;

        public static Vector2 lyraOffset = new(-150, -150);
        public static float lyraScale = 1.1f, lyraRot = 0f;

        public static Vector2 hercOffset = new(10, -310);
        public static float hercScale = 1.4f, hercRot = 0f;

        public static Vector2 orionOffset = new Vector2(310, 0); 
        public static float orionScale = 0.8f, orionRot = -0.5f;

        public static Vector2 cancerOffset = new Vector2(60, 330); 
        public static float cancerScale = 1f, cancerRot = 0.5f;

        //640,760
        public static Vector2[] HerculesPoses =
        [
            new Vector2(-33,97),
            new Vector2(-61,54),
            new Vector2(-53,44),
            new Vector2(-34,42),
            new Vector2(-13,37),
            new Vector2(8,29),
            new Vector2(54,69),
            new Vector2(58,-3),
            new Vector2(73,-4),
            new Vector2(93,13),
            new Vector2(100,29),
            new Vector2(9,-26),
            new Vector2(-3,-9),
            new Vector2(-37,-21),
            new Vector2(-77,3),
            new Vector2(-84,-49),
            new Vector2(-19,-50),
            new Vector2(-25,-69),
            new Vector2(-39,-91),
            new Vector2(-18,-93)
        ];
        public static Vector2[] HerculesEdges =
        [
            new Vector2(0,1),
            new Vector2(1,2),
            new Vector2(2,3),
            new Vector2(3,4),
            new Vector2(4,5),
            new Vector2(5,6),
            new Vector2(6,7),
            new Vector2(7,8),
            new Vector2(8,9),
            new Vector2(9,10),
            new Vector2(7,11),
            new Vector2(11,12),
            new Vector2(12,13),
            new Vector2(13,14),
            new Vector2(14,15),
            new Vector2(11,16),
            new Vector2(13,16),
            new Vector2(16,17),
            new Vector2(17,18),
            new Vector2(18,19),
            new Vector2(5,12)
        ];

        //120,200
        public static Vector2[] OrionPoses =
        [
            new Vector2(-50,144),
            new Vector2(45,134),
            new Vector2(-25,58),
            new Vector2(-12,51),
            new Vector2(1,42),
            new Vector2(25,-32),
            new Vector2(111,-70),
            new Vector2(122,-56),
            new Vector2(123,-34),
            new Vector2(119,-18),
            new Vector2(111,16),
            new Vector2(94,24),
            new Vector2(-1,-74),
            new Vector2(-58,-49),
            new Vector2(-76,-76),
            new Vector2(-85,-136),
            new Vector2(-44,-195)
        ];
        public static Vector2[] OrionEdges =
        {
            new Vector2(0,2),
            new Vector2(2,3),
            new Vector2(3,4),
            new Vector2(4,5),
            new Vector2(1,4),
            new Vector2(5,8),
            new Vector2(6,7),
            new Vector2(7,8),
            new Vector2(8,9),
            new Vector2(9,10),
            new Vector2(10,11),
            new Vector2(5,12),
            new Vector2(2,13),
            new Vector2(12,13),
            new Vector2(13,14),
            new Vector2(14,15),
            new Vector2(15,16),
        };

        //660,700
        public static Vector2[] CancerPoses =
        [
            new Vector2(-29,36),
            new Vector2(-1,-12),
            new Vector2(13,-15),
            new Vector2(41,-34),
            new Vector2(-35,-15)
        ];
        public static Vector2[] CancerEdges =
        [
            new Vector2(0,1),
            new Vector2(1,2),
            new Vector2(2,3),
            new Vector2(1,4),
        ];

        //680,790
        public static Vector2[] PegasusPoses = [
            new Vector2(97,-46),
            new Vector2(100,-17),
            new Vector2(59,8),
            new Vector2(48,11),
            new Vector2(21,21),
            new Vector2(-51,99),
            new Vector2(-100,20),
            new Vector2(-34,-27),
            new Vector2(-25,-52),
            new Vector2(-15,-88),
            new Vector2(-8,-26),
            new Vector2(-2,-26),
            new Vector2(18,-69),
            new Vector2(30,-90)
        ];

        public static Vector2[] PegasusEdges = [
            new Vector2(0,1),
            new Vector2(1,2),
            new Vector2(2,3),
            new Vector2(3,4),
            new Vector2(4,5),
            new Vector2(5,6),
            new Vector2(6,7),
            new Vector2(7,8),
            new Vector2(8,9),
            new Vector2(4,7),
            new Vector2(7,10),
            new Vector2(10,11),
            new Vector2(11,12),
            new Vector2(12,13)
        ];

        //510,800
        public static Vector2[] LyraPoses =
        [
            new Vector2(2,13),
            new Vector2(-8,22),
            new Vector2(-11,-8),
            new Vector2(0,-16),
            new Vector2(7,-26),
        ];
        public static Vector2[] LyraEdges = [
            new Vector2(0,1),
            new Vector2(1,2),
            new Vector2(2,3),
            new Vector2(3,4),
            new Vector2(0,3),
        ];

        /// <summary>
        /// 选择一个可以发动攻击的星座
        /// </summary>
        /// <returns>返回星座type，-1为没有可以发动攻击的星座</returns>
        public int ChooseToAttack()
        {
            List<int> choices = new List<int>();
            for(int i = 0;i < ConstellationProgress.Length;i++)
            {
                if(ConstellationProgress[i] >= 1f && ConstellationAtkCD[i] <= 1f)
                {
                    choices.Add(i);
                }
            }
            if(choices.Count == 0)
            {
                return -1;
            }
            return Main.rand.Next(choices);
        }
        /// <summary>
        /// 获取星座的绝对位置（加上玩家后的）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Vector2 GetConstellationCenter(ConstellationType type) => GetConstellationOffset(type) + Player.Center;
        /// <summary>
        /// 获取星座的相对位置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Vector2 GetConstellationOffset(ConstellationType type) => type switch
        {
            ConstellationType.Orion => orionOffset,
            ConstellationType.Cancer => cancerOffset,
            ConstellationType.Hercules => hercOffset,
            ConstellationType.Pegasus => pegasusOffset,
            ConstellationType.Lyra => lyraOffset,
            _ => Vector2.Zero
        };
        public static Vector2[] GetConstellationPoses(ConstellationType type) => type switch
        {
            ConstellationType.Orion => OrionPoses,
            ConstellationType.Cancer => CancerPoses,
            ConstellationType.Hercules => HerculesPoses,
            ConstellationType.Pegasus => PegasusPoses,
            ConstellationType.Lyra => LyraPoses,
            _ => []
        };
        public static Vector2[] GetConstellationEdges(ConstellationType type) => type switch
        {
            ConstellationType.Orion => OrionEdges,
            ConstellationType.Cancer => CancerEdges,
            ConstellationType.Hercules => HerculesEdges,
            ConstellationType.Pegasus => PegasusEdges,
            ConstellationType.Lyra => LyraEdges,
            _ => []
        };
        public static float GetConstellationScale(ConstellationType type) => type switch
        {
            ConstellationType.Orion => orionScale,
            ConstellationType.Cancer => cancerScale,
            ConstellationType.Hercules => hercScale,
            ConstellationType.Pegasus => pegasusScale,
            ConstellationType.Lyra => lyraScale,
            _ => 1f
        };
        public static float GetConstellationRotation(ConstellationType type) => type switch
        {
            ConstellationType.Orion => orionRot,
            ConstellationType.Cancer => cancerRot,
            ConstellationType.Hercules => hercRot,
            ConstellationType.Pegasus => pegasusRot,
            ConstellationType.Lyra => lyraRot,
            _ => 0f
        };
        public override void PreUpdateBuffs()
        {
            for(int i = 0;i < 5;i++)
            {
                if (ConstellationTimeleft[i] > 0)
                {
                    if (ConstellationTimeleft[i] == 1)
                        ConstellationProgress[i] = 0;
                    ConstellationTimeleft[i]--;
                }

                ConstellationProgress[i] = MathHelper.Clamp(ConstellationProgress[i], 0f, 1f);
                if (ConstellationProgress[i] > 0.9999f && ConstellationTimeleft[i] <= 0)
                {
                    ConstellationTimeleft[i] = 25 * 60;
                }

                if (ConstellationTimeleft[i] > 0)
                    ApplyConstellationBuff((ConstellationType)i);

                if (ConstellationAtkCD[i] > 0) ConstellationAtkCD[i]--;
            }
        }
        public void ApplyConstellationBuff(ConstellationType type)
        {
            int bufftype = type switch
            {
                ConstellationType.Orion => BuffType<OrionBuff>(),
                ConstellationType.Cancer => BuffType<CancerBuff>(),
                ConstellationType.Hercules => BuffType<HerculesBuff>(),
                ConstellationType.Pegasus => BuffType<PegasusBuff>(),
                ConstellationType.Lyra => BuffType<LyraBuff>(),
                _ => -1
            };
            if (bufftype < 0) return;

            Player.AddBuff(bufftype, 2);
        }
    }

    public class OrionComet : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaProjPath(79);
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 30);
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10 * 60;
        }
        public override bool? CanDamage() => Projectile.ai[2] >= 0;
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
                return;
            }

            if (factor < 0.95f)
            {
                if (!Projectile.Chase(2000, 29, 0.05f))
                {
                    Projectile.velocity += Projectile.velocity.Length(0.5f);
                }
                Vector2 normal = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitX);
                Vector2 disturb = normal * MathF.Sin(Main.GameUpdateCount * 0.1f + Projectile.whoAmI) * 0.8f;
                if (Projectile.velocity.Length() < 2f) disturb /= 2f;
                Projectile.velocity += disturb;
            }
            else
            {
                Projectile.velocity = Projectile.velocity * 0.96f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < 1; i++)
            {

                //Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 59, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
                //d.noGravity = true;
                //d.scale = 1.5f;
                float posfactor = Main.rand.NextFloat();
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * posfactor * Projectile.width / 2;
                float speedFactor = 0.15f * Utils.Remap(posfactor, 0, 1f, 1f, 0.2f);
                LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, Projectile.velocity * speedFactor, Main.hslToRgb(238f / 360f, 1f, 0.7f), 30);
            }

        }
        public void EndMove()
        {
            Projectile.penetrate++;
            Projectile.timeLeft = 20;
            Projectile.ai[2] = -1;

            StarSparkParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, new Color(40, 40, 135) with { A = 127 }, 20, 2f);

            StarSparkParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, Color.White.AdditiveColor(), 20, 1f);

            var p = StarPerishParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, 30, 0.3f);
            p.Rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            float speed = 10;
            for (int i = 0; i < 25; i++)
            {
                float distFactor = Main.rand.NextFloat(0.3f, 1f);
                Vector2 dir = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                Vector2 pos = Projectile.Center + dir * distFactor * 10;
                Vector2 vel = dir * speed * distFactor;
                float scaleFactor = Main.rand.NextFloat(0.5f, 1.5f) * 0.7f;
                float velmult = Main.rand.NextFloat(0.88f, 0.94f);
                float scaleMult = Main.rand.NextFloat(0.92f, 0.99f);
                var l2 = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.DarkBlue, 60, true);
                l2.SetVelMult(velmult);
                l2.SetScaleMult(scaleMult);
                l2.Scale = 3f * scaleFactor;
            }

            //for (int i = 0; i < 20; i++)
            //{
            //    Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 3) * 3;
            //    Dust d = Dust.NewDustDirect(Projectile.position - new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.width * 2, Projectile.height * 2, DustID.PurpleTorch, vel.X, vel.Y, Scale: 2f);
            //    d.noGravity = true;
            //}
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Texture2D line = TextureAssets.Extra[197].Value;
            Texture2D starspot = Request<Texture2D>(AssetDirectory.Extras + "Ball4_1", AssetRequestMode.ImmediateLoad).Value;



            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "Laser2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = TextureAssets.Extra[193].Value;

            Effect effect = ShaderLoader.GetShader("Trail");

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float width = 13 * Projectile.scale;

            int interval = 5;
            int lineoffset = (int)(Main.GameUpdateCount % interval);
            bool first = true;

            for (int i = 0; i < Projectile.oldPos.Length - 2; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i + 1] == Vector2.Zero) continue;

                var normal = Projectile.oldPos[i + 1] - Projectile.oldPos[i];
                if (normal.Length() < 0.1f) continue;
                normal = normal.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);

                var factor = 1 - i / (float)Projectile.oldPos.Length;//0是轨迹尾部,1是轨迹头部

                if (i % interval == lineoffset)
                {
                    int next = i + interval;
                    if (next < Projectile.oldPos.Length - 1 && Projectile.oldPos[next] != Vector2.Zero)
                    {
                        float maxoffset = (MathF.Sin((float)(Main.timeForVisualEffects * 0.035f + Projectile.whoAmI)) * 0.5f + 0.5f) * 30;

                        Vector2 startpos = Projectile.oldPos[i] + Projectile.Size / 2 + normal * MathF.Sin(Projectile.oldPos[i].X * 1293 + Projectile.oldPos[i].Y * -49711) * maxoffset;
                        Vector2 n1 = Projectile.oldPos[next] - Projectile.oldPos[next - 1];
                        n1 = n1.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitX);
                        Vector2 endpos = Projectile.oldPos[next] + Projectile.Size / 2 + n1 * MathF.Sin(Projectile.oldPos[next].X * 1293 + Projectile.oldPos[next].Y * -49711) * maxoffset;
                        float rot = startpos.AngleTo(endpos);
                        float scale = startpos.Distance(endpos) / (float)line.Height;
                        float alpha = (factor < 0.5f ? factor - 0.1f : factor) * Projectile.Opacity;
                        Color lineColor = Color.SkyBlue * alpha;
                        Utils.DrawLine(Main.spriteBatch, startpos, endpos, lineColor, lineColor, 1f);
                        //Main.spriteBatch.Draw(line, Vector2.Lerp(startpos, endpos, 0.5f) - Main.screenPosition, null, Color.LightSkyBlue.AdditiveColor(), rot, line.Size() / 2, new Vector2(scale, 0.2f), 0, 0);
                        Main.spriteBatch.Draw(starspot, endpos - Main.screenPosition, null, Color.White.AdditiveColor() * alpha, 0, starspot.Size() / 2, 0.07f, 0, 0);
                        Main.spriteBatch.Draw(starspot, endpos - Main.screenPosition, null, Color.White.AdditiveColor() * alpha, 0, starspot.Size() / 2, 0.04f, 0, 0);
                        if (first)
                        {
                            Main.spriteBatch.Draw(starspot, startpos - Main.screenPosition, null, Color.White.AdditiveColor() * alpha, 0, starspot.Size() / 2, 0.07f, 0, 0);
                            Main.spriteBatch.Draw(starspot, startpos - Main.screenPosition, null, Color.White.AdditiveColor() * alpha, 0, starspot.Size() / 2, 0.04f, 0, 0);
                            first = false;
                        }
                    }
                }

                {
                    Color color = Color.Lerp(Color.Black, Color.White, factor);
                    var alpha = (EaseManager.Evaluate(Ease.InQuint, factor, 1f) + 0.15f) * 0.8f * Projectile.Opacity;
                    float aWidth = MathHelper.Lerp(10f, width, factor);
                    Vector2 top = Projectile.oldPos[i] + Projectile.Size / 2 + normal * aWidth;
                    Vector2 bottom = Projectile.oldPos[i] + Projectile.Size / 2 - normal * aWidth;
                    bars.Add(new(top, color, new Vector3((float)Math.Sqrt(factor) * 1.5f, 1, alpha)));
                    bars.Add(new(bottom, color, new Vector3((float)Math.Sqrt(factor) * 1.5f, 0, alpha)));
                }
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;


                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
                var model = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                var view = Main.GameViewMatrix.TransformationMatrix;
                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * view * projection);
                effect.Parameters["timer"].SetValue((float)Main.timeForVisualEffects * 0.02f);
                effect.Parameters["maskFactor"].SetValue(0.8f);
                effect.Parameters["preMult"].SetValue(true);
                effect.Parameters["colorMult"].SetValue(2f);
                Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
                Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
                Main.graphics.GraphicsDevice.Textures[2] = mask;//蒙版
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }

            if (Projectile.ai[2] >= 0)
            {
                float floatScale = MathF.Sin((float)Main.timeForVisualEffects * 0.1f) * 0.1f + 1f;
                float floatScale2 = MathF.Cos((float)Main.timeForVisualEffects * 0.1f) * 0.08f + 1f;
                float baseScale = Projectile.scale * 1.1f;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = (MathHelper.TwoPi / 4f * i + (float)Main.timeForVisualEffects * 0.08f).ToRotationVector2() * 2 * floatScale;
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + offset, null, Color.MediumBlue.AdditiveColor() * 0.1f * Projectile.Opacity, 0, texture.Size() / 2, baseScale * floatScale * 1.5f, 0);
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + offset, null, Color.LightSkyBlue.AdditiveColor() * 0.1f * Projectile.Opacity, 0, texture.Size() / 2, baseScale * floatScale * 1.25f, 0);
                }
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, texture.Size() / 2, baseScale * floatScale2, 0);
            }

            return false;
        }
    }
    public class BlackHoleSystem : ModSystem
    {
        public struct BlackHoleData
        {
            public Vector2 ScreenPosition;
            public float Mass;
            public float Radius;
            public float EventHorizonSize;
            public float RotationSpeed;
        }

        public static bool anyBlackHole = false;
        public static List<BlackHoleData> activeBlackHoles = new List<BlackHoleData>();
        public static void AddBlackHole(BlackHoleData data)
        {
            if (activeBlackHoles.Count < 8)
                activeBlackHoles.Add(data);
        }
        public static void RemoveBlackHole(BlackHoleData data)
        {
            activeBlackHoles.Remove(data);
        }
        public override void PreUpdateProjectiles()
        {
            activeBlackHoles.Clear();
        }
        public override void PostUpdateProjectiles()
        {
            anyBlackHole = Main.projectile.Any(n => n.active && n.type == ProjectileType<CancerBlackHole>());
            if (!anyBlackHole) return;
            //BlackHoleScreen.Active = anyBlackHole;
        }
    }
    public class CancerBlackHole : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public ref float A => ref Projectile.ai[0];
        public ref float B => ref Projectile.ai[1];
        public ref float Radius => ref Projectile.ai[2];

        public ref float OriginalPosX => ref Projectile.localAI[0];
        public ref float OriginalPosY => ref Projectile.localAI[1];
        public ref float RotationSpeed => ref Projectile.localAI[2];

        public float Phase;
        public bool Blue = true;

        public override void Load()
        {
            CustomDrawer.FilterDrawer += BlackHoleDrawer;
        }
        public override void Unload()
        {
            CustomDrawer.FilterDrawer -= BlackHoleDrawer;
        }

        public static void BlackHoleDrawer()
        {
            if (!BlackHoleSystem.anyBlackHole) return;
            var gd = Main.graphics.GraphicsDevice;
            var sb = Main.spriteBatch;
            RenderTarget2D render2 = Main.screenTargetSwap;
            //return;
            {

                var shader = ShaderLoader.GetShader("BlackHole3");

                gd.SetRenderTarget(RenderHelper.render);
                gd.Clear(Color.Transparent);

                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                sb.End();

                gd.SetRenderTarget(Main.screenTarget);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                Vector2 screenResolution = new Vector2(Main.screenWidth, Main.screenHeight);//分辨率
                float aspectRatio = screenResolution.X / (float)screenResolution.Y;
                shader.Parameters["uScreenResolution"].SetValue(screenResolution);
                shader.Parameters["aspectRatio"].SetValue(aspectRatio);
                shader.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.1f);
                //shader.Parameters["distortionStrength"].SetValue(0.2f);
                //shader.Parameters["glowm"].SetValue(3f);

                int count = Math.Min(BlackHoleSystem.activeBlackHoles.Count, 8);
                shader.Parameters["ActiveHolesCount"].SetValue(count);
                for (int i = 0; i < count; i++)
                {
                    BlackHoleSystem.BlackHoleData hole = BlackHoleSystem.activeBlackHoles[i];

                    float eventHorizonSize = hole.EventHorizonSize / screenResolution.Y;

                    // 获取黑洞参数结构
                    EffectParameter holeParam = shader.Parameters["Holes"].Elements[i];
                    holeParam.StructureMembers["Position"].SetValue(hole.ScreenPosition);
                    holeParam.StructureMembers["Mass"].SetValue(hole.Mass * 1.2f);
                    holeParam.StructureMembers["Radius"].SetValue(hole.Radius / screenResolution.Y * 1.4f);
                    holeParam.StructureMembers["EventHorizonSize"].SetValue(eventHorizonSize);
                    holeParam.StructureMembers["GlowRadius"].SetValue(6f);
                    //holeParam.StructureMembers["GlowInnerRadius"].SetValue(eventHorizonSize * 1.05f);
                    //holeParam.StructureMembers["GlowOuterRadius"].SetValue(eventHorizonSize * 1.5f);
                    //holeParam.StructureMembers["GlowIntensity"].SetValue(0.5f);
                    //holeParam.StructureMembers["GlowPower"].SetValue(1f);
                    //holeParam.StructureMembers["GlowFrequency"].SetValue(0.4f);
                    holeParam.StructureMembers["RotationSpeed"].SetValue(hole.RotationSpeed);
                    holeParam.StructureMembers["AccretionColor"].SetValue(new Vector3(1, 0.9f, 0.5f));
                    holeParam.StructureMembers["FalloffPower"].SetValue(10f);
                }

                shader.CurrentTechnique.Passes[0].Apply();
                sb.Draw(RenderHelper.render, Vector2.Zero, Color.White);
                sb.End();
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (var p in Main.ActiveProjectiles)
            {
                if (p.type != ProjectileType<CancerBlackHole>()) continue;
                (p.ModProjectile as CancerBlackHole).DrawBloom(p.ai[2] * 1f);
                //(p.ModProjectile as CancerBlackHole).DrawHalfDisc(false);
            }
            Main.spriteBatch.End();
        }
        public static float GetEdgeGlow()
        {
            // 基于游戏时间变化的动态光效
            return 0.7f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.3f;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = (int)((256 + 512) / 1.5f);
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(10);
            Projectile.timeLeft = 10 * 60;
        }
        public override void OnSpawn(IEntitySource source)
        {
            OriginalPosX = Projectile.Center.X;
            OriginalPosY = Projectile.Center.Y;
        }
        public override void AI()
        {
            //BlackHoleScreen.Active = true;
            //Projectile.Center = Main.MouseWorld;
            float factor = Projectile.TimeleftFactor();
            float maxtime = Projectile.GetGlobalProjectile<AyaGlobalProjectile>().MaxTimeleft;
            float fadetime = 60f / maxtime;

            float maxRadius = 80f;

            if (factor > 1 - fadetime)
            {
                float value = EaseManager.Evaluate(Ease.OutQuint, Utils.Remap(factor, 1 - fadetime, 1f, 1f, 0f), 1f);
                Radius = value * maxRadius;
            }
            else
            {
                float value = EaseManager.Evaluate(Ease.OutQuint, Utils.Remap(factor, 0f, fadetime, 0f, 1f), 1f);
                Radius = value * maxRadius;
            }

            Projectile.Center = new Vector2(OriginalPosX, OriginalPosY) + new Vector2(
                    A * MathF.Sin(Phase + Main.GameUpdateCount * RotationSpeed),
                    B * MathF.Cos(Phase + Main.GameUpdateCount * RotationSpeed));

            Vector2 screenPos = Projectile.Center - Main.screenPosition;
            float normalizedX = screenPos.X / Main.screenWidth;
            float normalizedY = screenPos.Y / Main.screenHeight;
            float normalizedRadius = Radius / Main.screenHeight;
            BlackHoleSystem.AddBlackHole(new BlackHoleSystem.BlackHoleData
            {
                ScreenPosition = new Vector2(normalizedX, normalizedY),
                Mass = normalizedRadius * 0.08f,
                Radius = this.Radius * 2f,
                EventHorizonSize = this.Radius,
                RotationSpeed = 0.8f
            });
            Projectile.scale = Radius / maxRadius;
            Projectile.rotation += 0.005f;

            float devourRange = 1000f;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.lifeMax <= 5 || npc.life <= 0 || npc.knockBackResist <= 0f || npc.Distance(Projectile.Center) > devourRange) continue;
                float dist = npc.Distance(Projectile.Center);
                npc.position += npc.DirectionToSafe(Projectile.Center) * 1f;
                
                npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionToSafe(Projectile.Center) * (4f + dist / 40), 0.15f);
            }

            float maxSpeed = 40;
            if (Projectile.velocity.Length() > maxSpeed)
                Projectile.velocity = Projectile.velocity.Length(maxSpeed);
            //Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]);
            //Main.NewText($"{Projectile.timeLeft} {Main.time}");
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D ball5 = Request<Texture2D>(AssetDirectory.Extras + "Ball5", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball4 = Request<Texture2D>(AssetDirectory.Extras + "Ball4", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball2 = Request<Texture2D>(AssetDirectory.Extras + "Ball2", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D bullet = Request<Texture2D>(AssetDirectory.Extras + "bulletFa000", AssetRequestMode.ImmediateLoad).Value;

            //BlackHoleScreen.Active = true;
            Vector2 scale = new Vector2(1, 1f) * Projectile.scale * 1.2f;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            DrawDisc();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Color bloomColor = Blue ? new Color(100, 149, 237, 0) : new Color(234, 161, 217, 0);
            Color bloomColor2 = Blue ? Color.DarkBlue : Color.LightCoral;
            Main.EntitySpriteDraw(ball2, Projectile.Center - Main.screenPosition, null, bloomColor * 1f, 0, ball2.Size() / 2, Projectile.scale * 5f, 0);
            Main.EntitySpriteDraw(ball2, Projectile.Center - Main.screenPosition, null, bloomColor * 0.2f, 0, ball2.Size() / 2, Projectile.scale * 5f, 0);
            //Main.EntitySpriteDraw(ball2, Projectile.Center - Main.screenPosition, null, new Color(65, 105, 225, 0) * 1f, 0, ball2.Size() / 2, Projectile.scale * 5f, 0);
            //Main.EntitySpriteDraw(ball2, Projectile.Center - Main.screenPosition, null, new Color(100, 149, 237, 0) * 1f, 0, ball2.Size() / 2, scale * 1.2f * 4f, 0);
            Main.EntitySpriteDraw(ball4, Projectile.Center - Main.screenPosition, null, bloomColor2.AdditiveColor() * 0.3f, 0, ball4.Size() / 2, Projectile.scale * 0.8f, 0);
            //Main.EntitySpriteDraw(ball4, Projectile.Center - Main.screenPosition, null, Color.LightSkyBlue.AdditiveColor() * 0.4f, 0, ball4.Size() / 2, Projectile.scale * 0.5f, 0);
        }

        public void DrawDisc()
        {

            Texture2D flow = Request<Texture2D>(AssetDirectory.Extras + "GenFX_plasma_horizontal2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D cloud = Request<Texture2D>(AssetDirectory.Extras + "GFX_clouds1_withMotes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = Request<Texture2D>(AssetDirectory.Extras + "Laser2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D colorMap = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;
            if (!Blue)
                colorMap = Request<Texture2D>(AssetDirectory.Extras + "Purple-Map", AssetRequestMode.ImmediateLoad).Value;

            Vector2 scale = new Vector2(1, 1f) * Projectile.scale * 1.2f * Radius / 100f;
            Effect shader = ShaderLoader.GetShader("Polarize");

            var flowTime = Main.timeForVisualEffects * 0.006f;

            shader.Parameters["uMask"].SetValue(mask);
            shader.Parameters["uColorMap"].SetValue(colorMap);
            shader.Parameters["uCloud"].SetValue(cloud);
            shader.Parameters["preMultR"].SetValue(1f);
            shader.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.001f + Projectile.whoAmI * 8);
            shader.Parameters["uTime2"].SetValue((float)Main.timeForVisualEffects * 0.002f + Projectile.whoAmI * 8);
            //shader.Parameters["uTime2"].SetValue(25576.707f);
            shader.Parameters["maskFactor"].SetValue(0.5f);
            shader.Parameters["cloudScale"].SetValue(new Vector2(2f, 3f));

            if (!Blue) shader.Parameters["preMultR"].SetValue(1f);
            shader.Parameters["flowx"].SetValue((float)(flowTime + Projectile.whoAmI * 3));
            shader.Parameters["multx"].SetValue(2);
            shader.Parameters["thresholdY"].SetValue(0f);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(flow, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.9f, (float)(0/* + Main.timeForVisualEffects * 0.002f*/), flow.Size() / 2, scale * 0.7f, 0, 0);

            shader.Parameters["cloudOffset"].SetValue(new Vector2(0, 0.5f));
            shader.Parameters["flowx"].SetValue((float)(flowTime + Projectile.whoAmI * 3 + 0.8));
            shader.Parameters["maskFactor"].SetValue(0.7f);
            shader.Parameters["preMultR"].SetValue(1f);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(flow, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.8f, (float)(0/* + Main.timeForVisualEffects * 0.002f*/), flow.Size() / 2, scale * 1.1f, 0, 0);

            shader.Parameters["flowx"].SetValue((float)(flowTime * 6 + Projectile.whoAmI * 3 + 0.8));
            shader.Parameters["maskFactor"].SetValue(0f);
            shader.Parameters["multx"].SetValue(1);
            if (!Blue) shader.Parameters["preMultR"].SetValue(1.2f);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(flow, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.6f, (float)(0/* + Main.timeForVisualEffects * 0.002f*/), flow.Size() / 2, scale * 0.4f, 0, 0);

        }

        public void DrawBloom(float radius)
        {
            Texture2D ball2 = Request<Texture2D>(AssetDirectory.Extras + "Ball2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball1 = Request<Texture2D>(AssetDirectory.Extras + "Ball1", AssetRequestMode.ImmediateLoad).Value;

            int bloomCount = 90;
            Color bloomColor = Blue ? new Color(188, 190, 229, 0) : new Color(230, 183, 229, 0);
            for (int i = 0; i < bloomCount; i++)
            {
                Vector2 pos = Projectile.Center + (MathHelper.TwoPi / bloomCount * i + (float)Main.timeForVisualEffects * 0.2f).ToRotationVector2() * radius * 1.2f;

                Main.EntitySpriteDraw(ball2, pos - Main.screenPosition, null, bloomColor * 0.1f, 0, ball2.Size() / 2, Projectile.scale * 1.1f, 0);
                Vector2 pos2 = Projectile.Center + (MathHelper.TwoPi / bloomCount * i + (float)Main.timeForVisualEffects * 0.2f).ToRotationVector2() * radius * 1f;
                Main.EntitySpriteDraw(ball1, pos2 - Main.screenPosition, null, bloomColor * 1f, 0, ball1.Size() / 2, Projectile.scale * 0.25f, 0);
                //Main.EntitySpriteDraw(ball2, pos2 - Main.screenPosition, null, bloomColor * 0.5f, 0, ball2.Size() / 2, Projectile.scale * 0.2f, 0);

            }
            //Main.EntitySpriteDraw(ball2, Projectile.Center - Main.screenPosition, null, new Color(135, 206, 250, 0) * 0.3f, 0, ball2.Size() / 2, Projectile.scale * 1f, 0);
            Vector2 starScale = new Vector2(0.2f, 0.4f) * 1f;
            //RenderHelper.DrawRing(120, Projectile.Center, radius * 0.9f, Color.LightSkyBlue.AdditiveColor(), (float)Main.timeForVisualEffects, starScale);

            int pointCount = 120;
            float rot = (float)Main.timeForVisualEffects * 0.01f;
            Color drawColor = Blue ? new Color(0, 0, 205, 0) : new Color(144, 0, 206, 0);
            Color drawColor2 = Blue ? new Color(152, 162, 249, 0) : new Color(249, 172, 225, 0);
            var star = TextureAssets.Extra[98].Value;
            Vector2 origin = new Vector2(36, 36);
            float time = (float)(Main.timeForVisualEffects * 0.001f);
            for (int i = 0; i < pointCount; i++)
            {
                float dir = MathHelper.TwoPi / pointCount * i + rot;
                Vector2 vec = dir.ToRotationVector2();
                float ifac = MathF.Sin(MathHelper.TwoPi / pointCount * i + (float)(Main.timeForVisualEffects * 0.04f) + Projectile.whoAmI * 8) * 0.4f + 0.6f; /*MathF.Abs(MathF.Sin(MathHelper.TwoPi / pointCount * i * 2f)) * 0.5f + 0.2f + MathF.Abs(MathF.Cos((float)(Main.timeForVisualEffects * 0.02f))) * 0.3f*/
                float factor = MathF.Sin(MathHelper.TwoPi / pointCount * i + MathHelper.Pi + (float)(Main.timeForVisualEffects * 0.06f) + Projectile.whoAmI * 8) * 0.4f + 0.6f;
                //float factor = MathF.Cos((i * 0.1f ) / MathHelper.TwoPi)+MathF.Cos( (float)(Main.timeForVisualEffects * 0.0001f)) * 0.75f + 0.25f;
                float alpha = ifac;
                Vector2 drawpos = Projectile.Center + vec * radius * 0.8f;
                Main.spriteBatch.Draw(star, drawpos - Main.screenPosition, null, Color.Lerp(drawColor2, drawColor, factor) * alpha, dir, origin, starScale, 0, 0);
            }
        }
    }
    public class HerculesLaser : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Laser4";
        public ref float StartupProgress => ref Projectile.ai[0];
        public ref float StarRot => ref Projectile.localAI[0];
        public ref float DustFlag => ref Projectile.localAI[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6 * 60;
            Projectile.SetImmune(10);

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            float width = Projectile.width * 3f;
            float height = 2400;
            Vector2 endPoint = Projectile.Center;
            Vector2 startPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * height;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, endPoint, width, ref point);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.AliveCheck(Projectile.Center, 2000)) return;

            CameraPlayer cp = player.GetModPlayer<CameraPlayer>();
            GalaxyPlayer gp = player.GetModPlayer<GalaxyPlayer>();

            Projectile.Center = gp.GetConstellationCenter(ConstellationType.Hercules);

            var dir = Projectile.Center.DirectionToSafe(cp.MouseWorld);
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, dir.ToRotation(), 0.015f);

            int startupTimer = Projectile.GetGlobalProjectile<AyaGlobalProjectile>().MaxTimeleft - Projectile.timeLeft;
            StartupProgress = Utils.Remap(startupTimer, 0f, 45f, 0f, 1f);

            if(StartupProgress < 0.99f)
            {
                int dustamount = 10;
                float lengthfactor = Utils.Remap(StartupProgress, 0f, 1f, 1.5f, 0.2f);
                for (int i = 0; i < dustamount; i++)
                {
                    float distFactor = Main.rand.NextFloat(0.8f,1.2f);

                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * distFactor * 100f * lengthfactor;

                    float scale = Main.rand.NextFloat(0.7f, 1.4f);
                    Vector2 vel = pos.DirectionToSafe(Projectile.Center).RotateRandom(0.3f) * Utils.Remap(distFactor, 0.8f, 1.2f, 0.6f, 1.4f) * 1.3f;
                    Dust d = Dust.NewDustPerfect(pos, DustID.MushroomSpray, vel, Scale: 1f);
                    d.noGravity = true;
                    //var l2 = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.Blue, 60, true);
                    //l2.SetVelMult(0.95f);
                    //l2.SetScaleMult(0.9f);
                    //l2.Scale = scale;
                }
            }

            if (StartupProgress >= 1f && DustFlag < 1f)
            {
                int dustamount = 70;
                for (int i = 0; i < dustamount; i++)
                {

                    Vector2 pos = Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(20f);
                    Vector2 vel = (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 8f) * 1.5f;
                    //Dust d = Dust.NewDustPerfect(pos, DustID.BlueTorch, vel, Scale: 3f);
                    Dust d = Dust.NewDustPerfect(pos, DustID.MushroomSpray, vel, Scale: 2f);
                    d.noGravity = true;

                    //var l2 = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.Blue, 60, true);
                    //l2.SetVelMult(0.95f);
                    //l2.SetScaleMult(0.99f);
                    //l2.Scale = Main.rand.NextFloat(0.7f, 1.4f) * 2f;
                }

                DustFlag = 2f;
            }

            float rotIncrease = EaseManager.Evaluate(Ease.OutCubic, Utils.Remap(StartupProgress, 0f, 1f, 1f, 0f), 1f) * 0.2f;
            StarRot += rotIncrease;

            int dustcount = 50;
            Vector2 dire = Projectile.rotation.ToRotationVector2();
            var normal = dire.RotatedBy(MathHelper.PiOver2);
            float maxwidth = 130 * Projectile.scale;
            for (int i = 0; i < dustcount; i++)
            {
                float length = Main.rand.NextFloat();
                float widthoffset = Main.rand.NextFloat(-1, 1);
                float width = MathHelper.Lerp(maxwidth / 3, maxwidth, length);
                Vector2 pos = Projectile.Center + dire * length * 2000 + normal * widthoffset * width;
                Vector2 vel = dire * Utils.Remap(MathF.Abs(widthoffset), 0, 1f, 1f, 0.1f) * 20;
                //Dust d = Dust.NewDustPerfect(pos, 181, vel, Scale: 0.8f);
                //d.noGravity = true;

                //var l = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.DeepSkyBlue, 20);
                //l.SetScaleMult(0.9f);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = TextureAssets.Projectile[Type].Value;
            //Texture2D texture = TextureAssets.Extra[197].Value;
            Texture2D texture = Request<Texture2D>(AssetDirectory.Extras + "T_Repeating whisp Noise2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape2 = Request<Texture2D>(AssetDirectory.Extras + "Laser2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D star = Request<Texture2D>(AssetDirectory.Extras + "GenFX_Flare_White2", AssetRequestMode.ImmediateLoad).Value;
            float timefactor = Projectile.TimeleftFactor(-45);

            float timeFadeout = Utils.Remap(timefactor, 0, 0.1f, 0, 1f);
            float timeFadein = Utils.Remap(timefactor, 0.9f, 1f, 1f, 0f);
            //Texture2D shape = TextureAssets.Extra[197].Value;

            if(StartupProgress < 1f)
            {
                float starScale = MathF.Sin(StartupProgress * MathHelper.Pi) * 2f;
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor(), StarRot, star.Size() / 2, starScale, 0, 0);
            }

            if (timeFadein > 0f)
            {
                DrawLaser(shape2, timeFadein * timeFadeout);
                DrawFlowNebula(shape2, timeFadein, timeFadeout);
            }

            Texture2D ball = Request<Texture2D>(AssetDirectory.Extras + "Ball5", AssetRequestMode.ImmediateLoad).Value;

            float ballScale = Projectile.scale * timeFadein * timeFadeout * 0.7f;
            Main.EntitySpriteDraw(ball, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.05f, 0, ball.Size() / 2, ballScale * 1f, 0);

            Main.EntitySpriteDraw(ball, Projectile.Center - Main.screenPosition, null, Color.LightSkyBlue.AdditiveColor(), 0, ball.Size() / 2, ballScale * 0.5f, 0);

            Main.EntitySpriteDraw(ball, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor(), 0, ball.Size() / 2, ballScale * 0.4f, 0);

            Texture2D light = Request<Texture2D>(AssetDirectory.Extras + "Ball4_1", AssetRequestMode.ImmediateLoad).Value;

            Vector2 lightScale = new Vector2(5f, 0.2f) * Projectile.scale * timeFadein * timeFadeout;

            Main.EntitySpriteDraw(light, Projectile.Center - Main.screenPosition, null, Color.White * 0.2f, (float)(Main.timeForVisualEffects * 0.002f), light.Size() / 2, lightScale * 0.4f, 0);
            Main.EntitySpriteDraw(light, Projectile.Center - Main.screenPosition, null, Color.White * 0.2f, (float)(Main.timeForVisualEffects * 0.002f + MathHelper.PiOver2), light.Size() / 2, new Vector2(lightScale.X * 0.7f, lightScale.Y) * 0.4f, 0);


            return false;
        }

        public void DrawLaser(Texture2D shape2, float scaleFactor)
        {

            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "Laser5", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape3 = Request<Texture2D>(AssetDirectory.Extras + "Laser6", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask2 = Request<Texture2D>(AssetDirectory.Extras + "GFX_clouds1_withMotes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask3 = Request<Texture2D>(AssetDirectory.Extras + "GenFX_plasma_horizontal2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask4 = Request<Texture2D>(AssetDirectory.Extras + "GenFX_FX3_flame_fire_smoke_sojin_seamless_dense_horiz", AssetRequestMode.ImmediateLoad).Value;
            Texture2D colorMap2 = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = TextureAssets.Extra[193].Value;
            Effect effect = ShaderLoader.GetShader("Trail");

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bars2 = new List<CustomVertexInfo>();

            float width = 250 * Projectile.scale * scaleFactor;

            float length = 2000f;
            int count = 300;
            for (int i = 0; i < count; i++)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                var normal = dir.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);

                var factor = i / (float)count;//0是激光起始点

                Color color = Color.White with { A = 255 };
                float alphaFadein = Utils.Remap(factor, 0, 0.03f, 0, 1f);
                var alpha = 0.9f * Projectile.Opacity * alphaFadein;
                float aWidth =/* MathHelper.Lerp(30f, width, factor)*/MathHelper.Lerp(width / 3, width, factor);
                Vector2 pos = Projectile.Center + dir * length / count * i;
                Vector2 top = pos + normal * aWidth;
                Vector2 bottom = pos - normal * aWidth;
                bars.Add(new(top, color, new Vector3((float)Math.Sqrt(factor) * 1f, 1, alpha)));
                bars.Add(new(bottom, color, new Vector3((float)Math.Sqrt(factor) * 1f, 0, alpha)));

                bars2.Add(new(top, color, new Vector3(factor * 4, 1, alpha)));
                bars2.Add(new(bottom, color, new Vector3(factor * 4, 0, alpha)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
                var model = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                var view = Main.GameViewMatrix.TransformationMatrix;
                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * view * projection);
                effect.Parameters["timer"].SetValue((float)-Main.timeForVisualEffects * 0.02f);
                effect.Parameters["maskFactor"].SetValue(0.5f);
                effect.Parameters["preMult"].SetValue(true);
                effect.Parameters["colorMult"].SetValue(0.9f);

                Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
                Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
                Main.graphics.GraphicsDevice.Textures[2] = mask;//蒙版
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                {
                    effect.Parameters["timer"].SetValue((float)-Main.timeForVisualEffects * 0.03f);
                    effect.Parameters["maskFactor"].SetValue(0.5f);
                    effect.Parameters["colorMult"].SetValue(0.9f);
                    Main.graphics.GraphicsDevice.Textures[1] = shape2;//形状
                    Main.graphics.GraphicsDevice.Textures[2] = mask2;//蒙版

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                }

                {
                    effect.Parameters["timer"].SetValue((float)-Main.timeForVisualEffects * 0.01f);
                    effect.Parameters["maskFactor"].SetValue(0f);
                    effect.Parameters["colorMult"].SetValue(0.5f);
                    Main.graphics.GraphicsDevice.Textures[2] = mask3;//蒙版

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }

                }
                {
                    effect.Parameters["timer"].SetValue((float)-Main.timeForVisualEffects * 0.01f);
                    effect.Parameters["maskFactor"].SetValue(0f);
                    effect.Parameters["colorMult"].SetValue(0.5f);

                    Main.graphics.GraphicsDevice.Textures[0] = colorMap2;//色图
                    Main.graphics.GraphicsDevice.Textures[1] = shape3;//形状
                    Main.graphics.GraphicsDevice.Textures[2] = mask4;//蒙版

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
                    }
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }
        }

        public void DrawFlowNebula(Texture2D shape2, float fadein, float fadeout)
        {

            Texture2D flow = Request<Texture2D>(AssetDirectory.Extras + "GenFX_PlagueofMurlocs_Water_BW", AssetRequestMode.ImmediateLoad).Value;
            Texture2D cloud = Request<Texture2D>(AssetDirectory.Extras + "GFX_clouds1_sparseMotes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D colorMap = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map4", AssetRequestMode.ImmediateLoad).Value;
            Vector2 scale = new Vector2(1, 1f) * Projectile.scale * 1f * fadein;
            Effect shader = ShaderLoader.GetShader("Polarize");

            var flowTime = Main.timeForVisualEffects * 0.006f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            shader.Parameters["uMask"].SetValue(shape2);
            shader.Parameters["uColorMap"].SetValue(colorMap);
            shader.Parameters["uCloud"].SetValue(cloud);
            shader.Parameters["preMultR"].SetValue(1.5f);
            shader.Parameters["uTime"].SetValue((float)-Main.timeForVisualEffects * 0.004f + Projectile.whoAmI * 8);
            shader.Parameters["uTime2"].SetValue((float)-Main.timeForVisualEffects * 0.005f + Projectile.whoAmI * 8);
            //shader.Parameters["uTime2"].SetValue(25576.707f);
            shader.Parameters["maskFactor"].SetValue(0.7f);
            shader.Parameters["cloudScale"].SetValue(new Vector2(2f, 3f));
            shader.Parameters["thresholdY"].SetValue(0f);

            shader.Parameters["flowx"].SetValue((float)(flowTime + Projectile.whoAmI * 3));
            shader.Parameters["multx"].SetValue(2);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(flow, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.9f * fadein * fadeout, (float)(0/* + Main.timeForVisualEffects * 0.002f*/), flow.Size() / 2, scale * 0.7f, 0, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }

    public class PegasusRail : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaTexturePath("Extra_189");
        public ref float Width => ref Projectile.ai[0];
        public ref float TotalTime => ref Projectile.ai[1];
        public ref float Radius => ref Projectile.ai[2];
        public ref float RotFactor => ref Projectile.localAI[0];
        public ref float CurrentRot => ref Projectile.localAI[1];
        public ref float Count => ref Projectile.localAI[2];


        public bool StarFlag = false;
        public bool ClockWise = true;

        public static PegasusRail SpawnRail(IEntitySource source, Vector2 pos, float width, int totaltime, float radius, int damage, int owner, float rotFactor = 0.8f, int count = 4)
        {
            var rail = Projectile.NewProjectileDirect(source, pos, Vector2.Zero, ProjectileType<PegasusRail>(), damage, 0f, owner, width, totaltime, radius);
            rail.localAI[0] = rotFactor;
            rail.localAI[2] = count;
            rail.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            return (rail.ModProjectile as PegasusRail);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3 * 60;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Projectile.ai[1];
            //Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            CurrentRot = Utils.Remap(factor, RotFactor, 1f, MathHelper.TwoPi - MathHelper.PiOver2, 0f) * ClockWise.ToDirectionInt();

            if(MathF.Abs(CurrentRot) > 4.5f && !StarFlag)
            {
                Vector2 pos = Projectile.Center + (Projectile.rotation + CurrentRot).ToRotationVector2() * Radius;
                Vector2 vel = new Vector2(0, -1f).RotatedByRandom(0.4f) * 8f;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ProjectileType<PegasusStarVisual>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                StarFlag = true;
            }

            Projectile.rotation += 0.02f * ClockWise.ToDirectionInt();

            int spawnNextTime = 10;

            if (TotalTime - Projectile.timeLeft == spawnNextTime && Count > 0)
            {
                var rail = SpawnRail(Projectile.GetSource_FromAI(), Projectile.Center, Width, (int)TotalTime, Radius + Width * 0.75f, Projectile.damage, Projectile.owner, RotFactor - 0.1f, (int)(Count - 1));
                rail.ClockWise = !ClockWise;
                rail.Projectile.rotation = Projectile.rotation + MathHelper.PiOver4;
                //proj.rotation = dirRot;
            }

            int dustcount = 2;
            for (int i = 0; i < 2; i++)
            {
                float progress = Main.rand.NextFloat();
                Vector2 dir = (Projectile.rotation + CurrentRot * progress).ToRotationVector2() * (Radius + Width * MathF.Sin(i * 9491));
                Vector2 pos = Projectile.Center + dir;
                Vector2 vel = dir.RotatedBy(MathHelper.PiOver2 * ClockWise.ToDirectionInt()).Length(3);
                //Dust d = Dust.NewDustPerfect(pos, DustID.DungeonSpirit, vel, Scale: 0.75f);
                //Dust d = Dust.NewDustPerfect(pos, 206, vel, Scale: 0.75f);
                //Dust d = Dust.NewDustPerfect(pos, DustID.GemSapphire, vel, Scale: 1f);
                //Dust d1 = Dust.NewDustPerfect(pos, DustID.WhiteTorch, vel, Scale: 0.8f);
                //d1.noGravity = true;
                var l2 = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.Blue, 60, true);
                l2.Scale = 0.9f;
                l2.SetScaleMult(0.92f);
                l2.SetVelMult(0.96f);

                //LightSpotParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.White.AdditiveColor(), 0.1f, 60);
            }

            for (int i = 0; i < dustcount; i++)
            {
                float progress = Main.rand.NextFloat();
                Vector2 dir = (Projectile.rotation + CurrentRot * progress).ToRotationVector2() * (Radius + Width * MathF.Sin(i * 9491));
                Vector2 pos = Projectile.Center + dir;
                Vector2 vel = dir.RotatedBy(MathHelper.PiOver2 * ClockWise.ToDirectionInt()).Length(4);
                //Dust d = Dust.NewDustPerfect(pos, DustID.DungeonSpirit, vel, Scale: 0.75f);
                //Dust d = Dust.NewDustPerfect(pos, 206, vel, Scale: 0.75f);
                //Dust d = Dust.NewDustPerfect(pos, DustID.GemSapphire, vel, Scale: 1f);

                //Dust d = Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 1f);
                //d.noGravity = true;
                //Dust d2 = Dust.NewDustPerfect(pos, DustID.ManaRegeneration, vel, Scale: 0.8f);
                //d2.noGravity = true;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "Laser3", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = TextureAssets.Extra[189].Value;

            float timeFactor = Projectile.TimeleftFactor();
            float fadein = Utils.Remap(timeFactor, 0.8f, 1f, 1f, 0f);
            float fadeout = Utils.Remap(timeFactor, 0f, 0.2f, 0f, 1f);

            int length = (int)(20 + TotalTime / 3);

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            Effect effect = ShaderLoader.GetShader("Trail");
            Vector2 lastTrailPos = Vector2.Zero;

            int mult = 1;
            int total = (int)(length * mult - mult);
            float x = 0;
            for (int i = 0; i < total - 1; i++)
            {
                float factor = (float)i / total;//factor 1为轨迹尾部， 0为轨迹头部
                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                if (float.IsNaN(lerpFactor)) lerpFactor = 0f;

                Vector2 trailPos = Projectile.Center + (Projectile.rotation + CurrentRot * factor).ToRotationVector2() * Radius;

                float centerFactor = Utils.Remap(MathF.Abs(factor - 0.5f), 0.3f, 0.5f, 1, 0f);
                Color color = Color.Lerp(Color.Black, Color.White, centerFactor).AdditiveColor();
                float alpha = /*centerFactor **/ Projectile.Opacity * fadein * fadeout;
                var normalDir = lastTrailPos - trailPos;
                normalDir = normalDir.RotatedBy(MathHelper.PiOver2);
                normalDir = normalDir.SafeNormalize(Vector2.Zero);

                float traillength = trailPos.Distance(lastTrailPos) * 0.0015f;
                lastTrailPos = trailPos;
                if (i == 0) continue;

                Vector2 top = trailPos + normalDir * Width * 0.5f;
                Vector2 bottom = trailPos - normalDir * Width * 0.5f;
                x += traillength;
                bars.Add(new CustomVertexInfo(top, color * alpha, new Vector3(x, 0, alpha)));
                bars.Add(new CustomVertexInfo(bottom, color, new Vector3(x, 1, alpha)));

            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

                //RasterizerState rasterizerState = new RasterizerState();
                //rasterizerState.CullMode = CullMode.None;
                //rasterizerState.FillMode = FillMode.WireFrame;
                //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
                var model = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                var view = Main.GameViewMatrix.TransformationMatrix;
                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * view * projection);
                effect.Parameters["timer"].SetValue((float)-Main.timeForVisualEffects * 0.004f);
                effect.Parameters["maskFactor"].SetValue(0f);
                effect.Parameters["preMult"].SetValue(true);
                effect.Parameters["colorMult"].SetValue(2f);
                Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
                Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
                Main.graphics.GraphicsDevice.Textures[2] = mask;//蒙版

                //Main.graphics.GraphicsDevice.Textures[0] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[1] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[2] = (Texture)TextureAssets.MagicPixel;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                //Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;//形状


                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }

            return false;
        }
    }

    public class PegasusStarVisual : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6 * 60;
            Projectile.extraUpdates = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            Projectile.Opacity = Utils.Remap(factor, 0.8f, 1f, 1f, 0f);

            float gravity = Utils.Remap(factor, 0.6f, 1f, 0f, 0.3f);
            gravity /= 2f;
            Projectile.UseGravity(0.98f, gravity, 27);
            float liftingForce = Utils.Remap(factor, 0.4f, 0.7f, 0.45f, 0f);
            liftingForce /= 1.5f;
            Projectile.velocity += new Vector2(0, -liftingForce);

            Projectile.rotation += 0.015f;
            int dustamount = 2;

            if (factor < 0.7f /*&& Projectile.velocity.Y < 0*/)
            {
                float width = Projectile.width / 2 * Utils.Remap(factor, 0.5f, 0.6f, 1f, 0f);
                for (int i = 0; i < dustamount; i++)
                {
                    float speedx = Projectile.velocity.X + Main.rand.NextFloat(-2, 2);
                    float speedy = Projectile.velocity.Y + Main.rand.NextFloat(-2, 2);
                    float x = Projectile.timeLeft * 0.3f;
                    Vector2 pos = Projectile.Center + new Vector2(MathF.Cos(x) * width, 0);
                    Dust d = Dust.NewDustPerfect(pos, DustID.HallowSpray, Projectile.velocity * 0.2f, Scale:0.4f);
                    Vector2 pos2 = Projectile.Center + new Vector2(MathF.Sin(x) * width, 0);
                    //Dust d2 = Dust.NewDustPerfect(pos2, DustID.BlueFairy, Projectile.velocity * 0.2f, Scale: 0.5f);
                    Dust d2 = Dust.NewDustPerfect(pos2, 295, Projectile.velocity * 0.2f, Scale: 0.5f);
                    //d2.noGravity = true;
                    //d2.noGravity = true;
                    //Dust d = Dust.NewDustDirect(Projectile.position + Projectile.Size / 4, Projectile.width / 2, Projectile.height / 2, DustID.HallowSpray, speedx, speedy, Scale: 1.2f);
                    //d.noGravity = true;

                    //Projectile.scale = Utils.Remap(factor, 0, 0.2f, 0f, 1f);
                }
            }


            for (int i = 0; i < 1/* && Main.rand.NextBool()*/; i++)
            {
                Vector2 vel = new Vector2(MathF.Cos(Projectile.timeLeft * 0.15f) * 0.8f, Projectile.velocity.Y * 0.3f);
                //Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.ShimmerSpark, vel,Scale:1f);
                //Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, vel, Scale: 1f);
                //Dust d = Dust.NewDustPerfect(Projectile.Center, 172, vel, Scale: 1.2f);
                //Dust d = Dust.NewDustPerfect(Projectile.Center, 187, vel, Scale: 1.2f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, 181, vel, Scale: 1.2f);
                d.noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            int heldproj = Main.player[Projectile.owner].heldProj;
            if (heldproj < 0) return;
            Projectile camera = Main.projectile[heldproj];
            if (camera.ModProjectile is not GalaxyProjectorProj) return;
            Vector2 cameraPos = camera.Center;
            for (int j = 0; j < 2; j++)
            {
                List<int> selected = new List<int>() { -1 };
                float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
                float randRotRange = 0.3f;
                for (int i = 0; i < 4; i++)
                {
                    int n = -1;
                    while (selected.Contains(n))
                    {

                        n = Main.rand.NextFromList(0, 1, 2, 3);
                    }
                    selected.Add(n);
                    Vector2 targetPos = cameraPos + new Vector2(Main.rand.Next(50, 150) * 1.5f, 0).RotatedBy(n * MathHelper.PiOver2 + startRot + Main.rand.NextFloat(-randRotRange, randRotRange));
                    Vector2 origin = cameraPos + new Vector2((j % 2 == 0 ? -1 : 1) * (targetPos.X < cameraPos.X).ToDirectionInt() * Main.rand.Next(100, 300), Main.rand.Next(-1600, -1200) - i * 300 + j * 150);
                    Vector2 vel = origin.DirectionTo(targetPos) * 12f;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), origin, vel, ProjectileType<PegasusMeteorStar>(), Projectile.damage, 0f, Projectile.owner, targetPos.X, targetPos.Y);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float scaleX = 0.6f;
            float scaleY = 0.8f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            MeteorStar.DrawStar(Projectile,texture,Projectile.Center, Color.DarkBlue, 1f, scaleX, scaleY, 0.7f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);
            
            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, Color.White, 0.7f, scaleX, scaleY, 0.3f);


            return false;

        }
    }

    public class PegasusMeteorStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";
        public static MultedTrail strip = new MultedTrail();
        public ref float TargetX => ref Projectile.ai[0];
        public ref float TargetY => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 40);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6 * 60;
            Projectile.extraUpdates = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            float dist = Projectile.Distance(new Vector2(TargetX, TargetY));
            int time = (int)(dist / Projectile.velocity.Length());
            Projectile.timeLeft = time;
        }
        public override void AI()
        {

            Projectile.rotation += 0.03f;

            int dustcount = 1;
            for (int i = 0; i < dustcount; i++)
            {
                float distFactor = Main.rand.NextFloat();
                Vector2 pos = Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * distFactor * 20;

                float scale = Main.rand.NextFloat(0.7f, 1.4f);
                Vector2 vel = Projectile.velocity * 0.3f * Utils.Remap(distFactor, 0, 1f, 1.4f, 0.4f);
                //var l = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.White, 60);
                //l.SetVelMult(0.95f);
                //l.SetScaleMult(0.97f);
                //l.Scale = scale;
                var l2 =LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.Blue, 60, true);
                l2.SetVelMult(0.95f);
                l2.SetScaleMult(0.97f);
                l2.Scale = scale;
                //Dust d = Dust.NewDustDirect(Projectile.position + Projectile.Size / 4, Projectile.width, Projectile.height, DustID.IceTorch, speedx, speedy, Scale: 1.2f);
                //d.noGravity = true;

            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 256;
            Projectile.Center = Projectile.position;

            if (Projectile.owner == Main.myPlayer)
                Projectile.Damage();
            //RingParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Color.DarkBlue.AdditiveColor(), 20, 90, 1f, 0f, 0.1f, 0.6f, 45, 120, Ease.OutCubic, Ease.OutCubic);

            for(int i = 0; i < 1; i++)
            {
                StarSparkParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, new Color(40, 40, 135) with { A = 127 }, 20,2f);

                StarSparkParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, Color.White.AdditiveColor(), 20, 1f);

            }

            var p = StarPerishParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, 30, 0.5f);
            p.Rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;




            float speed = 10;
            for (int i = 0; i < 35; i++)
            {
                float distFactor = Main.rand.NextFloat(0.3f,1f);
                Vector2 dir = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                Vector2 pos = Projectile.Center + dir * distFactor * 10;
                Vector2 vel = dir * speed * distFactor;
                float scaleFactor = Main.rand.NextFloat(0.5f, 1.5f) * 0.7f;
                float velmult = Main.rand.NextFloat(0.88f,0.94f);
                float scaleMult = Main.rand.NextFloat(0.92f,0.99f);
                //var l = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.White, 60);
                //l.SetVelMult(velmult);
                //l.SetScaleMult(scaleMult);
                //l.Scale = 2f * scaleFactor;
                var l2 = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.DarkBlue, 60, true);
                l2.SetVelMult(velmult);
                l2.SetScaleMult(scaleMult);
                l2.Scale = 3f * scaleFactor;
            }
            //SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        public Color ColorFunction(float progress)
        {
            Color drawColor = new(0,0,135);
            float div = 0.2f;
            if (progress < 0.2f)
                return Color.Lerp(Color.White, drawColor, Utils.Remap(progress, 0, div, 0f, 1f)).AdditiveColor() * Projectile.Opacity;
            float factor = Utils.Remap(progress, div, 1f, 0f, 1f);
            return Color.Lerp(drawColor, Color.Black, factor).AdditiveColor() * Projectile.Opacity;
        }
        public float WidthFunction(float progress) => 20f;
        public float WidthFunction2(float progress) => 40f;
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0f, 0.1f, 0f, 1f);
            return EaseManager.Evaluate(Ease.InOutSine, 1f - progress, 1f) * Projectile.Opacity * 2f * fadeinFactor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;

            Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D sampler = TextureAssets.Extra[189].Value;
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            Effect effect = ShaderLoader.GetShader("Trail");
            
            float width = 20 * Projectile.scale;

            for (int i = 0; i < Projectile.oldPos.Length - 2; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i + 1] == Vector2.Zero) continue;

                var normal = Projectile.oldPos[i + 1] - Projectile.oldPos[i];
                normal = normal.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);

                var factor = 1 - i / (float)Projectile.oldPos.Length;
                Color color = Color.Lerp(Color.Black, Color.White, factor);
                var alpha = EaseManager.Evaluate(Ease.InSine, factor, 1f) * 1.05f * Projectile.Opacity;
                Vector2 top = Projectile.oldPos[i] + Projectile.Size / 2 + normal * width;
                Vector2 bottom = Projectile.oldPos[i] + Projectile.Size / 2 - normal * width;
                bars.Add(new(top, color, new Vector3((float)Math.Sqrt(factor) * 2.5f, 1, alpha)));
                bars.Add(new(bottom, color, new Vector3((float)Math.Sqrt(factor) * 2.5f, 0, alpha)));

            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
                var model = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                var view = Main.GameViewMatrix.TransformationMatrix;
                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * view * projection);
                effect.Parameters["timer"].SetValue((float)Main.timeForVisualEffects * 0.025f);
                Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
                Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
                Main.graphics.GraphicsDevice.Textures[2] = sampler;//蒙版
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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }


            strip.PrepareStrip(Projectile.oldPos, 3, ColorFunction, WidthFunction,
                Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            Main.graphics.GraphicsDevice.Textures[0] = shape;
            strip.DrawTrail();


            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float scaleX = 0.6f;
            float scaleY = 0.8f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, Color.DarkBlue, 1f, scaleX, scaleY, 0.7f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, Color.White, 0.7f, scaleX, scaleY, 0.4f);


            return false;
        }
    }
    public class LyraMusicalStaff : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Laser3";

        public ref float Radius => ref Projectile.ai[0];
        public ref float TotalRot => ref Projectile.ai[1];
        public ref float TotalTime => ref Projectile.ai[2];

        public ref float CurrentRot => ref Projectile.localAI[0];

        public ref float Width => ref Projectile.localAI[1];

        public static LyraMusicalStaff Spawn(IEntitySource source, Vector2 pos, float width, int time, float radius, int damage, int owner, float totalRot)
        {
            var proj = Projectile.NewProjectileDirect(source, pos, Vector2.Zero, ProjectileType<LyraMusicalStaff>(), damage, 0f, owner, radius, totalRot, time);
            proj.localAI[1] = width;
            var modproj = proj.ModProjectile as LyraMusicalStaff;
            return modproj;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3 * 60;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)TotalTime;
            //Projectile.Opacity = 0.7f;
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            CurrentRot = Utils.Remap(factor, 0.7f, 1f, TotalRot, 0f);

            if(factor > 0.7f && Projectile.timeLeft % 4 == 0)
            {
                Vector2 pos = Projectile.Center + (Projectile.rotation + CurrentRot).ToRotationVector2() * (Radius + Main.rand.NextFloat(-1f, 1f) * 100);
                Vector2 vel = Projectile.Center.DirectionToSafe(pos).RotatedBy(MathF.Sign(TotalRot) * MathHelper.PiOver2) * Main.rand.NextFloat(0.5f, 1.5f);
                var flare = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ProjectileType<LyraFlare>(), Projectile.damage, 0f, Projectile.owner);
                flare.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                flare.scale *= 0.5f;
            }

            int dustamount = 6;

            for(int i = 0; i < dustamount; i++)
            {
                float trailfactor = Main.rand.NextFloat();
                Vector2 pos = Projectile.Center + (Projectile.rotation + CurrentRot * trailfactor).ToRotationVector2() * (Radius + Main.rand.NextFloat(-1f, 1f) * 130);
                Vector2 vel = Projectile.Center.DirectionToSafe(pos).RotatedBy(MathF.Sign(TotalRot) * MathHelper.PiOver2) * Main.rand.NextFloat(0.5f, 1.5f);
                var l2 = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, Color.Blue, 60, true);
                l2.Scale = 1f;
                l2.SetScaleMult(0.92f);
                l2.SetVelMult(0.96f);

            }

            Projectile.rotation += 0.001f;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D shape = TextureAssets.Projectile[Type].Value;
            Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map4", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = Request<Texture2D>(AssetDirectory.Extras + "GenFX_plasma_horizontal2", AssetRequestMode.ImmediateLoad).Value;


            float timeFactor = Projectile.TimeleftFactor();
            float fadein = Utils.Remap(timeFactor, 0.8f, 1f, 1f, 0f);
            float fadeout = Utils.Remap(timeFactor, 0f, 0.2f, 0f, 1f);

            Effect effect = ShaderLoader.GetShader("Trail");
            int length = (int)(20 + TotalTime / 4);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);



            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //rasterizerState.FillMode = FillMode.WireFrame;
            //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
            var model = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            var view = Main.GameViewMatrix.TransformationMatrix;
            // 把变换和所需信息丢给shader
            effect.Parameters["uTransform"].SetValue(model * view * projection);
            effect.Parameters["timer"].SetValue((float)-Main.timeForVisualEffects * 0.02f);
            effect.Parameters["maskFactor"].SetValue(0.6f);
            effect.Parameters["preMult"].SetValue(true);
            effect.Parameters["colorMult"].SetValue(1f);
            Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
            Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
            Main.graphics.GraphicsDevice.Textures[2] = mask;//蒙版

            //Main.graphics.GraphicsDevice.Textures[0] = (Texture)TextureAssets.MagicPixel;
            //Main.graphics.GraphicsDevice.Textures[1] = (Texture)TextureAssets.MagicPixel;
            //Main.graphics.GraphicsDevice.Textures[2] = (Texture)TextureAssets.MagicPixel;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            //Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;//形状

            for (int j = -2; j < 3; j++)
            {


                List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
                Vector2 lastTrailPos = Vector2.Zero;

                int mult = 1;
                int total = (int)(length * mult - mult);
                float x = j * 0.3f;
                for (int i = 0; i < total - 1; i++)
                {
                    float factor = (float)i / total;//factor 1为轨迹尾部， 0为轨迹头部
                    float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                    if (float.IsNaN(lerpFactor)) lerpFactor = 0f;

                    Vector2 trailPos = Projectile.Center + (Projectile.rotation + CurrentRot * factor).ToRotationVector2() * (Radius + j * 50);

                    float centerFactor = Utils.Remap(MathF.Abs(factor - 0.5f), 0.3f, 0.48f, 1, 0f);
                    //if (factor > 0.5f) centerFactor = Utils.Remap(factor - 0.5f, 0.3f, 0.48f, 1f, 0f);
                    Color color = Color.Lerp(Color.Black, Color.White, centerFactor).AdditiveColor();
                    float alpha = /*centerFactor **/ Projectile.Opacity * fadein * fadeout;
                    var normalDir = lastTrailPos - trailPos;
                    normalDir = normalDir.RotatedBy(MathHelper.PiOver2);
                    normalDir = normalDir.SafeNormalize(Vector2.Zero);

                    float traillength = trailPos.Distance(lastTrailPos) * 0.003f;
                    lastTrailPos = trailPos;
                    if (i == 0) continue;

                    Vector2 top = trailPos + normalDir * Width * 0.5f;
                    Vector2 bottom = trailPos - normalDir * Width * 0.5f;
                    x += traillength;
                    bars.Add(new CustomVertexInfo(top, color * alpha, new Vector3(x , 0, alpha)));
                    bars.Add(new CustomVertexInfo(bottom, color * alpha, new Vector3(x , 1, alpha)));

                }

                if (bars.Count > 2)
                {
 
                    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;


                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }

                    Main.graphics.GraphicsDevice.RasterizerState = originalState;
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class LyraFlare : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "GenFX_Flare_White2";

        public ref float CurrentRotation => ref Projectile.localAI[0];
        public ref float ExtraRotation => ref Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2 * 60;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.SetImmune(-1);
        }
        public override bool? CanDamage() => Projectile.TimeleftFactor() < 0.4f;
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            Projectile.rotation += 0.005f;
            ExtraRotation = EaseManager.Evaluate(Ease.InOutSine, Utils.Remap(factor, 0f, 0.4f, 1f, 0f),1f) * MathHelper.PiOver4 / 2;

            //Projectile.Scale(1f);
            Projectile.localAI[2] = Utils.Remap(factor, 0f, 0.4f, 8f, 1f);

            {
                Projectile.position = Projectile.Center;
                Projectile.height = Projectile.width = (int)(32 * Projectile.scale * Projectile.localAI[2]);
                Projectile.Center = Projectile.position;
            }

            Projectile.Opacity = Utils.Remap(factor, 0f, 0.2f, 0f, 1f);

            CurrentRotation = Projectile.rotation + ExtraRotation;
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.MyClient())
            {
                Vector2 vel = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 12f;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<LyraNotes>(), Projectile.damage, 0f, Projectile.owner);
            }

            int dustcount = 25;
            for(int i = 0; i < dustcount; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(20f);
                Vector2 vel = (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 8f);
                Dust d = Dust.NewDustPerfect(pos, DustID.ManaRegeneration, vel, Scale:1.2f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            float factor = Projectile.TimeleftFactor();
            Color drawColor = Color.LightYellow;
            float scale = Projectile.scale * Projectile.localAI[2] * 0.3f;
            //float radius = 20f * Scale;
            //for (int i = 0; i < 4; i++)
            //{
            //    Vector2 dir = (i * MathHelper.PiOver2 + CurrentRotation).ToRotationVector2();
            //    Vector2 scale = Scale * new Vector2(1 / 2f, 1f);
            //    Main.spriteBatch.Draw(texture, Projectile.Center + dir * 0f * radius - Main.screenPosition, null, drawColor * Projectile.Opacity,
            //        (i + 1) * MathHelper.PiOver2 + CurrentRotation, new Vector2(36), scale, 0, 0);
            //}

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawColor.AdditiveColor() * Projectile.Opacity, CurrentRotation + MathHelper.PiOver2, texture.Size() / 2, scale, 0, 0);
            return false;
        }
    }

    public class LyraNotes : ModProjectile, ISnappableProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + "AstralNote_0";

        public override void Load()
        {
            CustomDrawer.PreProjectileDrawer += NotesDrawer;
        }
        public override void Unload()
        {
            CustomDrawer.PreProjectileDrawer -= NotesDrawer;
        }
        private static void NotesDrawer()
        {
            Texture2D starspot = Request<Texture2D>(AssetDirectory.Extras + "Ball4_1", AssetRequestMode.ImmediateLoad).Value;


            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            foreach (var player in Main.ActivePlayers)
            {
                int total = player.ownedProjectileCounts[ProjectileType<LyraNotes>()];
                if (total == 0) continue;

                List<Vector2> notePositions = new List<Vector2>();

                foreach (var projectile in Main.ActiveProjectiles)
                {
                    if (projectile.type != ProjectileType<LyraNotes>() || projectile.owner != player.whoAmI) continue;

                    float offset = MathF.Cos((float)(projectile.whoAmI * 143 + Main.timeForVisualEffects * 0.05f)) * 25f;
                    Vector2 pos = projectile.Center + Vector2.UnitY.RotatedBy(projectile.whoAmI * 19f) * offset;
                    notePositions.Add(pos);
                }

                for (int i = 0; i < notePositions.Count; i++)
                {
                    Vector2 start = notePositions[i];
                    Vector2 end = notePositions[(i + 1) % notePositions.Count];
                    Color color = Color.SkyBlue;
                    float alpha = 0.5f;

                    Utils.DrawLine(Main.spriteBatch, start, end, color, color, 1f);
                    Main.spriteBatch.Draw(starspot, start - Main.screenPosition, null, Color.White.AdditiveColor() * alpha, 0, starspot.Size() / 2, 0.07f, 0, 0);

                    Main.spriteBatch.Draw(starspot, start - Main.screenPosition, null, Color.White.AdditiveColor() * alpha, 0, starspot.Size() / 2, 0.04f, 0, 0);

                }
            }
            Main.spriteBatch.End();
        }


        public ref float CanDamageFlag => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3 * 60;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.SetImmune(-1);
        }
        public override bool? CanDamage() => CanDamageFlag > 0;
        public void OnSnap()
        {
            CanDamageFlag = 1;

            Projectile.Scale(7f);

            RingParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, new Color(20, 20, 135, 0), 20, Projectile.width / 2f, 1f, 0f, 0.2f, 0.7f, 30, 120, Ease.OutCubic, Ease.OutSine);

            //CameraFlash.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Color.White * 0.7f, Projectile.rotation, Projectile.width / 16f, Projectile.height / 16f, 30);

            Projectile.Damage();
            Projectile.Kill();
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(3);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.AliveCheck(Projectile.Center, 2000) || player.heldProj < 0) return;
            Projectile camera = Main.projectile[player.heldProj];
            if (camera != null && camera.active && camera.IsCameraProj())
            {
                var cameraProj = camera.ModProjectile as BaseCameraProj;
                int totalNotes = player.ownedProjectileCounts[Type];
                int index = Projectile.GetMyGroupIndex();
                float radius = (int)(0.8f * cameraProj.size);
                Vector2 offset = (MathHelper.TwoPi / totalNotes * index + Main.GameUpdateCount * 0.013f).ToRotationVector2() * radius;
                Vector2 targetPos = camera.Center + offset;

                float chaseFactor = Utils.Remap(Projectile.timeLeft, 2 * 60 + 30, 3 * 60, 0.08f, 0.01f);

                Projectile.velocity = Projectile.velocity * 0.99f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, chaseFactor);

                if (Projectile.timeLeft < 10) Projectile.timeLeft++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>(AssetDirectory.Projectiles + $"AstralNote_{Projectile.frame}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D ball2 = Request<Texture2D>(AssetDirectory.Extras + "Ball2", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(ball2, Projectile.Center - Main.screenPosition, null, Color.DarkBlue.AdditiveColor() * 0.6f, 0, ball2.Size() / 2, Projectile.scale * 1.2f, 0, 0);

            float bloomAlpha = Projectile.Opacity * 0.5f * (MathF.Sin((float)(Main.timeForVisualEffects * 0.05f)) * 0.5f + 1f);
            float noteScale = Projectile.scale * 1.3f;
            RenderHelper.DrawBloom(4, 2, texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * bloomAlpha, 0f, texture.Size() / 2, noteScale);
            RenderHelper.DrawBloom(4, 2, texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * bloomAlpha, 0f, texture.Size() / 2, noteScale);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity,
                0f, texture.Size() / 2, noteScale, 0, 0);

            float bloomAlpha2 = Projectile.Opacity * 0.5f * (MathF.Cos((float)(Main.timeForVisualEffects * 0.05f)) * 0.5f + 1f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * bloomAlpha2 * Projectile.Opacity,
                0f, texture.Size() / 2, noteScale, 0, 0);

            return false;
        }
    }
}

