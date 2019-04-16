using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace ArmA2Mod
{
    internal class RegistryManager
    {
        public static bool IsPresent(string registryKey)
        {
            string[] keys = registryKey.Split('\\');

            RegistryKey reg = Registry.LocalMachine;

            if (keys[0] == "HKLM")
                reg = Registry.LocalMachine;
            if (keys[0] == "HKCU")
                reg = Registry.CurrentUser;

            for (int i = 1; i < keys.Length; i++)
            {
                if (Array.IndexOf(reg.GetSubKeyNames(), keys[i]) == -1)
                    return false;

                reg = reg.OpenSubKey(keys[i]);
               
            }

            return true;

        }

        public static object GetValue(string registryKey)
        {
            string[] keys = registryKey.Split('\\');

            RegistryKey reg = Registry.LocalMachine;

            if (keys[0] == "HKLM")
                reg = Registry.LocalMachine;
            if (keys[0] == "HKCU")
                reg = Registry.CurrentUser;

            for (int i = 1; i < keys.Length - 1; i++)
            {
                if (Array.IndexOf(reg.GetSubKeyNames(), keys[i]) == -1)
                    return null;

                reg = reg.OpenSubKey(keys[i]);

            }

            return reg.GetValue(keys[keys.Length - 1],"");
        }


    }
}
