using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace SierraLib.XML
{
   
    public abstract class ISerializable
    {
        public ISerializable()
        {
            throw new ArgumentException("This class requires the type to be specified");
        }

        private Type m_inheritedType = null;
        public ISerializable(Type inheritedType)
        {
            if (inheritedType == null)
                throw new ArgumentNullException("inheritedType");

            else
                m_inheritedType = inheritedType;
        }

        public string Serialize()
        {
            XmlSerializer serial = new XmlSerializer(m_inheritedType);
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            serial.Serialize(writer, this);
            writer.Close();
            return sb.ToString();
        }

        public void Serialize(string path)
        {
            XmlSerializer serial = new XmlSerializer(m_inheritedType);
            FileStream writer = new FileStream(path, FileMode.Create, FileAccess.Write);
            serial.Serialize(writer, this);
            writer.Close();
        }
    }
}
