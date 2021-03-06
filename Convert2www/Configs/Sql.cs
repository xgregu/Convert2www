using System.Xml.Serialization;

namespace Convert2www.Configs
{
    public class Sql
    {
        [XmlAttribute]
        public string Database { get; set; }

        [XmlAttribute]
        public string Host { get; set; }

        [XmlAttribute]
        public int Port { get; set; }

        [XmlAttribute]
        public string Username { get; set; }

        [XmlAttribute]
        public string Password { get; set; }

        [XmlAttribute]
        public string Instance { get; set; }

        public static Sql Default()
        {
            return new Sql
            {
                Database = string.Empty,
                Host = "localhost",
                Port = 50502,
                Username = "sa",
                Password = string.Empty,
                Instance = "SQLEXPRESS",
            };
        }
    }
}