using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.IO;

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
        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<Item[]>("UsingFilm", out var usingfilms))
                for (int i = 0; i < UsingFilm.Length && i < usingfilms.Length; i++)
                    UsingFilm[i] = usingfilms[i];
            if (tag.TryGet<Item[]>("FilmVault", out var filmvault))
                for (int i = 0; i < FilmVault.Length && i < filmvault.Length; i++)
                    FilmVault[i] = filmvault[i];
        }

        public override void SaveData(TagCompound tag)
        {
            tag["UsingFilm"] = UsingFilm;
            tag["FilmVault"] = FilmVault;
        }
    }
}
