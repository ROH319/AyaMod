using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems.Trails;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SteelSeries.GameSense;
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

            Item.damage = 94;

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
            if (Main.rand.NextBool(16))
            {
                var lantern = Main.npc.FirstOrDefault(n => n.active && n.type == NPCType<SoulLantern>() && n.ai[0] == Projectile.whoAmI);
                if (lantern == null) return;

                int type = ProjectileType<SoulFlameRing>();
                int ringDamage = (int)(Projectile.damage * 0.5f);
                int soulcount = 10;
                float radius = 150;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), lantern.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5,10), type, ringDamage, 0f, Projectile.owner, soulcount, radius);
            }
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
        public ref float State => ref NPC.ai[2];
        public ref float Target => ref NPC.ai[3];
        public ref float SoulsCounter => ref NPC.localAI[0];
        public ref float SnapCounter => ref NPC.localAI[1];
        public List<PreparedDamage> AttackList;
        public float maxSoulsThreshold = 20;
        public float maxSnapsThreshold = 4;
        public MultedTrail trail;
        public class PreparedDamage()
        {
            public int damage;
            public int timer;
            public PreparedDamage(int damage, int timer) : this()
            {
                this.damage = damage;
                this.timer = timer;
            }
            public void Update()
            {
                timer--;
            }
        }
        public enum LanternState
        {
            /// <summary>
            /// 点燃
            /// </summary>
            Flaming,
            /// <summary>
            /// 熄灭
            /// </summary>
            Flameout
        }
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.friendly = true;
            NPC.chaseable = false;
            NPC.lifeMax = int.MaxValue / 2;
            NPC.ShowNameOnHover = false;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            Target = -1;
            trail = new MultedTrail();
            AttackList = [];
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return State == (int)LanternState.Flameout && projectile.type == ProjectileType<SoulCameraProj>();
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.HideCombatText();
            modifiers.DisableKnockback();
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            AttackList.Add(new PreparedDamage(damageDone * 4, 20));
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

            switch ((LanternState)State)
            {
                case LanternState.Flaming:

                    VisualFlame();

                    SpawnFlameAttack(player);

                    break;
                case LanternState.Flameout:

                    if(!AyaUtils.NPCExists((int)Target))
                    {
                        var target = AyaUtils.FindHighestHealthNPC(NPC.Center, 1200f, ignoreTile: true)?.whoAmI ?? -1;
                        if (target != -1) Target = target;
                    }
                    if (AyaUtils.NPCExists((int)Target)) 
                    {
                        var targetNPC = Main.npc[(int)Target];
                        HandleAttackList(targetNPC, player);
                    }
                    break;
                default: break;
            }
            //Main.NewText($"{SoulsCounter} {SnapCounter} {Main.GameUpdateCount}");
            Vector2 targetPos = player.Center + Vector2.UnitX * 50 * player.direction + -Vector2.UnitY * radius;
            NPC.Center = Vector2.Lerp(NPC.Center, targetPos, 0.2f);
        }
        private void HandleAttackList(NPC targetNPC, Player player)
        {
            AttackList.ForEach(d =>
            {
                float f = EaseManager.Evaluate(Ease.InCubic, Utils.Remap(d.timer, 20, 0, 0f, 1f), 1f);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Vector2.Lerp(NPC.Center, targetNPC.Center, f + Main.rand.NextFloat(-0.1f, 0.1f));
                    Dust dust = Dust.NewDustPerfect(pos, 235, Scale: 1.5f);
                    dust.noGravity = true;

                    Dust dust1 = Dust.NewDustPerfect(pos, 191, Scale: Main.rand.NextFloat(1.3f, 1.7f));
                    dust1.noGravity = true;
                }

                d.Update();
                if (d.timer == 0)
                {
                    var explosion = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), targetNPC.Center, Vector2.Zero,
                        ProjectileType<SoulExplosion>(), d.damage, 0f, player.whoAmI, targetNPC.whoAmI);
                    explosion.rotation = NPC.AngleToSafe(targetNPC.Center);

                    float damageFactor = Utils.Remap(d.damage, 110, 440, 0f, 1f);
                    int extraDustMult = (int)MathHelper.Lerp(1, 4, damageFactor);
                    float extraScaleMult = MathHelper.Lerp(1, 2, damageFactor);
                    for (int i = 0; i < 30 * extraDustMult; i++)
                    {
                        Vector2 dir = (MathHelper.TwoPi / 20f * i).ToRotationVector2();
                        dir.X /= 2f;
                        dir = dir.RotatedBy(NPC.AngleToSafe(targetNPC.Center));
                        Dust dust = Dust.NewDustPerfect(targetNPC.Center + dir * Main.rand.NextFloat(9, 11) * 2 * extraDustMult,
                            Main.rand.NextBool(4) ? 191 : 235,
                            dir * Main.rand.NextFloat(1.5f, 2.5f) * extraScaleMult,
                            Scale: Main.rand.NextFloat(1f, 2f) * extraScaleMult);
                        dust.noGravity = true;

                        Dust dust1 = Dust.NewDustPerfect(targetNPC.Center + dir * Main.rand.NextFloat(27, 33) * 2 * extraScaleMult,
                            Main.rand.NextBool(4) ? 235 : 191,
                            dir * Main.rand.NextFloat(2f, 3f) * extraScaleMult,
                            Scale: Main.rand.NextFloat(1.5f, 2.5f) * extraScaleMult);
                        dust1.noGravity = true;
                    }

                    int segments = (int)(NPC.Distance(targetNPC.Center) / 16);
                    float rot = NPC.AngleToSafe(targetNPC.Center);
                    for (int i = 0; i < segments; i++)
                    {
                        Vector2 pos2 = Vector2.Lerp(NPC.Center, targetNPC.Center, i / (float)segments);
                        var star = StarParticle.Spawn(NPC.GetSource_FromAI(), pos2, Vector2.Zero, Color.Red.AdditiveColor(),
                            1.5f, 0.25f, 0.75f, 0.9f, 1f, rot, 1f);
                        star.SetScaleMult(0.96f);
                    }

                    SnapCounter++;
                    if (SnapCounter >= maxSnapsThreshold)
                    {
                        SnapCounter = 0;
                        State = (int)LanternState.Flaming;
                    }
                }
            });

            AttackList.RemoveAll(d => d.timer <= 0);
        }
        public void VisualFlame()
        {

            int frequency = 5;

            if (Timer % frequency == 0)
            {
                int dustCount = 2;
                float alphaMult = Utils.Remap(SoulsCounter, 0, maxSoulsThreshold, 1, 0.2f);
                for (int j = 0; j < dustCount; j++)
                {

                    Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 4f);
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 15f);
                    float randscale = Main.rand.NextFloat(0.75f, 1.25f);
                    for (int i = 0; i < 2; i++)
                    {
                        var color = i == 0 ? new Color(111, 41, 216) : Color.White;
                        float scale = i == 0 ? 1f : 0.6f;
                        var p = SoulsParticle2.Spawn(NPC.GetSource_FromAI(), pos, vel, color.AdditiveColor(), scale * .25f * randscale, 20);
                        p.alpha = 0.85f * alphaMult;
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

            SoulsCounter++;
            if(SoulsCounter > maxSoulsThreshold)
            {
                SoulsCounter = 0;
                State = (int)LanternState.Flameout;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;

            if (State == (int)LanternState.Flameout)
            {
                if (AyaUtils.NPCExists((int)Target))
                {
                    NPC target = Main.npc[(int)Target];
                    float distance = NPC.Distance(target.Center);
                    Vector2[] poses = new Vector2[(int)(distance / 8)];
                    for (int i = 0; i < poses.Length; i++)
                    {
                        float factor = (float)i / poses.Length;
                        poses[i] = Vector2.Lerp(NPC.Center, target.Center, factor);
                    }
                    trail.PrepareStrip(poses, 2,
                        factor =>
                        {
                            float f = 0f;
                            foreach(var dmg in AttackList)
                            {
                                f += Utils.Remap(dmg.timer, 20, 0, 0f, 1f);
                            }
                            //Color highlight = Color.Black;
                            //foreach (var dmg in AttackList)
                            //{
                            //    float f = EaseManager.Evaluate(Ease.InSine, Utils.Remap(dmg.timer, 15, 0, 0f, 1f), 1f);
                            //    Color color = Color.Lerp(Color.Black, Color.Red, Utils.Remap(f - factor < 0f ? 1f : f - factor, 0f, 0.3f, 1f, 0f));
                            //    Color white = Color.Lerp(Color.Black, Color.White, Utils.Remap(f - factor < 0f ? 1f : f - factor, 0f, 0.2f, 1f, 0f));
                            //    highlight.R += (byte)(color.R + white.R);
                            //    highlight.G += (byte)(color.G + white.G);
                            //    highlight.B += (byte)(color.B + white.B);
                            //}
                            return Color.Lerp(new Color(139, 0, 0), Color.Red, f).AdditiveColor();
                        },
                        factor =>
                        {
                            float extrawidth = 0f;

                            //foreach (var dmg in AttackList)
                            //{
                            //    float f = EaseManager.Evaluate(Ease.InSine, Utils.Remap(dmg.timer, 15, 0, 0f, 1f), 1f);
                            //    float width = Utils.Remap(MathF.Abs(f - factor), 0.01f, 0.07f, 8f, 0f);
                            //    extrawidth += width;
                            //}
                            return 8f + extrawidth;
                        }, -Main.screenPosition,
                        factor => Utils.Remap(factor, 0, 0.1f, 0f, 1f) );
                    Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Extra[197].Value;
                    trail.DrawTrail();
                }

                #region 绘制熄灭状态的泛光

                RenderHelper.DrawBloom(8, 4f + MathF.Cos((float)(Main.timeForVisualEffects * 0.05f)) * 1.5f, texture, NPC.Center - Main.screenPosition, null,
                    (Color.Lerp(Color.MidnightBlue,Color.Purple,Utils.Remap(MathF.Cos((float)(Main.timeForVisualEffects * 0.05f)),-1,1,0f,1f))).AdditiveColor() * 0.9f, 0f, texture.Size() / 2, NPC.scale * 1.3f);

                #endregion
            }
            Main.EntitySpriteDraw(texture, NPC.Center - screenPos, null, drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, 0);
            return false;
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

    public class SoulExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float TargetNPC => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1000;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(20);
            Projectile.timeLeft = 2;
            
        }
        public override bool? CanHitNPC(NPC target)
        {
            return target.whoAmI == TargetNPC;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 9999;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life + damageDone < 800)
                target.StrikeInstantKill();

        }
    }

    public class SoulFlameRing : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Count => ref Projectile.ai[0];
        public ref float Radius => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            Projectile.timeLeft = 5 * 60;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            bool result = false;
            for(int i = 0;i< Count; i++)
            {
                float angle = MathHelper.TwoPi / Count * i + Projectile.rotation;
                Vector2 offset = new Vector2(Radius, 0).RotatedBy(angle);
                Vector2 pos = Projectile.Center + offset - new Vector2(Projectile.width / 2, Projectile.height / 2);
                Rectangle hitbox = new((int)pos.X, (int)pos.Y, Projectile.width, Projectile.height);
                if (hitbox.Intersects(targetHitbox))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public override void AI()
        {
            NPC target = Projectile.FindCloestNPC(1000f);
            if (target != null)
            {
                Vector2 targetPos = target.Center + target.DirectionToSafe(Projectile.Center) * Radius;
                float distance = Projectile.Distance(targetPos);
                float speed = Utils.Remap(distance, 0, 1000, 2, 16f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionToSafe(targetPos) * speed, 0.3f);
            }

            int frequency = 5;
            for (int k = 0; k < Count; k++)
            {

                if ((Projectile.timeLeft + k * 1311) % frequency == 0)
                {
                    int dustCount = 2;
                    Vector2 pos = Projectile.Center + (MathHelper.TwoPi / Count * k + Projectile.rotation).ToRotationVector2() * Radius;
                    for (int j = 0; j < dustCount; j++)
                    {

                        Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 4f);
                        Vector2 spawnPos = pos + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 10f);
                        float randscale = Main.rand.NextFloat(0.75f, 1.25f);
                        for (int i = 0; i < 2; i++)
                        {
                            var color = i == 0 ? new Color(111, 41, 216) : Color.White;
                            float scale = i == 0 ? 1f : 0.6f;
                            var p = SoulsParticle2.Spawn(Projectile.GetSource_FromAI(), spawnPos, vel, color.AdditiveColor(), scale * .25f * randscale * .8f, 20);
                            p.alpha = 0.85f;
                        }
                    }
                }
            }

            Projectile.rotation += 0.03f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int pointcount = 120;
            float factor = Projectile.TimeleftFactor();
            float alpha = Utils.Remap(factor, 0, 0.1f, 0f, 1f) * Utils.Remap(factor, 0.9f, 1f, 1f, 0f);
            RenderHelper.DrawRing(pointcount, Projectile.Center , Radius, new Color(85, 113, 255).AdditiveColor() * 0.25f * alpha, Projectile.rotation, new Vector2(0.1f,0.8f) * Projectile.scale);

            return false;
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
