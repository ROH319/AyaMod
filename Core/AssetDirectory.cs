using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;

namespace AyaMod.Core
{
    public static class AssetDirectory
    {
        public const string Assets = "AyaMod/Assets/";

        public const string Textures = Assets + "Textures/";

        public const string Buffs = Textures + "Buffs/";

        public const string Items = Textures + "Items/";

        public const string Accessories = Items + "Accessories/";

        public const string Cameras = Items + "Cameras/";

        public const string Lens = Items + "Lens/";

        public const string Films = Items + "Films/";

        public const string DyeFilms = Films + "DyeFilms/";

        public const string Particles = Textures + "Particles/";

        public const string Projectiles = Textures + "Projectiles/";

        public const string UI = Textures + "UI/";
        
        public const string EmptyTexturePass = Textures + "A";

        public const string StarTexturePass = Textures + "StarTexture";

        public const string Extras = Textures + "Extra/";

        public const string Effects = "AyaMod/Effects/";


        public static Texture2D StarTexture;

        public static Effect RevertTooltip;

        public static void LoadAsset()
        {
            StarTexture = Request<Texture2D>(StarTexturePass,AssetRequestMode.ImmediateLoad).Value;

            RevertTooltip = Request<Effect>(Effects + "RevertTooltip", AssetRequestMode.ImmediateLoad).Value;
        }

        public static void UnloadAsset()
        {
            StarTexture = null;
            RevertTooltip = null;
        }

        public static string VanillaTexturePath(string path) => $"Terraria/Images/{path}";

        public static string VanillaProjPath(int type) => $"{VanillaTexturePath($"Projectile_{type}")}";

        public static string VanillaItemPath(int type) => $"{VanillaTexturePath($"Item_{type}")}";
    }
}
