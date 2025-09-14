using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static AyaMod.Core.Systems.Trails.Trail;

namespace AyaMod.Core.Systems.Trails
{
    public class MultedTrail
    {
        public List<CustomVertexInfo> vertices = new();
        public void PrepareStrip(Vector2[] positions, float mult, StripColorFunction colorFunction, StripWidthFunction widthFunction, Vector2 offsetForAllPositions = default, StripAlphaFunction alphaFunction = null)
        {
            vertices.Clear();
            int total = (int)(positions.Length * mult - mult);
            Vector2 lastTrailPos = Vector2.Zero;

            for (int i = 0; i < total - 1; i++)
            {
                if (positions[(int)(i / mult)] == Vector2.Zero || positions[(int)(i / mult) + 1] == Vector2.Zero) continue;

                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                Vector2 trailPos = Vector2.Lerp(positions[(int)(i / mult)], positions[(int)(i / mult) + 1], lerpFactor);
                trailPos += offsetForAllPositions;


                float rot = (lastTrailPos - trailPos).ToRotation();
                lastTrailPos = trailPos;

                if (i == 0) continue;

                int indexOnVertexArray = i * 2;
                float progress = i / (float)total;
                AddVertex(colorFunction, widthFunction, trailPos, rot, indexOnVertexArray, progress, alphaFunction);
            }
        }
        public void AddVertex(StripColorFunction colorFunc, StripWidthFunction widthFunc, Vector2 pos, float rot, int indexOnVertexArray, float progress, StripAlphaFunction alphaFunc = null)
        {
            //while (indexOnVertexArray + 1 >= vertices.Count)
            //{
            //    Array.Resize(ref vertices, vertices.Count * 2);
            //}

            Color color = colorFunc(progress);
            float width = widthFunc(progress);
            float alpha = alphaFunc == null ? 1f : alphaFunc(progress);
            Vector2 normal = (rot - MathHelper.PiOver2).ToRotationVector2() * width;
            vertices.Add(new CustomVertexInfo(pos + normal, color * alpha, new Vector3(progress, 1f, alpha)));
            vertices.Add(new CustomVertexInfo(pos - normal, color * alpha, new Vector3(progress, 0f, alpha)));
            //vertices[indexOnVertexArray].Position = pos + normal;
            //vertices[indexOnVertexArray + 1].Position = pos - normal;
            //vertices[indexOnVertexArray].TexCoord = new Vector3(progress, 1f, alpha);
            //vertices[indexOnVertexArray + 1].TexCoord = new Vector3(progress, 0f, alpha);
            //vertices[indexOnVertexArray].Color = color;
            //vertices[indexOnVertexArray + 1].Color = color;
        }
        public void DrawTrail()
        {
            if (vertices.Count > 2)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                
            }
        }
    }
}
