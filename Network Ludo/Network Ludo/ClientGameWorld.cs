﻿using ComponentPattern;
using FactoryPattern;
using Ludo_Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace myClientTCP
{
    /// <summary>
    /// An enum used to switch turns between players
    /// </summary>
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
        private float timeElapsed;
        public GraphicsDeviceManager Graphics { get => _graphics; set => _graphics = value; }

        public static GameState TurnOrder;
        private int turn;

        private static List<Button> buttons = new List<Button>();

        public List<string> chatBox = new List<string>();

        public List<GameObject> pieceList = new List<GameObject>();

        private Vector2 piece1StartPos = new Vector2(50, 50);
        private Vector2 piece2StartPos = new Vector2(50, 150);
        private Vector2 piece3StartPos = new Vector2(50, 250);
        private Vector2 piece4StartPos = new Vector2(50, 350);

        public List<LudoPiece> playerList = new List<LudoPiece>();

        public static MouseState mouseState;
        public static MouseState newState;

        private bool joinGame = false;

        Vector2[] corners = new Vector2[] { new Vector2(170, 20), new Vector2(1000, 20), new Vector2(170, 1000), new Vector2(1000, 1000) };


        Color[] colors = new Color[] { Color.White, Color.Black, Color.Red, Color.Purple, Color.PaleGreen, Color.Yellow, Color.Orange, Color.Pink };
        List<GameObject> colorButtons = new List<GameObject>();

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

        /// <summary>
        /// Initializing the screen size as well as the game components
        /// </summary>
        protected override void Initialize()
        {
            client.GetMeGoing();

            _graphics.PreferredBackBufferWidth = 11 * 100 + 200;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 11 * 100 + 1;   // set this value to the desired height of your window
            _graphics.ApplyChanges();
            CreateColorBox();

            turn = 1;


            GameObject gridObject = new GameObject();
            Grid grid = gridObject.AddComponent<Grid>(4, 20, 100);

            Instantiate(gridObject);

            for (int i = 0; i < 4; i++)
            {
                GameObject ludoPiece = LudoPieceFactory.Instance.Create("", Color.Green);
                gameObjects.Add(ludoPiece);
                pieceList.Add(ludoPiece);

            }

            pieceList[0].Transform.Position = new Vector2(40, 50);
            pieceList[1].Transform.Position = new Vector2(40, 150);
            pieceList[2].Transform.Position = new Vector2(40, 250);
            pieceList[3].Transform.Position = new Vector2(40, 350);

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
        }

        /// <summary>
        /// Updating the game 60 times a second constantly checking for inputs and changes to the state of the game
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();

            mouseState = Mouse.GetState();

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeElapsed += DeltaTime;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                isPressed = true;
            }
            else
            {
                isPressed = false;
            }

            if (joinGame == true)
            {
                EnterMessage(keyState);
            }
            else
            {
                WriteText();
            }


            if (keyState.IsKeyDown(Keys.B) && timeElapsed >= 1)
            {
                int myDiceRoll = 5;
                string myDiceRollString = myDiceRoll.ToString();
                client.SendMessage(client.writer, new RollMessage
                {
                    roll = myDiceRollString
                });
                timeElapsed = 0;
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
                    else if (key == Keys.Space)
                    {
                        inputText += " ";
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

        /// <summary>
        /// Removes objects no longer used in the game. In this case the color boxes used to choose a color
        /// </summary>
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

            for (int i = 0; i < chatBox.Count; i++)
            {
                _spriteBatch.DrawString(font, chatBox[i], new Vector2(1000, 500 + 20 * i), Color.Black, 0, Origin(chatBox[i]), 1, SpriteEffects.None, 1f);
            }


            for (int i = 0; i < playerList.Count; i++)
            {
                _spriteBatch.DrawString(font, playerList[i].name, new Vector2(playerList[i].GameObject.Transform.Position.X, playerList[i].GameObject.Transform.Position.Y), playerList[i].color, 0, Origin(playerList[i].name), 1, SpriteEffects.None, 1f);
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
                        currentInputText = inputText;
                        ShowColorBoxes();
                        client.RunOnce(currentInputText);
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
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 0 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[0]), colors[0]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 1 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[1]), colors[1]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 2 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[2]), colors[2]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 3 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[3]), colors[3]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 4 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[4]), colors[4]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 5 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[5]), colors[5]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 6 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[6]), colors[6]));
            colorButtons.Add(ButtonFactory.Instance.CreateWithColor(new Vector2((_graphics.PreferredBackBufferWidth / 2) - 525 + 7 * 150, (_graphics.PreferredBackBufferHeight / 2) + 100), "", () => ChangeColor(colors[7]), colors[7]));


            foreach (var button in colorButtons)
            {
                gameObjects.Add(button);
            }
        }

        /// <summary>
        /// Method showing the boxes used to choose a color at the beginning of the game
        /// </summary>
        public void ShowColorBoxes()
        {
            inputText = string.Empty;

            foreach (var button in colorButtons)
            {
                button.IsActive = true;
            }
        }

        private void ChangeColor(Color chosenColor)
        {

            foreach (var button in colorButtons)
            {
                Destroy(button);
            }

            joinGame = true;

            client.SendMessage(client.writer, new ColorMessage { pieceColor = $"{chosenColor }" });
        }

        /// <summary>
        /// A method for switching turns between players as well as updating the pieces to their new position equalling the value of their roll
        /// </summary>
        /// <param name="roll"></param>
        public void CheckState(int roll)
        {
            switch (TurnOrder)
            {
                case GameState.Player1:
                    pieceList[0].Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player2;
                    break;
                case GameState.Player2:
                    pieceList[1].Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player3;
                    break;
                case GameState.Player3:
                    pieceList[2].Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player4;
                    break;
                case GameState.Player4:
                    pieceList[3].Transform.Position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player1;
                    break;
                default:
                    break;
            }

            //If a piece reaches the goal the pieces are reset to their starting position and the turn goes to Player1
            if (pieceList[0].Transform.Position.X > 1300 || pieceList[1].Transform.Position.X > 1300 || pieceList[2].Transform.Position.X > 1300 || pieceList[3].Transform.Position.X > 1300)
            {
                pieceList[0].Transform.Position = piece1StartPos;
                pieceList[1].Transform.Position = piece2StartPos;
                pieceList[2].Transform.Position = piece3StartPos;
                pieceList[3].Transform.Position = piece4StartPos;
                TurnOrder = GameState.Player1;
            }
        }
    }
}
