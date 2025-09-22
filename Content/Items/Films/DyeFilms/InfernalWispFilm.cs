using AyaMod.Core;
using AyaMod.Core.Prefabs;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class InfernalWispFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 2885;
        public static float WispDmgRegen = 125 / 60;
        public static int WispDmgMax = 500;
    }
}
