using AyaMod.Common;
using AyaMod.Content.UI.Elements;
using AyaMod.Core.Prefabs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace AyaMod.Content.UI
{
    public class ItemContainerUI : BaseUIState
    {
        public static ItemContainerUI Instance { get; private set; }
        public ItemContainerUI() => Instance = this;

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
        public IItemContainer Container { get; private set; }

        public DragablePanel Window;
        public UIPanel InnerPanel;
        public ItemContainerGrid ItemContainerGrid = new() { ListPadding = 5f };
        public UIText Title;
        public SimpleCross CloseButton;

        public TweenTimer OpenTimer = new(0f, 1f);

        public override void OnInitialize()
        {
            OpenTimer.Finish(false);

            float BackWidth = 400;
            float BackHeight = 200;
            Vector2 offset = new(Main.screenWidth / 2f - BackWidth / 2f, Main.screenHeight / 2f - BackHeight / 2f);

            Window = new DragablePanel();
            Window.Left.Set(offset.X, 0);
            Window.Top.Set(offset.Y, 0);
            Window.Width.Set(BackWidth, 0);
            Window.Height.Set(BackHeight, 0);
            Window.PaddingLeft = Window.PaddingRight = Window.PaddingTop = Window.PaddingBottom = 0;
            Window.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            InnerPanel = new();
            InnerPanel.Width.Set(BackWidth - 12, 0);
            InnerPanel.Height.Set(BackHeight - 70, 0);
            InnerPanel.Left.Set(6, 0);
            InnerPanel.Top.Set(34, 0);
            InnerPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            Title = new UIText("", 1f, true)
            {
                PaddingLeft = 12f
            };
            Title.Height.Set(0, 1f);
            Title.Width.Set(200f, 0f);

            ItemContainerGrid.SetPadding(8f);
            ItemContainerGrid.PaddingTop = 0f;
            ItemContainerGrid.Width.Set(0, 1f);
            ItemContainerGrid.Height.Set(0, 1f);
            ItemContainerGrid.OnLeftMouseDown += (_, _) =>
            {
                if (!Main.mouseItem.IsAir && !Main.LocalPlayer.ItemAnimationActive && Container.CanIntoContainer(Main.mouseItem))
                    Container.ItemIntoContainer(Main.mouseItem);

            };

            CloseButton = new SimpleCross();
            CloseButton.Left.Set(Width.Pixels - CloseButton.Width.Pixels, 0f);
            CloseButton.OnLeftClick += (_, _) => Close();

            Append(Window);
            Window.Append(InnerPanel);
            Window.Append(CloseButton);
            Window.Append(Title);
            InnerPanel.Append(ItemContainerGrid);
            
            base.OnInitialize();
        }
        public override void Update(GameTime gameTime)
        {
            //OpenTimer = new(0f, 1f);
            OpenTimer.Update(2f);
            
            base.Update(gameTime);
            //RemoveAllChildren();

            float BackWidth = 560;
            float BackHeight = 300;
            Vector2 offset = new(Main.screenWidth / 2f - BackWidth / 2f, Main.screenHeight / 2f - BackHeight / 2f);

            float timerFactor = (MathF.Cos((float)(Main.timeForVisualEffects * 0.04f)) * 0.25f + 0.75f);
            Window.Width.Set(BackWidth, 0);
            Window.Height.Set(BackHeight, 0);
            Window.PaddingLeft = Window.PaddingRight = Window.PaddingTop = Window.PaddingBottom = 0;
            Window.BorderColor = Color.Black * OpenTimer.Progress;
            Window.BackgroundColor = new Color(29, 33, 70) * 0.7f * OpenTimer.Progress;

            InnerPanel.Width.Set(BackWidth - 12, 0);
            InnerPanel.Height.Set(BackHeight - 70, 0);
            InnerPanel.Left.Set(6, 0);
            InnerPanel.Top.Set(42, 0);
            InnerPanel.BorderColor = Color.Black * OpenTimer.Progress;
            InnerPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f * OpenTimer.Progress;

            //Title = new UIText("", 1f, true);
            Title.PaddingLeft = 0f;
            Title.PaddingTop = 10f;
            Title.Left.Set(6, 0);
            Title.Top.Set(6, 0);
            var vector = FontAssets.MouseText.Value.MeasureString(Title.Text);
            Title.Height.Set(vector.Y,0);
            Title.Width.Set(vector.X, 0f);
            Title._textScale = 0.5f;
            Title.TextColor = Color.White * OpenTimer.Progress;
            Title.ShadowColor = Color.Black * OpenTimer.Progress;

            ItemContainerGrid.SetPadding(0f);
            ItemContainerGrid.PaddingTop = 0f;
            ItemContainerGrid.Width.Set(600, 0);
            ItemContainerGrid.Height.Set(600, 0f);
            ItemContainerGrid.ListPadding = 0f;
            ItemContainerGrid.Opacity = OpenTimer.Progress;

            CloseButton.Left.Set(Window.Width.Pixels - CloseButton.Width.Pixels, 0f);
            CloseButton.Top.Set(0, 0f);
            CloseButton.Width.Set(42, 0);
            CloseButton.Height.Set(42, 0);
            CloseButton.Opacity = OpenTimer.Progress;

            if (Window.IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("AyaMod: ItemContainer");

            //if (!Main.mouseText)
            //{
            //    Main.mouseText = true;
            //    Main.instance.MouseText($"{OpenTimer.Progress}");
            //}
            //Main.NewText($"Progress:{OpenTimer.Progress} Playing:{OpenTimer.IsPlaying} Reversed:{OpenTimer.IsReversed} Completed:{OpenTimer.IsCompleted}");
            Recalculate();
        }

        public void Open(IItemContainer container)
        {
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            //ItemContainerGrid.Clear();
            ItemContainerGrid.SetInventory(container.ItemContainer, 40);
            Title._text = container.Name;
            Container = container;
            Recalculate();
            ItemContainerGrid.OverflowHidden = false;
            ItemContainerGrid.RecalculateChildren();
        }
        public void Close()
        {
            Visible = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex((layer) => layer.Name == "Vanilla: Inventory");
    }
}
