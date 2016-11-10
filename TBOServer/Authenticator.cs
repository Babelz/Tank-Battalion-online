using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TBOLib;
using TBOLib.Packets;

namespace TBOServer
{
    public sealed class Authenticator
    {
        #region State helper struct
        private struct AuthenticationState
        {
            public TcpClient    client;
            public byte[]       buffer;
            public string       auth;
        }
        #endregion

        #region Events
        public event AuthenticatorEventHandler AuthenticationFailed;
        public event AuthenticatorEventHandler AuthenticationSuccess;
        #endregion

        public Authenticator()
        {
        }
        
        private void BeginSendCallback(IAsyncResult results)
        {
            var state = (AuthenticationState)results.AsyncState;

            state.client.Client.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, BeginReceiveCallback, state);
        }
        private void BeginReceiveCallback(IAsyncResult results)
        {
            var state     = (AuthenticationState)results.AsyncState;
            var packet    = PacketSerializer.Deserialize(state.buffer)[0];
            var contents  = ((AuthenticationPacket)packet).contents;

            if (contents == state.auth) AuthenticationSuccess?.Invoke(state.client);
            else                        AuthenticationFailed?.Invoke(state.client);
        }

        public void Authenticate(TcpClient client)
        {
            var packet = new AuthenticationPacket(string.Format("VERSION: {0} | TIME: {1}", Configuration.Version, DateTime.Now));

            var buffer = PacketSerializer.Serialize(packet);

            client.Client.BeginSend(buffer, 
                                    0, 
                                    buffer.Length, 
                                    SocketFlags.None, 
                                    BeginSendCallback, 
                                    new AuthenticationState()
                                    {
                                        client  = client,
                                        buffer  = new byte[4096],
                                        auth    = packet.contents
                                    });
        }

        public delegate void AuthenticatorEventHandler(TcpClient client);
    }
}
