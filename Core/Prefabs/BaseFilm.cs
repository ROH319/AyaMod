﻿using AyaMod.Content.Items.Films;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Core.Prefabs
{
    public abstract class BaseFilm : ModItem
    {
        public override string Texture => AssetDirectory.Films + Name;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.ammo = ModContent.ItemType<CameraFilm>();
        }

        public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
        {
            if (weapon.ModItem is BaseCamera)
            {
                return true;
            }
            return base.CanBeChosenAsAmmo(weapon, player);
        }


        public virtual void ModifyHitNPCFilm(BaseCameraProj projectile, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnSnap() { }
        public virtual void OnSnapInSight() { }
    }
}
