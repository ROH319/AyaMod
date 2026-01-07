using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.ModPlayers
{
    public class DataPlayer : ModPlayer
    {
        public Item[] UsingFilm = new Item[10];
        public Item[] FilmVault = new Item[80];

        public override void Initialize()
        {
            for (int i = 0; i < UsingFilm.Length; i++)
            {
                UsingFilm[i] ??= new Item();
            }
            for (int i = 0;i < FilmVault.Length; i++)
            { 
                FilmVault[i] ??= new Item(); 
            }
        }
    }
}
