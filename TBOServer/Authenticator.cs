using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TBOServer
{
    public sealed class Authenticator
    {
        #region Fields
        private readonly List<TcpClient> clients;
        #endregion

        #region Events

        #endregion

        public Authenticator()
        {
            clients = new List<TcpClient>();
        }

        public void Authenticate(TcpClient client)
        {
        }

        public delegate void AuthenticatorEventHandler(TcpClient client);
    }
}
