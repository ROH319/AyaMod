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

namespace AyaMod.Content.Items.Cameras
{
    public class AliceCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 54;

            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<AliceCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 152, 1.6f, 0.5f);
            SetCaptureStats(100, 5);
        }
    }

    public class AliceCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(174, 238, 180);
        public override Color innerFrameColor => new Color(248, 222, 96) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(196, 243, 240).AdditiveColor() * 0.5f;

        public override void OnSpawn(IEntitySource source)
        {
            for(int i = 0; i < 5; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<MarisaCircle>(), Projectile.damage, Projectile.knockBack, Projectile.owner, i, Projectile.whoAmI);
            }
        }
    }
    public class MarisaCircle : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.timeLeft++;

            float radiusFactor = MathF.Sin(Main.GameUpdateCount * 0.005f);
            float radius =radiusFactor * 350f;
            
            Projectile camera = Main.projectile[(int)Projectile.ai[1]];
            if (camera.TypeAlive(ProjectileType<AliceCameraProj>()))
            {
                if (Projectile.Opacity < 1f)
                    Projectile.Opacity += 0.02f;

                var player = (camera.ModProjectile as BaseCameraProj).player;
                if(player.AliveCheck(Projectile.Center,2000))
                {
                    if (player.itemTime != 0)
                    {
                        Projectile.localAI[1]++;
                    }
                    if (Projectile.localAI[1] > 14)
                    {
                        Vector2 target = camera.Center + camera.DirectionToSafe(Projectile.Center).RotatedBy(MathHelper.PiOver4 * 1.5f) * 120f;
                        Vector2 vel = Projectile.Center.DirectionToSafe(target) * 20f;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<MarisaStar>(), (int)(Projectile.damage * 0.2f), Projectile.knockBack, Projectile.owner);

                        Projectile.localAI[1] = 0;
                    }

                }

                Vector2 targetpos = camera.Center + (MathHelper.TwoPi / 5 * Projectile.ai[0] + Projectile.localAI[2]).ToRotationVector2() * radius;
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetpos, 0.9f);

            }
            else
            {
                Projectile.Kill();
            }
            float rotFactor = -MathF.Abs(MathF.Sin(Main.GameUpdateCount * 0.005f));
            Projectile.localAI[2] += 0.035f + rotFactor * 0.027f;
            Projectile.rotation += 0.02f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            Vector2 origin = texture.Size() / 2;
            Color color = new Color(242, 104, 20).AdditiveColor();

            float ringRadius = 42;

            RenderHelper.DrawRing(72, Projectile.Center, ringRadius, color, Projectile.rotation, new Vector2(0.25f, 0.8f) * 0.6f);
            RenderHelper.DrawRing(72, Projectile.Center, ringRadius * 0.8f, color, Projectile.rotation, new Vector2(0.25f, 0.8f) * 0.6f);

            Vector2 offset = Projectile.Center - Main.screenPosition;
            Vector2 scale1 = new Vector2(8 / 64f, 24 / 64f);
            for (int j = 0; j < 2; j++)
            {
                float radius = ringRadius * 0.9f + j * ringRadius * 0.4f;
                for (int i = 0; i < 5; i++)
                {
                    float dir1 = MathHelper.TwoPi / 5 * i + Projectile.rotation;
                    float dir2 = MathHelper.TwoPi / 5 * (i + 1) + Projectile.rotation;
                    Vector2 vec1 = dir1.ToRotationVector2() * radius;
                    Vector2 vec2 = dir2.ToRotationVector2() * radius;
                    Vector2 middle = Vector2.Lerp(vec1, vec2,0.5f) * 0.5f;

                    var dist = (int)(vec1.Distance(middle) / 4f);
                    var tomiddle = vec1.AngleTo(middle);
                    var tovec2 = middle.AngleTo(vec2);
                    for(int k = 1; k < dist; k++)
                    {
                        float factor = k / (float)dist;
                        Vector2 pos1 = Vector2.Lerp(vec1,middle,factor);
                        Main.spriteBatch.Draw(texture, pos1 + offset, null, color, tomiddle + MathHelper.PiOver2, origin, scale1, 0, 0);

                        Vector2 pos2 = Vector2.Lerp(middle, vec2, factor);
                        Main.spriteBatch.Draw(texture, pos2 + offset, null, color, tovec2 + MathHelper.PiOver2, origin, scale1, 0, 0);
                    }

                }
            }

            for(int i = 0; i < 5; i++)
            {
                float dir1 = MathHelper.TwoPi / 5 * i + -Projectile.rotation * 0.7f;
                Vector2 vec1 = dir1.ToRotationVector2() * ringRadius;
                Vector2 tonext = (dir1 + MathHelper.ToRadians(126)).ToRotationVector2();

                var dist = (int)(2 * ringRadius * 1.15f * MathF.Sin(MathHelper.ToRadians(36)) / 4f);
                for(int j = 1;j < dist - 2;j++)
                {
                    float factor = j / (float)dist;
                    Vector2 pos = vec1 + tonext * factor * dist * 4f;
                    Main.spriteBatch.Draw(texture, pos + offset, null, color, tonext.ToRotation() + MathHelper.PiOver2, origin, scale1, 0, 0);
                }

            }
            return false;
        }
    }

    public class MarisaStar : ModProjectile
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
            Projectile.localAI[2] = Main.rand.Next(11) * 36;
            //Projectile.localAI[2] = 10 * 36;
            Projectile.Opacity = 0f;
            Projectile.localAI[1] = Projectile.velocity.ToRotation();
            Projectile.localAI[0] = Projectile.velocity.Length();
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            if(factor < 0.84f)
            {
                Projectile.Chase(2000, 26f, 0.1f);
            }
            else
            {
                float min = -1.3f * Projectile.localAI[0];
                float length = Utils.Remap(factor, 0.8f, 1f, min, Projectile.localAI[0]);
                Projectile.velocity = Projectile.localAI[1].ToRotationVector2() * length;
                //Projectile.velocity -= Projectile.localAI[1].ToRotationVector2() * (0.2f - Projectile.velocity.Length() * 0.05f);
            }
            Projectile.Opacity += 0.03f;
            if(Projectile.Opacity > 1f)Projectile.Opacity = 1f;
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

            for(int i = 0; i < Projectile.oldPos.Length; i++)
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
