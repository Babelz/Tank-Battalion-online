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
            clients = new List<Client>();
        }

        #region Event handlers
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendPings();

            Thread.Sleep(1000);

            ReceivePings();

            CreateMatches();
        }
        #endregion

        private void CreateMatches()
        {
        }

        private void SendPings()
        {
            var ping    = new PingPacket("PING");
            var status  = new ServerStatusPacket(clients.Count, 0);
            
            for (var i = 0; i < clients.Count; i++) clients[i].Send(ping, status);
        }
        private void ReceivePings()
        {
            var disconnectedClients = new List<Client>();

            foreach (var client in clients)
            {
                if (!client.HasIncomingPackets())
                {
                    Console.WriteLine("player {0} disconnected during matchmaking...", client.Name);

                    disconnectedClients.Add(client);
                }
                else
                {
                    var pong = (PingPacket)client.Receive()[0];

                    if (pong.contents != "PONG")
                    {
                        Console.WriteLine("player {0} did not answer to ping request, disconnecting", client.Name);

                        disconnectedClients.Add(client);
                    }
                }
            }

            for (var i = 0; i < disconnectedClients.Count; i++) clients.Remove(disconnectedClients[i]);

            while (disconnectedClients.Count != 0)
            {
                disconnectedClients[0].Close();
                disconnectedClients.RemoveAt(0);
            } 
        }
       
        public void Add(Client client)
        {
            lock (clients) clients.Add(client);
        }

        public void Start()
        {
            var timer       = new Timer();
            timer.Elapsed   += Timer_Elapsed;
            timer.Interval  = 5000;
            timer.Enabled   = true;
            timer.AutoReset = true;

            timer.Start();
        }
    }
}
