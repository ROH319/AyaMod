using AyaMod.Core;
using AyaMod.Core.Prefabs;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class NegativeFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 2872;
    }
}
