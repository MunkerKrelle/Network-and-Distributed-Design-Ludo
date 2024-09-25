using ComponentPattern;
using FactoryPattern;
using CommandPattern;
using BuilderPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Ludo_Server;

namespace myClientTCP
{
    public enum GameState
    {
        Player1,
        Player2,
        Player3,
        Player4
    }

    public class ClientGameWorld : Game
    {
        private GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;

        private Client client = new Client();
        private string _stringValue = string.Empty;

        public static bool isPressed;
        public KeyboardState keyState;
        public KeyboardState previousKeyState;
        public string inputText = string.Empty;
        public string currentInputText = string.Empty;

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

        private Vector2 piece1StartPos = new Vector2(50, 50);
        private Vector2 piece2StartPos = new Vector2(50, 150);
        private Vector2 piece3StartPos = new Vector2(50, 250);
        private Vector2 piece4StartPos = new Vector2(50, 350);

        public List<Player> playerList = new List<Player>();

        public static MouseState mouseState;
        public static MouseState newState;

        private bool joinGame = false;

        Vector2[] corners = new Vector2[] { new Vector2(170, 20), new Vector2(1000, 20), new Vector2(170, 1000), new Vector2(1000, 1000) };


        Color[] colors = new Color[] { Color.White, Color.Black, Color.Red, Color.Purple, Color.PaleGreen, Color.Yellow, Color.Orange, Color.Pink };
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
        private static ClientGameWorld instance;

        public static ClientGameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientGameWorld();
                }
                return instance;
            }
        }

        public ClientGameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            client.GetMeGoing();
            client.RunOnce();

            _graphics.PreferredBackBufferWidth = 11 * 100 + 200;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 11 * 100 + 1;   // set this value to the desired height of your window
            _graphics.ApplyChanges();
            CreateColorBox();

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

            KeyboardState keyState = Keyboard.GetState();

            if (joinGame == true)
            {
                if (keyState.IsKeyDown(Keys.Enter) && client.isChatting != true)
                {
                    client.isChatting = true;
                }

                if (client.isChatting == true)
                {
                    EnterMessage(keyState);
                }
            }
            else
            {
                WriteText(keyState);
            }


            if (keyState.IsKeyDown(Keys.B))
            {
                int myDiceRoll = 5;
                string myDiceRollString = myDiceRoll.ToString();
                client.SendMessage(client.writer, new RollMessage
                {
                    roll = myDiceRollString
                });
            }

            foreach (GameObject go in gameObjects)
            {
                go.Update(gameTime);
            }
            base.Update(gameTime);
            Cleanup();
        }

        public void EnterMessage(KeyboardState keyState)
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
                        client.letters = inputText;
                        client.SendMessage(client.writer, new ChatMessage { message = client.letters });
                    }
                }
            }
            previousKeyState = keyState;

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

            if (joinGame == true)
                foreach (GameObject go in gameObjects)
                {
                    go.Draw(_spriteBatch);
                }
            else
            {
                foreach (GameObject button in gameObjects)
                {
                    if (button.GetComponent<Button>() != null)
                    {
                        button.Draw(_spriteBatch);
                    }
                }
            }

            for (int i = 0; i < playerList.Count; i++)
            {
                _spriteBatch.DrawString(font, playerList[i].playerName, corners[i], playerList[i].color, 0, Origin(playerList[i].playerName), 1, SpriteEffects.None, 1f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void WriteText(KeyboardState keyState)
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
                        currentInputText = inputText;
                        ShowColorBoxes();
                    }
                }
            }
            previousKeyState = keyState;
        }

        private Vector2 Origin(string input)
        {
            Vector2 fontLength = ClientGameWorld.font.MeasureString(input);

            Vector2 originText = new Vector2(fontLength.X / 2f, fontLength.Y / 2f);
            return originText;
        }

        public void CreateColorBox()
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 600 + í * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[i]), colors[i]));
            //    //colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2(_graphics.PreferredBackBufferWidth / 2 - 200 + i * 150, _graphics.PreferredBackBufferHeight / 2 - 100), "", () => JoinLudo(colors[colors.Length - 1 - i]), colors[colors.Length - 1 - i]));
            //}

            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 0 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[0]), colors[0]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 1 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[1]), colors[1]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 2 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[2]), colors[2]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 3 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[3]), colors[3]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 4 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[4]), colors[4]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 5 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[5]), colors[5]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 6 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[6]), colors[6]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 7 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => JoinLudo(colors[7]), colors[7]));


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

            player.AddComponent<Player>(currentInputText, chosenColor, corners[playerList.Count]);
            newGameObjects.Add(player);
            playerList.Add(player.GetComponent<Player>() as Player);
        }

        public void CheckState(int roll)
        {
            switch (TurnOrder)
            {
                case GameState.Player1:
                    piece1.Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player2;
                    break;
                case GameState.Player2:
                    piece2.Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player3;
                    break;
                case GameState.Player3:
                    piece3.Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player4;
                    break;
                case GameState.Player4:
                    piece4.Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player1;
                    break;
                default:
                    break;
            }

            if (piece1.Transform.Position.X > 1300 || piece2.Transform.Position.X > 1300 || piece3.Transform.Position.X > 1300 || piece4.Transform.Position.X > 1300)
            {
                piece1.Transform.Position = piece1StartPos;
                piece2.Transform.Position = piece2StartPos;
                piece3.Transform.Position = piece3StartPos;
                piece4.Transform.Position = piece4StartPos;
                TurnOrder = GameState.Player1;
            }
        }
    }
}
