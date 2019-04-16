using System;
using System.Collections.Generic;
using System.Text;

namespace ArmA2Mod.Server
{
    class WebLauncher
    {
        public WebLauncher()
        {
            RequiredMods = new List<string>();
        }

        public WebLauncher(string name, string address, string pass)
            : this()
        {
            ServerName = name;
            ServerAddress = address;
            ServerPassword = pass;
        }

        public WebLauncher(string name, string ip, string pass, List<string> mods)
            :this(name,ip,pass)
        {
            RequiredMods = mods;
        }

        public string ServerName
        { get; set; }

        public string ServerAddress
        { get; set; }

        public string ServerPassword
        { get; set; }

        public List<string> RequiredMods
        {
            get;
            set;
        }


    }
}
