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
using AyaMod.Common.Easer;
using ReLogic.Content;
using AyaMod.Core.ModPlayers;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class ScarletCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 70;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<ScarletCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.07f, 148, 1.7f,0.5f);
            SetCaptureStats(100, 5);
        }
    }

    public class ScarletCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(247, 36, 79);
        public override Color innerFrameColor => new Color(183, 243, 255) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(218, 216, 226).AdditiveColor() * 0.5f;

        public float[] fadeinFactor = new float[5];
        public float visualRotation = 0f;
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            if(EffectCounter < 5) fadeinFactor[EffectCounter] = 1f;
            if (++EffectCounter >= 6)
            {
                Vector2 vec = Projectile.Center - player.Center;
                int damage = (int)(Projectile.damage * 0.2f);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ProjectileType<KillerDoll>(), damage, Projectile.knockBack / 2f, Projectile.owner);
                EffectCounter = 0;
            }
        }

        public override void PostAI()
        {
            for(int i = 0; i < 5; i++)
            {
                if (fadeinFactor[i] > 0) 
                { 
                    fadeinFactor[i] -= 0.04f; 
                }
            }
            visualRotation += 0.012f;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D knife = Request<Texture2D>(AssetDirectory.Projectiles + "Knife").Value;

            int drawcount = 0;
            int maxknife = EffectCounter;
            for(int i = 0; i < maxknife; i++)
            {
                if (++drawcount > 5) break;
                float factor = Utils.Remap(this.fadeinFactor[i], 0f, 1f, 1f, 0f);
                factor = EaseManager.Evaluate(Ease.OutSine, factor, 1f);
                float rot = visualRotation + i * MathHelper.TwoPi / 5;
                Vector2 offset = rot.ToRotationVector2() * (20 + 120 * factor);
                Vector2 pos = Projectile.Center + offset - Main.screenPosition;
                Color color = Color.White * Projectile.Opacity * 0.3f * factor;

                //RenderHelper.DrawBloom(6, 2, knife, pos, null, color, rot + MathHelper.Pi, knife.Size() / 2, Projectile.scale * 0.7f);

                Main.spriteBatch.Draw(knife, pos, null, color, rot + MathHelper.Pi, knife.Size() / 2, Projectile.scale * 0.8f, 0, 0);

            }
        }
    }

    public class KillerDoll : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public ref float rotDirection => ref Projectile.localAI[0];
        public ref float rotOffset => ref Projectile.localAI[1];
        public ref float radius => ref Projectile.localAI[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 160;
        }

        public override void OnSpawn(IEntitySource source)
        {
            rotDirection = Main.rand.NextBool() ? 1 : -1;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {

            var owner = Main.player[Projectile.owner];
            if (owner.AliveCheck(Projectile.Center, 4000))
            {
                Projectile.Center = /*Vector2.Lerp(Projectile.Center, owner.Center, 0.2f);*/owner.Center;
            }
            rotOffset -= 0.05f * rotDirection;
            if (radius < 100)
            {
                radius += 3;
            }
            if(Projectile.timeLeft % 6 == 0 && Projectile.timeLeft > 40)
            {
                for(int i = 0; i < 2; i++)
                {
                    float rot = Projectile.rotation + rotOffset + i * MathHelper.Pi;
                    Vector2 dir = rot.ToRotationVector2();

                    Vector2 pos = Projectile.Center + dir * radius;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<FlyingKnife>(), Projectile.damage, Projectile.knockBack / 2f, Projectile.owner,Projectile.whoAmI);
                }
            }

            Projectile.rotation += MathHelper.Pi / 10f / 3f * rotDirection;
        }
    }

    public class FlyingKnife : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + "Knife";
        public ref float Owner => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float OwnerDir => ref Projectile.localAI[0];
        public ref float OwnerDist => ref Projectile.localAI[1];
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 12);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            Projectile.timeLeft = 7 * 60;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dustcount = 10;
            for(int i = 0;i < dustcount; i++)
            {
                Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(3f, 8f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.SilverFlame, vel);
                d.noGravity = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = AyaUtils.RandAngle;
            Projectile.scale = 0.7f;
            var proj = Main.projectile[(int)Owner];
            Vector2 offset = Projectile.Center - proj.Center;
            OwnerDir = offset.ToRotation();
            OwnerDist = offset.Length();
        }
        public override void AI()
        {
            Timer++;
            if (Timer > 45)
            {
                var owner = Main.player[Projectile.owner];
                if (owner.AliveCheck(Projectile.Center, 4000))
                {
                    var mousepos = owner.GetModPlayer<CameraPlayer>().MouseWorld;
                    Vector2 tomouse = Projectile.Center.DirectionToSafe(mousepos);
                    Projectile.velocity = tomouse * 15f;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Timer = int.MinValue;
                }
                
            }
            else if (Timer >= 0)
            {
                var proj = Main.projectile[(int)Owner];
                if (proj.type == ProjectileType<KillerDoll>())
                {
                    Vector2 offset = OwnerDir.ToRotationVector2() * OwnerDist;
                    Projectile.Center = proj.Center + offset;
                    Projectile.rotation += 0.25f;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.position += Projectile.velocity * 1.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2;
            Texture2D star = TextureAssets.Extra[98].Value;

            float timeleftFactor = MathHelper.Clamp(Timer / 45f, 0f, 1f);
            if (Timer < 0) timeleftFactor = 1f;
            float alpha = Projectile.Opacity * timeleftFactor;
            Color color = Color.White.AdditiveColor() * alpha * 0.7f;
            Color trailBaseColor = new Color(192, 192, 192).AdditiveColor() * alpha * 0.7f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = i == 0 ? Projectile.rotation : Projectile.oldRot[i];
                Vector2 drawpos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color trailColor = trailBaseColor * factor * 0.6f;
                if (Projectile.velocity.Length() < 0.1f)
                {
                    trailColor *= 0.6f;
                    Vector2 tipPos = drawpos + rot.ToRotationVector2() * 18;
                    Vector2 starScale = new Vector2(0.15f, 0.5f) * Projectile.scale;
                    for (int j = 0; j < 2; j++)
                    {
                        Main.spriteBatch.Draw(star, tipPos, null, trailColor.AdditiveColor()*1.4f, rot, star.Size() / 2, starScale, 0, 0);
                    }
                }
                Main.spriteBatch.Draw(texture, drawpos, null, trailColor, rot, origin, Projectile.scale, 0, 0);
            }

            RenderHelper.DrawBloom(10, 4, texture, Projectile.Center - Main.screenPosition, null, trailBaseColor.AdditiveColor() * alpha * 0.25f, Projectile.rotation, origin, Projectile.scale);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(210,210,210) * alpha, Projectile.rotation, origin, Projectile.scale, 0);

            return false;
        }
    }
}
