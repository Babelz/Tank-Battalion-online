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

        private static readonly string DefaultVersion = "v000";
        #endregion

        #region Fields
        private static readonly IniFile configuration;
        #endregion

        #region Properties
        public static int Port
        {
            get
            {
                return !configuration.KeyExists("port", "server") ? DefaultPort : int.Parse(configuration.GetValue("port", "server"));
            }
        }
        public static int Lobbies
        {
            get
            {
                return !configuration.KeyExists("lobbies", "server") ? DefaultLobbiesCount : int.Parse(configuration.GetValue("lobbies", "server"));
            }
        }
        public static string Version
        {
            get
            {
                return !configuration.KeyExists("version", "server") ? DefaultVersion : configuration.GetValue("version", "server");
            }
        }
        #endregion

        static Configuration()
        {
            configuration = new IniFile("cfg.ini");
        }
    }
}
