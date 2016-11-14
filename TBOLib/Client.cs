using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TBOLib.Packets;

namespace TBOLib
{
    public sealed class Client
    {
        #region Fields
        private byte[] receiveBuffer;
        private byte[] sendBuffer; 

        private TcpClient connection;

        private string name;
        #endregion

        #region Events
        public event ClientReceiveEventHandler Received;

        public event ClientEventHandler Connected;
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return name;
            }
        }
        public IPEndPoint Address
        {
            get
            {
                return connection.Client.RemoteEndPoint as IPEndPoint;
            }
        }
        #endregion

        public Client(string name)
            : this(new TcpClient(), name)
        {
        }
        public Client(TcpClient connection, string name)
        {
            Debug.Assert(connection != null);

            this.connection = connection;
            this.name       = name;
        }

        private void ReserveBufferStorage(ref byte[] buffer, int requiredLength)
        {
            if (buffer == null)
            {
                buffer = new byte[requiredLength];

                return;
            }

            if (buffer.Length < requiredLength) Array.Resize(ref buffer, requiredLength);
        }

        private void BeginReceiveCallback(IAsyncResult results)
        {
            if (!IsConnected()) return;

            var bytes = connection.Available;

            ReserveBufferStorage(ref receiveBuffer, bytes);
            
            connection.Client.Receive(receiveBuffer, 4, bytes, SocketFlags.None);

            var packets = PacketSerializer.Deserialize(receiveBuffer);

            for (var i = 0; i < packets.Length; i++) Received(this, packets[i]);
            
            connection.Client.EndReceive(results);

            InternalBeginListen();
        }
        private void BeginSendCallback(IAsyncResult results)
        {
            if (!IsConnected()) return;

            connection.Client.EndSend(results);
        }

        private void InternalBeginListen()
        {
            ReserveBufferStorage(ref receiveBuffer, 4096);
            
            if (connection.Client == null || !connection.Client.Connected) return;

            if (!IsConnected()) return;

            connection.Client.BeginReceive(receiveBuffer, 0, 4, SocketFlags.None, BeginReceiveCallback , null);
        }
        
        public void Connect(string address, int port)
        {
            connection.BeginConnect(IPAddress.Parse(address), port, (state) =>
            {
                Connected?.Invoke(this);
            }, null);
        }

        public void ListenOnce()
        {
            if (!IsConnected()) return;

            if (Available())
            {
                ReserveBufferStorage(ref receiveBuffer, connection.Available);

                connection.Client.Receive(receiveBuffer, 0, connection.Available, SocketFlags.None);

                var packets = PacketSerializer.Deserialize(receiveBuffer);

                for (var i = 0; i < packets.Length; i++) Received(this, packets[i]);
            }
        }
        public void BeginListen()
        {
            InternalBeginListen();
        }

        /// <summary>
        /// Returns true if the client has some 
        /// data in the receive buffer.
        /// </summary>
        public bool Available()
        {
            return connection.Client.Available != 0;
        }

        public void Send(IPacket packet)
        {
            if (!IsConnected()) return;
            
            var bytes = Packet.GetSize(packet.Type) + Packet.HeaderSize + Packet.PacketTypeSize;

            ReserveBufferStorage(ref sendBuffer, bytes);
            
            PacketSerializer.Serialize(packet, ref sendBuffer);
            
            var socket = connection.Client;

            if (socket == null || !socket.Connected) return;

            socket.Send(sendBuffer, 0, bytes, SocketFlags.None);
        }
        public void Send(params IPacket[] packets)
        {
            if (!IsConnected()) return;

            var bytes = packets.Sum(p => Packet.GetSize(p.Type)) + Packet.HeaderSize + Packet.PacketTypeSize * packets.Length;

            ReserveBufferStorage(ref sendBuffer, bytes);

            PacketSerializer.Serialize(packets, ref sendBuffer);

            var socket = connection.Client;

            if (socket == null || !socket.Connected) return;
            
            socket.Send(sendBuffer, 0, bytes, SocketFlags.None);
        }

        public bool IsConnected()
        {
            return connection.Client != null && (connection.Client.Connected || connection.Client.Available == 0);
        }

        public void Close()
        {
            connection.Close();
        }

        public delegate void ClientEventHandler(Client client);
        public delegate void ClientReceiveEventHandler(Client client, IPacket packet);
    }
}
