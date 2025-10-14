using AyaMod.Content.Items.Films;
using AyaMod.Content.Items.Lens;
using AyaMod.Core.Configs;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Core.Prefabs
{
    public class BaseCameraProj : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public CameraStat CameraStats;

        public float size;

        public ILens lens;

        public float FocusFactor;

        public int EffectCounter;

        public bool DealDamageThisFrame;

        public int HighestHealthTarget = -1;

        public Vector2 ComputedVelocity;

        // 额外伤害点（0~1之间，基于进度百分比）
        public List<float> DamagePoints { get; } = new();

        // 防止重复触发的记录
        private readonly HashSet<float> triggeredPoints = new();

        public Player player => Main.player[Projectile.owner];

        public List<BaseFilm> films = new();

        public virtual bool BaseDamageCondition => player.itemTime == 1;

        public virtual float FloatingFactor() => 1f + MathF.Sin(Main.GameUpdateCount * 0.05f) * 0.1f;

        public virtual float Size => floatingsize * FocusFactor;

        public virtual float floatingsize => size * FloatingFactor();

        public bool CanSpawnFlash = true;

        public virtual Color outerFrameColor => Color.Red;
        public virtual Color innerFrameColor => Color.White;
        public virtual Color focusCenterColor => Color.White;
        public virtual Color flashColor => Color.White.AdditiveColor() * 0.5f;

        public sealed override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.netUpdate = true;
            Projectile.netImportant = true;

            Projectile.DamageType = ReporterDamage.Instance;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 2;

            SetOtherDefault();
        }

        public virtual void SetOtherDefault() { }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (lens != null)
            {
                return lens.Colliding(Projectile.Center, Size, Projectile.rotation, targetHitbox);
            }
            return false;
        }
        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HighestHealthTarget >= 0 && target.whoAmI == HighestHealthTarget)
                modifiers.FinalDamage *= (1 + player.Camera().SingleTargetMultiplier);

            ModifyHitNPCAlt(target, ref modifiers);

            //因为匿名方法不能传ref参数，直接简单遍历了
            foreach (var film in films)
            {
                film.ModifyHitNPCFilm(this, target, ref modifiers);
            }
        }
        public virtual void ModifyHitNPCAlt(NPC target, ref NPC.HitModifiers modifiers) { }

        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;

            OnHitNPCAlt(target, hit, damageDone);

            UpdateFilm((film => film.OnHitNPCFilm(this, target, hit, damageDone)));
        }
        public virtual void OnHitNPCAlt(NPC target, NPC.HitInfo hit, int damageDone) { }


        public override bool? CanDamage()
        {
            if (DealDamageThisFrame)
                return true;

            return false;
        }

        public override bool PreAI()
        {
            if (player.HeldItem.ModItem is not BaseCamera) return false;
            RecalculateDamage();
            GetCameraStats();

            var film = player.ChooseFilms(player.HeldItem, CameraStats.FilmSlot);
            film.ForEach(film => films.Add((BaseFilm)film.ModItem));

            UpdateFilm(film => film.PreAI(this));
            //Item film = player.ChooseAmmo(player.HeldItem);
            //if (film != null)
            //    films.Add((BaseFilm)film.ModItem);
            DealDamageThisFrame = CheckCanDamage() && CheckInSight();
            //Main.NewText($"{player.itemTime} {player.itemAnimation}");
            return base.PreAI();
        }
        
        public NPC FindHighestHealthTarget()
        {
            NPC target = null;
            float maxHP = 0;
            foreach(var npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy() || npc.life <= maxHP) continue;
                if (Projectile.Colliding(Projectile.GetHitbox(), npc.getRect()))
                {
                    target = npc;
                }
            }
            return target;
        }

        public override void PostAI()
        {

        }

        public bool CheckInSight()
        {
            //Collision.LaserScan()
            bool colli = AyaUtils.CheckLineCollisionTile(Projectile.Center, player.Center, 8);
            //var colli = Collision.CanHit(Projectile.Center, 1, 1, player.Center, 1, 1);
            //Main.NewText($"{colli}");
            return player.GetModPlayer<CameraPlayer>().CanSnapThroughWall() || colli;
        }

        public bool CheckCanDamage()
        {

            float progress = 1f - (player.itemTime / (float)player.itemTimeMax);

            // 检查额外伤害点
            foreach (var point in DamagePoints)
            {
                if (IsCurrentFrame(point, progress, player.itemTimeMax) && !triggeredPoints.Contains(point))
                {
                    triggeredPoints.Add(point);
                    return true;
                }
            }

            if (player.itemTime == 1)
            {
                triggeredPoints.Clear();
                //Main.NewText($"啊啊啊宝宝你是一个弹药{Main.time}");
                //var ammo = player.PickAmmo(player.HeldItem, out _, out _, out _, out _, out _);
            }
            return BaseDamageCondition;
        }

        // 判断给定点是否在当前帧中
        private bool IsCurrentFrame(float targetPoint, float currentProgress, int totalTime)
        {
            // 计算当前帧的时间跨度
            float prevProgress = currentProgress - 1f / totalTime;
            return currentProgress >= targetPoint && prevProgress <= targetPoint;
        }

        // 添加伤害点的方法
        public void AddDamagePoint(float progress)
        {
            if (progress < 0 || progress > 1)
                throw new ArgumentException("伤害点必须在0到1之间(进度百分比)");

            DamagePoints.Add(progress);
        }

        public virtual void RecalculateDamage()
        {
            Player player = Main.player[Projectile.owner];

            long weaponDamage = player.GetWeaponDamage(player.HeldItem);
            int weaponCrit = player.GetWeaponCrit(player.HeldItem);
            Projectile.damage = (int)Math.Round((double)weaponDamage, 0, MidpointRounding.ToPositiveInfinity);
            //Main.NewText($"{weaponDamage} {Projectile.damage}");
            Projectile.CritChance = (int)Math.Round((double)weaponCrit,0, MidpointRounding.ToPositiveInfinity);

        }

        public virtual void GetCameraStats()
        {
            if (player.HeldItem.ModItem == null) return;
            var camera = player.HeldItem.ModItem as BaseCamera;
            if (camera == null) return;

            CameraStats = camera.CameraStats;
            size = player.Camera().SizeModifier.ApplyTo(camera.CameraStats.Size) * camera.Item.GetGlobalItem<CameraGlobalItem>().SizeMult;
            lens = player.GetModPlayer<CameraPlayer>().GetLens();
        }

        public sealed override void AI()
        {
            if (!player.AliveCheck(Projectile.Center, int.MaxValue)) return;
            var mplr = player.GetModPlayer<CameraPlayer>();

            player.heldProj = Projectile.whoAmI;

            MoveMent(mplr);

            CheckHoverNPC();
            CheckHoverProjectile();

            if (DealDamageThisFrame)
            {
                var target = FindHighestHealthTarget();
                if (target != null) HighestHealthTarget = target.whoAmI;
                else HighestHealthTarget = -1;
            }

            //Main.NewText($"{player.itemAnimation} {player.itemTime} {player.altFunctionUse}");
            if (player.itemTime != 0)
            {
                if (player.altFunctionUse == 2 && mplr.CameraAltCooldown == 0)
                {
                    Projectile.Center = mplr.MouseWorld;
                    ProjectileRemoval();
                    player.itemTime = player.itemAnimation = 0;
                    mplr.CameraAltCooldown = CameraStats.CaptureCooldown;
                }
                else
                {
                    if (DealDamageThisFrame)
                    {
                        mplr.OnSnap();
                        Snap();
                    }
                    FocusFactor = Utils.Remap(player.itemTime, 0, player.itemTimeMax, 1, CameraStats.MaxFocusScale);
                }
            }
            else
            {
                FocusFactor = CameraStats.MaxFocusScale;
                triggeredPoints.Clear();
                DamagePoints.Clear();
            }
        }

        public virtual void MoveMent(CameraPlayer mplr)
        {
            float slowedchase = CameraStats.ChaseFactor;
            //if (mplr.Player.itemTime != 0) slowedchase *= CameraStats.SlowFactor;
            Vector2 previous = Projectile.Center;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mplr.MouseWorld, slowedchase);
            ComputedVelocity = Projectile.Center - previous;
            Projectile.rotation = mplr.Player.AngleToSafe(Projectile.Center);

            //bool canhit = AyaUtils.CheckLineCollisionTile(Projectile.Center, player.Center,8);
            //Main.NewText($"{canhit}");
        }

        public void ProjectileRemoval()
        {
            var rects = lens.GetRectanglesAgainstEntity(Projectile.Center, Size, Projectile.rotation);
            int captureCount = 0;
            PreClear();
            UpdateFilm(film => film.PreClearProjectile(this));

            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (!CanClear(projectile)) continue;

                PreCollidingProjectile(projectile);
                
                bool colliding = false;
                Rectangle projRect = projectile.GetHitbox();
                foreach (var rect in rects)
                {
                    if (projectile.Colliding(projRect, rect))
                    {
                        colliding = true; break;
                    }
                }
                if (!colliding) continue;

                OnClearProjectile(projectile);
                UpdateFilm(film => film.OnClearProjectile(this));
                projectile.Kill();
                captureCount++;
            }
            PostClear(captureCount);
            UpdateFilm(film => film.PostClearProjectile(this, captureCount));
        }

        public virtual void PreClear() { }

        public virtual void PreCollidingProjectile(Projectile projectile) { }

        public virtual bool CanClear(Projectile projectile) => projectile.hostile && projectile.damage <= CameraStats.CameraDamage;

        public virtual void OnClearProjectile(Projectile projectile) { }

        public virtual void PostClear(int captureCount) { }

        public void CheckHoverNPC()
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                var hitbox = npc.Hitbox;
                if (lens.Colliding(Projectile.Center, Size, Projectile.rotation, hitbox))
                    HoverNPC(npc);
                else NotHoverNPC(npc);
            }

        }
        public virtual void HoverNPC(NPC npc) { }
        public virtual void NotHoverNPC(NPC npc) { }

        public void CheckHoverProjectile()
        {
            foreach(var projectile in Main.ActiveProjectiles)
            {
                var hitbox = projectile.GetHitbox();
                if(lens.Colliding(Projectile.Center,Size, Projectile.rotation, hitbox))
                    HoverProjectile(projectile);
                else NotHoverProjectile(projectile);
            }
        }

        public virtual void HoverProjectile(Projectile projectile) { }
        public virtual void NotHoverProjectile(Projectile projectile) { }

        public void Snap()
        {
            var rects = lens.GetRectanglesAgainstEntity(Projectile.Center, Size, Projectile.rotation);
            foreach(var rect in rects)
            {
                Utils.PlotTileLine(new Vector2(rect.Left, rect.Center.Y), new Vector2(rect.Right, rect.Center.Y), rect.Height, new Utils.TileActionAttempt(DelegateMethods.CutTiles));
            }

            OnSnap();

            bool canhit = CheckInSight();

            //Main.NewText($"CanHitLine:{canhit}");
            if (canhit)
                OnSnapInSight();

            UpdateFilm((film => film.OnSnap(this)));
            if (canhit)
                UpdateFilm((film => film.OnSnapInSight(this)));

            CheckSnapProjectile(rects);

            if (ClientConfig.Instance.SnapFlash && CanSpawnFlash)
            {
                SpawnFlash();
            }

            PostSnap();
        }

        public void CheckSnapProjectile(List<Rectangle> rects)
        {
            foreach(var projectile in Main.ActiveProjectiles)
            {
                var hitbox = projectile.GetHitbox();
                if (lens.Colliding(Projectile.Center, Size, Projectile.rotation, hitbox))
                    OnSnapProjectile(projectile);
            }
        }

        /// <summary>
        /// 拍摄且镜头中心在视线内时触发
        /// </summary>
        public virtual void OnSnapInSight() { }

        /// <summary>
        /// 拍摄时触发
        /// </summary>
        public virtual void OnSnap() { }

        /// <summary>
        /// 拍摄弹幕时触发
        /// </summary>
        /// <param name="projectile"></param>
        public virtual void OnSnapProjectile(Projectile projectile) { }

        public virtual void PostSnap() { }

        public void UpdateFilm(Action<BaseFilm> action)
        {
            foreach(var film in films)
            {
                action(film);
            }
        }

        public virtual void SpawnFlash()
        {
            lens.SpawnFlash(Projectile.Center,flashColor * ClientConfig.Instance.SnapFlashAlpha,floatingsize,Projectile.rotation, (int)(Projectile.knockBack * 2));
        }

        public virtual void ApplyStun(NPC npc, ref NPC.HitModifiers modifiers)
        {
            var knockback = modifiers.GetKnockback(Projectile.knockBack * 2);
            var kbstat = player.GetTotalKnockback(ReporterDamage.Instance);
            var stuntime = kbstat.ApplyTo(knockback);
            npc.GetGlobalNPC<CameraGlobalNPC>().StunTimer += (int)stuntime;
            //Main.NewText($"{stuntime}");
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Utils.DrawLine(Main.spriteBatch, Projectile.Center, player.Center, Color.Red, Color.Red, 1);
            if (lens != null)
                lens.DrawCamera(Main.spriteBatch, player, Projectile.Center,
                    Projectile.rotation, floatingsize, FocusFactor, CameraStats.MaxFocusScale, outerFrameColor, innerFrameColor, focusCenterColor);

            return true;
        }
    }
}
