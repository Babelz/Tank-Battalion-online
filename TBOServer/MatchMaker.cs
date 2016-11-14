using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using TBOLib;
using TBOLib.Packets;
using Timer = System.Timers.Timer;

namespace TBOServer
{
    public sealed class Matchmaker
    {
        #region Fields
        private readonly List<Client> clients;

        private readonly List<Client> disconnected;
        #endregion

        #region Properties
        public int PlayersSearching
        {
            get
            {
                return clients.Count;
            }
        }
        #endregion

        public Matchmaker()
        {
            clients         = new List<Client>();
            disconnected    = new List<Client>();
        }

        #region Event handlers
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ValidateAndPing();
            
            CreateMatches();
        }
        #endregion

        private void CreateMatches()
        {
        }

        private void ValidateAndPing()
        {
            // Check last ping results. If there still are clients at 
            // disconnected list, they have not responded to our ping
            // request and are disconnected. Remove them.
            for (var i = 0; i < disconnected.Count; i++) disconnected[i].ListenOnce();

            while (disconnected.Count != 0)
            {
                Console.WriteLine("client {0} did not respond to ping, disconnecting...", disconnected[0].Name);

                disconnected[0].Close();
                disconnected.RemoveAt(0);
            }

            // Send new ping and status packet.
            var ping    = new PingPacket("PING");
            var status  = new ServerStatusPacket(clients.Count, 0);

            foreach (var client in clients)
            {
                client.Received += Client_Received;

                client.Send(ping, status);

                disconnected.Add(client);
            }
        }

        private void Client_Received(Client client, IPacket packet)
        {
            // Get pong packet.
            var pong = (PingPacket)packet;
            
            // Unhook event.
            client.Received -= Client_Received;

            // Check contents.
            if (pong.contents != "PONG") Console.WriteLine("player {0} did not answer to ping request, disconnecting", client.Name);
            else                         disconnected.Remove(client);
        }
       
        public void Add(Client client)
        {
            lock (clients) clients.Add(client);
        }

        public void Start()
        {
            var timer       = new Timer();
            timer.Elapsed   += Timer_Elapsed;
            timer.Interval  = 250;
            timer.Enabled   = true;
            timer.AutoReset = true;

            timer.Start();
        }
    }
}
