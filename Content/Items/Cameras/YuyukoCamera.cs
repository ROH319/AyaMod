using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using AyaMod.Helpers;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using AyaMod.Core.Configs;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class YuyukoCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 60;

            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<YuyukoCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 152, 1.6f, 0.5f);
            SetCaptureStats(100, 5);
        }
    }

    public class YuyukoCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(231, 145, 172);
        public override Color innerFrameColor => new Color(162, 244, 238) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(235, 220, 225).AdditiveColor() * 0.5f;

        public override void OnHitNPCAlt(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),target.Center,Projectile.DirectionToSafe(target.Center).RotatedByRandom(MathHelper.Pi) * 24,ProjectileType<YoumuSpirit>(),Projectile.damage,Projectile.knockBack,Projectile.owner,target.whoAmI);
            base.OnHitNPCAlt(target, hit, damageDone);
        }
        public override void OnSnapInSight()
        {
            base.OnSnapInSight();
        }

        public override void PostAI()
        {
            var mouseworld = player.GetModPlayer<CameraPlayer>().MouseWorld;

            float range = 480;
            if(Projectile.Distance(mouseworld) > range/* && Projectile.ai[0] < 1*/)
            {
                if (Projectile.ai[1] < 1)
                    Projectile.ai[0]++;
            }
            else
            {
                Projectile.ai[0]--; Projectile.ai[0] = MathHelper.Clamp(Projectile.ai[0], 0f, 20f);
            }
            if (Projectile.ai[1] > 0) Projectile.ai[1] = MathHelper.Clamp(--Projectile.ai[1], 0, 100);
            if (Projectile.ai[0] > 0)
            {
                float factor = MathHelper.Clamp((Projectile.ai[0]) / 20f, 0f, 1f);
                Projectile.Center = Vector2.Lerp(Projectile.Center,Projectile.oldPosition + Projectile.Size/2, factor);
                
                Projectile.ai[0]++;
            }
            if (Projectile.ai[0] >= 20)
            {
                Helper.PlayPitched("BladeSlash", 1f, position: player.Center);


                Vector2 prev = Projectile.Center;

                {
                    float length = Projectile.Distance(mouseworld);
                    float dir = Projectile.AngleToSafe(mouseworld);
                    Vector2 vel = dir.ToRotationVector2() * 18;

                    Projectile.Center = mouseworld + vel * 4;

                    var slash = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), prev, vel, ProjectileType<YoumuSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, length,15);
                    slash.rotation = dir;
                }

                int slashcount = 8;
                float baseRot = AyaUtils.RandAngle;
                for(int i = 0;i < slashcount;i++)
                {
                    Helper.PlayPitched("BladeSlash", 0.1f, position: player.Center);


                    Vector2 pos = Projectile.Center +  (MathHelper.TwoPi / slashcount * i + baseRot).ToRotationVector2() * 240;

                    float dir = pos.AngleTo(Projectile.Center) + MathHelper.PiOver4;
                    
                    Vector2 vel = dir.ToRotationVector2() * 18;
                    pos -= vel * 3.5f;
                    float length = 340;
                    var slash = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ProjectileType<YoumuSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, length,25);
                    slash.rotation = dir;
                }

                Projectile.ai[0] = 0;
                Projectile.ai[1] = 20;
            }
            //Main.NewText($"{Projectile.ai[0]}");
        }
    }

    public class YoumuSpirit : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 26);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);

            Projectile.timeLeft = 120;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[2] = Main.rand.NextBool() ? 1 : -1;
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (target != null && target.CanBeChasedBy())
            {
                Vector2 totarget = Projectile.DirectionToSafe(target.Center) * 20f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, totarget, 0.03f);

                if(Projectile.velocity.AngleBetween(totarget) < MathHelper.PiOver4 && Projectile.velocity.Length() > 6f && Projectile.timeLeft < 90)
                {
                    Helper.PlayPitched("BladeSlash", 0.5f, position: Projectile.Center);

                    float length = 400;
                    var slash = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, totarget, ProjectileType<YoumuSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, length);
                    slash.rotation = totarget.ToRotation();
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.Opacity -= 0.02f;
                if (Projectile.Opacity < 0.02f) Projectile.Kill();
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(0.02f * Projectile.localAI[2]);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Extra[59].Value;
            Vector2 origin = texture.Size() / 2;

            Vector2 baseScale = new Vector2(0.2f, 0.2f);
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = i == 0 ? Projectile.rotation : /*(Projectile.oldPos[i - 1] - Projectile.oldPos[i]).ToRotation()*/Projectile.oldRot[i];
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color color = lightColor.AdditiveColor() * factor * 0.5f * Projectile.Opacity;

                var trailScale = baseScale * factor * Projectile.scale;
                Main.spriteBatch.Draw(texture, drawPos, null, color, rot, origin, trailScale, 0, 0);
            }

            return false;
        }
    }
    public class YoumuSlash : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Ball4";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(-1);
            Projectile.timeLeft = 30;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.ai[0], 4, ref point);
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] > 0)
            {
                Projectile.timeLeft = (int)Projectile.ai[1];
            }
        }
        public override void AI()
        {
            //Projectile.position -= Projectile.velocity * 0.8f;
            Projectile.velocity *= 0.85f;

            float factor = Projectile.TimeleftFactor();
            Projectile.Opacity = factor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(0,texture.Height / 2);
            float alpha = Projectile.Opacity;
            Vector2 scale = new Vector2(Projectile.ai[0] / 256, 4f / 256) * Projectile.scale;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, origin, scale, 0, 0);

            return false;
        }
    }
}
