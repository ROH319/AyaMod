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
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using AyaMod.Content.Buffs;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class ShadowCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 45;

            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<ShadowCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 4, 0, 0));
            SetCameraStats(0.04f, 120, 1.8f,0.5f);
            SetCaptureStats(100, 5);
        }

        public static int ShadowSuckDotDmg = 60;
    }

    public class ShadowCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(139, 42, 156);
        public override Color innerFrameColor => new Color(73, 69, 151) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(204, 121, 219).AdditiveColor() * 0.5f;

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowZone>(), Projectile.damage, 0, Projectile.owner, Projectile.whoAmI);
        }

        public override void OnHitNPCAlt(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override void PostAI()
        {
            base.PostAI();
            if (player.ItemTimeIsZero) return;
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ai[2] < 0 || projectile.localAI[2] > 0 || projectile.type != ModContent.ProjectileType<ShadowFeather>()) continue;
                projectile.rotation = projectile.rotation.AngleLerp(projectile.AngleToSafe(Projectile.Center), 0.05f);
            }
        }
        public override void OnSnapInSight()
        {


            int count = Main.rand.Next(2, 4);
            for (int i = 0; i < count; i++)
            {
                Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(4, 7) * 1.5f;
                var feather = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<ShadowFeather>(), (int)(Projectile.damage * 0.15f), 0, Projectile.owner, ai2: -1);
                feather.rotation = vel.ToRotation();
            }

            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ai[2] < 0 || projectile.localAI[2] > 0 || projectile.type != ModContent.ProjectileType<ShadowFeather>()) continue;
                projectile.ai[1] = 30;
                float dist = projectile.Distance(Projectile.Center);
                dist = MathHelper.Clamp(dist, 0, 400);
                projectile.localAI[2] = dist / 2f * 2f / 30f;
                projectile.velocity = projectile.DirectionToSafe(Projectile.Center) * projectile.localAI[2];
                projectile.rotation = projectile.velocity.ToRotation();
            }
        }
        
    }

    public class ShadowZone : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Mist";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.hide = true;
            
            base.SetDefaults();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.rotation = AyaUtils.RandAngle;
        }

        public override void AI()
        {
            Projectile.scale = 2f;
            Projectile.rotation += 0.02f;

            if (!AyaUtils.ProjectileExists((int)Projectile.ai[0], ModContent.ProjectileType<ShadowCameraProj>()))
                Projectile.Kill();

            var owner = Main.player[Projectile.owner];
            if (!owner.Alive()) return;


            if (Projectile.timeLeft < 70)Projectile.timeLeft++;

            Projectile.Center = owner.Center;

            float factor = Projectile.TimeleftFactor();
            Projectile.Opacity = Utils.Remap(factor, 0.75f, 1f, 1f, 0.2f);


            int dustamount = (int)(5 * Projectile.scale);
            float alphafactor = Utils.Remap(factor, 0.75f, 1f, 100, 255);
            float lengthfactor = Utils.Remap(factor, 0.75f, 1f, 1.2f, 0.5f);
            for (int i = 0; i < dustamount; i++)
            {
                Vector2 pos = Projectile.Center + (MathHelper.TwoPi / dustamount * i).ToRotationVector2().RotateRandom(0.5f) * Main.rand.NextFloat(20, 90) * lengthfactor * Projectile.scale;
                Vector2 vel = pos.DirectionToSafe(Projectile.Center).RotateRandom(0.3f) * Main.rand.NextFloat(1f, 3f) + owner.velocity;
                var d = Dust.NewDustPerfect(pos, DustID.PurpleTorch, vel, (int)alphafactor, Scale: 1.2f);
                d.noGravity = true;
            }


            float maxRange = 90 * lengthfactor * Projectile.scale;
            foreach (var projectile in Main.ActiveProjectiles)
            {
                
                if (projectile.hostile && (projectile.ModProjectile == null || (projectile.ModProjectile.CanHitPlayer(owner) && (bool)projectile.ModProjectile.CanDamage())))
                {
                    if (projectile.Hitbox.Distance(owner.Center) < maxRange)
                    {
                        projectile.Aya().SpeedModifier *= 0.8f;
                        projectile.Camera().ShadowTimer++;
                    }
                }
            }

            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.lifeMax == 5 || npc.Hitbox.Distance(Projectile.Center) > maxRange) continue;

                npc.AddBuff(BuffType<ShadowSuckBuff>(), 2);

                npc.Aya().SpeedModifier -= 0.3f;
            }

            if (Main.rand.NextBool(50))
            {

                int count = Main.rand.Next(1, 3);
                for (int i = 0; i < count; i++)
                {
                    Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(3, 5) * 1f;
                    var feather = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<ShadowFeather>(), (int)(Projectile.damage * 0.3f), 0, Projectile.owner, ai2: Projectile.whoAmI);
                    feather.rotation = vel.ToRotation();
                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color color = new Color(80,255,80,255) * 1f * Projectile.Opacity;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.ReverseSubtract, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            int drawcount = 20;
            float radius = 30 * Projectile.scale;
            for (int i = 0; i < drawcount; i++)
            {
                Vector2 offset = (MathHelper.TwoPi / drawcount * i).ToRotationVector2() * radius;

                Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, color * (5f / drawcount), Projectile.rotation + MathF.Sin(i * 0.5f), texture.Size() / 2, Projectile.scale, 0, 0);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);


            return false;
        }
    }
    
    public class ShadowFeather : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;

        public override void SetStaticDefaults()
        {
            //Projectile.SetTrail(2, 30);
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.penetrate = -1;
            Projectile.SetImmune(30);
            //Projectile.usesIDStaticNPCImmunity = true;
            //Projectile.idStaticNPCHitCooldown = 20;
            Projectile.timeLeft = 4 * 60;
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[60];
            Projectile.oldRot = new float[60];
            base.OnSpawn(source);
        }

        public override bool? CanDamage()
        {
            if (Projectile.TimeleftFactor() > 0.75f && Projectile.ai[2] < 0) return false;
            return base.CanDamage();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            Projectile.localAI[2] = 2;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            if (((factor > 0.7f || Projectile.ai[1] < 1) && Projectile.localAI[2] < 1) && Projectile.ai[2] < 0)
            {
                //Projectile.rotation += 0.03f;
                Projectile.velocity *= 0.97f;
                //NPC npc = Projectile.FindCloestNPC(500);
                //if (npc!= null)
                //{
                //    Projectile.Chase(npc, 20, 0.06f);
                //    if (MathF.Abs(Projectile.AngleToSafe(npc.Center)) < 0.2f) Projectile.timeLeft = 3 * 60 - 1;
                //}
            }
            if ((factor <= 0.75f || (Projectile.ai[2] > 0 && factor <= 0.9f)) && Projectile.localAI[2] < 2)
            {
                NPC npc = Projectile.FindCloestNPC(500);
                if (npc != null)
                {
                    Projectile.ai[1] = 20;
                    Projectile.localAI[2] = Projectile.Distance(npc.Center) / 2f * 3f / 30f;
                    Projectile.velocity = Projectile.DirectionToSafe(npc.Center) * Projectile.localAI[2];
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            if (Projectile.ai[2] >= 0 && Projectile.localAI[2] < 1)
            {
                Projectile.velocity *= 0.96f;
            }

            if (Projectile.ai[1] > 0)
            {
                while (Projectile.ai[1] > 0)
                {
                    Projectile.ai[1]--;
                    Projectile.Damage();
                    Projectile.position += Projectile.velocity;
                    UpdateTrail();
                }
            }
            

            if ((Projectile.localAI[2] > 0 && Projectile.ai[1] < 1) || Projectile.velocity.Length() < 1f)
            {
                Projectile.velocity *= 0.99f;
                if (Projectile.ai[2] < 0) Projectile.velocity *= 0.98f;
                Projectile.Opacity -= 0.02f;
                if (Projectile.Opacity <= 0.04f)
                    Projectile.Kill();
            }

            if (Projectile.ai[2] >= 0)
            {
                Projectile zone = Main.projectile[(int)Projectile.ai[2]];
                if (zone != null && AyaUtils.ProjectileExists(zone.whoAmI,ModContent.ProjectileType<ShadowZone>()))
                {

                    float maxRange = 90 * 1.2f * zone.scale;
                    if (Projectile.Distance(zone.Center) > maxRange)
                    {
                        Projectile.localAI[2] = 2f;
                    }
                }
            }
            UpdateTrail();
            base.AI();
        }

        public void UpdateTrail()
        {

            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }
            Projectile.oldPos[0] = Projectile.position;
            Projectile.oldRot[0] = Projectile.rotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 origin = texture.Size() / 2;

            Vector2 baseScale = new Vector2(1f, 0.5f);
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = i == 0 ? Projectile.rotation : /*(Projectile.oldPos[i - 1] - Projectile.oldPos[i]).ToRotation()*/Projectile.oldRot[i];
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color color = lightColor.AdditiveColor() * factor * 0.5f * Projectile.Opacity;
                if (Projectile.localAI[2] < 2) color *= 0.2f;
                Main.spriteBatch.Draw(texture, drawPos, null, color, rot, origin, baseScale * Projectile.scale * factor, 0, 0);


            }

            RenderHelper.DrawBloom(6, 4, texture, Projectile.Center - Main.screenPosition, null, lightColor.AdditiveColor() * Projectile.Opacity, Projectile.rotation, origin, baseScale * Projectile.scale);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, origin, baseScale * Projectile.scale, 0);

            return false;
        }
    }
}
