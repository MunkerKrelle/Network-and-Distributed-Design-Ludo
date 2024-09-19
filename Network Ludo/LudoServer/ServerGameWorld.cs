using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace LudoServer
{
    public class ServerGameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public GraphicsDeviceManager Graphics { get => _graphics; set => _graphics = value; }

        private static ServerGameWorld instance;

        public static ServerGameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerGameWorld();
                }
                return instance;
            }
        }

        public ServerGameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Thread ini = new Thread(Server.Instance.server.Start);
            ini.IsBackground = true;
            ini.Start();

            ThreadForWaitingForClient();

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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void WhileLoopThread()
        {
            Thread.Sleep(1000);
            while (true)
            {
                TcpClient client = Server.Instance.server.AcceptTcpClient();
                Thread clientThread = new Thread(() => Server.Instance.HandleClient(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }

        private void ThreadForWaitingForClient()
        {
            Thread test = new Thread(WhileLoopThread);
            test.IsBackground = true;
            test.Start();
        }
    }
}
