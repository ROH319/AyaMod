using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems.ParticleSystem;
using AyaMod.Core.Systems.Trails;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
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
using Terraria.Graphics;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class MiracleCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 85;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<MiracleCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 9f;

            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(0, 0, 60, 0));
            SetCameraStats(0.08f, 164, 1.5f, 0.6f);
            SetCaptureStats(1000, 60);
        }
    }

    public class MiracleCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(53, 255, 218);
        public override Color innerFrameColor => new Color(81, 255, 113) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(112, 255, 212).AdditiveColor() * 0.5f;

        public float OrbitRotation;
        public float StarRadius = 350;
        public float StarFactor;
        public int StarStack;
        public int MaxItemTime;

        public override void PostAI()
        {
            if (player.Aya().itemTimeLastFrame <= 1 && player.itemTime > 10)
            {
                MaxItemTime = player.itemTime;
                if(StarStack * 0.2f + StarFactor <= 1f)
                {
                    float trueFactor = StarStack * 0.2f + StarFactor;
                    var rail = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<MiracleRail>(), Projectile.damage, 0f, Projectile.owner,
                            0, Projectile.whoAmI, MaxItemTime);
                    rail.localAI[0] = StarStack;
                }
            }

            if(!player.ItemTimeIsZero && MaxItemTime > player.itemTime)
            {
                StarFactor = Utils.Remap(player.itemTime,1,MaxItemTime,0.2f,0);
                if (player.itemTime == 1) StarStack++;

                if(player.controlUseItem && player.itemTime % 3 == 0 && StarStack * 0.2f + StarFactor < 1f)
                {
                    float trueFactor = StarStack * 0.2f + StarFactor;
                    int stardmg = (int)(Projectile.damage * 0.2f);

                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<MiracleStar>(), stardmg, 0f, Projectile.owner,
                            0, Projectile.whoAmI, trueFactor);
                }
            }

            if (!player.controlUseItem)
            {
                foreach (var projectile in Main.ActiveProjectiles)
                {
                    if(StarStack > 4 && projectile.type == ProjectileType<MiracleRail>() && projectile.owner == Projectile.owner)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            SoundEngine.PlaySound(SoundID.Item35 with
                            {
                                MaxInstances = 20,
                                Volume = 0.6f
                            }, projectile.Center);
                        }
                        RingParticle.Spawn(projectile.GetSource_FromAI(), projectile.Center, new Color(72, 206, 132).AdditiveColor(), 10, 120, 0.8f, 0f,
                    0.15f, 0.5f, 60, 120, Ease.OutCirc, Ease.OutCubic);

                        int stardmg = (int)(projectile.damage * 0.2f);
                        Projectile.NewProjectileDirect(projectile.GetSource_FromAI(), projectile.Center, Projectile.DirectionToSafe(projectile.Center).Length(4f + Main.rand.Next(8)).RotatedBy(Main.rand.NextBool() ? -MathHelper.PiOver4 : MathHelper.PiOver4), ProjectileType<MiracleStarHoming>(), stardmg, projectile.knockBack, projectile.owner);
                        projectile.Kill();
                    }

                    if (projectile.type != ProjectileType<MiracleStar>() || projectile.localAI[0] > 0 || projectile.ai[1] != Projectile.whoAmI) continue;
                    projectile.localAI[0] = 1;
                    Vector2 starcenter = Projectile.Center;
                    //projectile.timeLeft = 4 * 60;
                    //var dist = projectile.Distance(starcenter);
                    //projectile.velocity = starcenter.DirectionToSafe(projectile.Center).RotatedBy(MathHelper.PiOver2) * dist / 40f;

                    //projectile.timeLeft = 4 * 60 - 15 + Main.rand.Next(30);
                    //projectile.velocity = starcenter.DirectionToSafe(projectile.Center).RotatedBy(MathF.Cos((projectile.ai[2] % 0.2f) / 0.2f) / MathHelper.Pi + 1f).RotatedBy(MathHelper.PiOver2) * 3f;

                    projectile.timeLeft = 4 * 60;
                    float speed = (MathF.Cos((projectile.ai[2] % 0.2f) / 0.2f)) * 5;
                    projectile.velocity = starcenter.DirectionToSafe(projectile.Center).RotatedBy(MathF.Cos((projectile.ai[2] % 0.2f) / 0.2f) / MathHelper.Pi + 1f).RotatedBy(MathHelper.Pi / 2.5) * speed;
                }
                StarStack = 0;
                StarFactor = 0;
                MaxItemTime = 0;
            }
            OrbitRotation += 0.01f;

        }
    }

    public class MiracleStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";
        public ref float Owner => ref Projectile.ai[1];
        public ref float TrueFactor => ref Projectile.ai[2];
        /// <summary>
        /// 大于1时为释放状态
        /// </summary>
        public ref float Released => ref Projectile.localAI[0];
        public ref float Hue => ref Projectile.localAI[2];

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 5);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(-1);
            Projectile.timeLeft = 5 * 60;
            Projectile.penetrate = 1;
            Projectile.scale = 0.8f;
            Projectile.ArmorPenetration = 40;
        }

        public override bool? CanDamage()
        {
            return Released > 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            float value = (TrueFactor % 0.2f);
            float min = 0.4f;
            float factor = 1;
            if (value < 0.1f)
            {
                factor = Utils.Remap(value, 0, 0.1f, 0.8f, min);
            }
            else
            {
                factor = Utils.Remap(value, 0.1f, 0.2f, min, 0.8f);
            }
            float fadeinfactor = Utils.Remap(value, 0, 0.15f, 1f, min);
            float fadeoutfactor = Utils.Remap(value, 0.05f, 0.2f, min, 1f);
            Projectile.Opacity = 1f * factor;
            Hue = Utils.Remap(value, 0, 0.2f, 108, 180);

        }

        public override void AI()
        {
            Projectile camera = Main.projectile[(int)Owner];

            if (camera.TypeAlive(ProjectileType<MiracleCameraProj>()))
            {
                var miracleCamera = camera.ModProjectile as MiracleCameraProj;
                var player = (camera.ModProjectile as BaseCameraProj).player;

                if (player.AliveCheck(Projectile.Center, 3000))
                {
                    if(Released <= 0)
                    {
                        if (Projectile.timeLeft < 4 * 60) Projectile.timeLeft++;
                        float trueFactor = TrueFactor;
                        trueFactor = trueFactor % 1f;
                        Vector2 pos = AyaUtils.GetPentagramPos(camera.Center, miracleCamera.StarRadius * (1 + MathF.Cos(Main.GameUpdateCount * 0.04f) * 0.1f),
                            trueFactor, MathHelper.TwoPi / 10f + miracleCamera.OrbitRotation);
                        float chaseFactor = Utils.Remap(Projectile.timeLeft, 4 * 60, 5 * 60, 0.9f, 0.2f);
                        Projectile.Center = Vector2.Lerp(Projectile.Center, pos, chaseFactor);

                    }
                }
            }
            else if (Released <= 0) Projectile.Kill();

            if(Released > 0)
            {
                #region 1
                //float factor = (float)Projectile.timeLeft / (4f * 60f);
                //if (Projectile.Opacity < 0.8f) Projectile.Opacity += 0.01f;
                //var acc = 0.18f - 0.32f * (1-factor);
                //var rot = 0.04f * factor + 0.02f;
                //Projectile.velocity += Projectile.velocity.Length(acc);
                //Projectile.velocity = Projectile.velocity.RotatedBy(rot);
                #endregion

                #region 2
                //if (Main.GameUpdateCount % 3 == 0) Projectile.timeLeft++;

                //float acc = 0.1f;
                //float rot = -0.05f;
                //if (Projectile.timeLeft < 178)
                //{

                //    acc -= 0.15f;
                //    if (Projectile.timeLeft < 120) { acc += 0.08f; rot += 0.005f; }
                //    rot += 0.005f;
                //}
                //if (Projectile.Opacity < 0.8f) Projectile.Opacity += 0.01f;
                ////if (Projectile.timeLeft > 120)
                //Projectile.velocity += Projectile.velocity.Length(acc);
                //Projectile.velocity = Projectile.velocity.RotatedBy(rot);
                //if (Projectile.timeLeft < 20) Projectile.Kill();
                #endregion
                if (Projectile.Opacity < 0.8f) Projectile.Opacity += 0.01f;
                float acc = 0.08f;
                float rot = 0.01f;
                Projectile.velocity += Projectile.velocity.Length(acc);
                Projectile.velocity = Projectile.velocity.RotatedBy(rot);
            }
            Projectile.rotation += 0.03f;

        }
        public override void OnKill(int timeLeft)
        {
            int dusttype = Hue switch
            {
                < 144 => DustID.CursedTorch,
                _ => DustID.HallowSpray
            };

            int dustamount = 60;
            float startRot = Projectile.rotation + MathHelper.TwoPi / 10 + AyaUtils.RandAngle;
            float length = 10;
            for (int i = 0; i < dustamount; i++)
            {
                float factor = (float)i / dustamount;
                float rot = MathHelper.TwoPi * factor + startRot;
                Vector2 dir = rot.ToRotationVector2();
                float radius = length * 0.8f + MathF.Sin(factor * MathHelper.TwoPi * 5) * length / 1.5f;
                Vector2 pos = Projectile.Center + dir * radius;
                Vector2 vel = (pos - Projectile.Center).Length(2);
                Dust d = Dust.NewDustPerfect(pos, dusttype, vel, Scale: 1f);
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;


            //float alphaFactor = Utils.Remap(MathF.Sin((float)(Projectile.ai[2] * 0.1f)), -1f, 1f, 0.2f, 1f);
            float alpha = Projectile.Opacity /** alphaFactor*/;
            Color color = AyaUtils.HSL2RGB(Hue, 1f, 0.5f);
            float scaleX = 0.7f;
            float scaleY = 0.9f;
            float scaleMult = 0.3f;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - (float)i / Projectile.oldPos.Length;

                MeteorStar.DrawStar(Projectile, texture, Projectile.oldPos[i] + Projectile.Size / 2, color, factor * 0.4f * alpha, scaleX, scaleY, scaleMult);

            }
            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, color, 0.8f * alpha, scaleX, scaleY, scaleMult);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, Color.White, 0.8f * alpha, scaleX, scaleY, 0.23f);


            return false;
        }
    }

    public class MiracleRail : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "StarTexture";
        public ref float Owner => ref Projectile.ai[1];
        public ref float MaxItemTime => ref Projectile.ai[2];
        public ref float CurrentStack => ref Projectile.localAI[0];
        public ref float Released => ref Projectile.localAI[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)(MaxItemTime * 2f);
        }

        public override void AI()
        {
            Projectile camera = Main.projectile[(int)Owner];
            if (!camera.TypeAlive(ProjectileType<MiracleCameraProj>())) return;

            if (Projectile.timeLeft < MaxItemTime - 1) Projectile.timeLeft++;

            if (Released > 0) { Projectile.Kill();return; }

            var miracleCamera = camera.ModProjectile as MiracleCameraProj;
            var player = (camera.ModProjectile as BaseCameraProj).player;


            float factor = Projectile.TimeleftFactor();

            float trueFactor = CurrentStack * 0.2f;
            trueFactor = trueFactor % 1f;

            float tlFactor = Utils.Remap(Projectile.timeLeft, Projectile.MaxTimeleft() - 30, Projectile.MaxTimeleft(), 1f, 0f);

            Vector2 pos = AyaUtils.GetPentagramPos(camera.Center, miracleCamera.StarRadius * tlFactor * (1 + MathF.Cos(Main.GameUpdateCount * 0.04f) * 0.1f),
                trueFactor, MathHelper.TwoPi / 10f + miracleCamera.OrbitRotation);

            Projectile.Center = Vector2.Lerp(Projectile.Center, pos, 0.9f);
            Vector2 nextPos = camera.Center + (pos - camera.Center).RotatedBy(MathHelper.ToRadians(144));
            Projectile.velocity = nextPos - pos;

            foreach (var particle in ParticleManager.Particles)
            {
                if (particle is RingParticle && particle.Source is EntitySource_Parent parent && parent.Entity is Projectile parentproj)
                {
                    if (parentproj != Projectile) continue;
                    particle.Center = nextPos;
                }
            }

            if (Projectile.timeLeft == MaxItemTime || (miracleCamera.StarStack > 4 && player.itemTime == 1 /*&& (miracleCamera.StarStack - 1) % 5 == CurrentStack*/))
            {
                float volume =1f;
                float radius = 90;
                int playtime = 3;
                if (miracleCamera.StarStack == 4 && CurrentStack == 4)
                {
                    //volume *= 1.5f;
                    playtime += 2;
                    radius *= 2f;
                }
                if (miracleCamera.StarStack > 5) playtime -= 2;
                for (int i = 0; i < playtime; i++)
                {
                    SoundEngine.PlaySound(SoundID.Item35 with
                    {
                        MaxInstances = 20,
                        Volume = volume
                    }, nextPos);
                }
                RingParticle.Spawn(Projectile.GetSource_FromAI(), nextPos, new Color(72, 206, 132).AdditiveColor(), 10, radius, 0.8f, 0f,
                    0.15f, 0.5f, 30, 120, Ease.OutCirc, Ease.OutCubic);

                int stardmg = (int)(Projectile.damage * 0.2f);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), nextPos, Projectile.velocity.Length(9).RotatedBy(Main.rand.NextBool()? -MathHelper.PiOver4 : MathHelper.PiOver4), ProjectileType<MiracleStarHoming>(), stardmg, Projectile.knockBack, Projectile.owner);

            }

            if (!player.controlUseItem)
            {
                Released = 1;
                
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[98].Value;
            Texture2D ball1 = Request<Texture2D>(AssetDirectory.Extras + "Ball", AssetRequestMode.ImmediateLoad).Value;
            float factor = Utils.Remap(Projectile.TimeleftFactor(), 0.5f, 1f, 0f, 1f);
            float length = Projectile.velocity.Length();
            float rot = Projectile.velocity.ToRotation();
            Vector2 dir = rot.ToRotationVector2();

            float drawFactor = Utils.Remap(factor, 0, 1f, 1f, 0);
            int drawcount = (int)(length / 2);

            float xScale = 0.5f;
            float yScale = 1.2f;
            float width = 16;
            for (int i = 0; i < drawcount; i++)
            {
                float totalFactor = (float)i / drawcount;
                if (totalFactor > drawFactor) continue;
                var ndir = dir.RotatedBy(MathHelper.PiOver2);
                Vector2 offset = ndir * 2;
                Color drawColor = Color.Lerp(new Color(28, 255, 144), new Color(25, 255, 251), totalFactor);
                float alphafactor = MathHelper.Lerp(1f, 0f, totalFactor);
                Color color = drawColor.AdditiveColor() * 0.1f * totalFactor * alphafactor;

                for (int j = -1; j < 2; j += 2)
                {
                    Vector2 pos = Projectile.Center - Main.screenPosition + dir * totalFactor * length + new Vector2(0, width * j).RotatedBy(rot) * Projectile.scale;
                    Rectangle sourceRect = new Rectangle(18 - 18 * j, 0, 36, 72);
                    Vector2 origin = new Vector2(18 + 18 * j, 36);
                    Main.spriteBatch.Draw(star, pos, sourceRect, color, rot + MathHelper.PiOver2, origin, Projectile.scale * new Vector2(xScale, yScale), 0, 0);

                }

                if (i == drawcount - 1)
                {
                    Vector2 pos = Projectile.Center - Main.screenPosition + dir * totalFactor * length;
                    Color ballColor = Color.LightGreen.AdditiveColor();
                    for (int j = 0; j < 5; j++)
                    {
                        Main.spriteBatch.Draw(ball1, pos, null, ballColor * 0.8f, Main.GameUpdateCount * 0.1f, ball1.Size() / 2, Projectile.scale * (0.35f + j * 0.15f), 0, 0);

                    }
                }
            }
            return false;
        }
    }

    public class MiracleStarHoming : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";
        public ref float Hue => ref Projectile.localAI[2];

        public static MultedTrail strip = new MultedTrail();
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 25);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(-1);
            Projectile.timeLeft = 10 * 60;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.3f;
            Projectile.ArmorPenetration = 40;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Hue = Main.rand.NextFromList(4,6) * 36;
        }
        public override void AI()
        {
            if (!Projectile.Chase(1000, 25, 0.02f))
                Projectile.velocity += Projectile.velocity.Length(0.06f);
            Projectile.rotation += 0.02f;
        }

        public override void OnKill(int timeLeft)
        {
            int dusttype = Helper.GetHuedDustType((int)Hue);

            int dustamount = 60;
            float startRot = Projectile.rotation + MathHelper.TwoPi / 10 + AyaUtils.RandAngle;
            float length = 15;
            for (int i = 0; i < dustamount; i++)
            {
                float factor = (float)i / dustamount;
                float rot = MathHelper.TwoPi * factor + startRot;
                Vector2 dir = rot.ToRotationVector2();
                float radius = length * 0.8f + MathF.Sin(factor * MathHelper.TwoPi * 5) * length / 1.5f;
                Vector2 pos = Projectile.Center + dir * radius;
                Vector2 vel = (pos - Projectile.Center).Length(3);
                Dust d = Dust.NewDustPerfect(pos, dusttype, vel, Scale: 1.5f);
                d.noGravity = true;
            }
        }
        public Color ColorFunction(float progress)
        {
            Color drawColor = AyaUtils.HSL2RGB(Hue, 1f, 0.6f);
            float extraAlpha = 1f;
            float div = 0.2f;
            if (progress < div)
                return Color.Lerp(Color.White, drawColor, Utils.Remap(progress, 0, div, 0f, 1f)).AdditiveColor() * Projectile.Opacity* extraAlpha;
            float factor = EaseManager.Evaluate(Ease.OutSine, Utils.Remap(progress, div, 1f, 0f, 1f), 1f);
            return Color.Lerp(drawColor, Color.Black, factor).AdditiveColor() * Projectile.Opacity* extraAlpha;
        }
        public float WidthFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0, 0.4f, 0.7f, 1);
            float fadeoutFactor = Utils.Remap(progress, 0f, 0.8f, 1f, 0f);
            return 30f * fadeinFactor * fadeoutFactor;
        }
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0, 0.05f, 0.3f,1);
            return EaseManager.Evaluate(Ease.OutSine, 1f - progress, 1f) * Projectile.Opacity * fadeinFactor * 0.9f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D ball1 = Request<Texture2D>(AssetDirectory.Extras + "Ball", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D star = TextureAssets.Extra[98].Value;
            Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "Laser4", AssetRequestMode.ImmediateLoad).Value;

            Color color = AyaUtils.HSL2RGB(Hue, 1f, 0.5f);

            float mult = 4;

            //int length = Projectile.oldPos.Length - 1;
            //for(int i = 1; i < length; i++)
            //{
            //    float factor = (float)i / length;
            //    float rot = /*i == 0 ? Projectile.rotation : */(Projectile.oldPos[i + 1] - Projectile.oldPos[i]).ToRotation();
            //    Color trailColor = ColorFunction(factor) * (1 - factor);
            //    Vector2 scale = Projectile.scale * new Vector2(0.7f, 0.5f);

            //    Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition;
            //    Main.spriteBatch.Draw(star, trailPos, null, trailColor, rot + MathHelper.PiOver2, star.Size() / 2, scale, 0, 0);
            //}

            strip.PrepareStrip(Projectile.oldPos, mult, ColorFunction, WidthFunction,
    Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            Main.graphics.GraphicsDevice.Textures[0] = shape;
            strip.DrawTrail();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone,null,Main.GameViewMatrix.TransformationMatrix);
            
            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, color, 0.8f, 0.6f, 0.8f, 0.3f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone,null, Main.GameViewMatrix.TransformationMatrix);



            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color ballColor = AyaUtils.HSL2RGB(Hue, 1f, 0.7f).AdditiveColor();
            for (int j = 0; j < 5; j++)
            {
                Main.spriteBatch.Draw(ball1, pos, null, ballColor * 1f, Main.GameUpdateCount * 0.1f, ball1.Size() / 2, Projectile.scale * (0.2f + j * 0.1f), 0, 0);

            }
            return false;
        }
    }
}
