using Network_Ludo;
using System;

namespace Network_Ludo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            GameWorld.Instance.Run();
        }
    }
}
