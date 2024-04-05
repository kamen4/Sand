using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Sand
{
  
    public enum Type { CollorSelected, CanvasClicked }
    public class Game1 : Game
    {

        private const int 
            W = 100, 
            H = 100, 
            btnH = 30, 
            spacing = 10,
            dawingRadius = 3;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _canvasTexture;
        private Texture2D _buttonTexture;

        private Color[] _colorDataCanvas;
        private Color[] buttons = { Color.Yellow, Color.Blue, Color.Red, Color.Transparent };
        private Color selectedColor = Color.Yellow;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 120d);
        }

        protected override void Initialize()
        {
            _canvasTexture = new(GraphicsDevice, W, H);
            _colorDataCanvas = new Color[W * H];
            Array.Fill(_colorDataCanvas, Color.Transparent);
            _canvasTexture.SetData(_colorDataCanvas);

            _buttonTexture = new(GraphicsDevice, 1, 1);
            _buttonTexture.SetData(new Color[] {Color.White});

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = W;
            _graphics.PreferredBackBufferHeight = H + btnH;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton.HasFlag(ButtonState.Pressed))
            {
                Type? response = ButtonsHandle(mouseState.Position);
                if (response == Type.CanvasClicked)
                {
                    for (int i = -dawingRadius; i < dawingRadius; i++)
                        for (int j = -dawingRadius; j <= dawingRadius; j++)
                            if (mouseState.Y + i < H && mouseState.Y - i >= 0 && mouseState.Y + i > 0 && mouseState.X - j >= 0 && mouseState.X + j < W)
                                _colorDataCanvas[(mouseState.Y + i) * W + mouseState.X + j] = selectedColor;
                }
            }

            for (int i = H; i >= 0; --i)
            {
                int j = -1;
                if ((i & 1) == 0) j = W;
                while ((i & 1) == 0 && j >= 0 || (i & 1) == 1 && j < W)
                {
                    if ((i & 1) == 0) --j; else ++j;

                    if (At(j, i) == Color.Yellow)
                    {
                        if (At(j, i + 1) != Color.Yellow)
                        {
                            Swap(j, i, j, i + 1);

                        }
                        else if (At(j, i + 1) == Color.Yellow)
                        {
                            if (At(j + 1, i + 1) != Color.Yellow)
                                Swap(j, i, j + 1, i + 1);
                            else if (At(j - 1, i + 1) != Color.Yellow)
                                Swap(j, i, j - 1, i + 1);

                        }
                    }

                    if (At(j, i) == Color.Blue)
                    {
                        if (At(j, i + 1) == Color.Transparent)
                        {
                            Swap(j, i, j, i + 1);
                        }

                        if (At(j + 1, i) == Color.Transparent)
                        {
                            Swap(j, i, j + 1, i);
                        }
                        else if (At(j - 1, i) == Color.Transparent)
                        {
                            Swap(j, i, j - 1, i);
                        }
                    }
                }
            }
            _canvasTexture.SetData(_colorDataCanvas);

            base.Update(gameTime);
        }

        Color? At(int x, int y)
        {
            if (x < 0 || y < 0 || x >= W || y >= H)
                return null;
            return _colorDataCanvas[x + y * W]; 
        }
        void Swap(int x1, int y1, int x2, int y2)
        {
            if (x1 < 0 || y1 < 0 || x1 >= W || y1 >= H &&
                x2 < 0 || y2 < 0 || x2 >= W || y2 >= H)
                return;
            (_colorDataCanvas[x1 + y1 * W], _colorDataCanvas[x2 + y2 * W]) =
                (_colorDataCanvas[x2 + y2 * W], _colorDataCanvas[x1 + y1 * W]);
        }
        private Type? ButtonsHandle(Point position)
        {
            if (position.Y < H)
            {
                return Type.CanvasClicked;
            }
            for (int i = 0; i < buttons.Length; i++)
                if (position.X < W / buttons.Length * (i + 1))
                {
                    selectedColor = buttons[i];
                    return Type.CollorSelected;
                }
            return null;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_canvasTexture, new Rectangle(0, 0, W, H), Color.White);

            for (int i = 0; i < buttons.Length; i++)
            {
                _spriteBatch.Draw(_buttonTexture, new Rectangle(i * (W / buttons.Length) + spacing, H + spacing, W / buttons.Length - spacing*2, H - spacing*2), buttons[i]);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }


}