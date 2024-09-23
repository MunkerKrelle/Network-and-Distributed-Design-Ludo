using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Network_Ludo
{
    public enum MessageType { Join = 0, Chat = 1, List = 2, Roll = 3 }
    [MessagePackObject]
    public abstract class Message
    {
        [IgnoreMember]
        public abstract MessageType type { get; }
    }

    public class JoinMessage : Message
    {
        [IgnoreMember]
        public override MessageType type => MessageType.Join;

        [Key(0)]
        public string name;
    }

    public class ChatMessage : Message
    {
        [IgnoreMember]
        public override MessageType type => MessageType.Chat;

        [Key(0)]
        public string message;
    }

    public class ListMessage : Message
    {
        [IgnoreMember]
        public override MessageType type => MessageType.List;
    }

    public class RollMessage : Message
    {
        [Key(0)]
        public int roll;

        [IgnoreMember]
        public override MessageType type => MessageType.Roll;
    }
}
