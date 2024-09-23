using Microsoft.Xna.Framework;
using System.IO;

namespace LudoServer
{
    public class ClientInfo
    {
        public string name;
        public Color playerColor;
        public Vector2 position;
        public BinaryWriter writer;
    }
}
