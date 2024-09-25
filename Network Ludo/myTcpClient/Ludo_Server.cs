using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Ludo_Server
{
    public class Ludo_Server : Game
    {
        private static Ludo_Server instance;
        public static Ludo_Server Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Ludo_Server();
                }
                return instance;
            }
        }
        public float DeltaTime { get; private set; }
        private float timeElapsed;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Ludo_Server()
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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeElapsed += DeltaTime;
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
            Thread WaitForClient = new Thread(WhileLoopThread);
            WaitForClient.IsBackground = true;
            WaitForClient.Start();
        }

    }
}
