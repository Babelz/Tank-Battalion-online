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
        public static void StartMatch(Client a, Client b)
        {
            Match match = new Match();

            var apacket = new MessagePacket("found an opponent!\nmatch starting in {s}\nplaying against " + b.Name);
            var bpacket = new MessagePacket("found an opponent!\nmatch starting in {s}\nplaying against " + a.Name);

            a.Send(apacket);
            b.Send(bpacket);

            match.Start(Maps.Map1v1, a, b);

            match.Ended += Match_Ended;
            match.Started += Match_Started;
        }

        private static void Match_Started(Match match)
        {
            Console.WriteLine("New match started, {0} vs {1}", match[0], match[1]);
        }
        private static void Match_Ended(Match match)
        {
            Console.WriteLine("New match ended, {0} vs {1}", match[0], match[1]);
        }
    }
}
