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

        public const string Buffs_Films = Buffs + "Films/";

        public const string Dusts = Textures + "Dusts/";

        public const string Items = Textures + "Items/";

        public const string Accessories = Items + "Accessories/";

        public const string Armors = Items + "Armors/";

        public const string Cameras = Items + "Cameras/";

        public const string Lens = Items + "Lens/";

        public const string Films = Items + "Films/";

        public const string DyeFilms = Films + "DyeFilms/";

        public const string PrefixHammers = Items + "PrefixHammers/";

        public const string Testing = Items + "Testing/";

        public const string Particles = Textures + "Particles/";

        public const string Projectiles = Textures + "Projectiles/";

        public const string Projectiles_Films = Projectiles + "Films/";

        public const string UI = Textures + "UI/";
        
        public const string EmptyTexturePass = Textures + "A";

        public const string StarTexturePass = Textures + "StarTexture";

        public const string Extras = Textures + "Extra/";

        public const string Effects = "AyaMod/Effects/";


        public static Texture2D StarTexture;

        public static Effect RevertTooltip;
        public static Effect Trail;
        public static Effect Trail2;
        public static Effect SimpleGradient;
        

        public static void LoadAsset()
        {
            StarTexture = Request<Texture2D>(StarTexturePass,AssetRequestMode.ImmediateLoad).Value;

            RevertTooltip = Request<Effect>(Effects + "RevertTooltip", AssetRequestMode.ImmediateLoad).Value;
            Trail = Request<Effect>(Effects + "Trail", AssetRequestMode.ImmediateLoad).Value;
            Trail2 = Request<Effect>(Effects + "Trail2", AssetRequestMode.ImmediateLoad).Value;
            SimpleGradient = Request<Effect>(Effects + "SimpleGradient", AssetRequestMode.ImmediateLoad).Value;
        }

        public static void UnloadAsset()
        {
            StarTexture = null;
            RevertTooltip = null;
            Trail = null;
            Trail2 = null;
            SimpleGradient = null;
        }

        public static string VanillaTexturePath(string path) => $"Terraria/Images/{path}";

        public static string VanillaProjPath(int type) => $"{VanillaTexturePath($"Projectile_{type}")}";

        public static string VanillaItemPath(int type) => $"{VanillaTexturePath($"Item_{type}")}";
    }
}
