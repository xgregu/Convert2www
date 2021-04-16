using System.IO;
using System.Xml.Serialization;

namespace Convert2www.Utils
{
    internal static class XmlHelpers
    {
        public static T DeserializeXml<T>(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                var serialize = new XmlSerializer(typeof(T));
                var output = (T)serialize.Deserialize(stream);
                return output;
            }
        }

        public static void Save<T>(T file, string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, file, ns);
            }
        }
    }
}