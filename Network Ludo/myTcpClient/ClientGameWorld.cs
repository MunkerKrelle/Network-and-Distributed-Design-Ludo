using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            if (keyState.IsKeyDown(Keys.B))
            {
                Console.WriteLine("key was pressed");
            }
            if (keyState.IsKeyDown(Keys.Enter))
            {
                client.isChatting = !client.isChatting;
            }
            EnterMessage(keyState);

            //client.MyMessages("i am so awesome");
            base.Update(gameTime);
        }

        public void EnterMessage(KeyboardState keyState) 
        {
            if (client.isChatting == true) 
            {
                var keys = keyState.GetPressedKeys();

                if (keys.Length > 0)
                {
                    var keyValue = keys[0].ToString();
                    if (keyValue != "Enter")
                    {
                        client.letters += keyValue; //needs a cooldown
                    }
                }
                //if (keyState.IsKeyDown(Keys.Home))
                //{
                //    client.isChatting = false;
                //}
            }
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
