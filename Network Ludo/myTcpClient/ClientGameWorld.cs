using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Network_Ludo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace myTcpClient
{
    public class ClientGameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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
            client.GetMeGoing();
            client.RunOnce();
           
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
                client.SendMessage(client.writer, new RollDiceMessage { rollRequest = myDiceRollString });

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
                        client.SendMessage(client.writer, new ChatMessage { message = client.letters});
                    }
                }
            }
            previousKeyState = keyState;

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            

            base.Draw(gameTime);
        }
    }
}
