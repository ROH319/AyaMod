using AyaMod.Core;
using AyaMod.Core.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;


namespace AyaMod.Content.Items.PrefixHammers
{
    public class ReforgeHammer : BasePrefixHammer
    {
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

            hammer?.OnReforge(item);

            return false;
        }

        public override void OnReforge(Item item)
        {
            int prefix = item.prefix;
            if (prefix < PrefixID.Count && prefix != 0) return;

            ModPrefix modprefix = PrefixLoader.GetPrefix(prefix);
            ModPrefix myprefix = PrefixToForge;
            if (myprefix != null)
            {
                item.SetDefaults(item.type);
                item.Prefix(myprefix.Type);
            }
            else
            {
                PrefixToForge = modprefix;
                item.SetDefaults(item.type);
            }

            if (prefix == 0) PrefixToForge = null;
            else PrefixToForge = modprefix;
            SoundEngine.PlaySound(SoundID.Item37);
        }
    }
}
