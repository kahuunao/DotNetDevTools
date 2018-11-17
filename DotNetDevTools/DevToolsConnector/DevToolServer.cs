using Fleck;

using NLog;

using System;
using System.Collections.Generic;

namespace DevToolsConnector
{
    public class DevToolServer : IDevToolServer
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        private WebSocketServer _server;
        List<DevSocket> _sockets = new List<DevSocket>();

        public void Init(IDevRequestHandler pHandler, int? pPort = null)
        {
            FleckLog.LogAction = (level, message, error) => Console.WriteLine($"{level} - {message} - {error}");

            _server = new WebSocketServer($"ws://0.0.0.0:{pPort ?? 12000}");
            _server.Start(socket =>
            {
                var s = new DevSocket(socket, pHandler);
                _sockets.Add(s);
                s.OnClosed += OnSocketClosedHandler;
            });
        }

        public void Close()
        {
            _server?.Dispose();
            _server = null;
        }

        private void OnSocketClosedHandler(object sender, EventArgs e)
        {
            if (sender is DevSocket d)
            {
                _sockets.Remove(d);
                d.OnClosed -= OnSocketClosedHandler;
            }
        }
    }
}
