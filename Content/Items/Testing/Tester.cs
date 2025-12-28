using AyaMod.Content.Projectiles;
using AyaMod.Core;
using AyaMod.Helpers;
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
    public class Tester : ModItem
    {
        public override string Texture => AssetDirectory.Testing + Name;
        public override void SetDefaults()
        {
            Item.width = Item.height = 38;
            Item.damage = 50;
            Item.knockBack = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 8;
            Item.useTime = Item.useAnimation = 10;
            Item.DamageType = DamageClass.Melee;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int index = 0;

            switch (index)
            {
                case 0:
                    var aura = BaseBuffAura.Spawn(source, Main.MouseWorld, 2 * 60, BuffID.Ichor, 60, 400, new Color(20,20,20,128), new Color(168,74,69,255), player.whoAmI, true);
                    aura.SetRadiusFadein(0.4f, Common.Easer.Ease.OutCubic);
                    aura.SetRadiusFadeout(0.6f, Common.Easer.Ease.OutCubic);
                    break;
                default:break;
            }

            return false;
        }
    }
}
