using Convert2www.Models;
using System.Collections.Generic;

namespace Convert2www.Interfaces
{
    internal interface IContext
    {
        string FtpName { get; }
        string MainPath { get; }
        string TempPath { get; }
        List<WaresList> WareList { get; set; }
        List<ContractorsList> ContractorList { get; set; }
    }
}