using AyaMod.Core.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Helpers
{
    public static class AyaExtensions
    {
        #region Projectiles
        public static bool MyClient(this Projectile projectile) => projectile.owner == Main.myPlayer;

        public static float TimeleftFactor(this Projectile projectile) => projectile.timeLeft / projectile.MaxTimeleft();

        public static float TimeleftFactorPositive(this Projectile projectile) => Utils.Remap(projectile.TimeleftFactor(), 0, 1f, 1f, 0f);

        public static float MaxTimeleft(this Projectile projectile) => projectile.GetGlobalProjectile<AyaGlobalProjectile>().MaxTimeleft;

        public static void SetTrail(this Projectile projectile, int type, int length)
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = type;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = length;
        }

        public static void SetImmune(this Projectile projectile, int length)
        {
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = length;
        }

        public static void Scale(this Projectile projectile, float scale, bool setscale = true)
        {
            projectile.position = projectile.Center;
            if (setscale) projectile.scale *= scale;
            projectile.width = (int)(projectile.width * scale);
            projectile.height = (int)(projectile.height * scale);
            projectile.Center = projectile.position;
        }

        public static void SetHitbox(this Projectile projectile, int x, int y = -1)
        {
            if (y == -1) y = x;
            projectile.position = projectile.Center;
            projectile.width = x;
            projectile.height = y;
            projectile.Center = projectile.position;
        }

        public static void UseGravity(this Projectile projectile, float vMult = 0.98f, float YAdd = 0.15f, float YMax = 30f)
        {
            projectile.velocity *= vMult;
            if (projectile.velocity.Y < YMax) projectile.velocity.Y += YAdd;
        }

        public static Rectangle GetHitbox(this Projectile projectile)
        {
            Rectangle result = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, (int)projectile.width, (int)projectile.height);
            var type = projectile.type;
            if (type == 101)
                result.Inflate(30, 30);

            if (type == 85)
            {
                int num = (int)Utils.Remap(projectile.localAI[0], 0f, 72f, 10f, 40f);
                result.Inflate(num, num);
            }

            if (type == 188)
                result.Inflate(20, 20);

            if (projectile.aiStyle == 29)
                result.Inflate(4, 4);

            if (type == 967)
                result.Inflate(10, 10);
            ProjectileLoader.ModifyDamageHitbox(projectile, ref result);
            return result;
        }


        public static bool Chase(this Projectile projectile, float range = 16000f, float maxSpeed = 32f, float lerpAmount = 0.05f)
        {
            NPC target = projectile.FindCloestNPC(range, false, !projectile.tileCollide);
            if (target == null) return false;
            projectile.Chase(target, maxSpeed, lerpAmount);
            return true;
        }
        public static bool Chase(this Projectile projectile, NPC target, float maxSpeed = 32f, float lerpAmount = 0.05f)
        {
            projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionToSafe(target.Center) * maxSpeed, lerpAmount);
            return true;
        }

        #endregion

        public static List<Item> ChooseFilms(this Player player, Item weapon, int count = 1)
        {
            var sItem = weapon;
            List<Item> list = new List<Item>();

            for(int i = 54; i < 58 && list.Count >= count; i++)
            {
                if (player.inventory[i].stack > 0 && ItemLoader.CanChooseAmmo(sItem, player.inventory[i],player))
                {
                    list.Add(player.inventory[i]);
                }
            }
            for(int i = 0; i < 54 && list.Count >= count; i++)
            {
                if (player.inventory[i].stack > 0 && ItemLoader.CanChooseAmmo(sItem, player.inventory[i], player))
                {
                    list.Add(player.inventory[i]);
                }
            }

            return list;
        }

        public static bool AliveCheck(this Player player, Vector2 checkPos, float distMax)
        {
            return player != null && player.active && !player.dead && !player.ghost && player.Distance(checkPos) <= distMax;
        }

        public static void ClearBuff(this NPC npc, int type)
        {
            if (!npc.HasBuff(type)) return;
            int index = npc.FindBuffIndex(type);
            npc.DelBuff(index);
        }

        public static void ClearBuff<T>(this NPC npc) where T : ModBuff
        {
            if (!npc.HasBuff<T>()) return;
            int index = npc.FindBuffIndex(ModContent.BuffType<T>());
            npc.DelBuff(index);
        }

        public static NPC FindCloestTargetWithWhoAmI(this Projectile projectile, float whoami, out bool goodwhoami, float range = int.MaxValue, bool ignoreTile = false)
        {
            NPC npc;
            if (whoami < 0 || whoami > Main.maxNPCs) npc = null;
            else npc = Main.npc[(int)whoami];
            goodwhoami = true;
            if (npc == null || !npc.CanBeChasedBy(projectile))
            {
                goodwhoami = false;
                npc = projectile.FindCloestNPC(range, false, ignoreTile);
            }
            return npc;
        }

        public static NPC FindCloestNPC(this Projectile projectile, float range = int.MaxValue, bool ignoreImmune = false, bool ignoreTile = false)
        {
            NPC target = null;
            float minDist = range;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy())
                {
                    if (ignoreImmune || (!(projectile.usesLocalNPCImmunity && projectile.localNPCImmunity[npc.whoAmI] != 0)
                    && !(projectile.usesIDStaticNPCImmunity && Projectile.perIDStaticNPCImmunity[projectile.type][npc.whoAmI] > Main.GameUpdateCount) && !(npc.immune[projectile.owner] != 0)))
                    {
                        if (minDist == -1 || npc.Distance(projectile.Center) < minDist)
                        {
                            if (ignoreTile || Collision.CanHit(projectile.Center, 1, 1, npc.position, npc.width, npc.height))
                            {
                                target = npc;
                                minDist = npc.Distance(projectile.Center);
                            }
                        }
                    }
                }
            }
            return target;
        }

        public static Vector2 Length(this Vector2 vector, float length) => vector.Length() == 0 ? Vector2.Zero : vector / vector.Length() * length;

        public static float AngleFromSafe(this Entity entity, Vector2 position)
        {
            return entity.DirectionFromSafe(position).ToRotation();
        }

        public static float AngleToSafe(this Entity entity, Vector2 position)
        {
            return entity.DirectionToSafe(position).ToRotation();
        }

        public static Vector2 DirectionFromSafe(this Entity entity, Vector2 position)
        {
            return -entity.DirectionToSafe(position);
        }

        public static Vector2 DirectionToSafe(this Entity entity, Vector2 position)
        {
            Vector2 vector = entity.DirectionTo(position);
            if (vector.HasNaNs())
            {
                vector = Vector2.Zero;
            }

            return vector;
        }

        public static Vector2 DirectionToSafe(this Vector2 self, Vector2 position)
        {
            Vector2 vector = self.DirectionTo(position);
            if (vector.HasNaNs()) vector = Vector2.Zero;
            return vector;
        }

        public static Vector2 DirectionFromSafe(this Vector2 self, Vector2 position)
        {
            return -self.DirectionToSafe(position);
        }

        public static bool Alive(this Player player) => player != null && player.active && !player.dead && !player.ghost;
        public static bool Alive(this Projectile projectile) => projectile != null && projectile.active;
        public static bool Alive(this NPC npc) => npc != null && npc.active;
        public static AyaGlobalNPC Aya(this NPC npc) => npc.GetGlobalNPC<AyaGlobalNPC>();
        public static AyaGlobalProjectile Aya(this Projectile projectile) => projectile.GetGlobalProjectile<AyaGlobalProjectile>();
        public static CameraGlobalNPC Camera(this NPC npc) => npc.GetGlobalNPC<CameraGlobalNPC>();
        public static CameraGlobalProjectile Camera(this Projectile projectile) => projectile.GetGlobalProjectile<CameraGlobalProjectile>();
    }
}
