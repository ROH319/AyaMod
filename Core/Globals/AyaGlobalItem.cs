using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Globals
{
    public class AyaGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool WingUpdate(int wings, Player player, bool inUse)
        {
            var ayaPlayer = player.Aya();
            if (ayaPlayer.freeFlyFrame > 0)
            {
                ayaPlayer.freeFlyFrame--;
                player.wingTime++;
            }

            return base.WingUpdate(wings, player, inUse);
        }
    }
}
