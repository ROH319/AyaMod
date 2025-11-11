using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems.ParticleSystem;
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
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class MiracleCameraEX : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 85;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<MiracleCameraEXProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 9f;

            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(0, 0, 60, 0));
            SetCameraStats(0.08f, 164, 1.5f, 0.6f);
            SetCaptureStats(1000, 60);
        }
    }

    public class MiracleCameraEXProj : BaseCameraProj
    {

        public override Color outerFrameColor => new Color(53, 255, 218);
        public override Color innerFrameColor => new Color(81, 255, 113) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(112, 255, 212).AdditiveColor() * 0.5f;

        public float OrbitRotation;
        public float StarOrbitRadius = 350;
        public float StarRadius = 170;
        public float StarFactor;
        public int StarStack;
        public int MaxItemTime = 0;

        public override void OnSpawn(IEntitySource source)
        {
            
        }

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            int count = StarStack switch
            {
                0 => 1,
                1 => 3,
                2 => 5,
                3 => 7,
                4 => 9,
                _ => 5
            };
            for(int i = 0; i < count; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 7) * 4;
                //Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<MiracleStarHoming>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void PostAI()
        {
            //StarOrbitRadius = 350;
            //StarRadius = 200;
            if (player.Aya().itemTimeLastFrame <= 1 && player.itemTime > 10)
            {
                MaxItemTime = player.itemTime;
                if (StarStack * 0.2f + StarFactor <= 1f)
                {
                    float trueFactor = StarStack * 0.2f + StarFactor;
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 starpos = Projectile.Center + (OrbitRotation + i * MathHelper.TwoPi / 5).ToRotationVector2() * StarOrbitRadius;
                        Vector2 pos = starpos/*AyaUtils.GetPentagramPos(starpos, StarRadius, trueFactor)*/;
                        var rail = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<MiracleRailEX>(), Projectile.damage, 0f, Projectile.owner, 
                            i, Projectile.whoAmI, MaxItemTime);
                        rail.localAI[0] = StarStack;
                    }
                }
            }
            if (!player.ItemTimeIsZero && MaxItemTime >= player.itemTime)
            {
                StarFactor = Utils.Remap(player.itemTime,1,MaxItemTime,0.2f,0);
                if (player.itemTime == 1) StarStack++;

                if (player.controlUseItem && player.itemTime % 4 == 0 && StarStack * 0.2f + StarFactor < 1f)
                {
                    float trueFactor = StarStack * 0.2f + StarFactor;
                    int stardmg = (int)(Projectile.damage * 0.2f);
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 starpos = Projectile.Center + (OrbitRotation + i * MathHelper.TwoPi / 5).ToRotationVector2() * StarOrbitRadius;
                        Vector2 pos = starpos/*AyaUtils.GetPentagramPos(starpos, StarRadius, trueFactor)*/;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<MiracleStarEX>(), stardmg, 0f, Projectile.owner,
                            i, Projectile.whoAmI, trueFactor);
                    }
                }
            }

            if(!player.controlUseItem)
            {
                if(StarStack * 0.2f + StarFactor >= 0.9999999999f)
                {
                    for(int i = 0; i < 5; i++)
                    {
                        Vector2 starcenter = Projectile.Center + (OrbitRotation + MathHelper.TwoPi / 5 * i).ToRotationVector2() * StarOrbitRadius;
                        RingParticle.Spawn(Projectile.GetSource_FromAI(), starcenter, new Color(72, 206, 132).AdditiveColor() * 0.8f, 30, 280, 0.8f, 0f,
                            0.2f, 0.6f, 60, 120, Ease.InOutSine, Ease.InOutSine);

                    }
                }
                foreach(var projectile in Main.ActiveProjectiles)
                {
                    if (projectile.type != ProjectileType<MiracleStarEX>() || projectile.localAI[0] > 0 || projectile.ai[1] != Projectile.whoAmI) continue;
                    projectile.localAI[0] = 1;
                    Vector2 starcenter = Projectile.Center + (OrbitRotation + projectile.ai[0] * MathHelper.TwoPi / 5).ToRotationVector2() * StarOrbitRadius;

                    projectile.velocity = starcenter.DirectionToSafe(projectile.Center).RotatedBy(MathF.Cos(projectile.ai[2] / 5f) / MathHelper.Pi + 1f).RotatedBy(MathHelper.Pi) * 3f;
                }
                StarStack = 0;
                StarFactor = 0;
                MaxItemTime = 0;
            }
            //for(int i = 0; i < 36; i++)
            //{
            //    float factor = (float)i / 36;
            //    Vector2 pos = AyaUtils.GetPentagramPos(Projectile.Center, 300, factor);
            //    Dust d = Dust.NewDustPerfect(pos, DustID.RedTorch, Vector2.Zero);
            //    d.noGravity = true;
            //}
            OrbitRotation += 0.01f;
            base.PostAI();
        }
    }
    public class MiracleStarEX : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";
        public ref float Offset => ref Projectile.ai[0];
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
            Projectile.scale = 0.75f;
        }
        public override bool? CanDamage()
        {
            return Released > 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            //float colortype = Projectile.ai[2] switch
            //{
            //    <= 0.2f => 0,
            //    <= 0.4f => 1,
            //    <= 0.6f => 3,
            //    <= 0.8f => 6,
            //    <= 1 => 8,
            //    _ => 0
            //};
            //float colortype = Projectile.ai[0] switch
            //{
            //    0 => 0,
            //    1 => 1,
            //    2 => 3,
            //    3 => 6,
            //    4 => 8,
            //    _ => 0
            //};
            //float colortype = Main.rand.NextFromList(0, 1, 3, 6);
            

            float value = (TrueFactor % 0.2f);
            float min = 0.4f;
            float factor = 1;
            if(value < 0.1f)
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
            
            float colortype = 5;
            if (Main.rand.NextBool()) colortype = 3;
            //float colortype = Projectile.ai[2] switch
            //{
            //    <= 0.2f => 3,
            //    <= 0.4f => 5,
            //    <= 0.6f => 3,
            //    <= 0.8f => 5,
            //    <= 1f => 3,
            //    _ => 5
            //};
            Hue = Utils.Remap(value, 0, 0.2f, 108, 180);
            //Projectile.localAI[2] = colortype * 36;
        }
        public override void AI()
        {

            Projectile camera = Main.projectile[(int)Owner];

            if (camera.TypeAlive(ProjectileType<MiracleCameraEXProj>()))
            {
                //if (Projectile.Opacity < 1f) Projectile.Opacity += 0.04f;

                var miracleCamera = camera.ModProjectile as MiracleCameraEXProj;
                var player = (camera.ModProjectile as BaseCameraProj).player;

                if (player.AliveCheck(Projectile.Center, 3000))
                {

                    if (Released <= 0)
                    {
                        if (Projectile.timeLeft < 4 * 60) Projectile.timeLeft++;
                        Vector2 starpos = camera.Center + (miracleCamera.OrbitRotation + Offset * MathHelper.TwoPi / 5).ToRotationVector2() * miracleCamera.StarOrbitRadius;
                        float trueFactor = TrueFactor + 0.2f * Offset;
                        trueFactor = trueFactor % 1f;
                        Vector2 pos = AyaUtils.GetPentagramPos(starpos, miracleCamera.StarRadius * (1 + MathF.Cos(Main.GameUpdateCount * 0.04f + Offset * 1.2f) * 0.1f), 
                            trueFactor, MathHelper.TwoPi / 10f + miracleCamera.OrbitRotation);
                        float chaseFactor = Utils.Remap(Projectile.timeLeft, 4 * 60, 5 * 60, 0.9f, 0.2f);
                        //if (Projectile.timeLeft > 5 * 60 + 1) chaseFactor = 0.2f;
                        Projectile.Center = Vector2.Lerp(Projectile.Center, pos, chaseFactor);
                    }
                    else
                    {
                        
                        if (Projectile.Opacity < 0.8f) Projectile.Opacity += 0.01f;
                        
                        Projectile.velocity += Projectile.velocity.Length(0.2f);
                    }
                }
            }
            else if (Released <= 0) Projectile.Kill();

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

    public class MiracleRailEX : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "StarTexture";
        public ref float Offset => ref Projectile.ai[0];
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
            if (camera.TypeAlive(ProjectileType<MiracleCameraEXProj>()))
            {
                
                if (Projectile.timeLeft < MaxItemTime - 1) Projectile.timeLeft++;
                if (Released <= 0)
                {

                    var miracleCamera = camera.ModProjectile as MiracleCameraEXProj;
                    var player = (camera.ModProjectile as BaseCameraProj).player;

                    if (!player.controlUseItem) Released = 1;
                    float factor = Projectile.TimeleftFactor();
                    Vector2 starpos = camera.Center + (miracleCamera.OrbitRotation + Offset * MathHelper.TwoPi / 5).ToRotationVector2() * miracleCamera.StarOrbitRadius;
                    float trueFactor = CurrentStack * 0.2f + 0.2f * Offset;
                    trueFactor = trueFactor % 1f;
                    float tlFactor = Utils.Remap(Projectile.timeLeft, Projectile.MaxTimeleft() - 30, Projectile.MaxTimeleft(), 1f, 0f);

                    Vector2 pos = AyaUtils.GetPentagramPos(starpos, miracleCamera.StarRadius * tlFactor * (1 + MathF.Cos(Main.GameUpdateCount * 0.04f + Offset * 1.2f) * 0.1f),
                        trueFactor, MathHelper.TwoPi / 10f + miracleCamera.OrbitRotation);
                    
                    Projectile.Center = Vector2.Lerp(Projectile.Center, pos, 0.9f);
                    Vector2 nextPos = starpos + (pos - starpos).RotatedBy(MathHelper.ToRadians(144));
                    Projectile.velocity = nextPos - pos;

                    foreach(var particle in ParticleManager.Particles)
                    {
                        if (particle is RingParticle && particle.Source is EntitySource_Parent parent && parent.Entity is Projectile parentproj)
                        {
                            if (parentproj != Projectile) continue;
                            particle.Center = nextPos;
                        }
                    }
                    //(Main.GameUpdateCount /* + 30 * Projectile.ai[0]*/) % (150 ) == (int)(30 + 30 * Projectile.localAI[0])
                    if(Projectile.timeLeft == MaxItemTime || (miracleCamera.StarStack > 4 && player.itemTime == 1 && (miracleCamera.StarStack - 1) % 5 == Projectile.localAI[0]))
                    {
                        float volume = 0.6f;
                        float radius = 90;
                        int playtime = 1;
                        if (miracleCamera.StarStack == 4 && CurrentStack == 4)
                        {
                            //volume *= 1.5f;
                            playtime+= 2;
                            radius *= 2f;
                        }
                        for (int i = 0; i < playtime; i++)
                        {
                            SoundEngine.PlaySound(SoundID.Item35 with
                            {
                                MaxInstances = 20,
                                Volume = volume
                            }, nextPos);
                        }
                        RingParticle.Spawn(Projectile.GetSource_FromAI(), nextPos, new Color(72,206,132).AdditiveColor(), 10, radius, 0.8f, 0f,
                            0.15f, 0.5f, 30, 120, Ease.OutCirc, Ease.OutCubic);

                        int stardmg = (int)(Projectile.damage * 0.2f);
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), nextPos, Projectile.velocity.Length(7), ProjectileType<MiracleStarHomingEX>(), stardmg, Projectile.knockBack, Projectile.owner);

                    }
                }
                else
                {
                    Projectile.Kill();
                }
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
            for(int i = 0; i < drawcount; i++)
            {
                float totalFactor = (float)i / drawcount;
                if (totalFactor > drawFactor) continue;
                var ndir = dir.RotatedBy(MathHelper.PiOver2);
                Vector2 offset = ndir * 2;
                Color drawColor = Color.Lerp(new Color(28, 255, 144), new Color(25, 255, 251), totalFactor);
                float alphafactor = MathHelper.Lerp(1f, 0f, totalFactor);
                Color color = drawColor.AdditiveColor() * 0.075f * totalFactor * alphafactor;

                for(int j = -1; j < 2; j += 2)
                {
                    Vector2 pos = Projectile.Center - Main.screenPosition + dir * totalFactor * length + new Vector2(0, width * j).RotatedBy(rot) * Projectile.scale;
                    Rectangle sourceRect = new Rectangle(18 - 18 * j, 0, 36, 72);
                    Vector2 origin = new Vector2(18 + 18 * j, 36);
                    Main.spriteBatch.Draw(star, pos, sourceRect, color, rot + MathHelper.PiOver2, origin, Projectile.scale * new Vector2(xScale, yScale), 0, 0);

                }

                if(i == drawcount - 1)
                {
                    Vector2 pos = Projectile.Center - Main.screenPosition + dir * totalFactor * length;
                    Color ballColor = Color.LightGreen.AdditiveColor();
                    for(int j = 0; j < 5; j++)
                    {
                        Main.spriteBatch.Draw(ball1, pos, null, ballColor * 0.6f, Main.GameUpdateCount * 0.1f, ball1.Size() / 2, Projectile.scale * (0.3f + j * 0.1f), 0, 0);

                    }
                }
            }
            return false;
        }
    }
    public class MiracleStarHomingEX : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";
        public ref float Hue => ref Projectile.localAI[2];
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 5);
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
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
        }

        public override void OnSpawn(IEntitySource source)
        {
            Hue = Main.rand.NextFromList(0, 1, 3, 6) * 36;
        }

        public override void AI()
        {
            Projectile.Chase(800, 20,0.03f);
            Projectile.rotation += 0.04f;
        }
        public override void OnKill(int timeLeft)
        {
            int dusttype = Helper.GetHuedDustType((int)Hue);

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
            Texture2D ball1 = Request<Texture2D>(AssetDirectory.Extras + "Ball", AssetRequestMode.ImmediateLoad).Value;

            Color color = AyaUtils.HSL2RGB(Hue, 1f, 0.5f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - (float)i / Projectile.oldPos.Length;

                MeteorStar.DrawStar(Projectile, texture, Projectile.oldPos[i] + Projectile.Size / 2, color, factor * 0.4f, 0.6f, 0.8f, 0.3f);

            }
            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, color, 0.8f, 0.6f, 0.8f, 0.3f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, Color.White, 0.8f, 0.6f, 0.8f, 0.23f);

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color ballColor = AyaUtils.HSL2RGB(Hue, 1f, 0.8f).AdditiveColor();
            for (int j = 0; j < 5; j++)
            {
                Main.spriteBatch.Draw(ball1, pos, null, ballColor * 1f, Main.GameUpdateCount * 0.1f, ball1.Size() / 2, Projectile.scale * (0.3f + j * 0.1f), 0, 0);

            }

            return false;
        }
    }
}
