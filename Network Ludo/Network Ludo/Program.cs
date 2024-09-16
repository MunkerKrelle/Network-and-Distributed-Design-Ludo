using Network_Ludo;
using System;

public static class Program
{
    [STAThread]
    static void Main()
    {
        GameWorld.Instance.Run();
    }
}
