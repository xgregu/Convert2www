using Convert2www.Interfaces;
using Convert2www.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Convert2www.Model
{
    public class Context : IContext
    {
        public string FtpName { get; }
        public string MainPath { get; }
        public string TempPath { get; }
        public List<WaresList> WareList { get; set; }
        public List<ContractorsList> ContractorList { get; set; }

        public Context(string arg)
        {
            FtpName = arg;
            MainPath = AppDomain.CurrentDomain.BaseDirectory;
            TempPath = GetTempPath();
            WareList = new List<WaresList>();
            ContractorList = new List<ContractorsList>();
        }

        private static string GetTempPath()
        {
            var tempPath = Path.GetTempPath();

            return Directory.Exists(tempPath)
                ? Path.Combine(tempPath, Constants.CleanAppName)
                : $@"C:\tmp\{Constants.CleanAppName}\";
        }
    }
}