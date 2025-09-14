using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Systems.Trails
{
    public class Trail
    {
        public delegate Color StripColorFunction(float progress);
        public delegate float StripWidthFunction(float progress);
        public delegate float StripRotationFunction(float progress);
        public delegate float StripAlphaFunction(float progress);

        public CustomVertexInfo[] vertices = new CustomVertexInfo[1];
        public short[] indices = new short[1];

        public void PrepareStrip(Vector2[] positions, StripRotationFunction rotFunction, StripColorFunction colorFunction, StripWidthFunction widthFunction, Vector2 offsetForAllPositions = default, StripAlphaFunction alphaFunction = null)
        {
            int length = positions.Length;
            for (int i = 0; i < length; i++)
            {
                if (positions[i] == Vector2.Zero || i == length - 1 || positions[i + 1] == Vector2.Zero) break;

                Vector2 pos = positions[i] + offsetForAllPositions;
                int indexOnVertexArray = i * 2;
                float progress = i / (float)(length - 1);
                AddVertex(rotFunction, colorFunction, widthFunction, pos, indexOnVertexArray, progress, alphaFunction);
            }
            PrepareIndices(length, includeBacksides: true);
        }
        public void PrepareStrip(Vector2[] positions, float[] rotations, StripColorFunction colorFunction, StripWidthFunction widthFunction, Vector2 offsetForAllPositions = default, StripAlphaFunction alphaFunction = null)
        {
            int length = positions.Length;
            for (int i = 0; i < length; i++)
            {
                if (positions[i] == Vector2.Zero || i == length - 1 || positions[i + 1] == Vector2.Zero) break;

                Vector2 pos = positions[i] + offsetForAllPositions;
                float rot = MathHelper.WrapAngle(rotations[i]);
                int indexOnVertexArray = i * 2;
                float progress = i / (float)(length - 1);
                AddVertex(colorFunction, widthFunction, pos, rot, indexOnVertexArray, progress, alphaFunction);
            }
            PrepareIndices(vertices.Length, includeBacksides: true);
        }
        public void AddVertex(StripRotationFunction rotFunc, StripColorFunction colorFunc, StripWidthFunction widthFunc, Vector2 pos, int indexOnVertexArray, float progress, StripAlphaFunction alphaFunc = null)
        {
            while (indexOnVertexArray + 1 >= vertices.Length)
            {
                Array.Resize(ref vertices, vertices.Length * 2);
            }

            Color color = colorFunc(progress);
            float width = widthFunc(progress);
            float rot = rotFunc(progress);
            float alpha = alphaFunc == null ? 1f : alphaFunc(progress);
            Vector2 normal = MathHelper.WrapAngle(rot - MathHelper.PiOver2).ToRotationVector2() * width;
            vertices[indexOnVertexArray].Position = pos + normal;
            vertices[indexOnVertexArray + 1].Position = pos - normal;
            vertices[indexOnVertexArray].TexCoord = new Vector3(progress, 1f, alpha);
            vertices[indexOnVertexArray + 1].TexCoord = new Vector3(progress, 0f, alpha);
            vertices[indexOnVertexArray].Color = color;
            vertices[indexOnVertexArray + 1].Color = color;
        }
        public void AddVertex(StripColorFunction colorFunc, StripWidthFunction widthFunc, Vector2 pos, float rot, int indexOnVertexArray, float progress, StripAlphaFunction alphaFunc = null)
        {
            while (indexOnVertexArray + 1 >= vertices.Length)
            {
                Array.Resize(ref vertices, vertices.Length * 2);
            }

            Color color = colorFunc(progress);
            float width = widthFunc(progress);
            float alpha = alphaFunc == null ? 1f : alphaFunc(progress);
            Vector2 normal = MathHelper.WrapAngle(rot - MathHelper.PiOver2).ToRotationVector2() * width;
            vertices[indexOnVertexArray].Position = pos + normal;
            vertices[indexOnVertexArray + 1].Position = pos - normal;
            vertices[indexOnVertexArray].TexCoord = new Vector3(progress, 1f, alpha);
            vertices[indexOnVertexArray + 1].TexCoord = new Vector3(progress, 0f, alpha);
            vertices[indexOnVertexArray].Color = color;
            vertices[indexOnVertexArray + 1].Color = color;
        }

        public void PrepareIndices(int vertexPaidsAdded, bool includeBacksides)
        {
            int pairs = vertexPaidsAdded - 1;
            int indicesPerPair = 6 + includeBacksides.ToInt() * 6;
            int indicesCount = pairs * indicesPerPair;
            if (indices.Length < indicesCount)
                Array.Resize(ref indices, indicesCount);

            for(short i = 0; i < pairs; i++)
            {
                short indice = (short)(i * indicesPerPair);
                int start = i * 2;
                indices[indice] = (short)start;
                indices[indice + 1] = (short)(start + 1);
                indices[indice + 2] = (short)(start + 2);
                indices[indice + 3] = (short)(start + 2);
                indices[indice + 4] = (short)(start + 1);
                indices[indice + 5] = (short)(start + 3);
                if (includeBacksides)
                {
                    indices[indice + 6] = (short)(start + 2);
                    indices[indice + 7] = (short)(start + 1);
                    indices[indice + 8] = (short)start;
                    indices[indice + 9] = (short)(start + 2);
                    indices[indice + 10] = (short)(start + 3);
                    indices[indice + 11] = (short)(start + 1);
                }
            }
        }

        public void DrawTrail()
        {
            if(vertices.Length > 2)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertices.Length - 2);
            }
            //if (vertices.Length > 2)
            //    Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, vertices.Length / 3);
        }
    }
}
