using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace AyaMod.Core.Loaders
{
    public class UILoader : ModSystem, ILoader
    {
        public static List<UserInterface> UserInterfaces;
        public static List<BaseUIState> UIStates;

        public void PreLoad(Mod mod)
        {
            UserInterfaces = [];
            UIStates = [];
        }
        public void Load(Mod mod, Type type)
        {
            if (type.IsSubclassOf(typeof(BaseUIState)))
            {
                var state = (BaseUIState)Activator.CreateInstance(type, null);
                var userInterface = new UserInterface();
                userInterface.SetState(state);

                UIStates?.Add(state);
                UserInterfaces?.Add(userInterface);
            }
        }
        public void PreUnload(Mod mod)
        {
            UserInterfaces = null;
            UIStates = null;
        }

        public static void AddLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible)
        {
            string name = state == null ? "???" : state.ToString();
            layers.Insert(index, new LegacyGameInterfaceLayer("AyaMod: " + name, delegate
            {
                if (visible)
                    state.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.UI));
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            for (int i = 0; i < UIStates.Count; i++)
            {
                var state = UIStates[i];
                AddLayer(layers, UserInterfaces[i], state, state.UILayer(layers), state.Visible);
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            foreach(UserInterface ui in UserInterfaces)
            {
                if (ui?.CurrentState != null && ((BaseUIState)ui.CurrentState).Visible)
                    ui.Update(gameTime);
            }
        }

        public static T GetUIState<T>() where T : BaseUIState => UIStates.FirstOrDefault(s => s is T) as T;

        public static UserInterface GetUserInterface<T>() where T : BaseUIState => UserInterfaces.FirstOrDefault(s => s.CurrentState is T);
    }
}
