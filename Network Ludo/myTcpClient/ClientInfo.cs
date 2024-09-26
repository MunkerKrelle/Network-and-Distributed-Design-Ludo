using System.IO;
using System.Numerics;

namespace Ludo_Server
{
    /// <summary>
    /// Client info for the server to store
    /// </summary>
    public class ClientInfo
    {
        public string name;
        public BinaryWriter writer;
        public string color;
        public Vector2 position;
        public int index;
    }
}
