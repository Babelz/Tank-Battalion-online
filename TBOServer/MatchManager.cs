using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
