using DevToolsConnector;
using Fleck;
using System;

namespace DevToolsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new DevToolServer();
            server.Init(new DevRequestHandler());
            Console.WriteLine("Enter to stop console.");
            Console.ReadLine();
            server.Close();
        }
    }
}
