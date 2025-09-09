using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class MiracleCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 100;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<MiracleCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 9f;

            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(0, 0, 60, 0));
            SetCameraStats(0.08f, 164, 1.5f, 0.6f);
            SetCaptureStats(100, 5);
        }
    }

    public class MiracleCameraProj : BaseCameraProj
    {

        public override Color outerFrameColor => new Color(165, 228, 138);
        public override Color innerFrameColor => new Color(182, 196, 28) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(107, 203, 0).AdditiveColor() * 0.5f;

        public float OrbitRotation;
        public float StarOrbitRadius = 400;
        public float StarRadius = 120;
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
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<MiracleStarHoming>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void PostAI()
        {
            StarOrbitRadius = 350;
            StarRadius = 200;
            if (player.Aya().itemTimeLastFrame <= 1 && player.itemTime > 10)
            {
                MaxItemTime = player.itemTime;
            }
            if (!player.ItemTimeIsZero && MaxItemTime >= player.itemTime)
            {
                StarFactor = Utils.Remap(player.itemTime,1,MaxItemTime,0.2f,0);
                if (player.itemTime == 1) StarStack++;

                if (player.controlUseItem && player.itemTime % 3 == 0 && StarStack * 0.2f + StarFactor < 1f)
                {
                    float trueFactor = StarStack * 0.2f + StarFactor;
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 starpos = Projectile.Center + (OrbitRotation + i * MathHelper.TwoPi / 5).ToRotationVector2() * StarOrbitRadius;
                        Vector2 pos = starpos/*AyaUtils.GetPentagramPos(starpos, StarRadius, trueFactor)*/;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<MiracleStar>(), Projectile.damage, 0f, Projectile.owner, i, Projectile.whoAmI, trueFactor);
                    }
                }
            }

            if(!player.controlUseItem)
            {
                foreach(var projectile in Main.ActiveProjectiles)
                {
                    if (projectile.type != ProjectileType<MiracleStar>() || projectile.localAI[0] > 0 || projectile.ai[1] != Projectile.whoAmI) continue;
                    projectile.localAI[0] = 1;
                    Vector2 starpos = Projectile.Center + (OrbitRotation + projectile.ai[0] * MathHelper.TwoPi / 5).ToRotationVector2() * StarOrbitRadius;

                    projectile.velocity = starpos.DirectionToSafe(projectile.Center).RotatedBy(MathF.Cos(projectile.ai[2] / 5f) / MathHelper.Pi + 1f).RotatedBy(MathHelper.Pi) * 6.5f;
                }
                StarStack = 0;
                StarFactor = 0;
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
    public class MiracleStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";
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
            Projectile.timeLeft = 6 * 60;
            Projectile.penetrate = 1;
            Projectile.scale = 0.75f;
        }
        public override bool? CanDamage()
        {
            return Projectile.localAI[0] > 0;
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
            Projectile.localAI[2] = colortype * 36;

            Projectile.Opacity = 0.3f;
        }
        public override void AI()
        {

            Projectile camera = Main.projectile[(int)Projectile.ai[1]];

            if (camera.TypeAlive(ProjectileType<MiracleCameraProj>()))
            {
                if (Projectile.Opacity < 1f) Projectile.Opacity += 0.04f;

                var miracleCamera = camera.ModProjectile as MiracleCameraProj;
                var player = (camera.ModProjectile as BaseCameraProj).player;

                if (player.AliveCheck(Projectile.Center, 3000))
                {

                    if (Projectile.localAI[0] <= 0)
                    {
                        if (Projectile.timeLeft < 5 * 60) Projectile.timeLeft++;
                        Vector2 starpos = camera.Center + (miracleCamera.OrbitRotation + Projectile.ai[0] * MathHelper.TwoPi / 5).ToRotationVector2() * miracleCamera.StarOrbitRadius;
                        float trueFactor = Projectile.ai[2] + 0.2f * Projectile.ai[0];
                        trueFactor = trueFactor % 1f;
                        Vector2 pos = AyaUtils.GetPentagramPos(starpos, miracleCamera.StarRadius * (1 + MathF.Cos(Main.GameUpdateCount * 0.04f + Projectile.ai[0] * 1.2f) * 0.1f), trueFactor, MathHelper.TwoPi / 10f + miracleCamera.OrbitRotation);
                        float chaseFactor = Utils.Remap(Projectile.timeLeft, 5 * 60, 6 * 60, 0.9f, 0.2f);
                        //if (Projectile.timeLeft > 5 * 60 + 1) chaseFactor = 0.2f;
                        Projectile.Center = Vector2.Lerp(Projectile.Center, pos, chaseFactor);
                    }
                    else
                    {
                        if (Projectile.Opacity < 1f) Projectile.Opacity += 0.04f;
                    }
                }
            }
            else if (Projectile.localAI[0] <= 0) Projectile.Kill();

            Projectile.rotation += 0.03f;
        }
        public override void OnKill(int timeLeft)
        {
            int dusttype = (Projectile.localAI[2]) switch
            {
                0 * 36 => DustID.TheDestroyer,
                1 * 36 => DustID.GemTopaz,
                2 * 36 => DustID.DryadsWard,
                3 * 36 => DustID.CursedTorch,
                4 * 36 => DustID.PureSpray,
                5 * 36 => DustID.HallowSpray,
                6 * 36 => DustID.MushroomSpray,
                7 * 36 => DustID.GiantCursedSkullBolt,
                8 * 36 => DustID.VenomStaff,
                9 * 36 => DustID.CrystalPulse,
                10 * 36 => DustID.TheDestroyer,
                _ => DustID.GemTopaz
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
            Color color = AyaUtils.HSL2RGB(Projectile.localAI[2], 1f, 0.5f);
            float scaleX = 0.7f;
            float scaleY = 0.9f;
            float scaleMult = 0.3f;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - (float)i / Projectile.oldPos.Length;

                MeteorStar.DrawStar(Projectile, texture, Projectile.oldPos[i] + Projectile.Size / 2, color, factor * 0.4f * alpha, scaleX, scaleY, scaleMult);

            }
            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, color, 0.8f * alpha, scaleX, scaleY, scaleMult);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, Color.White, 0.8f * alpha, scaleX, scaleY, 0.23f);


            return false;
        }
    }
    public class MiracleStarHoming : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";
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
            Projectile.timeLeft = 10 * 60;
            Projectile.penetrate = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[2] = Main.rand.NextFromList(0, 1, 3, 6) * 36;
        }

        public override void AI()
        {
            Projectile.Chase(800, 20);
            Projectile.rotation += 0.04f;
        }
        public override void OnKill(int timeLeft)
        {
            int dusttype = (Projectile.localAI[2]) switch
            {
                0 * 36 => DustID.TheDestroyer,
                1 * 36 => DustID.GemTopaz,
                2 * 36 => DustID.DryadsWard,
                3 * 36 => DustID.CursedTorch,
                4 * 36 => DustID.PureSpray,
                5 * 36 => DustID.HallowSpray,
                6 * 36 => DustID.MushroomSpray,
                7 * 36 => DustID.GiantCursedSkullBolt,
                8 * 36 => DustID.VenomStaff,
                9 * 36 => DustID.CrystalPulse,
                10 * 36 => DustID.TheDestroyer,
                _ => DustID.GemTopaz
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

            Color color = AyaUtils.HSL2RGB(Projectile.localAI[2], 1f, 0.5f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - (float)i / Projectile.oldPos.Length;

                MeteorStar.DrawStar(Projectile, texture, Projectile.oldPos[i] + Projectile.Size / 2, color, factor * 0.4f, 0.6f, 0.8f, 0.3f);

            }
            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, color, 0.8f, 0.6f, 0.8f, 0.3f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            MeteorStar.DrawStar(Projectile, texture, Projectile.Center, Color.White, 0.8f, 0.6f, 0.8f, 0.23f);


            return false;
        }
    }
}
