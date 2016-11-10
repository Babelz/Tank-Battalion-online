using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TBOLib.Packets;

namespace TBOServer
{
    public sealed class Authenticator
    {
        #region Events
        public event AuthenticatorEventHandler AuthenticationFailed;
        public event AuthenticatorEventHandler AuthenticationSuccess;
        #endregion

        public Authenticator()
        {
        }
        
        private void BeginSendCallback(IAsyncResult results)
        {
        }

        public void Authenticate(TcpClient client)
        {
            var data    = string.Format("VERSION: {0} | TIME: {1}", Configuration.Version, DateTime.Now);

            var packet  = new AuthenticationPacket(data);

            var 

            client.Client.BeginSend(buffer, 0, buffer.Lenght, SocketFlags.None, BeginSendCallback, client);
        }

        public delegate void AuthenticatorEventHandler(TcpClient client);
    }
}
