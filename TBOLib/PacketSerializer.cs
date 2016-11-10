using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TBOLib.Packets;

namespace TBOLib
{
    public sealed class PacketSerializer
    {
        public PacketSerializer()
        {
        }

        /*
                Packet structure:
                   
                [4-bytes count of packets][5-bytes * count of packets (type and size)][packet data]
         */
    
        public byte[] Serialize(IPacket[] packets)
        {
            // 4-byte header that contains the count of the packets.
            // 5-byte values that contain the type of the packet and the size of the packet after the header.
            // first packet location = 4-bytes + 4-bytes * count_of_packets.
            
            var count       = packets.Length;
            var sizes       = packets.Select(p => Marshal.SizeOf(p)).ToArray();
            var types       = packets.Select(p => (byte)p.Type).ToArray();
            
            var buffer      = new byte[sizeof(int) + (sizeof(int) + 1) * sizes.Length];
            var bufferIndex = 4;

            // Write header and packets information.
            Array.Copy(BitConverter.GetBytes(count), buffer, sizeof(int));
            
            for (int i = 0; i < sizes.Length; i++)
            {
                Array.Copy(types, i, buffer, bufferIndex, 1);
                bufferIndex++;

                Array.Copy(BitConverter.GetBytes(sizes[i]), 0, buffer, bufferIndex, sizeof(int));
                bufferIndex += 4;
            }

            var formatter = new BinaryFormatter();

            // Write packets.
            for (int i = 0; i < packets.Length; i++)
            {
                var bytes = formatter.Serialize()
            }
        }

        public List<IPacket> Deserialize(byte[] buffer)
        {
        }
    }
}
