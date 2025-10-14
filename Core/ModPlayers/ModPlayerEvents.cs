using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.ModPlayers
{
    public class ModPlayerEvents
    {
        public delegate void PostUpdateDelegate(Player player);
        
        public delegate void ModifyWeaponDamageDelegate(Player player, Item item, ref StatModifier modifier);

        public delegate void ModifyHitByBothDelegate(Player player, ref Player.HurtModifiers modifier);
    }
}
