using AyaMod.Core;
using AyaMod.Core.Prefabs;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class StardustFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3529;
    }
}
