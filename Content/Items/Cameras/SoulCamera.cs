using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
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

namespace AyaMod.Content.Items.Cameras
{
    public class SoulCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 85;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<SoulCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.06f, 156, 1.6f, 0.5f);
            SetCaptureStats(1000, 60);
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
                
        }
    }

    public class SoulCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(100, 250, 136);
        public override Color innerFrameColor => new Color(39, 232, 79) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(189, 249, 139).AdditiveColor() * 0.5f;

        public override void OnSpawn(IEntitySource source)
        {
            //for(int i = 0; i < 4; i++)
            //{

            //    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SoulLantern>(), 0, 0, Projectile.owner, Projectile.whoAmI, i);
            //}
        }

        public override void OnSnap()
        {
        }
        public override void PostAI()
        {

            if (!Main.npc.Any(n => n.type == NPCType<SoulLantern>() && n.active))
                NPC.NewNPCDirect(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y, NPCType<SoulLantern>(), ai0: Projectile.whoAmI);
        }
    }

    public class SoulLantern : ModNPC
    {
        public override string Texture => AssetDirectory.VanillaItemPath(3779);
        public ref float Owner => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.friendly = true;
            NPC.chaseable = false;
            NPC.lifeMax = int.MaxValue / 2;
            NPC.ShowNameOnHover = false;
            
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return projectile.type == ProjectileType<SoulCameraProj>();
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.HideCombatText();
        }
        public override void AI()
        {
            NPC.life = NPC.lifeMax;
            float radius = 100f;

            Projectile camera = Main.projectile[(int)Owner];
            if (!camera.TypeAlive(ProjectileType<SoulCameraProj>()))
            {
                NPC.active = false;
                return;
            }

            var player = (camera.ModProjectile as BaseCameraProj).player;

            if (!player.AliveCheck(NPC.Center, 3000)) return;
            NPC.damage = player.GetWeaponDamage(player.HeldItem);
            Timer++;

            VisualFlame();

            SpawnFlameAttack(player);

            Vector2 targetPos = player.Center + Vector2.UnitX * 50 * player.direction + -Vector2.UnitY * radius;
            NPC.Center = Vector2.Lerp(NPC.Center, targetPos, 0.9f);
        }
        public void VisualFlame()
        {

            int frequency = 5;

            if (Timer % frequency == 0)
            {
                int dustCount = 2;
                for (int j = 0; j < dustCount; j++)
                {

                    Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 4f);
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 15f);
                    float randscale = Main.rand.NextFloat(0.75f, 1.25f);
                    for (int i = 0; i < 2; i++)
                    {
                        var color = i == 0 ? new Color(111, 41, 216) : Color.White;
                        float scale = i == 0 ? 1f : 0.6f;
                        var p = SoulsParticle2.Spawn(NPC.GetSource_FromAI(), pos, vel, color.AdditiveColor(), scale * .2f * randscale, 20);
                        p.alpha = 0.8f;
                    }
                }
            }
        }
        public void SpawnFlameAttack(Player player)
        {

            int frequency = 60;
            if (player.Aya().ItemTimer > 0) frequency /=2;

            if (!(Timer % frequency == 0)) return;

            var target = AyaUtils.FindCloestNPC(NPC.Center, 1000f);

            Vector2 vel = target != null ? NPC.DirectionToSafe(target.Center).RotateRandom(MathHelper.Pi * 2 / 3) * 8f : Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 5);
            int type = ProjectileType<WispShot>();
            int damage = (int)(NPC.damage * 0.8f);
            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel,
                type, damage, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {


            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }

    public class WispShot : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.SetImmune(15);
            Projectile.timeLeft = 300 * (1 + Projectile.extraUpdates);
            Projectile.ArmorPenetration = 15;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dustcount = (int)Utils.Remap(Projectile.velocity.Length(), 0, 15f, 8, 15);
            for (int j = 0; j < dustcount; j++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 1.5f);
                //Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 4f);

                //Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 20f) + Vector2.UnitY * target.height;
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 5f);
                float randscale = Main.rand.NextFloat(0.75f, 1.25f);
                for (int i = 0; i < 2; i++)
                {
                    var color = i == 0 ? new Color(111, 41, 216) : Color.White;
                    float scale = i == 0 ? 1f : 0.6f;
                    var p = SoulsParticle2.Spawn(Projectile.GetSource_FromAI(), pos, vel, color.AdditiveColor(), scale * .25f * randscale, 40);
                    p.alpha = 0.9f;
                }
            }
        }
        public override void AI()
        {
            if (!Projectile.Chase(1000, 10, 0.04f))
                Projectile.velocity *= 0.98f;

            int frequency = (int)Utils.Remap(Projectile.velocity.Length(), 0f, 15f, 5f, 1f);
            int maxtimeleft = (int)Utils.Remap(Projectile.velocity.Length(), 0f, 15f, 20f, 10f);
            float alpha = Utils.Remap(Projectile.velocity.Length(), 0f, 15f, 0.9f, 0.6f);
            if (Projectile.timeLeft % frequency == 0)
            {
                int dustCount = (int)Utils.Remap(Projectile.velocity.Length(), 0f, 10f, 2f, 5f);
                for (int j = 0; j < dustCount; j++)
                {

                    Vector2 vel = Projectile.velocity * Main.rand.NextFloat(0.1f, 0.35f) + -Vector2.UnitY * Main.rand.NextFloat(2f, 4f);
                    //Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(4f, 5f);
                    var offset = Utils.Remap(Projectile.velocity.Length(), 0, 10f, 15f, 1f);

                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, offset);
                    float randscale = Main.rand.NextFloat(0.75f, 1.25f);
                    float scaleMult = .2f * Utils.Remap(Projectile.velocity.Length(),0,15f,1f,.8f);
                    for (int i = 0; i < 2; i++)
                    {
                        var color = i == 0 ? new Color(111, 41, 216) : Color.White;
                        float scale = i == 0 ? 1f : 0.6f;
                        var p = SoulsParticle2.Spawn(Projectile.GetSource_FromAI(), pos, vel, color.AdditiveColor(), scale * scaleMult * randscale, maxtimeleft);
                        p.alpha = alpha;
                    }
                }
            }
        }
    }

    //public class SoulLantern : ModProjectile
    //{
    //    public override string Texture => AssetDirectory.Projectiles + Name;
    //    public ref float Owner => ref Projectile.ai[0];
    //    public ref float Offset => ref Projectile.ai[1];
    //    public ref float OrbitRot => ref Projectile.localAI[0];

    //    public override void SetDefaults()
    //    {
    //        Projectile.width = Projectile.height = 64;
    //        Projectile.friendly = true;
    //        Projectile.ignoreWater = true;
    //        Projectile.tileCollide = false;
    //        Projectile.penetrate = -1;
    //    }

    //    public override bool? CanDamage() => false;
    //    public override void OnSpawn(IEntitySource source)
    //    {

    //    }
    //    public override void AI()
    //    {
    //        Projectile.timeLeft++;

    //        float radius = 250f;

    //        Projectile camera = Main.projectile[(int)Owner];
    //        if (camera.TypeAlive(ProjectileType<SoulCameraProj>()))
    //        {
    //            var player = (camera.ModProjectile as BaseCameraProj).player;

    //            if (player.AliveCheck(Projectile.Center, 3000))
    //            {

    //            }

    //            Vector2 targetPos = player.Center + (MathHelper.TwoPi / 4f * Offset + OrbitRot).ToRotationVector2() * radius;
    //            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.9f);
    //        }
    //        else Projectile.Kill();
    //        OrbitRot += 0.01f;
    //    }

    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        Texture2D texture = TextureAssets.Projectile[Type].Value;

    //        Texture2D ball = Request<Texture2D>(AssetDirectory.Extras + "Ball",AssetRequestMode.ImmediateLoad).Value;

    //        Color drawColor = new Color(31, 236, 239) * Projectile.Opacity;
    //        int drawcount = 24;
    //        Vector2 posOffset = Vector2.UnitY * 5;
    //        for(int i = 0; i < drawcount; i++)
    //        {

    //            float factor = (float)i / drawcount;
    //            if (factor > 0.2f && factor < 0.6f) continue;
    //            Color color = drawColor.AdditiveColor() * EaseManager.Evaluate(Ease.InCirc, factor, 1f);

    //            float scale = Projectile.scale * Utils.Remap(factor, 0, 1f, 1.5f, 0.1f);
    //            Main.EntitySpriteDraw(ball, Projectile.Center + posOffset - Main.screenPosition, null, color, 0f, ball.Size() / 2, scale, SpriteEffects.None, 0);
    //        }

    //        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

    //        Main.EntitySpriteDraw(ball, Projectile.Center + posOffset - Main.screenPosition, null, drawColor.AdditiveColor() * 0.1f, Projectile.rotation, ball.Size() / 2, Projectile.scale * 1f, 0, 0);
    //        Main.EntitySpriteDraw(ball, Projectile.Center + posOffset - Main.screenPosition, null, drawColor.AdditiveColor() * 0.4f, Projectile.rotation, ball.Size() / 2, Projectile.scale * 0.4f, 0, 0);
    //        return false;
    //    }
    //}

}
