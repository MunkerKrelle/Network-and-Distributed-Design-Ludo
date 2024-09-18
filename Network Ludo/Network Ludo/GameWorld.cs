using ComponentPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

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

        public static MouseState mouseState;
        public static MouseState newState;
        public static bool isPressed;

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
            foreach (GameObject go in gameObjects)
            {
                go.Awake();
            }
            
            _graphics.PreferredBackBufferWidth = 11 * 100 + 200;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 11 * 100 + 1;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

            Server.Instance.server.Start();

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
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeElapsed += DeltaTime;
            mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                isPressed = true;
            }
            else
            {
                isPressed = false;
            }

            //while (true)
            //{
                TcpClient client = Server.Instance.server.AcceptTcpClient();
                Thread clientThread = new Thread(() => Server.Instance.HandleClient(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            //}

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

            foreach (GameObject go in gameObjects)
            {
                go.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
