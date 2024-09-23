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
using System.Drawing;

namespace LudoServer
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

        private Dictionary<Guid, ClientInfo> idToClientInfo = new Dictionary<Guid, ClientInfo>();

        public List <Color> availableColors = new List<Color> { Color.White, Color.Black, Color.Red, Color.Purple, Color.PaleGreen, Color.Yellow, Color.Orange, Color.Pink };


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

                    byte messageType = reader.ReadByte();
                    MessageType recievedType = (MessageType)messageType;

                    byte[] payLoadAsBytes = reader.ReadBytes(messageLength);

                    switch (recievedType)
                    {
                        case MessageType.Join:
                            JoinMessage joinMsg = MessagePackSerializer.Deserialize<JoinMessage>(payLoadAsBytes);
                            idToClientInfo.Add(clientId, new ClientInfo { name = joinMsg.name, playerColor = Color.AliceBlue, writer = writer });
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

        public Color CheckColors(Color chosenColor)
        {
            foreach (Color c in availableColors)
                if (c == chosenColor)
                {
                    availableColors.Remove(c);

                    return chosenColor;
                }
            return Color.Empty;
        }
    }
}

