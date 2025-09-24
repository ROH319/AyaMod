using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace AyaMod.Core.Prefabs
{
    public abstract class BaseDyeFilm : BaseFilm
    {
        public override string Texture => AssetDirectory.DyeFilms + Name;
        public virtual int DyeID => 0;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (DyeID == 0) return true;

            var shader = GameShaders.Armor.GetShaderFromItemId(DyeID);
            var data = new DrawData(TextureAssets.Item[Type].Value, position + Main.screenPosition, frame, itemColor, 0, origin, scale, 0);
            //shader.Apply(Item, data);
            //shader.Shader.CurrentTechnique.Passes[0].Apply();
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0], spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, shader.Shader, Main.UIScaleMatrix);
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (DyeID == 0) return;

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0], spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }
    }
}
