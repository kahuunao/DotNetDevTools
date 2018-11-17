using Fleck;
using System;

namespace DevToolsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            FleckLog.LogAction = (level, message, error) => Console.WriteLine($"{level} - {message} - {error}");

            var server = new WebSocketServer("ws://0.0.0.0:12000");
            server.Start(socket =>
            {
                socket.OnOpen = () => Console.WriteLine($"Open from : {socket.ConnectionInfo}");
                socket.OnClose = () => Console.WriteLine($"Close from : {socket.ConnectionInfo}");
                socket.OnError = (ex) => Console.WriteLine($"Error : {ex} from : {socket.ConnectionInfo}");
                socket.OnMessage = (message) => Console.WriteLine($"Receive message : {message} from : {socket.ConnectionInfo}");
                
            });
            Console.WriteLine("Enter to stop console.");
            Console.ReadLine();
            server.Dispose();
        }
    }
}
