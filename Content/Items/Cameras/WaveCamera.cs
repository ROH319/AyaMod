using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AyaMod.Core.Globals;
using Terraria.Audio;
using AyaMod.Core;
using AyaMod.Content.Particles;

namespace AyaMod.Content.Items.Cameras
{
    public class WaveCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {

            Item.width = 52;
            Item.height = 48;

            Item.damage = 16;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<WaveCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.04f, 92, 2f);
            SetCaptureStats(100, 5);
        }
    }

    public class WaveCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(43, 178, 255);
        public override Color innerFrameColor => new Color(0, 73, 30);
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(145, 255, 245).AdditiveColor() * 0.5f;
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            float dmgmult = 0.7f;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScaleWave>(),
                (int)(Projectile.damage * dmgmult), 0, Projectile.owner);
        }
    }

    public class ScaleWave : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Ball";
        public int TrailCount = 10;

        public ref float CurrentRadius => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {


            Projectile.oldRot = new float[TrailCount];
            for(int i = 0;i < TrailCount; i++)
            {
                Projectile.oldRot[i] = 25;
            }
            CurrentRadius = 25;

            for (int i = 0; i < 4; i++)
            {
                SoundEngine.PlaySound(SoundID.Splash with
                {
                    MaxInstances = 30,
                    Volume = 1f
                }, Projectile.Center);
            }


        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.noTileCollide || Collision.CanHitLine(Projectile.Center,1,1,target.position,target.width,target.height);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckRingCollision(targetHitbox, Projectile.Center, CurrentRadius - 10, CurrentRadius + 10);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float factor = Projectile.TimeleftFactor();
            modifiers.FinalDamage *= Utils.Remap(factor, 0, 1, 0.5f, 1f);
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            float vel = 1f;

            vel = Utils.Remap(factor, 1f, 0f, 6f, 0f);
            Projectile.Opacity = factor;
            Projectile.oldRot[0] = CurrentRadius;
            for(int i = TrailCount - 1; i > 0; i--)
            {
                Projectile.oldRot[i] = Projectile.oldRot[0] - i * 6;
            }


            int dustcount = 16;
            for (int i = 0; i < dustcount; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * CurrentRadius;
                Dust d = Dust.NewDustPerfect(pos, DustID.Water, Projectile.Center.DirectionToSafe(pos) * vel * 0.4f);
                d.noGravity = true;
            }

            CurrentRadius += vel * 0.8f;

            int checkCount = (int)Utils.Remap(CurrentRadius, 25, 500, 4, 46);
            for(int i = 0; i < checkCount - 1; i++)
            {
                Vector2 p1 = Projectile.Center + (MathHelper.TwoPi / (float)checkCount * i).ToRotationVector2() * CurrentRadius;
                Vector2 p2 = Projectile.Center + (MathHelper.TwoPi / (float)checkCount * (i + 1)).ToRotationVector2() * CurrentRadius;
                Utils.PlotTileLine(p1, p2,3, new Utils.TileActionAttempt(DelegateMethods.CutTiles));
            }
            {
                Vector2 p1 = Projectile.Center + (MathHelper.TwoPi / (float)checkCount * (checkCount - 1)).ToRotationVector2() * CurrentRadius;
                Vector2 p2 = Projectile.Center + Vector2.UnitX * CurrentRadius;
                Utils.PlotTileLine(p1, p2, 3, new Utils.TileActionAttempt(DelegateMethods.CutTiles));

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D star = AssetDirectory.StarTexture;

            float timeleftFactor = Projectile.TimeleftFactor();

            int drawcount = (int)Utils.Remap(CurrentRadius,25,500,25,500);
            for(int j = 0;j < drawcount; j++)
            {
                float rotFactor = MathHelper.TwoPi / drawcount * j;

                for (int i = 0; i < TrailCount; i++)
                {
                    Vector2 pos = Projectile.Center + rotFactor.ToRotationVector2() * Projectile.oldRot[i];
                    float trailFactor = i / (float)TrailCount;

                    float alphaFactor = Utils.Remap(trailFactor,0,1f,1f,0);
                    alphaFactor *= Utils.Remap(timeleftFactor, 0, 1, 1f, 0.6f);
                    Color color = new Color(43, 178, 255).AdditiveColor() * alphaFactor * 0.07f * Projectile.Opacity;
                    if (i == 0) color *= 2f;
                    Main.spriteBatch.Draw(texture, pos - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.6f, 0, 0);

                }
                {
                    Color bloomcolor = Color.White.AdditiveColor() * 0.2f * Projectile.Opacity;
                    Vector2 pos = Projectile.Center + rotFactor.ToRotationVector2() * MathHelper.Lerp(Projectile.oldRot[0], Projectile.oldRot[1],0.5f) - Main.screenPosition;
                    Main.spriteBatch.Draw(star, pos, null, bloomcolor, rotFactor, star.Size() / 2, new Vector2(0.5f, 1f), 0, 0);
                }
            }

            return false;
        }
    }
}
