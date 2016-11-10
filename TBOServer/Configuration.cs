using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBOLib;

namespace TBOServer
{
    public static class Configuration
    {
        #region Default values
        private const int DefaultPort           = 1337;
        private const int DefaultLobbiesCount   = 1;
        #endregion

        #region Fields
        private static readonly IniFile configuration;
        #endregion

        #region Properties
        public static int Port
        {
            get
            {
                return configuration == null ? DefaultPort : int.Parse(configuration.Read("port"));
            }
        }
        public static int Lobbies
        {
            get
            {
                return configuration == null ? DefaultLobbiesCount : int.Parse(configuration.Read("lobbies"));
            }
        }
        #endregion

        static Configuration()
        {
            configuration = new IniFile("cfg");
        }
    }
}
