using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TBOServer
{
    public sealed class Listener
    {
        #region Fields
        private readonly TcpListener listener;
        #endregion
        
        public Listener()
        {
            listener = new TcpListener()
        }
    }
}
