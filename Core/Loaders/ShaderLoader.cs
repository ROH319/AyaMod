using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static Dictionary<string, Effect> shaders = new();

        public static void LoadALLShader()
        {
            ShaderLoader.shaders.Clear();

            MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            var file = (TmodFile)info.Invoke(AyaMod.Instance, null);

            var shaders = file.Where(x => x.Name.StartsWith("Effects/") && x.Name.EndsWith(".xnb"));
            foreach (var shader in shaders)
            {
                string name = shader.Name.Replace(".xnb", "").Replace("Effect/", "");
                string path = shader.Name.Replace(".xnb", "");
                LoadShader(name, path);
            }

        }
        public void PostLoad(Mod mod)
        {
            if (Main.dedServ)
                return;
            LoadALLShader();
        }
        public void PreUnLoad(Mod mod)
        {
            shaders = null;
        }
        public static Effect GetShader(string key)
        {
            if (!shaders.ContainsKey(key)) LoadShader(key, $"Effects/{key}");
            return shaders[key];
        }
        public static void LoadShader(string name, string path)
        {
            if (!shaders.ContainsKey(name))
            {
                shaders.Add(name, AyaMod.Instance.Assets.Request<Effect>(path).Value);
            }
        }
    }
}