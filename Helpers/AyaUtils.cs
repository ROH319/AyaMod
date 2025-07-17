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
    public static class AyaUtils
    {
        public static bool GoodNetCode => Main.netMode != NetmodeID.MultiplayerClient;
        #region Projectiles
        public static bool ProjectileExists(int whoami, params int[] types)
        {
            return whoami > -1 && whoami < Main.maxProjectiles && Main.projectile[whoami].active && (types.Length == 0 || types.Contains(Main.projectile[whoami].type));
        }

        #endregion

        #region Players

        #endregion


        #region NPC


        #endregion

        public static Vector2[] GetCameraRect(Vector2 center, float rot, float size)
        {
            List<Vector2> pos = new();
            
            for(float i = -1; i < 2; i += 2)
            {
                for(float j = -1; j < 2; j += 2)
                {
                    pos.Add(center + new Vector2(i * size / 2,j * size / 2).RotatedBy(rot));
                }
            }
            return pos.ToArray();
        }

        public static Vector2[] GetCameraRect(Vector2 center, float rot, float sizeX, float sizeY)
        {
            List<Vector2> pos = new();
            /*
                            0   2
            player----------camera
                            1   3
            */
            for (float i = -1; i < 2; i += 2)
            {
                for(float j = -1; j < 2; j += 2)
                {
                    pos.Add(center + new Vector2(i * sizeX / 2, j * sizeY / 2).RotatedBy(rot));
                }
            }
            return pos.ToArray();
        }


        public static float RandAngle => Main.rand.NextFloat(0, MathHelper.TwoPi);

    }
}
