using System;
using System.Collections.Generic;
using System.IO;
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
        #region Packet data helper struct
        private struct PacketData
        {
            public PacketType   packetType;
            public Type         type;
            public int          size;
        }
        #endregion

        public PacketSerializer()
        {
        }

        // Packet structure:
        // [4-bytes count of packets][5-bytes * count of packets (type and size)][packet data]
    
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
            
            for (var i = 0; i < sizes.Length; i++)
            {
                // Copy type (1-byte)
                Array.Copy(types, i, buffer, bufferIndex, 1);
                bufferIndex++;

                // Copy size (4-bytes)
                Array.Copy(BitConverter.GetBytes(sizes[i]), 0, buffer, bufferIndex, sizeof(int));
                bufferIndex += 4;
            }
            
            // Write packets.
            var formatter       = new BinaryFormatter();
            var packetBuffer    = new byte[sizes.Max()];

            var ptr             = Marshal.AllocHGlobal(packetBuffer.Length);

            for (var i = 0; i < packets.Length; i++)
            {
                var size = sizes[i];

                Marshal.StructureToPtr(packets[i], ptr, true);
                Marshal.Copy(ptr, buffer, bufferIndex, size);

                bufferIndex += size;
            }

            Marshal.FreeHGlobal(ptr);

            return buffer;
        }

        public IPacket[] Deserialize(byte[] buffer)
        {
            var count       = BitConverter.ToInt32(buffer, 0);
            var packetDatas = new PacketData[count];
            var bufferIndex = 4;

            // Extract packet data.
            for (var i = 0; i < count; i++)
            {
                var packetData = new PacketData();

                packetData.packetType = (PacketType)buffer[bufferIndex];
                bufferIndex++;

                packetData.size       = BitConverter.ToInt32(buffer, bufferIndex);
                bufferIndex           += 4;
                
                packetData.type       = Type.GetType(packetData.packetType.ToString() + "Packet");
                
                packetDatas[i]        = packetData;
            }

            // Create packets.
            var packets     = new IPacket[packetDatas.Length];
            var formatter   = new BinaryFormatter();

            using (var memoryStream = new MemoryStream(buffer))
            {
                memoryStream.Position = sizeof(int) + (sizeof(int) + 1) * count;

                for (int i = 0; i < packetDatas.Length; i++)
                {
                    var data                = packetDatas[i];
                    var packet              = (IPacket)formatter.Deserialize(memoryStream);

                    memoryStream.Position   += data.size;

                    packets[i]              = packet;
                }
            }

            return packets;
        }
    }
}
