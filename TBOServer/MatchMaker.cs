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

        private readonly List<Client> pinged;

        private readonly List<Client> responded;
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
            pinged          = new List<Client>();
            responded       = new List<Client>();
        }

        #region Event handlers
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckLastPingResults();

            CreateMatches();

            SendNewPingPackets();
        }
        #endregion

        private void CreateMatches()
        {
            if (clients.Count < 2) return;

            var a = clients[0];
            var b = clients[1];

            clients.Remove(a);
            clients.Remove(b);

            MatchManager.StartMatch(a, b);
        }
        
        private void CheckLastPingResults()
        {
            // Check last ping results. If there still are clients at 
            // disconnected list, they have not responded to our ping
            // request and are disconnected. Remove them.
            for (var i = 0; i < pinged.Count; i++) pinged[i].ListenOnce();

            for (var i = 0; i < responded.Count; i++)
            {
                pinged.Remove(responded[i]);
                clients.Add(responded[i]);
            }

            while (pinged.Count != 0)
            {
                Console.WriteLine("client {0} did not respond to ping, disconnecting...", pinged[0].Name);

                pinged[0].Close();
                pinged.RemoveAt(0);
            }

            responded.Clear();
        }
        private void SendNewPingPackets()
        {
            // Send new ping and status packet.
            var ping    = new PingPacket("PING");
            var status  = new ServerStatusPacket(clients.Count, 0);

            foreach (var client in clients)
            {
                client.Received += Client_Received;

                client.Send(ping, status);

                pinged.Add(client);
            }

            clients.Clear();
        }

        private void Client_Received(Client client, IPacket packet)
        {
            // Get pong packet.
            var pong = (PingPacket)packet;

            // Unhook event.
            client.Received -= Client_Received;

            // Check contents.
            if (pong.contents == "PONG") responded.Add(client);
        }
       
        public void Add(Client client)
        {
            clients.Add(client);
        }

        public void Start()
        {
            var timer       = new Timer();
            timer.Elapsed   += Timer_Elapsed;
            timer.Interval  = 2500;
            timer.Enabled   = true;
            timer.AutoReset = true;

            timer.Start();
        }
    }
}
