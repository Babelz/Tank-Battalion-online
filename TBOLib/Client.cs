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

        public Client()
            : this(new TcpClient())
        {
        }
        public Client(TcpClient connection)
        {
            Debug.Assert(connection != null);

            this.connection = connection;
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

        public void Connect(string address, int port)
        {
            connection.BeginConnect(IPAddress.Parse(address), port, (state) =>
            {
                Connected?.Invoke(this);
            }, null);
        }

        public void Send(IPacket packet)
        {
            ReserveBufferStorage(ref sendBuffer, Packet.GetSize(packet.Type) + Packet.HeaderSize + Packet.PacketTypeSize);
            
            PacketSerializer.Serialize(packet, ref sendBuffer);

            var socket = connection.Client;

            socket.Send(sendBuffer);
        }
        public IPacket[] Receive()
        {
            if (!HasIncomingPackets()) return null;

            var socket = connection.Client;
            
            ReserveBufferStorage(ref receiveBuffer, socket.Available);

            socket.Receive(receiveBuffer);

            return PacketSerializer.Deserialize(receiveBuffer);
        }

        public bool HasIncomingPackets()
        {
            return connection.Client.Available != 0;
        }

        public delegate void ClientEventHandler(Client client);
    }
}
