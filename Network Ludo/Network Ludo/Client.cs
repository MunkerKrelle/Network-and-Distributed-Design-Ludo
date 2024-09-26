using System.IO;
using System;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using MessagePack;
using Ludo_Server;
using System.Linq;



namespace myClientTCP
{
    public class Client
    {
        TcpClient client = new TcpClient();
        public int myTestInt;
        public bool test = true;
        //client.Connect("localhost", 12000);
        public BinaryWriter writer;
        //Console.WriteLine("Connected to server...");
        public bool isChatting = false;
        //private object locker = new object();
        public string letters;

        public void GetMeGoing()
        {
            //ClientGameWorld.Instance.myClientsList.Add(client);
            myTestInt = 5;
            test = true;
            //ClientGameWorld.Instance.myClientsList[0].Connect("localhost", 12000);
            //ClientGameWorld.Instance.myClientsList[0].Connect("10.131.67.156", 12000);
            //ClientGameWorld.Instance.myClientsList[0].Connect("192.168.87.116", 12000);
            //client.Connect("192.168.87.116", 12000); 10.131.66.102
            client.Connect("10.131.66.112", 12000);
        }
        public BinaryReader reader;
        public void RunOnce()
        {
            writer = new BinaryWriter(client.GetStream());
            reader = new BinaryReader(client.GetStream());

            while (test == true)
            {
                string userName = "bob"; //my change
                //RollMessage rollMes = new RollMessage();
                userName.Replace(" ", "");
                if (userName.Length > 0)
                {

                    SendMessage(writer, new JoinMessage { name = userName });
                    //SendMessage(writer, rollMes);
                    //Console.WriteLine(rollMes);
                    test = false;
                    break;
                }
                test = false;
                break; //this is to be removed later
            }

            // Start a thread to receive messages
            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        public void MyMessages(string message)
        {
            //string message = Console.ReadLine(); //old code
            //string message = "i am so awesome";
            if (message == "list")
            {
                SendMessage(writer, new ListMessage()); //this needs to be reintroduced when sending message.
            }
            else
            {
                SendMessage(writer, new ChatMessage { message = message });
                Console.WriteLine();
            }
        }

        void ReceiveMessages(TcpClient client)
        {
            Thread.Sleep(100);
            BinaryReader reader = new BinaryReader(client.GetStream());
            while (client.Connected)
            {
                try
                {
                    int messageLength = reader.ReadInt32();
                    byte messageType = reader.ReadByte();
                    byte[] payLoadAsBytes = reader.ReadBytes(messageLength);

                    MessageType recievedType = (MessageType)messageType;

                    switch (recievedType)
                    {
                        case MessageType.Chat:
                            ChatMessage chatMessage = MessagePackSerializer.Deserialize<ChatMessage>(payLoadAsBytes);
                            Console.WriteLine("Chat: " + chatMessage.message);

                            break;

                        case MessageType.List:
                            string clientList = MessagePackSerializer.Deserialize<string>(payLoadAsBytes);
                            Console.WriteLine("Client List: " + clientList);

                            break;

                        case MessageType.Roll:
                            string rollMessage = MessagePackSerializer.Deserialize<string>(payLoadAsBytes);
                            HandleRollMessage(rollMessage);
                            break;

                        default:
                            Console.WriteLine("Unknown message type received");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error receiving message: " + ex.Message);
                    break;
                }
            }
            if (!client.Connected)
            {
                Console.WriteLine("Client disconnected.");
            }
        }

        // Håndter Roll-beskeder specifikt
        void HandleRollMessage(string message)
        {
            if (message.StartsWith("CodeRoll"))
            {
                message = message.Remove(0, 8); // Fjern "CodeRoll" præfix
                if (int.TryParse(message, out int rollResult) && rollResult >= 1 && rollResult <= 6)
                {
                    ClientGameWorld.Instance.CheckState(rollResult);
                }
            }
        }


        public void SendMessage(BinaryWriter writer, Message message)
        {
            byte[] data = new byte[0];

            switch (message.type)
            {
                case MessageType.Join:
                    data = MessagePackSerializer.Serialize((JoinMessage)message);
                    break;
                case MessageType.Chat:
                    data = MessagePackSerializer.Serialize((ChatMessage)message);
                    break;
                case MessageType.List:
                    data = MessagePackSerializer.Serialize((ListMessage)message);
                    break;
                case MessageType.Roll:
                    data = MessagePackSerializer.Serialize((RollMessage)message);
                    break;
                default:
                    Console.WriteLine($"Unable to serialize type: " + message.type);
                    break;
            }


            writer.Write(data.Length);

            writer.Write((byte)message.type);

            writer.Write(data);

            writer.Flush();
        }

    }
}
