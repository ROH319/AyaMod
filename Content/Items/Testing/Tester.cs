using AyaMod.Content.Particles;
using AyaMod.Content.Projectiles.Auras;
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

            int index = 1;

            switch (index)
            {
                case 0:
                    var aura = AuraFriendly.Spawn(source, Main.MouseWorld, 2 * 60, BuffID.Ichor, 60, 400, new Color(20,20,20,128), new Color(168,74,69,255), player.whoAmI);
                    aura.SetRadiusFadein(0.4f, Common.Easer.Ease.OutCubic);
                    aura.SetRadiusFadeout(0.6f, Common.Easer.Ease.OutCubic);
                    break;
                case 1:
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(4f,5f);
                        Vector2 pos = Main.MouseWorld + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 30f);
                        float randscale = Main.rand.NextFloat(0.75f, 1.25f);
                        for (int i = 0; i < 2; i++)
                        {
                            var color = i == 0 ? new Color(49, 92, 219) : Color.White;
                            float scale = i == 0 ? 1f : 0.6f;
                            var p = SoulsParticle2.Spawn(source, pos, vel, color.AdditiveColor(), scale * .3f * randscale, 40);
                            p.alpha = 0.9f;
                            //var p = LightSpotParticle.Spawn(source, pos, vel, color, scale * .4f * randscale, 40);
                            //p.alpha = 0.5f;
                            //p.SetAlphaMult(0.86f);
                            //p.SetScaleMult(0.99f);
                            //p.UseAlpha = true;
                        }
                    }
                    break;
                default:break;
            }

            return false;
        }
    }
}
