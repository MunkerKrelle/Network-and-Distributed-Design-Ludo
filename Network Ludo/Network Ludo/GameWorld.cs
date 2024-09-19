using ComponentPattern;
using FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Network_Ludo
{
    enum GameState
    {
        Player1,
        Player2,
        Player3,
        Player4
    }

    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;

        private List<GameObject> gameObjects = new List<GameObject>();

        private List<GameObject> newGameObjects = new List<GameObject>();

        private List<GameObject> destroyedGameObjects = new List<GameObject>();
        public float DeltaTime { get; private set; }
        public GraphicsDeviceManager Graphics { get => _graphics; set => _graphics = value; }

        private static List<Button> buttons = new List<Button>();
        private GameObject specificButton;

        public List<Player> playerList = new List<Player>();

        public static MouseState mouseState;
        public static MouseState newState;
        public static bool isPressed;
        public KeyboardState keyState;
        public KeyboardState previousKeyState;
        public static string inputText = string.Empty;
        Vector2[] corners = new Vector2[] { new Vector2(170, 20), new Vector2(1000, 20), new Vector2(170, 1000), new Vector2(1000, 1000) };


        Color[] colors = new Color[] { Color.AntiqueWhite, Color.Black, Color.Red, Color.Purple, Color.PaleGreen, Color.Yellow, Color.Orange, Color.Pink };
        List<GameObject> colorButtons = new List<GameObject>();

        private float timeElapsed;

        public static SpriteFont font;

        public List<GameObject> GameObjects
        {
            get
            {
                return gameObjects;
            }
        }
        private static GameWorld instance;

        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        public GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public Dictionary<string, Texture2D> sprites { get; private set; } = new Dictionary<string, Texture2D>();

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 11 * 100 + 200;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 11 * 100 + 1;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

            CreateColorBox();

            foreach (GameObject go in gameObjects)
            {
                go.Awake();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (GameObject go in gameObjects)
            {
                go.Start();
            }
            font = Content.Load<SpriteFont>("textType");

            //CreateColorBox();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeElapsed += DeltaTime;

            mouseState = Mouse.GetState();
            WriteText();

            if (mouseState.LeftButton == ButtonState.Pressed && timeElapsed>1)
            {
                isPressed = true;
            }
            else
            {
                isPressed = false;
            }

            foreach (GameObject go in gameObjects)
            {
                go.Update(gameTime);
            }

            base.Update(gameTime);

            Cleanup();
        }

        private void Cleanup()
        {
            // Adding newly instantiated GameObjects
            for (int i = 0; i < newGameObjects.Count; i++)
            {
                gameObjects.Add(newGameObjects[i]);
                newGameObjects[i].Awake(); // Initializing new GameObjects
                newGameObjects[i].Start(); // Starting new GameObjects
            }

            // Removing destroyed GameObjects
            for (int i = 0; i < destroyedGameObjects.Count; i++)
            {
                gameObjects.Remove(destroyedGameObjects[i]);
            }
            destroyedGameObjects.Clear(); // Clearing destroyed GameObjects list
            newGameObjects.Clear(); // Clearing new GameObjects list
        }

        public void Instantiate(GameObject go)
        {
            newGameObjects.Add(go);
        }

        public void Destroy(GameObject go)
        {
            destroyedGameObjects.Add(go);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack);

            _spriteBatch.DrawString(font, inputText, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), Color.Black, 0, Origin(inputText), 1, SpriteEffects.None, 1f);

            foreach (GameObject go in gameObjects)
            {
                go.Draw(_spriteBatch);
            }

            for (int i = 0; i < playerList.Count; i++)
            {
                _spriteBatch.DrawString(font, playerList[i].playerName, corners[i], playerList[i].color, 0, Origin(playerList[i].playerName), 1, SpriteEffects.None, 1f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void WriteText()
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Back) && previousKeyState.IsKeyUp(Keys.Back))
            {
                if (inputText.Length > 0)
                    inputText = inputText[..^1]; // Remove last character
            }

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (keyState.IsKeyDown(key) && previousKeyState.IsKeyUp(key))
                {
                    // Check if the key is a character key
                    if (key >= Keys.A && key <= Keys.Z)
                    {
                        inputText += key.ToString();
                    }
                    else if (key >= Keys.D0 && key <= Keys.D9)
                    {
                        inputText += (key - Keys.D0).ToString();
                    }
                    else if (key == Keys.Enter)
                    {
                        ShowColorBoxes();
                    }

                }
            }
            previousKeyState = keyState;
        }

        private Vector2 Origin(string input)
        {
            Vector2 fontLength = GameWorld.font.MeasureString(input);

            Vector2 originText = new Vector2(fontLength.X / 2f, fontLength.Y / 2f);
            return originText;
        }

        public void CreateColorBox()
        {
            for (int i = 0; i < 4; i++)
            {
                colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 200 + i * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[i]), colors[i]));
                colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2(_graphics.PreferredBackBufferWidth / 2 - 200 + i * 150, _graphics.PreferredBackBufferHeight / 2 - 100), "", () => JoinLudo(colors[colors.Length - 1 - i]), colors[colors.Length - 1 - i]));
            }

            foreach (var button in colorButtons)
            {
                gameObjects.Add(button);
            }
        }

        public void ShowColorBoxes()
        {
            inputText = string.Empty;

            foreach (var button in colorButtons)
            {
                button.IsActive = true;
            }
        }

        private void JoinLudo(Color chosenColor)
        {

            foreach (var button in colorButtons)
            {
                Destroy(button);
            }

            GameObject player = new GameObject();

            player.AddComponent<Player>(inputText, chosenColor, corners[playerList.Count]);
            newGameObjects.Add(player);
            playerList.Add(player.GetComponent<Player>() as Player);

        }
    }
}
