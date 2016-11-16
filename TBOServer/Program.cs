using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBOLib;
using TBOLib.Packets;

namespace TBOServer
{
    public sealed class Program
    {
        #region Fields
        private static Matchmaker matchmaker;
        private static Authenticator authenticator;
        private static Listener listener;
        #endregion

        private static void Main(string[] args)
        {
            Console.WriteLine("starting server...");

            matchmaker      = new Matchmaker();
            authenticator   = new Authenticator();
            listener        = new Listener();

            authenticator.AuthenticationFailed  += Authenticator_AuthenticationFailed;
            authenticator.AuthenticationSuccess += Authenticator_AuthenticationSuccess;

            listener.ClientConnectd             += Listener_ClientConnectd;

            listener.BeginListen();
            matchmaker.Start();
            
            Console.WriteLine("server started, waiting for clients");

            Thread.CurrentThread.Join();
        }

        #region Event handlers
        private static void Listener_ClientConnectd(TcpClient client)
        {
            Console.WriteLine("client connected from {0}", (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString());
            Console.WriteLine("autheticating...");

            authenticator.Authenticate(client);

            listener.BeginListen();
        }
        private static void Authenticator_AuthenticationSuccess(TcpClient client, AuthenticationPacket packet)
        {
            Console.WriteLine("authentication ok, adding player to the matchmaker...");
            Console.WriteLine("client responded: " + packet.response);

            var name = packet.response.Substring(packet.response.IndexOf(":") + 1).Trim();
            var guid = Guid.Parse(packet.guid);

            matchmaker.Add(new Client(client, name, guid));
        }
        private static void Authenticator_AuthenticationFailed(TcpClient client)
        {
            Console.WriteLine("authentication failed, disconnecting client");

            client.Close();
        }
        #endregion
    }
}
