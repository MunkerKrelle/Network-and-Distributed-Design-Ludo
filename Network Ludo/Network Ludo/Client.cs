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
    /// <summary>
    /// This class is used to connect to the game server as well as instantiating the necessary components and methods for the client to send and recieve messages
    /// </summary>
    public class Client
    {
        TcpClient client = new TcpClient();
        public BinaryWriter writer;
        public bool isChatting = false;
        public string letters;

        /// <summary>
        /// The client connects to the correct IP, thus joining the game
        /// </summary>
        public void GetMeGoing()
        {
            //ClientGameWorld.Instance.myClientsList.Add(client);
            //ClientGameWorld.Instance.myClientsList[0].Connect("localhost", 12000);
            //ClientGameWorld.Instance.myClientsList[0].Connect("10.131.67.156", 12000);
            //ClientGameWorld.Instance.myClientsList[0].Connect("192.168.87.116", 12000);
            //client.Connect("192.168.87.116", 12000); 10.131.66.102
            client.Connect("localhost", 12000);
        }

        public void RunOnce(string userName)
        {
            writer = new BinaryWriter(client.GetStream());
            userName.Replace(" ", "");
            if (userName.Length > 0)
            {

                SendMessage(writer, new JoinMessage { name = userName });
                //SendMessage(writer, rollMes);
                //Console.WriteLine(rollMes);

                //break;
            }
            // Start a thread to receive messages
            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        /// <summary>
        /// Recieve serialized messages from the server, which is to be deserialized.
        /// </summary>
        /// <param name="client"></param>
        void ReceiveMessages(TcpClient client)
        {
            Thread.Sleep(100);
            BinaryReader reader = new BinaryReader(client.GetStream());
            while (client.Connected)
            {
                int messageLength = reader.ReadInt32();
                byte[] payLoadAsBytes = reader.ReadBytes(messageLength);
                string message = MessagePackSerializer.Deserialize<string>(payLoadAsBytes);
                if (message == "CodeRoll1" || message == "CodeRoll2" || message == "CodeRoll3" || message == "CodeRoll4" || message == "CodeRoll5" || message == "CodeRoll6")
                {
                    message = message.Remove(0, 8);
                    if (Int32.Parse(message) >= 1 && Int32.Parse(message) <= 6)
                    {
                        int returnedRoll = Int32.Parse(message);
                        ClientGameWorld.Instance.CheckState(returnedRoll);
                    }
                }
                else
                {
                    if (message.Contains("is now"))
                    {
                        string pieceString = message.Substring(message.Length - 1, 1);

                        int pieceNumber = int.Parse(pieceString);
                        string colorString = getBetween(message, "now ", "Player");
                        Color color = ConvertRgbStringToColor(colorString);
                        ClientGameWorld.Instance.pieceList[pieceNumber].Transform.Color = color;
                    }
                    else
                    {
                        ClientGameWorld.Instance.chatBox.Add(message);
                    }

                }
                if (client.Connected == false)
                {
                    Console.WriteLine("no more client");
                }
            }
        }

        /// <summary>
        /// Sends messages with requests to the server as a byte array of data, containing the request and the nessacary requirements 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
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
                case MessageType.Color:
                    data = MessagePackSerializer.Serialize((ColorMessage)message);
                    break;
                default:
                    Console.WriteLine($"Unable to serialize type: " + message.type);
                    break;
            }

            //Send the length of the message as 4 - byte integer
            writer.Write(data.Length);
            // Send the type as a single byte
            writer.Write((byte)message.type);
            // Send rest of data
            writer.Write(data);

            writer.Flush();
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        public static Color ConvertRgbStringToColor(string rgbString)
        {
            // Handle RGB format "R, G, B" or "R G B"
            var parts = rgbString.Split(new char[] { ',', ' ', ':', 'R', 'G', 'B', 'A', '}', '{' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4)
            {
                int r = int.Parse(parts[0].Trim());
                int g = int.Parse(parts[1].Trim());
                int b = int.Parse(parts[2].Trim());
                int t = int.Parse(parts[3].Trim());
                return new Color(r, g, b, t);
            }

            throw new FormatException("Invalid RGB format.");
        }
    }
}

