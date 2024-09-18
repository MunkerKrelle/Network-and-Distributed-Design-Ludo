using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

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
                Console.WriteLine("key was pressed");
                client.isChatting = true;
                client.MyMessages("i am so awesome");
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
