using AyaMod.Common.Easer;
using AyaMod.Core;
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
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

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
            SetCaptureStats(1000, 60);
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
        public override void OnSpawn(IEntitySource source)
        {
            for(int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ProjectileType<SakuyaCircle>(), 0, 0f, player.whoAmI, Projectile.whoAmI, i);
            }
        }
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;


            int dmg = (int)(Projectile.damage * 0.25f);
            SpawnKnives(Projectile.Center, 4, 300f, 10, dmg, Projectile.knockBack, Projectile.GetSource_FromAI(), player.whoAmI);

            if (EffectCounter < 5) fadeinFactor[EffectCounter] = 1f;
            if (++EffectCounter >= 7)
            {
                float radius = 20f;
                foreach(var proj in Main.ActiveProjectiles)
                {
                    if (proj.type != ProjectileType<SakuyaCircle>()) continue;
                    proj.localAI[0] = 4 * 60;

                    
                    for(int i = 0; i < 30; i++)
                    {
                        Vector2 dir = (MathHelper.TwoPi / 20 * i).ToRotationVector2();
                        for (int j = 0; j < 2; j++)
                        {
                            Vector2 pos = proj.Center + dir * (radius + 18f * j);
                            float speed = 1f + 1.5f * j + Main.rand.NextFloat(0f, 0.5f);
                            Dust d = Dust.NewDustPerfect(pos, 180, dir.RotateRandom(0.2f) * speed, Scale: 1.75f + 0.75f * j);
                            d.noGravity = true;
                        }
                    }
                }
                //Vector2 vec = Projectile.Center - player.Center;
                //int damage = (int)(Projectile.damage * 0.2f);
                //Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ProjectileType<KillerDoll>(), damage, Projectile.knockBack / 2f, Projectile.owner);
                EffectCounter = 0;
            }
        }
        public static void SpawnKnives(Vector2 center, int count, float range, float speed, int damage, float knockback, IEntitySource source, int owner, float maxRandRot = MathHelper.PiOver4 / 2)
        {
            int type = ProjectileType<FlyingKnifeSimple>();
            float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < count; i++)
            {
                float rot = MathHelper.TwoPi / count * i + startRot + Main.rand.NextFloat(1f);
                Vector2 dir = rot.ToRotationVector2();
                Vector2 spawnpos = center + dir * range;
                Vector2 velocity = spawnpos.DirectionToSafe(center).RotatedByRandom(maxRandRot) * speed;
                var p = Projectile.NewProjectileDirect(source, spawnpos, velocity, type, damage, knockback / 2f, owner);
                p.scale = 0.85f;
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
                float factor = Utils.Remap(fadeinFactor[i], 0f, 1f, 1f, 0f);
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
    public class SakuyaCircle : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Owner => ref Projectile.ai[0];
        public ref float Offset => ref Projectile.ai[1];
        public ref float OrbitRot => ref Projectile.ai[2];
        public ref float AttackTimer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0f;
        }
        public override void AI()
        {
            Projectile.timeLeft++;

            float attackFadein = EaseManager.Evaluate(Ease.InOutSine, Utils.Remap(AttackTimer, 180, 240, 1f, 0f), 1f);
            float attackFadeout = EaseManager.Evaluate(Ease.InOutSine, Utils.Remap(AttackTimer, 0, 60, 0f, 1f), 1f);
            float attackFactor = attackFadein * attackFadeout;
            float radius = 100f + 50f * attackFactor;

            Projectile camera = Main.projectile[(int)Owner];
            if (camera.TypeAlive(ProjectileType<ScarletCameraProj>()))
            {
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.015f, 0f, 1f);

                var player = (camera.ModProjectile as ScarletCameraProj).player;
                if (!player.AliveCheck(Projectile.Center, 2000))
                {
                    Projectile.Kill(); return;
                }

                if(AttackTimer > 40 && AttackTimer < 180 && AttackTimer % 6 == 0)
                {

                    for (int i = 0; i < 2; i++)
                    {
                        float rot = Projectile.rotation + OrbitRot + i * MathHelper.Pi;
                        Vector2 dir = rot.ToRotationVector2();

                    }

                    Vector2 pos = Projectile.Center;
                    int damage = (int)(camera.damage * 0.2f);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<FlyingKnife>(), damage, Projectile.knockBack / 2f, Projectile.owner, Projectile.whoAmI);
                }

                var proj = (ScarletCameraProj)camera.ModProjectile;
                Vector2 targetpos = player.Center + (Offset * MathHelper.Pi + OrbitRot).ToRotationVector2() * radius;
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetpos, 0.9f);
            }
            else Projectile.Kill();

            float revolutionSpeed = 0.015f + attackFactor * 0.035f;
            OrbitRot += revolutionSpeed;
            Projectile.rotation += 0.02f + attackFactor * 0.015f;
            AttackTimer = MathHelper.Clamp(AttackTimer - 1, 0f, float.MaxValue);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            Vector2 origin = texture.Size() / 2;

            float attackFadein = EaseManager.Evaluate(Ease.InOutSine, Utils.Remap(AttackTimer, 120, 180, 1f, 0f), 1f);
            float attackFadeout = EaseManager.Evaluate(Ease.InOutSine, Utils.Remap(AttackTimer, 0, 60, 0f, 1f), 1f);
            float attackFactor = attackFadein * attackFadeout;

            Color colora = new Color(12, 31, 107) * 1.4f;
            Color colorb = new Color(109, 153, 226) * 0.6f;
            //Color colorb = new Color(152, 198, 250) * 0.9f;
            Color baseColor = Color.Lerp(colora, colorb, attackFactor);

            Color color = baseColor.AdditiveColor() * Projectile.Opacity;
            float ringRadius = 38f + 8f * attackFactor;

            RenderHelper.DrawRing(72, Projectile.Center, ringRadius, color, Projectile.rotation, new Vector2(0.25f, 0.8f) * 0.6f);
            RenderHelper.DrawRing(72, Projectile.Center, ringRadius * 0.8f, color, Projectile.rotation, new Vector2(0.25f, 0.8f) * 0.6f);

            Vector2 offset = Projectile.Center - Main.screenPosition;
            Vector2 scale1 = new(8 / 64f, 24 / 64f);
            for (int j = 0; j < 2; j++)
            {
                float radius = ringRadius * 0.8f + j * ringRadius * 0.5f - ringRadius * 0.3f;
                float rotmodifier = 1f;
                //rotmodifier -= j * clickingFactor * 0.3f;
                float extraRot = Projectile.rotation + attackFactor * j * MathHelper.TwoPi / 10f;
                for (int i = 0; i < 5; i++)
                {
                    float dir1 = MathHelper.TwoPi / 5 * i + extraRot;
                    float dir2 = MathHelper.TwoPi / 5 * (i + 1) + extraRot;
                    Vector2 vec1 = dir1.ToRotationVector2() * radius;
                    Vector2 vec2 = dir2.ToRotationVector2() * radius;
                    Vector2 middle = Vector2.Lerp(vec1, vec2, 0.5f) * (0.5f + 0.25f);

                    var dist = (int)(vec1.Distance(middle) / 4f);
                    var tomiddle = vec1.AngleTo(middle);
                    var tovec2 = middle.AngleTo(vec2);
                    for (int k = 1; k < dist; k++)
                    {
                        float factor = k / (float)dist;
                        Vector2 pos1 = Vector2.Lerp(vec1, middle, factor);
                        Main.spriteBatch.Draw(texture, pos1 + offset, null, color, tomiddle + MathHelper.PiOver2, origin, scale1, 0, 0);

                        Vector2 pos2 = Vector2.Lerp(middle, vec2, factor);
                        Main.spriteBatch.Draw(texture, pos2 + offset, null, color, tovec2 + MathHelper.PiOver2, origin, scale1, 0, 0);
                    }

                }
            }


            return false;
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
    public class FlyingKnifeSimple : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + "Knife";
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 24);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 15 * (1 + Projectile.extraUpdates);
        }
        public override bool? CanDamage() => Projectile.ai[2] >= 0;
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            if (Main.rand.NextBool(3, 5))
            {
                Dust d = Dust.NewDustDirect(Projectile.position + Projectile.Size / 4, Projectile.width / 2, Projectile.height / 2, DustID.DungeonSpirit);
                d.noGravity = true;
                d.velocity = d.velocity * 0.75f + Projectile.velocity * 0.25f;
                //d.scale *= 0.75f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

        }
        public void EndMove()
        {
            Projectile.timeLeft = 24;
            Projectile.ai[2] = -1;
            Projectile.friendly = false;
            Projectile.velocity = Vector2.Zero;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2;

            float factor = Projectile.TimeleftFactor();
            float timeleftFactor = Utils.Remap(factor, 0.6f, 1f, 1f, 0f) * Utils.Remap(factor, 0f, 0.4f, 0f, 1f);
            float alpha = Projectile.Opacity * timeleftFactor;
            Color color = Color.White.AdditiveColor() * alpha * 0.7f;
            Color trailBaseColor = new Color(192, 192, 192).AdditiveColor() * alpha * 0.7f;
            for (int i = 0; i < Projectile.oldPos.Length; i+= (1+Projectile.extraUpdates))
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i] == Projectile.position) continue;
                float trailFactor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = i == 0 ? Projectile.rotation : Projectile.oldRot[i];
                Vector2 drawpos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color trailColor = trailBaseColor * trailFactor * 0.6f;
                Main.spriteBatch.Draw(texture, drawpos, null, trailColor, rot, origin, Projectile.scale, 0, 0);
            }

            if (Projectile.ai[2] >= 0)
            {
                RenderHelper.DrawBloom(10, 4, texture, Projectile.Center - Main.screenPosition, null, trailBaseColor.AdditiveColor() * alpha * 0.25f, Projectile.rotation, origin, Projectile.scale);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(210, 210, 210) * alpha, Projectile.rotation, origin, Projectile.scale, 0);
            }
            return false;
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
            Projectile.SetTrail(2, 12 * 6);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            //Projectile.scale = 1.5f;
            Projectile.timeLeft = 7 * 60 * (1+Projectile.extraUpdates);
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
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position + Projectile.Size / 4, Projectile.width / 2, Projectile.height / 2, DustID.DungeonSpirit);
                d.noGravity = true;
                d.velocity = d.velocity * 0.75f + Projectile.velocity * 0.25f;
            }
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = AyaUtils.RandAngle;
            //Projectile.scale = 0.7f;
            var player = Main.player[Projectile.owner];
            Vector2 offset = Projectile.Center - player.Center;
            OwnerDir = offset.ToRotation();
            OwnerDist = offset.Length();
        }
        public override void AI()
        {
            Timer++;
            if (Timer > 45 * (1+Projectile.extraUpdates))
            {
                var owner = Main.player[Projectile.owner];
                if (owner.AliveCheck(Projectile.Center, 4000))
                {
                    var mousepos = owner.GetModPlayer<CameraPlayer>().MouseWorld;
                    Vector2 tomouse = Projectile.Center.DirectionToSafe(mousepos);
                    Projectile.velocity = tomouse * 9f;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Timer = int.MinValue;
                    Projectile.ai[2] = 2;
                }

            }
            else if (Timer >= 0)
            {
                var player = Main.player[Projectile.owner];
                if (player.AliveCheck(Projectile.Center,3000))
                {
                    Vector2 offset = OwnerDir.ToRotationVector2() * OwnerDist;
                    Projectile.Center = player.Center + offset;
                    Projectile.rotation += 0.25f / (1 + Projectile.extraUpdates);
                }
                else
                {
                    Projectile.Kill();
                }

            }

            if (Projectile.ai[2] > 0)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust d = Dust.NewDustDirect(Projectile.position + Projectile.Size / 4, Projectile.width / 2, Projectile.height / 2, DustID.DungeonSpirit);
                    d.noGravity = true;
                    d.velocity = d.velocity * 0.75f + Projectile.velocity * 0.25f;
                }
            }
            else
            {

                if (Timer % (1 + Projectile.extraUpdates) == 0)
                {
                    Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 16f * Projectile.scale;
                    Dust d = Dust.NewDustPerfect(pos, DustID.DungeonSpirit);
                    //Dust d = Dust.NewDustDirect(Projectile.position + Projectile.Size / 4, Projectile.width / 2, Projectile.height / 2, DustID.DungeonSpirit);
                    d.noGravity = true;
                    d.velocity = d.velocity * 0.75f + Projectile.velocity * 0.25f;
                }
            }
            //Projectile.position += Projectile.velocity * 1.5f / (1+Projectile.extraUpdates);
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
                if (Projectile.oldPos[i] == Vector2.Zero || i % (1 + Projectile.extraUpdates) != 0) continue;
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
