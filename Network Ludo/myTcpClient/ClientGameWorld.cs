using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Network_Ludo;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace myTcpClient
{
    public class ClientGameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static SpriteFont font;

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
        private Client client = new Client();
        public List<TcpClient> myClientsList = new List<TcpClient>();
        private string _stringValue = string.Empty;

        public static bool isPressed;
        public KeyboardState keyState;
        public KeyboardState previousKeyState;
        public string inputText = string.Empty;
        public string currentInputText = string.Empty;


        public ClientGameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 11 * 100 + 150;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 11 * 100 + 1;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

            client.GetMeGoing();
            client.RunOnce();
           
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("textType");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Enter) && client.isChatting != true)
            {
                client.isChatting = true;
            }

            if (client.isChatting == true) 
            {
                EnterMessage(keyState);
            }

            
            if (keyState.IsKeyDown(Keys.B))
            {
                //GameWorld.Instance.CheckState(4);
                //InputHandler.Instance.AddUpdateCommand(Keys.R, new RollCommand(die));
                int myDiceRoll = 5;
                string myDiceRollString = myDiceRoll.ToString();
                client.SendMessage(client.writer, new RollMessage { roll = myDiceRollString });

            }

            //client.MyMessages("i am so awesome");
            base.Update(gameTime);
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
        private Vector2 Origin(string input)
        {
            Vector2 fontLength = GameWorld.font.MeasureString(input);

            Vector2 originText = new Vector2(fontLength.X / 2f, fontLength.Y / 2f);
            return originText;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (ClientInfo chat in Server.Instance.idToClientInfo.Values)
            {
                _spriteBatch.DrawString(font,inputText, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), Color.Black, 0, Origin(inputText), 1, SpriteEffects.None, 1f);
            }

            base.Draw(gameTime);
        }
    }
}
