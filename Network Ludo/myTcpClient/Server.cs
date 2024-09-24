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

namespace Ludo_Server
{
    public class Server
    {
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

        Dictionary<Guid, ClientInfo> idToClientInfo = new Dictionary<Guid, ClientInfo>();

        public TcpListener server = new TcpListener(IPAddress.Any, 12000);
        //server.Start();
        //Console.WriteLine("Server started... listening on port 12000");

        void SendToClients(string message, params ClientInfo[] clients)
    {
            byte[] data = MessagePackSerializer.Serialize(message);
            foreach (ClientInfo client in clients)
            {
                //Send the length of the message as 4 - byte integer
                client.writer.Write(data.Length);
                client.writer.Write(data);
                client.writer.Flush();
            }
        }

        void MovePieceForClients(int message, params ClientInfo[] clients)
        {
            byte[] data = MessagePackSerializer.Serialize(message.ToString());
            foreach (ClientInfo client in clients)
            {
                //Send the length of the message as 4 - byte integer
                client.writer.Write(data.Length);
                client.writer.Write(data);
                client.writer.Flush();
            }
        }

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
                            idToClientInfo.Add(clientId, new ClientInfo { name = joinMsg.name, writer = writer });
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
                            roll = myRandom.Next(1,7);
                            RollMessage rolledRequest = MessagePackSerializer.Deserialize<RollMessage>(payLoadAsBytes);
                            //string chatMsgWithName = idToClientInfo[clientId].name + ": " + chatMsg.message;
                            //Console.WriteLine(chatMsgWithName);
                            //if (Int32.Parse(rolledRequest.roll) <= 6 && Int32.Parse(rolledRequest.roll) >= 1)
                            //{
                            string rollMsg = ($"CodeRoll{roll}");
                            SendToClients(rollMsg, idToClientInfo.Values.ToArray());
                            //MovePieceForClients(roll, idToClientInfo.Values.ToArray());
                            //ClientGameWorld.Instance.CheckState(roll);



                            //}

                            break;
                        //case MessageType.Roll:
                        //    RollMessage rollMsg = MessagePackSerializer.Deserialize<RollMessage>(payLoadAsBytes);
                        //    roll = GameWorld.Instance.CheckState(roll);

                        //    break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred for client {clientId}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Client disconnected: {clientId}");

                client.Dispose();
            }

        }

}
}

