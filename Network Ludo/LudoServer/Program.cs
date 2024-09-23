using LudoServer;
using System;

namespace LudoServer
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            ServerGameWorld.Instance.Run();
        }
    }
}
