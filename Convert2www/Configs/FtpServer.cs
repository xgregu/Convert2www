using System.Collections.Generic;
using System.Xml.Serialization;

namespace Convert2www.Configs
{
    public class FtpServer
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Host { get; set; }

        [XmlAttribute]
        public int Port { get; set; }

        [XmlAttribute]
        public string Username { get; set; }

        [XmlAttribute]
        public string Password { get; set; }

        [XmlAttribute]
        public string Directory { get; set; }

        [XmlAttribute]
        public bool IsSendContractor { get; set; }

        [XmlAttribute]
        public bool IsSendWare { get; set; }

        public static List<FtpServer> Default()
        {
            var defaultFtpServerList = new List<FtpServer>
            {
                new FtpServer
                {
                    Name = string.Empty,
                    Host = string.Empty,
                    Port = 21,
                    Username = string.Empty,
                    Password = string.Empty,
                    Directory = string.Empty,
                    IsSendContractor = true,
                    IsSendWare = true
                }
            };

            return defaultFtpServerList;
        }
    }
}