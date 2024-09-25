using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ludo_Server
{
    public class ClientInfo
    {
        public string name;
        public BinaryWriter writer;
        public string color;
        public Vector2 position;
    }
}
