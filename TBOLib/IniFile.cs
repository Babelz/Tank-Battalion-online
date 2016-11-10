using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib
{
    public sealed class IniFile   
    {
        #region Fields
        private readonly string path;
        private readonly string exe;
        #endregion
        
        public IniFile(string iniPath = null)
        {
            exe     = Assembly.GetExecutingAssembly().GetName().Name;
            path    = new FileInfo(iniPath ?? exe + ".ini").FullName.ToString();
        }
        
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public string Read(string key, string section = null)
        {
            var value = new StringBuilder(255);

            GetPrivateProfileString(section ?? exe, key, "", value, 255, path);

            return value.ToString();
        }
        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? exe, key, value, path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? exe);
        }
        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? exe);
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }
}
