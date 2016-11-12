using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBOLib;

namespace TBOClient
{
    public static class Configuration
    {
        #region Default values
        private static readonly string DefaultName      = "tunematon_uskomaton";
        private static readonly string DefaultAddress   = "127.0.0.1";

        private static readonly Color DefaultColor      = Color.Red;

        private const int DefaultPort                   = 1337;
        #endregion

        #region Fields
        private static readonly IniFile config;
        #endregion

        #region Properties
        public static string Name
        {
            get
            {
                return !config.KeyExists("name", "player") ? DefaultName : config.GetValue("name", "player");
            }
        }

        public static Color PreferedColor
        {
            get
            {
                if (!config.KeyExists("color", "player")) return DefaultColor;

                var colorName = config.GetValue("color", "player");

                var property = typeof(Color).GetProperty(colorName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                if (property == null) return DefaultColor;
                
                return (Color)property.GetValue(null);
            }
        }

        public static string ServerAddress
        {
            get
            {
                return !config.KeyExists("address", "server") ? DefaultAddress : config.GetValue("address", "server");
            }
        }

        public static int Port
        {
            get
            {
                return !config.KeyExists("port", "server") ? DefaultPort : int.Parse(config.GetValue("port", "server"));
            }
        }
        #endregion

        static Configuration()
        {
            config = new IniFile("cfg.ini");
        }
    }
}
