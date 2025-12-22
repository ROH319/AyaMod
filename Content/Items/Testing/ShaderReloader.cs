using AyaMod.Core;
using AyaMod.Core.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AyaMod.Content.Items.Testing
{
    public class ShaderReloader : ModItem
    {
        public override string Texture => AssetDirectory.VanillaTexturePath("UI/ButtonCloudActive");
        public override bool IsLoadingEnabled(Mod mod)
        {
#if DEBUG
            return true;
#else 
            return false;
#endif
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 22;

            Item.damage = 114514;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 10;

            Item.shoot = 10;
            Item.shootSpeed = 2;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Main.QueueMainThreadAction(() => ShaderLoader.LoadALLShader());
            

            return false;
        }
    }
}
