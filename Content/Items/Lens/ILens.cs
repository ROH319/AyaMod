using AyaMod.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Items.Lens
{
    public interface ILens
    {
        public bool Colliding(Vector2 center, float size, float rot, Rectangle targetRect);

        public List<Rectangle> GetRectanglesAgainstEntity(Vector2 center, float size, float rot);

        public void DrawCamera(SpriteBatch spriteBatch, Player player, Vector2 center, float rot, float size, 
            float focusdScale, float maxFocusScale, Color outerFrameColor, Color innerFrameColor, Color focusCenterColor);

        public virtual void SpawnFlash(Vector2 center, Color color, float size, float rot, int flashTime)
        {
            //TODO：填写正确的生成源
            CameraFlash.Spawn(null, center, color, rot, size / 16f, size * 1.4f / 16f, flashTime);
        }
    }
}
