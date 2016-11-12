using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TBOLib.Packets;

namespace TBOLib
{
    public sealed class Client
    {
        #region Fields
        private TcpClient connection;
        #endregion

        public Client(TcpClient connection)
        {
            Debug.Assert(connection != null);

            this.connection = connection;
        }

        public void Send(IPacket packet)
        {
            var bytes = PacketSerializer.Serialize(packet);
        }

        public IPacket[] Receive()
        {
            if (HasIncomingPackets()) return null;
        }

        public bool HasIncomingPackets()
        {
            return connection.Client.Available != 0;
        }
    }
}
