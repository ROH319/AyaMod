using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.Core;

namespace AyaMod.Core.Loaders
{
    public class ShaderLoader : ILoader
    {
        private static Dictionary<string, Asset<Effect>> shaders = new();

        public void Load(Mod mod, Type type)
        {
            if (Main.dedServ)
                return;

            MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            var file = (TmodFile)info.Invoke(AyaMod.Instance, null);

            var shaders = file.Where(x => x.Name.StartsWith("Effects/") && x.Name.EndsWith(".xnb"));
            foreach(var shader in shaders)
            {
                string name = shader.Name.Replace(".xnb", "").Replace("Effect/", "");
                string path = shader.Name.Replace(".xnb", "");
                LoadShader(name, path);
            }
        }
        public void Unload(Mod mod, Type type)
        {

        }
        public static Effect GetShader(string key)
        {
            if (!shaders.ContainsKey(key)) LoadShader(key, $"Effects/{key}");
            return shaders[key].Value;
        }
        public static void LoadShader(string name, string path)
        {
            if (!shaders.ContainsKey(name))
                //new(() => AyaMod.Instance.Assets.Request<Effect>(path,AssetRequestMode.ImmediateLoad))
                shaders.Add(name, AyaMod.Instance.Assets.Request<Effect>(path));
        }
    }
}