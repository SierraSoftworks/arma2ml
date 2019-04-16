using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ArmA2Mod
{
    static class TextLoader
    {
        public static List<string> Process(string fileName)
        {
            List<string> lst = new List<string>();

            if (!File.Exists(fileName))
                return lst;

            StreamReader sr = new StreamReader(fileName);

            string file = sr.ReadToEnd();

            sr.Close();

            string[] split = file.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in split)
            {
                lst.Add(str.Trim());
            }

            return lst;
        }

        public static List<string> Process(string[] fileName)
        {
            List<string> lst = new List<string>();
            foreach (string file in fileName)
                lst.AddRange(Process(file));

            return lst;
        }
    }
}
