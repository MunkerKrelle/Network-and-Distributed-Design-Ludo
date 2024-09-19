using ComponentPattern;
using FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Network_Ludo.BuilderPattern;
using Network_Ludo.CommandPattern;
using Network_Ludo.ComponentPattern;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Network_Ludo
{
    public enum GameState
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

        public static GameState TurnOrder;
        private int turn;

        private static List<Button> buttons = new List<Button>();

        GameObject piece1;
        GameObject piece2;
        GameObject piece3;
        GameObject piece4;


        public static MouseState mouseState;
        public static MouseState newState;
        public static bool isPressed;
        KeyboardState keyState;
        KeyboardState previousKeyState;
        string inputText = string.Empty;

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
            Director director = new Director(new DieBuilder());
            GameObject dieGo = director.Construct();
            Die die = dieGo.GetComponent<Die>() as Die;
            turn = 1;
            gameObjects.Add(dieGo);

            InputHandler.Instance.AddUpdateCommand(Keys.R, new RollCommand(die));

            GameObject gridObject = new GameObject();
            Grid grid = gridObject.AddComponent<Grid>(4, 20, 100);

            Instantiate(gridObject);

            piece1 = LudoPieceFactory.Instance.Create(Color.Blue, "Poul");
            piece2 = LudoPieceFactory.Instance.Create(Color.Green, "Frank");
            piece3 = LudoPieceFactory.Instance.Create(Color.Red, "Lars");
            piece4 = LudoPieceFactory.Instance.Create(Color.Yellow, "John");

            gameObjects.Add(piece1);
            gameObjects.Add(piece2);
            gameObjects.Add(piece3);
            gameObjects.Add(piece4);

            piece1.Transform.Position = new Vector2(40, 50);
            piece2.Transform.Position = new Vector2(40, 150);
            piece3.Transform.Position = new Vector2(40, 250);
            piece4.Transform.Position = new Vector2(40, 350);


            foreach (GameObject go in gameObjects)
            {
                go.Awake();
            }

            _graphics.PreferredBackBufferWidth = 11 * 100 + 200;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 10 * 100 + 1;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

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

            //JoinGame();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeElapsed += DeltaTime;

            mouseState = Mouse.GetState();
            WriteText();

            if (mouseState.LeftButton == ButtonState.Pressed)
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

            foreach (GameObject go in gameObjects)
            {
                go.Update(gameTime);
            }

            if (timeElapsed >= .3f)
            {
                InputHandler.Instance.Execute();
                timeElapsed = 0;
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

            _spriteBatch.DrawString(font, inputText, new Vector2(100, 100), Color.Black);

            foreach (GameObject go in gameObjects)
            {
                go.Draw(_spriteBatch);
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
                        JoinGame();
                    }

                }
            }
            previousKeyState = keyState;
        }

        public void JoinGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
                for (int i = 0; i < 4; i++)
                {
                    Instantiate(LudoPieceFactory.Instance.Create(Color.Red, inputText));
                }
            inputText = string.Empty;
        }

        public void CheckState(int roll)
        {
            switch (TurnOrder)
            {
                case GameState.Player1:
                    //MOVE
                    TurnOrder = GameState.Player2;
                    break;
                case GameState.Player2:
                    //MOVE
                    TurnOrder = GameState.Player3;
                    break;
                case GameState.Player3:
                    //MOVE
                    TurnOrder = GameState.Player4;
                    break;
                case GameState.Player4:
                    //MOVE
                    TurnOrder = GameState.Player1;
                    break;
                default:
                    break;       
            }
        }
    }
}
