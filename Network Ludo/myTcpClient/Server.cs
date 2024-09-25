using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using MessagePack;
using Microsoft.Xna.Framework;

namespace Ludo_Server
{
    /// <summary>
    /// The class Server upholds the responsibility of creating an instance of the server. This class also handles new clients and sends messages out to the clients.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Making this class a Singleton
        /// </summary>
        private static Server instance;
        public static Server Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Server();
                }
                return instance;
            }
        }

        private Dictionary<Guid, ClientInfo> idToClientInfo = new Dictionary<Guid, ClientInfo>();

        public TcpListener server = new TcpListener(IPAddress.Any, 12000);

        private List<ClientInfo> joinedPlayers = new List<ClientInfo>();
        
        //public int playersJoined;

        //private List<Vector2> startPos = new List<Vector2>() {
        //    new Vector2(50, 50),
        //    new Vector2(50, 150),
        //    new Vector2(50, 250),
        //    new Vector2(50, 350) };

        //server.Start();
        //Console.WriteLine("Server started... listening on port 12000");

        /// <summary>
        /// SendToClients sends messages to the clients containing data, for instance the value of a roll, or a new player joining the game
        /// These messages are sent as bytes, with the length of the message sent first, as a 4-byte integer
        /// </summary>
        /// <param name="message"></param>
        /// <param name="clients"></param>
        void SendToClients(string message, params ClientInfo[] clients)
    {
            byte[] data = MessagePackSerializer.Serialize(message);
            foreach (ClientInfo client in clients)
            {
                client.writer.Write(data.Length);
                client.writer.Write(data);
                client.writer.Flush();
            }
        }

        /// <summary>
        /// HandleClient is used to handling every type of client request, such as join, chat or roll.
        /// Requests from clients are deserialized and reacts to theses requests
        /// </summary>
        /// <param name="client"></param>
        public void HandleClient(TcpClient client)
    {
        Guid clientId = Guid.NewGuid();

        
        BinaryWriter writer = new BinaryWriter(client.GetStream());
        BinaryReader reader = new BinaryReader(client.GetStream());

            try
            {
                while (client.Connected)
                {
                    int messageLength = reader.ReadInt32();
                    Random myRandom = new Random();
                    int roll = 0;

                    byte messageType = reader.ReadByte();
                    MessageType recievedType = (MessageType)messageType;

                    byte[] payLoadAsBytes = reader.ReadBytes(messageLength);

                    switch (recievedType)
                    {
                        case MessageType.Join:
                            JoinMessage joinMsg = MessagePackSerializer.Deserialize<JoinMessage>(payLoadAsBytes);
                            ClientInfo newInfoClient = new ClientInfo { name = joinMsg.name, writer = writer };
                            idToClientInfo.Add(clientId, newInfoClient);
                            joinedPlayers.Add(newInfoClient);
                            string welcomeMsg = "New user joined!! Welcome: " + joinMsg.name;
                            Console.WriteLine(welcomeMsg);
                            SendToClients(welcomeMsg, idToClientInfo.Values.ToArray());
                            break;
                        case MessageType.Chat:
                            ChatMessage chatMsg = MessagePackSerializer.Deserialize<ChatMessage>(payLoadAsBytes);
                            string chatMsgWithName = idToClientInfo[clientId].name + ": " + chatMsg.message;
                            Console.WriteLine(chatMsgWithName);
                            SendToClients(chatMsgWithName, idToClientInfo.Values.ToArray());
                            break;
                        case MessageType.List:
                            string listOfClients = string.Join("\n", idToClientInfo.Values.Select(x => x.name));
                            SendToClients(listOfClients, idToClientInfo[clientId]);
                            break;
                        case MessageType.Roll:
                            //If a roll is requested, the int roll is set to a new random (Range 1-6) and this value is used to move the client's piece.
                            //Afterwards the value of the roll is sent to the clients
                            roll = myRandom.Next(1,7);
                            RollMessage rolledRequest = MessagePackSerializer.Deserialize<RollMessage>(payLoadAsBytes);
                            //string chatMsgWithName = idToClientInfo[clientId].name + ": " + chatMsg.message;
                            //Console.WriteLine(chatMsgWithName);
                            string rollMsg = ($"CodeRoll{roll}");
                            SendToClients(rollMsg, idToClientInfo.Values.ToArray());
                            //MovePieceForClients(roll, idToClientInfo.Values.ToArray());
                            //ClientGameWorld.Instance.CheckState(roll);
                            break;
                        case MessageType.Color:
                            ColorMessage createPiece = MessagePackSerializer.Deserialize<ColorMessage>(payLoadAsBytes);
                            idToClientInfo[clientId].color = createPiece.pieceColor;
                            //string rollMsg = ($"CodeRoll{roll}");
                            string colorMSG = ($"{idToClientInfo[clientId].name} is now {idToClientInfo[clientId].color}");
                            SendToClients(colorMSG, idToClientInfo.Values.ToArray());
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Console.WriteLine($"Client disconnected: {clientId}");
                //playersJoined--;
                client.Dispose();
            }

        }

}
}

