using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TBOLib.Packets;

namespace TBOLib
{
    public static class PacketSerializer
    {
        #region Packet data helper struct
        private struct PacketData
        {
            public PacketType   packetType;
            public Type         type;
            public int          size;
        }
        #endregion

        #region Packet types dictionary
        private static readonly Dictionary<PacketType, Type> types;
        #endregion

        static PacketSerializer()
        {
            types = new Dictionary<PacketType, Type>()
            {
                { PacketType.Ping, typeof(PingPacket) },
                { PacketType.Authentication, typeof(AuthenticationPacket) }
            };
        }

        // Packet structure:
        // [4-bytes count of packets][packet types (byte per each)][packet data]

        public static byte[] Serialize(IPacket packet)
        {
            return Serialize(new IPacket[] { packet });
        }

        public static byte[] Serialize(IPacket[] packets)
        {
            var count       = packets.Length;
            var types       = packets.Select(p => (byte)p.Type).ToArray();

            var headerSize  = sizeof(int);
            var typesSize   = sizeof(byte) * packets.Length;
            var dataSize    = packets.Sum(p => Marshal.SizeOf(p));

            var buffer      = new byte[headerSize + typesSize + dataSize];
            var bufferIndex = 4;

            // Write header and packets information (4-bytes)
            Array.Copy(BitConverter.GetBytes(count), buffer, sizeof(int));
            
            for (var i = 0; i < packets.Length; i++)
            {
                // Copy type (1-byte)
                Array.Copy(types, i, buffer, bufferIndex, 1);
                bufferIndex++;
            }
            
            // Write packets.
            var packetBuffer    = new byte[packets.Max(p => Marshal.SizeOf(p))];
            var handle          = Marshal.AllocHGlobal(packetBuffer.Length);

            // Start writing from the end of header + types region.
            bufferIndex = headerSize + typesSize;

            for (var i = 0; i < packets.Length; i++)
            {
                var packet      = packets[i];
                var packetSize  = Marshal.SizeOf(packet);
                
                Marshal.StructureToPtr(packet, handle, false);
                Marshal.Copy(handle, buffer, bufferIndex, packetSize);

                bufferIndex += packetSize;
            }

            Marshal.FreeHGlobal(handle);

            return buffer;
        }

        public static IPacket[] Deserialize(byte[] buffer)
        {
            var headerSize  = sizeof(int);
            var count       = BitConverter.ToInt32(buffer, 0);
            var packetDatas = new PacketData[count];
            var bufferIndex = headerSize;

            // Extract packet data.
            for (var i = 0; i < count; i++)
            {
                PacketData packetData;

                packetData.packetType = (PacketType)buffer[bufferIndex];
                packetData.type       = types[packetData.packetType];
                packetData.size       = Marshal.SizeOf(packetData.type);

                packetDatas[i]        = packetData;

                bufferIndex++;
            }

            // Create packets.
            var packets     = new IPacket[packetDatas.Length];
            var handle      = Marshal.AllocHGlobal(packetDatas.Max(p => p.size));

            bufferIndex     = headerSize + count; 

            for (var i = 0; i < packets.Length; i++)
            {
                var data    = packetDatas[i];
                
                Marshal.Copy(buffer, bufferIndex, handle, data.size);
                
                var packet = (IPacket)Marshal.PtrToStructure(handle, data.type);
                
                bufferIndex += data.size;
                packets[i]  = packet;
            }
            
            Marshal.FreeHGlobal(handle);

            return packets;
        }
    }
}
