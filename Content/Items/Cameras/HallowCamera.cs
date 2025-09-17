using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class HallowCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 50;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<HallowCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.05f, 150, 1.6f, 0.5f);
            SetCaptureStats(100, 5);
        }

        public static int radiusa = 320;
        public static int radiusb = 120;
        public static float rotSpeed = 0.06f;
    }

    public class HallowCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(250, 232, 136);
        public override Color innerFrameColor => new Color(232, 79, 39) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(249, 239, 189).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;
            if(++EffectCounter >= 1)
            {
                float direction = Main.rand.NextBool() ? 1 : -1;
                float dmgmult = 0.25f;
                var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HallowPoint>(),
                    (int)(Projectile.damage * dmgmult), Projectile.knockBack, Projectile.owner, Projectile.whoAmI, direction);
                proj.rotation = Projectile.ai[0];
                Projectile.ai[0] += (MathHelper.PiOver4 + Main.rand.NextFloat(-0.2f,0.2f)) * -direction;

                EffectCounter = 0;
            }


            int dustcount = 24;
            for (int i = 0; i < dustcount; i++)
            {
                Vector2 vec = Main.rand.NextVector2Unit();
                Vector2 pos = Projectile.Center + vec * Main.rand.NextFloat(24, 48);
                Vector2 vel = vec * Main.rand.NextFloat(2f,3f) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f,2f);
                Color color = new Color(250, 232, 136) * Main.rand.NextFloat(0.5f,1f);
                var light = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel * 0.8f, color, 30);
                light.Scale = 1.5f;
            }
        }
    }

    public class HallowPoint : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Owner => ref Projectile.ai[0];
        public ref float Direction => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 140;
        }
        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            for(int i = 0; i < 8; i++)
            {
                float offset = MathHelper.TwoPi / 8f * i;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<HallowLightBall>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner,Projectile.whoAmI, Direction, offset);
            }
        }

        public override void AI()
        {
            if (Main.projectile[(int)Owner].TypeAlive(ProjectileType<HallowCameraProj>()))
            {
                var owner = Main.projectile[(int)Owner];
                Projectile.Center = Vector2.Lerp(Projectile.Center, owner.Center, 0.35f);
            }
            else
            {
                Projectile.Kill();
            }

            Projectile.rotation += 0.02f * Direction;
        }
    }

    public class HallowLightBall : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Ball4";

        public static Color DrawColor = new Color(250, 232, 136);

        public ref float Owner => ref Projectile.ai[0];
        public ref float Direction => ref Projectile.ai[1];
        public ref float Offset => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(20);
            Projectile.ArmorPenetration = 20;
            //Projectile.usesIDStaticNPCImmunity = true;
            //Projectile.idStaticNPCHitCooldown = 6;
            Projectile.timeLeft = 130;
        }
        public override bool? CanDamage() => Projectile.timeLeft < 120;
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void AI()
        {
            var alpha = getAlpha(Projectile);
            Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f),250 / 255f * alpha,232 / 255f *alpha,136 / 255f *alpha);


            Vector2 tocenter = Vector2.Zero;
            if (Main.projectile[(int)Owner].TypeAlive(ProjectileType<HallowPoint>()))
            {
                var owner = Main.projectile[(int)Owner];

                float radiusa = HallowCamera.radiusa;
                float radiusb = HallowCamera.radiusb;
                float rotspeed = HallowCamera.rotSpeed;
                float targetX = MathF.Cos(Offset + Main.GameUpdateCount * rotspeed * Direction) * radiusa;
                float targetY = MathF.Sin(Offset + Main.GameUpdateCount * rotspeed * Direction) * radiusb;
                Vector2 targetPos = owner.Center + new Vector2(targetX, targetY).RotatedBy(owner.rotation);
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.1f);

                tocenter = Projectile.Center.DirectionToSafe(owner.Center);
            }
            else
            {
                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 5);
                }
                Projectile.velocity *= 0.98f;
            }

            float factor = Projectile.TimeleftFactor();
            int dustcount = 1;
            Vector2 vel = Projectile.position - Projectile.oldPosition;
            if (Main.rand.NextBool(4) && factor < 0.95f)
            {
                for (int i = 0; i < dustcount; i++)
                {
                    int type = Utils.SelectRandom(Main.rand, new int[] { /*DustID.AmberBolt,*/ /*DustID.GoldFlame*/DustID.YellowTorch });


                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 32);
                    float alphaFactor = Utils.Remap(factor, 0, 0.6f, 0f, 1f);
                    var light = LightParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel * Main.rand.NextFloat(0.15f,0.35f), DrawColor * alphaFactor, 15);
                    light.Scale = 1.5f;
                }
            }
        }
        public static float getAlpha(Projectile projectile)
        {
            float factor = projectile.TimeleftFactor();
            float fadeinFactor = Utils.Remap(factor, 0.8f, 1f, 1f, 0f);
            var fac = MathHelper.Lerp(0, 1, EaseManager.Evaluate(Ease.InCubic, factor, 1));
            float fadeoutFactor = Utils.Remap(fac, 0f, 0.2f, 0f, 1f);
            return fadeinFactor * fadeoutFactor;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color baseColor = DrawColor;

            var alpha = getAlpha(Projectile);

            float targetY = MathF.Sin(Offset + Main.GameUpdateCount * HallowCamera.rotSpeed * Direction);
            float vecYFactor = Utils.Remap(targetY, 1f, 0f, 0.1f, 1f);

            Color drawColor = (baseColor * alpha * vecYFactor).AdditiveColor();
            Color drawColor2 = (baseColor * alpha).AdditiveColor();
            float scale = 0.35f * Projectile.scale;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, texture.Size() / 2, scale, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, texture.Size() / 2, scale * 0.5f, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor2, Projectile.rotation, texture.Size() / 2, scale * 0.25f, 0);
            return false;
        }
    }
}
