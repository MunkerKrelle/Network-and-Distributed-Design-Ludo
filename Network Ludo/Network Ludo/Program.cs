using myClientTCP;
using System;

namespace myClientTCP
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            ClientGameWorld.Instance.Run();
        }
    }
}
