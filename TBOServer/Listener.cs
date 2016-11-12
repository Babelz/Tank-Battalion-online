using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TBOServer
{
    /// <summary>
    /// Listener that listens for incoming connections.
    /// </summary>
    public sealed class Listener
    {
        #region Fields
        private readonly TcpListener listener;
        #endregion

        #region Events
        public event ListenerConnectionEventHandler ClientConnectd;
        #endregion

        public Listener()
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Configuration.Port);
        }

        private void BeginAcceptCallback(IAsyncResult results)
        {
            var listener    = results.AsyncState as TcpListener;

            var client      = listener.EndAcceptTcpClient(results);

            ClientConnectd?.Invoke(client);
        }

        public void BeginListen()
        {
            listener.Start();

            listener.BeginAcceptSocket(BeginAcceptCallback, listener);
        }

        public delegate void ListenerConnectionEventHandler(TcpClient client);
    }
}
