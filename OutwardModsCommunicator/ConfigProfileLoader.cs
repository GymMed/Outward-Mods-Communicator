using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace OutwardModsCommunicator
{
    public static class ConfigProfileLoader
    {
        public static ConfigOverrides? LoadFromXml(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                var serializer = new XmlSerializer(typeof(ConfigOverrides));
                using var stream = File.OpenRead(filePath);
                return serializer.Deserialize(stream) as ConfigOverrides;
            }
            catch
            {
                return null;
            }
        }
    }
}
