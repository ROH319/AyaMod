using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
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
