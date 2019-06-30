using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MathExpressionsNET.GUI
{
    public class Settings
    {
        public string InputExpression { get; set; }

        public bool RealTimeUpdate { get; set; }

        public string Derivatives { get; set; }

        public static bool TryLoad(string filePath, out Settings settings)
        {
            settings = null;
            
            if (!File.Exists(filePath))
            {
                return false;
            }
            
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                string xml = File.ReadAllText(filePath);

                using (var stringReader = new StringReader(xml))
                {
                    using (var reader = XmlReader.Create(stringReader))
                    {
                        settings = (Settings)serializer.Deserialize(reader);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool TrySave(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                string xml = "";

                using (var stringWriter = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(stringWriter))
                    {
                        serializer.Serialize(writer, this);
                        xml = stringWriter.ToString();
                    }
                }

                File.WriteAllText(filePath, xml);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}