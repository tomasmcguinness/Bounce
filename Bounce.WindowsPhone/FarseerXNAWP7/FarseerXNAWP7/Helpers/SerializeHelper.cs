using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace FarseerXNAWP7.Helpers
{
    public static class SerializeHelper
    {
        public static List<T> DeserializeParams<T>(string LevelFilePath)
        {
            XDocument doc = new XDocument();
            doc = XDocument.Load(new StreamReader(Microsoft.Xna.Framework.TitleContainer.OpenStream(LevelFilePath)));
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            XmlReader reader = doc.CreateReader();

            List<T> result = (List<T>)serializer.Deserialize(reader);
            reader.Close();

            return result;
        }

        public static List<string> ReadTextFile(string filePath)
        {
            List<string> stageTitles = new List<string>();

            StreamReader streamReader = new StreamReader(Microsoft.Xna.Framework.TitleContainer.OpenStream(filePath));
            string line;

            while ((line = streamReader.ReadLine()) != null)
            {
                stageTitles.Add(line);
            }

            streamReader.Close();

            return stageTitles;
        }
    }
}
