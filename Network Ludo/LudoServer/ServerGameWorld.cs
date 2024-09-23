using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;

namespace LudoServer
{

    public enum GameState { Player1, Player2, Player3, Player4 }

    public class ServerGameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public GraphicsDeviceManager Graphics { get => _graphics; set => _graphics = value; }

        private static ServerGameWorld instance;

        private List<string> messageList = new List<string>();

        List<ClientInfo> clients = new List<ClientInfo>();
        public static GameState TurnOrder;
        private List<Vector2> startPos = new List<Vector2>() {
            new Vector2(50, 50),
            new Vector2(50, 150),
            new Vector2(50, 250),
            new Vector2(50, 350) };

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

            GetPlayers();

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

            Vector2 starpoint = new Vector2(500,500);

            foreach (string message in messageList)
            {
                _spriteBatch.DrawString(Font, message, new Vector2 (starpoint.X, starpoint.Y+ 50));
            }

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

        private void GetPlayers()
        {        
            foreach (ClientInfo player in Server.Instance.idToClientInfo.Values)
            {
                clients.Add(player);
            }
        }

        public void CheckState(int roll)
        {
            switch (TurnOrder)
            {
                case GameState.Player1:
                    clients[0].position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player2;
                    break;
                case GameState.Player2:
                    clients[1].position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player3;
                    break;
                case GameState.Player3:
                    clients[2].position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player4;
                    break;
                case GameState.Player4:
                    clients[3].position += new Vector2((100 * roll), 0);
                    TurnOrder = GameState.Player1;
                    break;
                default:
                    break;
            }

            if (clients[0].position.X > 1300 || clients[0].position.X > 1300 || clients[0].position.X > 1300 || clients[0].position.X > 1300)
            {
                clients[0].position = startPos[0];
                clients[1].position = startPos[1];
                clients[2].position = startPos[2];
                clients[3].position = startPos[3];
                TurnOrder = GameState.Player1;
            }
        }
    }
}
