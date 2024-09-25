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

            // Indlæs skrifttypen fra Content-mappen
            font = Content.Load<SpriteFont>("textType");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();

            // Check if chat mode is active
            if (client.isChatting == true)
            {
                EnterMessage(keyState);  // Håndter brugerens chatinput
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

            // Check for backspace to delete last character
            if (keyState.IsKeyDown(Keys.Back) && previousKeyState.IsKeyUp(Keys.Back))
            {
                if (inputText.Length > 0)
                    inputText = inputText[..^1]; // Fjern sidste tegn
            }

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (keyState.IsKeyDown(key) && previousKeyState.IsKeyUp(key))
                {
                    // Tjek for bogstavstaster
                    if (key >= Keys.A && key <= Keys.Z)
                    {
                        inputText += key.ToString();
                    }
                    // Tjek for tal
                    else if (key >= Keys.D0 && key <= Keys.D9)
                    {
                        inputText += (key - Keys.D0).ToString();
                    }
                    // Tjek for mellemrum
                    else if (key == Keys.Space)
                    {
                        inputText += " ";
                    }
                    // Når Enter er trykket, send beskeden
                    else if (key == Keys.Enter)
                    {
                        client.SendMessage(client.writer, new ChatMessage { message = inputText });
                        inputText = string.Empty;  // Nulstil input efter besked er sendt
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

            _spriteBatch.Begin();

            // Tegn den aktuelle indtastede besked
            if (!string.IsNullOrEmpty(inputText))
            {
                _spriteBatch.DrawString(font, inputText, new Vector2(10, _graphics.PreferredBackBufferHeight - 30), Color.Black);
            }

            // Tegn alle modtagne chatbeskeder (hvis du allerede gemmer dem)
            int yOffset = 10;
            foreach (var chat in Server.Instance.idToClientInfo.Values)
            {
                string chatMessage = chat.name + ": " + inputText;  // Tilføj din logik til at hente beskeder fra serveren
                _spriteBatch.DrawString(font, chatMessage, new Vector2(10, yOffset), Color.Black);
                yOffset += 20;  // Placer beskederne under hinanden
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
