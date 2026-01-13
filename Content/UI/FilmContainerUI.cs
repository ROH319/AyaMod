using AyaMod.Common;
using AyaMod.Content.UI.Elements;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace AyaMod.Content.UI
{
    public class FilmContainerUI : BaseUIState
    {
        public static FilmContainerUI Instance { get; private set; }
        public FilmContainerUI() => Instance = this;

        public override bool Visible
        {
            get
            {
                if (_visible && !Main.playerInventory)
                {
                    _visible = false;
                    OpenTimer.Play(false);
                }
                return OpenTimer.NegativePlaying || _visible;
            }
            set
            {
                _visible = value;
                OpenTimer.Play(_visible);
            }
        }

        private static bool _visible;

        public DragablePanel Window;
        public UIPanel UsingFilmPanel;
        public UIPanel FilmContainerPanel;
        public UsingFilmGrid UsingFilmGrid = new() { ListPadding = 0f };
        public FilmContainerGrid FilmContainerGrid = new() { ListPadding = 0f };
        public FilmHideTab HideTab;
        public UIText Title;
        public SimpleCross CloseButton;

        //public bool Floating = false;
        //public Vector2 basePos = Vector2.Zero;
        //public Vector2 offset = Vector2.Zero;

        public TweenTimer OpenTimer = new(0, 1f);

        public override void OnInitialize()
        {
            OpenTimer.Finish(false);

            float BackWidth = 560;
            float BackHeight = 400;
            Vector2 offset = new(Main.screenWidth / 2f - BackWidth / 2f, Main.screenHeight / 2f - BackHeight / 2f);



            Window = new DragablePanel();
            Window.Left.Set(offset.X, 0);
            Window.Top.Set(offset.Y, 0);
            Window.Width.Set(BackWidth, 0);
            Window.Height.Set(BackHeight, 0);
            Window.PaddingLeft = Window.PaddingRight = Window.PaddingTop = Window.PaddingBottom = 0;
            Window.BackgroundColor = new Color(29, 33, 70) * 0.7f;


            UsingFilmPanel = new();
            UsingFilmPanel.Width.Set(BackWidth - 12, 0);
            UsingFilmPanel.Height.Set(70, 0);
            UsingFilmPanel.Left.Set(6, 0);
            UsingFilmPanel.Top.Set(42, 0);
            UsingFilmPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            FilmContainerPanel = new();
            FilmContainerPanel.Width.Set(BackWidth - 12, 0);
            FilmContainerPanel.Height.Set(BackHeight - 140, 0);
            FilmContainerPanel.Left.Set(6, 0);
            FilmContainerPanel.Top.Set(UsingFilmPanel.Top.Pixels+ UsingFilmPanel.Height.Pixels + 8, 0);
            FilmContainerPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            Title = new UIText(Language.GetOrRegister($"Mods.{AyaMod.Instance.Name}.UI.FilmContainerTitle"), 1f, true)
            {
                PaddingLeft = 12f
            };
            Title.Height.Set(0, 1f);
            Title.Width.Set(200f, 0f);


            UsingFilmGrid.SetPadding(8f);
            UsingFilmGrid.PaddingTop = 0f;
            UsingFilmGrid.PaddingBottom = 0f;
            UsingFilmGrid.Width.Set(0, 1f);
            UsingFilmGrid.Height.Set(0, 1f);

            FilmContainerGrid.SetPadding(8f);
            FilmContainerGrid.PaddingTop = 0f;
            FilmContainerGrid.Width.Set(0, 1f);
            FilmContainerGrid.Height.Set(0, 1f);

            CloseButton = new SimpleCross();
            CloseButton.Left.Set(Width.Pixels - CloseButton.Width.Pixels, 0f);
            CloseButton.OnLeftClick += (_, _) => Close();


            FilmContainerGrid.Scrollbar.Height.Set(FilmContainerPanel.Height.Pixels - 16, 0);
            FilmContainerGrid.Scrollbar.Left.Set(FilmContainerPanel.Width.Pixels - FilmContainerGrid.Scrollbar.Width.Pixels - 18, 0);

            Append(Window);
            Window.Append(UsingFilmPanel);
            Window.Append(FilmContainerPanel);
            Window.Append(Title);
            Window.Append(CloseButton);
            UsingFilmPanel.Append(UsingFilmGrid);
            FilmContainerPanel.Append(FilmContainerGrid);

            base.OnInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            OpenTimer.Update(2f);

            base.Update(gameTime);

            //Main.NewText($"{Window.CanDrag}");

            Window.BorderColor = Color.Black * OpenTimer.Progress;
            Window.BackgroundColor = new Color(29, 33, 70) * 0.7f * OpenTimer.Progress;
            Window.Height.Set(460, 0);
            Window.Width.Set(580,0);

            UsingFilmPanel.BorderColor = Color.Black * OpenTimer.Progress;
            UsingFilmPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f * OpenTimer.Progress;
            UsingFilmPanel.Width.Set(Window.Width.Pixels - 12, 0);
            

            FilmContainerPanel.BorderColor = Color.Black * OpenTimer.Progress;
            FilmContainerPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f * OpenTimer.Progress;
            FilmContainerPanel.Height.Set(460 - 130,0);
            FilmContainerPanel.Width.Set(Window.Width.Pixels - 12, 0);

            Title.TextColor = Color.White * OpenTimer.Progress;
            Title.ShadowColor = Color.Black * OpenTimer.Progress; 
            var vector = FontAssets.MouseText.Value.MeasureString(Title.Text);
            Title.Height.Set(vector.Y, 0);
            Title.Width.Set(vector.X, 0f);
            Title._textScale = 0.5f;
            Title.PaddingLeft = 0f;
            Title.Top.Set(12, 0);
            Title.Left.Set(6, 0);

            UsingFilmGrid.Opacity = OpenTimer.Progress;
            UsingFilmGrid.PaddingBottom = 0f;

            FilmContainerGrid.Opacity = OpenTimer.Progress;
            FilmContainerGrid.PaddingBottom = 0f;

            CloseButton.Width.Set(42, 0);
            CloseButton.Height.Set(42, 0);
            CloseButton.Left.Set(Window.Width.Pixels - CloseButton.Width.Pixels, 0f);
            CloseButton.Opacity = OpenTimer.Progress;

            if (Window.IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("AyaMod: FilmContainer");

            Recalculate();
        }
        public void Open(int slot = 0)
        {
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            UsingFilmGrid.Open(Main.LocalPlayer.GetModPlayer<DataPlayer>().UsingFilm, slot);
            FilmContainerGrid.Open(Main.LocalPlayer.GetModPlayer<DataPlayer>().FilmVault);
        }

        public void Close()
        {
            Visible = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex((layer) => layer.Name == "Vanilla: Inventory");
    }
}
