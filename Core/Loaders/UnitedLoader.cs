using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Core;

namespace AyaMod.Core.Loaders
{
    public class UnitedLoader : ModSystem
    {
        public static List<ILoader> LoaderList = new List<ILoader>();

        public override void Load()
        {
            LoaderList.Clear();
            LoaderList = GetLoaderList();

            for (int i = 0; i < LoaderList.Count; i++)
                LoaderList[i].PreLoad(Mod);

            foreach(var mod in ModLoader.Mods)
                if(mod is IAyaMod or AyaMod)
                    foreach(Type type in AssemblyManager.GetLoadableTypes(mod.Code))
                        for (int i = 0; i < LoaderList.Count; i++)
                            LoaderList[i].Load(mod, type);

            for (int i = 0; i < LoaderList.Count; i++)
                LoaderList[i].PostLoad(Mod);
        }

        public override void PostSetupContent()
        {
            for (int i = 0; i < LoaderList.Count; i++)
                LoaderList[i].PreSetUp(Mod);

            foreach (var mod in ModLoader.Mods)
                if (mod is IAyaMod or AyaMod)
                    foreach (Type type in AssemblyManager.GetLoadableTypes(mod.Code))
                        for (int i = 0; i < LoaderList.Count; i++)
                            LoaderList[i].SetUp(mod, type);

            for (int i = 0; i < LoaderList.Count; i++)
                LoaderList[i].PostSetUp(Mod);
        }

        public override void Unload()
        {
            for (int i = 0; i < LoaderList.Count; i++)
                LoaderList[i].PreUnLoad(Mod);

            foreach (var mod in ModLoader.Mods)
                if (mod is IAyaMod or AyaMod)
                    foreach (Type type in AssemblyManager.GetLoadableTypes(mod.Code))
                        for (int i = 0; i < LoaderList.Count; i++)
                            LoaderList[i].UnLoad(mod, type);
        }


        public List<ILoader> GetLoaderList()
        {
            List<ILoader> list = new List<ILoader>();

            foreach(var mod in ModLoader.Mods)
            {
                if(mod is IAyaMod or AyaMod)
                {
                    foreach(Type type in AssemblyManager.GetLoadableTypes(mod.Code))
                    {
                        if(!type.IsAbstract && type.GetInterfaces().Contains(typeof(ILoader)))
                        {
                            var instance = Activator.CreateInstance(type);
                            list.Add(instance as ILoader);
                        }


                    }
                }
            }
            return list;
        }
    }
}
