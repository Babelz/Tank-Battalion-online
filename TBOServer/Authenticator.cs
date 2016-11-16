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
        #region Constant fields
        private const int AuthenticationReceiveTimeout = 5000;
        #endregion

        #region State helper struct
        private struct AuthenticationState
        {
            public TcpClient            client;
            public byte[]               buffer;
            public AuthenticationPacket auth;
            public Guid                 guid;
        }
        #endregion

        #region Events
        public event AuthenticationFailedEventHandler AuthenticationFailed;
        public event AuthenticationSuccessEventHandler AuthenticationSuccess;
        #endregion

        public Authenticator()
        {
        }
        
        private void BeginSendCallback(IAsyncResult results)
        {
            var state = (AuthenticationState)results.AsyncState;

            state.client.ReceiveTimeout = AuthenticationReceiveTimeout;

            var asyncResult = state.client.Client.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, BeginReceiveCallback, state);
            var success     = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(AuthenticationReceiveTimeout));

            if (!success)
            {
                AuthenticationFailed?.Invoke(state.client);

                state.client.Close();
            }
            else
            {
                state.client.ReceiveTimeout = 0;
            }
        }
        private void BeginReceiveCallback(IAsyncResult results)
        {
            var state     = (AuthenticationState)results.AsyncState;

            // Timeout, socket has been closed.
            if (state.client.Client == null) return;

            var packet    = (AuthenticationPacket)PacketSerializer.Deserialize(state.buffer)[0];
            var auth      = state.auth;

            if (packet.version == auth.version &&
                packet.time == auth.time &&
                !string.IsNullOrEmpty(packet.response) &&
                Guid.Parse(packet.guid) == state.guid)
            {
                AuthenticationSuccess?.Invoke(state.client, packet);
            }
            else
            {
                AuthenticationFailed?.Invoke(state.client);
            }
        }

        public void Authenticate(TcpClient client)
        {
            var guid   = Guid.NewGuid();
            var packet = new AuthenticationPacket(Configuration.Version, DateTime.Now.ToString(), guid.ToString());

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
                                        auth    = packet,
                                        guid    = guid
                                    });
        }

        public delegate void AuthenticationFailedEventHandler(TcpClient client);
        public delegate void AuthenticationSuccessEventHandler(TcpClient client, AuthenticationPacket packet);
    }
}
