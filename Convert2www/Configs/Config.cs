using Convert2www.Interfaces;
using Convert2www.Utils;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Convert2www.Configs
{
    [Serializable]
    [XmlRoot("Settings")]
    public class Config : IConfig
    {
        public Sql Sql { get; set; } = Sql.Default();

        [XmlArray("FtpServers")]
        [XmlArrayItem("FtpServer")]
        public List<FtpServer> FtpServers { get; set; } = FtpServer.Default();

        public void SaveSettings()
        {
            XmlHelpers.Save(this, Constants.ConfigFile);
        }
    }
}