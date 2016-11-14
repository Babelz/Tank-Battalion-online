using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBOLib;
using TBOLib.Packets;

namespace TBOServer
{
    public static class MatchManager
    {
        #region Fields
        private static readonly List<Match> matches;
        #endregion

        static MatchManager()
        {
            matches = new List<Match>();
        }

        public static void StartMatch(Client a, Client b)
        {
            Match match = new Match();

            var apacket = new MessagePacket("found an opponent!\nmatch starting in {s}\nplaying against " + b.Name);
            var bpacket = new MessagePacket("found an opponent!\nmatch starting in {s}\nplaying against " + a.Name);

            a.Send(apacket);
            b.Send(bpacket);

            match.Initialize(Maps.Map1v1, a, b);
        }
    }
}
