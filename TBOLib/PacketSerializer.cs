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
        // Packet structure:
        // [4-bytes count of packets][packet types (byte per each)][packet data]

        public static byte[] Serialize(IPacket packet)
        {
            return Serialize(new IPacket[] { packet });
        }

        public static byte[] Serialize(IPacket[] packets)
        {
            var packetsCount = packets.Length;
            var types        = packets.Select(p => (byte)p.Type).ToArray();

            var headerSize   = sizeof(int);
            var typesSize    = sizeof(byte) * packets.Length;
            var dataSize     = packets.Sum(p => Marshal.SizeOf(p));

            var buffer       = new byte[headerSize + typesSize + dataSize];
            var bufferIndex  = 4;

            // Write header and packets information (4-bytes)
            Array.Copy(BitConverter.GetBytes(packetsCount), buffer, sizeof(int));
            
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
            var headerSize   = sizeof(int);
            var packetsCount = BitConverter.ToInt32(buffer, 0);
            var packetTypes  = new PacketType[packetsCount];
            var bufferIndex  = headerSize;

            // Extract packet data.
            for (var i = 0; i < packetsCount; i++)
            {
                packetTypes[i] = (PacketType)buffer[bufferIndex];
                
                bufferIndex++;
            }

            // Create packets.
            var packets     = new IPacket[packetTypes.Length];
            var handle      = Marshal.AllocHGlobal(packetTypes.Max(p => Packet.GetSize(p)));

            bufferIndex     = headerSize + packetsCount; 

            for (var i = 0; i < packets.Length; i++)
            {
                var packetType = packetTypes[i];
                var packetSize = Packet.GetSize(packetType);
                var type       = Packet.GetType(packetType);

                Marshal.Copy(buffer, bufferIndex, handle, packetSize);
                
                var packet = (IPacket)Marshal.PtrToStructure(handle, type);
                
                bufferIndex += packetSize;
                packets[i]  = packet;
            }
            
            Marshal.FreeHGlobal(handle);

            return packets;
        }
    }
}
