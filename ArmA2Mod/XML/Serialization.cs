using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace SierraLib.XML
{
    public static class Serialization
    {

        public static string Serialize<T>(T item) where T:class
        {
            XmlSerializer serial = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            serial.Serialize(writer, item);
            writer.Close();
            return sb.ToString();
        }

        public static void Serialize<T>(T item, string FilePath) where T:class
        {
            FileStream writer = null;
            try
            {
                XmlSerializer serial = new XmlSerializer(typeof(T));
                writer = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                serial.Serialize(writer, item);
            }
            finally
            {
                if(writer != null)
                    writer.Close();
            }
        }

        public static void Serialize<T>(T item, StreamWriter FileStream) where T : class
        {
            XmlSerializer serial = new XmlSerializer(typeof(T));
            serial.Serialize(FileStream, item);
            FileStream.Flush();
        }

        public static T Deserialize<T>(string FilePath) where T : class
        {
            if (IsValidPath(FilePath))
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));

                FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);

                T res = (T)xml.Deserialize(fs);

                fs.Close();

                return res;
            }
            else
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                return (T)xml.Deserialize(new StringReader(FilePath));
            }
        }

        public static T Deserialize<T>(StreamReader fileStream) where T : class
        {

                XmlSerializer xml = new XmlSerializer(typeof(T));

                T res = (T)xml.Deserialize(fileStream);
            
                return res;
        }


/// <summary>
/// Gets whether the specified path is a valid absolute file path.
/// </summary>
/// <param name="path">Any path. OK if null or empty.</param>
static public bool IsValidPath( string path )
{
    Regex r = new Regex( @"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$" );
    return r.IsMatch( path );
}
    }
}
