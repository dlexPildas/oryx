using System;
using System.IO;
using System.Xml.Serialization;

namespace OryxDomain.Utilities
{
    public static class XmlUtils
    {
        public static T Deserialize<T>(string strXml)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(strXml)) return default(T);

            try
            {
                TextReader reader = new StringReader(strXml);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                returnObject = (T)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnObject;
        }

        public static string FindValueByIndexOf(string xml, string initTag, string endTag)
        {
            int init = xml.IndexOf(initTag) + initTag.Length;
            int end = xml.IndexOf(endTag) - init;
            return xml.Substring(init, end);
        }
    }
}
