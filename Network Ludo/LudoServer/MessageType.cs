using MessagePack;

namespace LudoServer
{
    public enum MessageType { Join = 0, Chat = 1, List = 2 }
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
}
