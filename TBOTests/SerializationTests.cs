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
            var packetData  = 5;
            var packetSize  = pingPacket.Size();

            Assert.AreEqual(header + packetData + packetSize, bytes.Length);
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

        public void Shuffle<T>(List<T> list)
        {
            var rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            const int Count = 255;

            var packets     = new List<IPacket>();
            var contents    = new List<string>();
            
            for (int i = 0; i < Count; i++) contents.Add("STR: " + i.ToString());

            for (int i = 0; i < Count / 2; i++)     packets.Add(new PingPacket(contents[i]));
            for (int i = Count / 2; i < Count; i++) packets.Add(new AuthenticationPacket(contents[i]));
            
            var bytes = PacketSerializer.Serialize(packets.ToArray());

            var size = contents.Sum(p => p.Length) + packets.Count * 5 + 4;

            Assert.AreEqual(size, bytes.Length);

            packets = PacketSerializer.Deserialize(bytes).ToList();
        
            for (int i = 0; i < Count / 2; i++)     Assert.AreEqual(((PingPacket)packets[i]).contents, contents[i]);
            for (int i = Count / 2; i < Count; i++) Assert.AreEqual(((AuthenticationPacket)packets[i]).contents, contents[i]);
        }
    }
}
