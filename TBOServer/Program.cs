using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TBOServer
{
    public sealed class Program
    {
        #region Fields
        private static Authenticator authenticator;
        private static Listener listener;
        #endregion

        private static void Main(string[] args)
        {
            Console.WriteLine("starting server...");

            authenticator   = new Authenticator();
            listener        = new Listener();

            authenticator.AuthenticationFailed  += Authenticator_AuthenticationFailed;
            authenticator.AuthenticationSuccess += Authenticator_AuthenticationSuccess;

            listener.ClientConnectd             += Listener_ClientConnectd;

            listener.BeginListen();
            
            Console.WriteLine("server started, waiting for clients");

            Thread.CurrentThread.Join();
        }

        #region Event handlers
        private static void Listener_ClientConnectd(TcpClient client)
        {
            Console.WriteLine("client connected from {0}", (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString());
            Console.WriteLine("autheticating...");

            authenticator.Authenticate(client);
        }
        private static void Authenticator_AuthenticationSuccess(TcpClient client, string response)
        {
            Console.WriteLine("authentication ok, adding player to the matchmaker...");
            Console.WriteLine("client responded: " + response);
        }
        private static void Authenticator_AuthenticationFailed(TcpClient client)
        {
            Console.WriteLine("authentication failed, disconnecting client");

            client.Close();
        }
        #endregion
    }
}
