using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Helpers
{
    public static class Helper
    {


        /// <summary>
        /// 向箱子第一个空格子加入一个物品
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="itemtype"></param>
        public static void AddItem<T>(this Chest chest) where T : ModItem
        {
            foreach (var item in chest.item)
            if (item.IsAir)
            {
                item.SetDefaults(ModContent.ItemType<T>());
                return;
            }
        }
        /// <summary>
        /// 向箱子第一个空格子加入一个物品
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="itemtype"></param>
        public static void AddItem<T>(this Chest chest, int itemType)
        {
            foreach (var item in chest.item)
            if (item.IsAir)
            {
                item.SetDefaults(itemType);
                return;
            }
        }

        public static SlotId PlayPitched(string path, float volume = 1f, float pitch = 0, Vector2? position = null)
        {
            if (Main.dedServ)
                return SlotId.Invalid;
            var style = new SoundStyle($"{nameof(AyaMod)}/Assets/Sounds/{path}")
            {
                Volume = volume,
                Pitch = pitch,
                MaxInstances = 40
            };
            return SoundEngine.PlaySound(style, position);
        }

        public static int GetHuedDustType(int hue)
        {
            int dustType = hue switch
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
            return dustType;
        }

        public static void GenDustRand(int count, int type, Vector2 pos, float distMin, float distMax, float maxSpeed, float minSpeed, int alpha = 0, float scale = 1f, bool noGravity = false)
        {
            for(int i = 0; i < count; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(minSpeed, maxSpeed);
                Dust d = Dust.NewDustPerfect(pos, type, vel, alpha, Scale:scale);
                d.noGravity = noGravity;
            }
        }
        public static void DashHandle(this Player player, out int dir, out bool dashing)
        {
            dir = 0;
            dashing = true;
            if(player.controlRight && player.releaseRight)
            {
                dir = 1;
                dashing = true;
                player.timeSinceLastDashStarted = 0;
            }
            else if (player.controlLeft && player.releaseLeft)
            {
                dir = -1;
                dashing = true;
                player.timeSinceLastDashStarted = 0;
            }

            //dir = 0;
            //dashing = true;
            //if (player.dashTime > 0) player.dashTime--;
            //if (player.dashTime < 0) player.dashTime++;

            //if(player.controlRight && player.releaseRight)
            //{
            //    if (player.dashTime > 0)
            //    {
            //        dir = 1;
            //        dashing = true;
            //        player.dashTime = 0;
            //        player.timeSinceLastDashStarted = 0;
            //    }
            //    else player.dashTime = 15;
            //}
            //else if (player.controlLeft && player.releaseLeft)
            //{
            //    if (player.dashTime < 0)
            //    {
            //        dir = -1;
            //        dashing = true;
            //        player.dashTime = 0;
            //        player.timeSinceLastDashStarted = 0;
            //    }
            //    else player.dashTime = -15;
            //}
        }

        /// <summary>
        /// 求解过两点且半径为r的圆心（返回0/1/2个圆心）
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <param name="radius">圆半径</param>
        /// <returns>符合条件的圆心列表</returns>
        public static List<Vector2> FindCircleCenters(Vector2 p1, Vector2 p2, float radius)
        {
            List<Vector2> centers = new List<Vector2>();

            // 1. 计算两点间距和中点
            float d = Vector2.Distance(p1, p2);
            Vector2 mid = (p1 + p2) / 2f;

            // 边界1：两点重合（无数个圆心，返回空或自定义逻辑）
            if (d < float.Epsilon)
            {
                // 两点重合时，以p1为圆心、radius为半径的圆都满足，此处返回空（可根据需求修改）
                return centers;
            }

            // 边界2：两点间距超过直径（无满足条件的圆）
            float halfD = d / 2f;
            if (halfD > radius + float.Epsilon) // 加浮点误差容错
            {
                return centers;
            }

            // 边界3：两点间距等于直径（仅1个圆心：中点）
            if (Math.Abs(halfD - radius) < float.Epsilon)
            {
                centers.Add(mid);
                return centers;
            }

            // 2. 计算中垂线方向和高度h
            float h = (float)Math.Sqrt(radius * radius - halfD * halfD); // 中垂线上的偏移距离

            // 两点连线的方向向量（p2 - p1），旋转90°得到中垂线方向
            Vector2 dir = p2 - p1;
            // 垂直向量（顺时针旋转90°：(x,y)→(y,-x)；逆时针：(-y,x)，对应两个圆心）
            Vector2 perp1 = new Vector2(dir.Y, -dir.X); // 顺时针垂直向量
            Vector2 perp2 = new Vector2(-dir.Y, dir.X); // 逆时针垂直向量

            // 归一化垂直向量，避免长度干扰
            perp1 = Vector2.Normalize(perp1);
            perp2 = Vector2.Normalize(perp2);

            // 3. 计算两个圆心
            Vector2 center1 = mid + perp1 * h;
            Vector2 center2 = mid + perp2 * h;

            centers.Add(center1);
            centers.Add(center2);

            return centers;
        }
        /// <summary>
        /// 判断以a为圆心时，b/c中哪个是劣弧的顺时针起始点
        /// </summary>
        /// <param name="a">圆心</param>
        /// <param name="b">点B</param>
        /// <param name="c">点C</param>
        /// <returns>劣弧顺时针起始点（b/c）；共线时返回Vector2.Zero</returns>
        public static Vector2 GetClockwiseStartOfMinorArc(Vector2 a, Vector2 b, Vector2 c)
        {
            // 1. 构建以a为原点的向量
            Vector2 ab = b - a;
            Vector2 ac = c - a;

            // 边界：b/c与圆心重合，或b=c（无弧）
            if (ab.LengthSquared() < float.Epsilon || ac.LengthSquared() < float.Epsilon || b == c)
            {
                return Vector2.Zero;
            }

            // 2. 计算二维向量叉乘的Z轴分量（核心：判断旋转方向）
            // 叉乘公式：ab × ac = ab.X * ac.Y - ab.Y * ac.X
            float crossProduct = ab.X * ac.Y - ab.Y * ac.X;

            // 3. 处理共线情况（叉乘=0，劣弧退化为直线）
            if (Math.Abs(crossProduct) < float.Epsilon)
            {
                return Vector2.Zero;
            }

            // 4. 判断顺时针起始点
            if (crossProduct > 0)
            {
                // 叉乘>0：c在ab逆时针方向 → 从b顺时针到c是劣弧 → 起始点为b
                return b;
            }
            else
            {
                // 叉乘<0：c在ab顺时针方向 → 从c顺时针到b是劣弧 → 起始点为c
                return c;
            }
        }

        /// <summary>
        /// 检测矩形与圆环的碰撞
        /// </summary>
        /// <param name="rect">目标矩形</param>
        /// <param name="ringCenter">圆环中心坐标</param>
        /// <param name="minDist">圆环内径</param>
        /// <param name="maxDist">圆环外径</param>
        /// <returns>是否发生碰撞</returns>
        public static bool CheckRingCollision(Rectangle rect, Vector2 ringCenter, float minDist, float maxDist)
        {
            // 1. 检查矩形是否在圆环外
            if (!IsWithinCircle(rect, ringCenter, maxDist))
                return false;

            // 2. 检查矩形是否完全在圆环内
            return !IsFullyContained(rect, ringCenter, minDist);
        }

        // 判断矩形是否完全包含在圆内
        private static bool IsFullyContained(Rectangle rect, Vector2 center, float radius)
        {
            // 计算圆半径平方
            float radiusSq = radius * radius;

            // 检查矩形四个角是否都在圆内
            if (Vector2.DistanceSquared(center, new Vector2(rect.Left, rect.Top)) > radiusSq) return false;
            if (Vector2.DistanceSquared(center, new Vector2(rect.Right, rect.Top)) > radiusSq) return false;
            if (Vector2.DistanceSquared(center, new Vector2(rect.Left, rect.Bottom)) > radiusSq) return false;
            if (Vector2.DistanceSquared(center, new Vector2(rect.Right, rect.Bottom)) > radiusSq) return false;

            return true;
        }

        // 判断矩形是否接触或越出圆环外边界
        private static bool IsWithinCircle(Rectangle rect, Vector2 center, float radius)
        {
            // 计算圆半径平方
            float radiusSq = radius * radius;

            // 1. 中心点在圆内 - 立即返回true
            if (rect.Contains((int)center.X, (int)center.Y))
                return true;

            // 2. 检查矩形最近的边到圆心的距离
            // 计算矩形上与圆心最近的点
            Vector2 closest = new Vector2(
                MathHelper.Clamp(center.X, rect.Left, rect.Right),
                MathHelper.Clamp(center.Y, rect.Top, rect.Bottom)
            );

            // 检查最近点是否在圆内
            float distanceSq = Vector2.DistanceSquared(center, closest);
            return distanceSq <= radiusSq;
        }

        // 获取旋转矩形的四个顶点
        public static List<Vector2> GetRotatedRectangleVertices(Vector2 center, Vector2 size, float rotation)
        {
            Vector2 halfSize = size / 2f;

            // 计算旋转矩阵
            Matrix transform = Matrix.CreateRotationZ(rotation) *
                               Matrix.CreateTranslation(center.X, center.Y, 0);

            // 未旋转前的四个角点
            Vector2[] corners = new Vector2[]
            {
            new Vector2(-halfSize.X, -halfSize.Y),
            new Vector2(halfSize.X, -halfSize.Y),
            new Vector2(halfSize.X, halfSize.Y),
            new Vector2(-halfSize.X, halfSize.Y)
            };

            // 应用旋转
            return corners.Select(corner => Vector2.Transform(corner, transform)).ToList();
        }

        // 计算顶点数组的包围盒
        public static (Vector2 min, Vector2 max) GetBoundingBox(Vector2[] vertices)
        {
            Vector2 min = vertices[0];
            Vector2 max = vertices[0];

            foreach (Vector2 vertex in vertices)
            {
                min.X = Math.Min(min.X, vertex.X);
                min.Y = Math.Min(min.Y, vertex.Y);
                max.X = Math.Max(max.X, vertex.X);
                max.Y = Math.Max(max.Y, vertex.Y);
            }

            return (min, max);
        }

        // 判断轴对齐矩形是否在旋转矩形内
        public static bool IsRectInRotatedRect(Rectangle rect, Vector2 center, Vector2 size, float rotation)
        {
            // 使用局部坐标转换检测
            Matrix invertRotation = Matrix.CreateRotationZ(-rotation);

            // 检查矩形的四个角和中心
            Vector2[] testPoints = new Vector2[]
            {
            new Vector2(rect.Left, rect.Top),
            new Vector2(rect.Right, rect.Top),
            new Vector2(rect.Right, rect.Bottom),
            new Vector2(rect.Left, rect.Bottom),
            new Vector2(rect.Center.X, rect.Center.Y)
            };

            // 检查所有测试点
            foreach (Vector2 point in testPoints)
            {
                Vector2 localPoint = point - center;
                Vector2 transformed = Vector2.Transform(localPoint, invertRotation);

                if (Math.Abs(transformed.X) <= size.X / 2 && Math.Abs(transformed.Y) <= size.Y / 2)
                    return true;
            }

            return false;
        }

        // 合并相邻的矩形
        public static List<Rectangle> MergeAdjacentRects(List<Rectangle> rects)
        {
            List<Rectangle> mergedRects = new List<Rectangle>(rects);
            bool merged;

            // 水平合并
            do
            {
                merged = false;
                for (int i = 0; i < mergedRects.Count; i++)
                {
                    for (int j = i + 1; j < mergedRects.Count; j++)
                    {
                        Rectangle rectA = mergedRects[i];
                        Rectangle rectB = mergedRects[j];

                        // 检查是否可以水平合并
                        if (rectA.Y == rectB.Y && rectA.Height == rectB.Height)
                        {
                            //左A右B
                            if (rectA.Right == rectB.Left)
                            {
                                mergedRects.Add(new Rectangle(rectA.X, rectA.Y, rectA.Width + rectB.Width, rectA.Height));
                                mergedRects.RemoveAt(j);
                                mergedRects.RemoveAt(i);
                                merged = true;
                                break;
                            }
                            //左B右A
                            else if (rectA.Left == rectB.Right)
                            {
                                mergedRects.Add(new Rectangle(rectB.X, rectB.Y, rectA.Width + rectB.Width, rectB.Height));
                                mergedRects.RemoveAt(j);
                                mergedRects.RemoveAt(i);
                                merged = true;
                                break;
                            }
                        }
                        // 检查是否可以垂直合并
                        else if (rectA.X == rectB.X && rectA.Width == rectB.Width)
                        {
                            if (rectA.Bottom == rectB.Top)
                            {
                                mergedRects.Add(new Rectangle(rectA.X, rectA.Y, rectA.Width, rectA.Height + rectB.Height));
                                mergedRects.RemoveAt(j);
                                mergedRects.RemoveAt(i);
                                merged = true;
                                break;
                            }
                            else if (rectA.Top == rectB.Bottom)
                            {
                                mergedRects.Add(new Rectangle(rectB.X,rectB.Y,rectB.Width,rectA.Height + rectB.Height));
                                mergedRects.RemoveAt(j);
                                mergedRects.RemoveAt(i);
                                merged = true;
                                break;
                            }
                        }
                    }
                    if (merged) break;
                }
            } while (merged);

            return mergedRects;
        }
    }
}
