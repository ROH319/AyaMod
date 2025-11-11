using AyaMod.Core.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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

        public static string GetText(string key, params object[] args)
        {
            return Language.Exists(key) ? Language.GetTextValue(key, args) : key;
        }

        /// <summary>
        /// 返回从start到end的直线段上，是否有碰撞到实心方块
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="interval"></param>
        /// <returns>为true表示没碰撞</returns>
        public static bool CheckLineCollisionTile(Vector2 start, Vector2 end, int interval = 8)
        {
            bool canhit = true;
            int checkCount = (int)(start.Distance(end) / interval);
            for (int i = 0; i <= checkCount; i++)
            {
                var checkpos = Vector2.Lerp(start, end, i / (float)checkCount);
                if (PointInTile(checkpos))
                {
                    canhit = false;
                    break;
                }
            }
            return canhit;
        }

        public static bool PointInTile(Vector2 point)
        {
            var startCoords = new Point((int)(point.X / 16), (int)(point.Y / 16));
            for(int x = -1;x<=1;x++)
            {
                for(int y = -1;y<=1;y++)
                {
                    var coords = startCoords + new Point(x, y);
                    if (WorldGen.InWorld(coords.X, coords.Y))
                    {
                        var tile = Main.tile[coords.X, coords.Y];
                        if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                        {
                            var rect = new Rectangle(coords.X * 16, coords.Y * 16, 16, 16);
                            if (rect.Contains(point.ToPoint()))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public static Vector2 GetPentagramPos(Vector2 center, float radius, float factor, float extraRot = -MathHelper.PiOver2, bool reversed = false)
        {
            float n = (int)(factor / 0.2f);
            float innerFactor = factor % 0.2f;
            Vector2 offset = Vector2.Zero;
            Vector2 startPos = (n * MathHelper.ToRadians(144) + extraRot).ToRotationVector2() * radius;
            Vector2 endPos = ((n + 1) * MathHelper.ToRadians(144) + extraRot).ToRotationVector2() * radius;
            offset = Vector2.Lerp(startPos, endPos, innerFactor / 0.2f);
            return center + offset;
        }

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

        public static Vector3 RGB2HSL(this Color color)
        {
            // 将RGB值归一化到[0,1]
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(Math.Max(r, g), b);
            float min = Math.Min(Math.Min(r, g), b);
            float delta = max - min;

            // 计算亮度
            float l = (max + min) / 2f;

            // 灰度情况（饱和度为0）
            if (delta < 1e-6)
                return new(0f, 0f, l);

            // 计算饱和度
            float s = l < 0.5f ?
                delta / (max + min) :
                delta / (2f - max - min);

            // 计算色相
            float h = 0f;
            if (max == r)
            {
                h = (g - b) / delta;
                if (h < 0) h += 6f;
            }
            else if (max == g)
                h = 2f + (b - r) / delta;
            else if (max == b)
                h = 4f + (r - g) / delta;

            // 将色相转换到[0,360]度
            h *= 60f;
            if (h < 0f) h += 360f;

            return new(h, s, l);
        }
        public static Color HSL2RGB(float h, float s, float l, byte alpha = 255)
        {
            // 规范化输入
            h = MathHelper.Clamp(h, 0f, 360f) % 360f;
            s = MathHelper.Clamp(s, 0f, 1f);
            l = MathHelper.Clamp(l, 0f, 1f);

            // 饱和度接近0 -> 灰度
            if (s < 1e-6)
            {
                byte value = (byte)(l * 255);
                return new Color(value, value, value, alpha);
            }

            // 转换色相到[0,6)范围
            float h6 = h / 60f;
            // 中间计算变量
            float c = (1f - Math.Abs(2f * l - 1f)) * s;
            float x = c * (1f - Math.Abs(h6 % 2f - 1f));

            float r = 0f, g = 0f, b = 0f;

            // 6个色相区段计算RGB值
            switch ((int)Math.Floor(h6))
            {
                case 0: // 红-黄: 0-60°
                    (r, g, b) = (c, x, 0);
                    break;
                case 1: // 黄-绿: 60-120°
                    (r, g, b) = (x, c, 0);
                    break;
                case 2: // 绿-青: 120-180°
                    (r, g, b) = (0, c, x);
                    break;
                case 3: // 青-蓝: 180-240°
                    (r, g, b) = (0, x, c);
                    break;
                case 4: // 蓝-紫: 240-300°
                    (r, g, b) = (x, 0, c);
                    break;
                case 5: // 紫-红: 300-360°
                    (r, g, b) = (c, 0, x);
                    break;
            }

            // 调整明度
            float m = l - c / 2f;

            // 合并并转换到[0,255]范围
            return new Color(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255),
                alpha
            );
        }
        public static Color HSL2RGB(Vector3 hsl, byte alpha = 255) => HSL2RGB(hsl.X, hsl.Y, hsl.Z, alpha);


    }
}
