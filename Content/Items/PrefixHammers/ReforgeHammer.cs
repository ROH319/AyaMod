using AyaMod.Core;
using AyaMod.Core.Globals;
using System;
using Terraria;
using Terraria.ID;


namespace AyaMod.Content.Items.PrefixHammers
{
    public class ReforgeHammer : BasePrefixHammer
    {
        public override int PrefixTypeToForge => -1;

        public override void Load()
        {
            CameraGlobalItem.OnCanRightClick += TryReforgeCamera;
        }

        public bool TryReforgeCamera(Item item)
        {
            if (!Main.mouseRight || !Main.mouseRightRelease) return false;
            Player player = Main.player[Main.myPlayer];

            Item mouseItem = Main.mouseItem;

            if (item.DamageType != ReporterDamage.Instance || mouseItem.IsAir || mouseItem.ModItem is not BasePrefixHammer) return false;

            var hammer = mouseItem.ModItem as BasePrefixHammer;

            item.SetDefaults(item.type);
            item.Prefix(hammer.PrefixTypeToForge);

            return false;
        }
    }
}
