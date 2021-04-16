using Convert2www.Configs;
using System.Collections.Generic;

namespace Convert2www.Interfaces
{
    internal interface IConfig
    {
        Sql Sql { get; set; }
        List<FtpServer> FtpServers { get; set; }

        void SaveSettings();
    }
}