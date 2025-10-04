using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using AyaMod.Helpers;
using Terraria.ModLoader;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class BeeCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 35;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;

            Item.shootSpeed = 8;
            Item.knockBack = 8f;
            Item.shoot = ModContent.ProjectileType<BeeCameraProj>();
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 2, 0, 0));
            SetCameraStats(0.04f, 104, 2f);
            SetCaptureStats(1000, 60);
        }
    }

    public class BeeCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(254, 246, 37);
        public override Color innerFrameColor => new Color(181, 213, 219) * 0.7f;
        public override Color focusCenterColor => new Color(227,125,22);
        public override Color flashColor => new Color(254, 194, 20).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            int type = player.beeType();
            int count = 1;
            if (Main.rand.NextBool(6,10)) count++;

            int damage = player.beeDamage(Projectile.damage / 3);
            for(int i = 0;i < count; i++)
            {
                var dir = AyaUtils.RandAngle.ToRotationVector2();
                Vector2 pos = Projectile.Center + dir * Main.rand.NextFloat(40, 80);
                var bee = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, dir * 2, type, damage, Projectile.knockBack / 4, player.whoAmI);
                bee.DamageType = ReporterDamage.Instance;
                bee.SetImmune(10);
            }
        }
    }
}
