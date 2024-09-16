using System.IO;
using System;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using MessagePack;


namespace myTcpClient
{
    public class Client
    {
        TcpClient client = new TcpClient();
        public int myTestInt;
        public bool test = false;
        //client.Connect("localhost", 12000);
        public BinaryWriter writer;
        //Console.WriteLine("Connected to server...");

        public void GetMeGoing()
        {
            ClientGameWorld.Instance.myClientsList.Add(client);
            myTestInt = 5;
            test = true;
            ClientGameWorld.Instance.myClientsList[0].Connect("localhost", 12000);

        }

        public void Repeat() 
        {
            //while (true)
            //{
            //    myTestInt++;
            //}
            BinaryWriter writer = new BinaryWriter(client.GetStream());
            while (true)
            {
                Console.WriteLine("Write a username...");
                //string userName = Console.ReadLine(); //original code
                string userName = "bob"; //my change
                userName.Replace(" ", "");
                if (userName.Length > 0)
                {
                    
                    //SendMessage(writer, new JoinMessage { name = userName });
                    break;
                }
                break; //this is to be removed later
            }

            // Start a thread to receive messages
            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.IsBackground = true;
            receiveThread.Start();

            // Input-and-send loop
            //while (true)
            //{
            //    string message = Console.ReadLine();
            //    if (message == "list")
            //    {
            //        //SendMessage(writer, new ListMessage());
            //    }
            //    else
            //    {
            //        //SendMessage(writer, new ChatMessage { message = message });
            //    }

            //}
        }

        public void MyMessages() 
        {
            string message = Console.ReadLine();
            if (message == "list")
            {
                //SendMessage(writer, new ListMessage());
            }
            else
            {
                //SendMessage(writer, new ChatMessage { message = message });
            }
        }

        void ReceiveMessages(TcpClient client)
        {
            BinaryReader reader = new BinaryReader(client.GetStream());
            while (client.Connected)
            {
                int messageLength = reader.ReadInt32();
                byte[] payLoadAsBytes = reader.ReadBytes(messageLength);
                string message = MessagePackSerializer.Deserialize<string>(payLoadAsBytes);

                if (message != null)
                {
                    Console.WriteLine(message);
                }
            }
        }

        //static void SendMessage(BinaryWriter writer, Message message)
        //{
        //    byte[] data = new byte[0];
            
        //    switch (message.type)
        //    {
        //        case MessageType.Join:
        //            data = MessagePackSerializer.Serialize((JoinMessage)message);
        //            break;
        //        case MessageType.Chat:
        //            data = MessagePackSerializer.Serialize((ChatMessage)message);
        //            break;
        //        case MessageType.List:
        //            data = MessagePackSerializer.Serialize((ListMessage)message);
        //            break;
        //        default:
        //            Console.WriteLine($"Unable to serialize type: " + message.type);
        //            break;
        //    }

        //    // Send the length of the message as 4-byte integer
        //    writer.Write(data.Length);
        //    // Send the type as a single byte
        //    writer.Write((byte)message.type);
        //    // Send rest of data
        //    writer.Write(data);

        //    writer.Flush();
        //}
    }
}
