using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBOLib;
using TBOLib.Packets;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace TBOTests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Serialize()
        {
            var pingPacket = new PingPacket("hello");

            var bytes = PacketSerializer.Serialize(pingPacket);

            var header      = 4;
            var packets     = 1;
            var packetSize  = Marshal.SizeOf(pingPacket);

            Assert.AreEqual(header + packets + packetSize, bytes.Length);
        }

        [TestMethod]
        public void Deserialize()
        {
            var pingPacket  = new PingPacket("hello");

            var bytes       = PacketSerializer.Serialize(pingPacket);

            var packets     = PacketSerializer.Deserialize(bytes);

            Assert.IsTrue(packets.Length == 1);
            Assert.AreEqual("hello", ((PingPacket)packets[0]).contents);
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            const int Count = 255;

            var packets     = new List<IPacket>();
            var contents    = new List<string>();
            
            for (int i = 0; i < Count; i++) contents.Add("STR: " + i.ToString());

            for (int i = 0; i < Count / 2; i++)     packets.Add(new PingPacket(contents[i]));
            for (int i = Count / 2; i < Count; i++) packets.Add(new AuthenticationPacket(contents[i], contents[i]));
            
            var bytes = PacketSerializer.Serialize(packets.ToArray());
            
            packets = PacketSerializer.Deserialize(bytes).ToList();
        
            for (int i = 0; i < Count / 2; i++)     Assert.AreEqual(contents[i], ((PingPacket)packets[i]).contents);
            for (int i = Count / 2; i < Count; i++) Assert.AreEqual(contents[i], ((AuthenticationPacket)packets[i]).version);
            for (int i = Count / 2; i < Count; i++) Assert.AreEqual(contents[i], ((AuthenticationPacket)packets[i]).time);
        }
    }
}
