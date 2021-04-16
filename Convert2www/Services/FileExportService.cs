using Convert2www.Interfaces;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Convert2www.Services
{
    internal class FileExportService
    {
        private const string _fileNameWare = "materia.txt";
        private const string _fileNameContractor = "kontrah.txt";

        private readonly ILogger _windowLogger;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly string _dataDir;
        private readonly string _dataArchiwDir;
        private readonly string _dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");

        public FileExportService(IConfig config, IContext context)
        {
            _context = context;
            _config = config;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _dataDir = Path.Combine(_context.MainPath, "DANE", _context.FtpName);
            _dataArchiwDir = Path.Combine(_dataDir, "___archiwum");
            if (!Directory.Exists(_dataDir))
                Directory.CreateDirectory(_dataArchiwDir);
            if (!Directory.Exists(_dataDir))
                Directory.CreateDirectory(_dataArchiwDir);
        }

        public void PrepareExportFile()
        {
            var ftpServer = _config.FtpServers.First(x => x.Name == _context.FtpName);

            if (ftpServer == null)
            {
                _windowLogger.Warn("Błąd podczas pracy programu. Szczegóły w log");
                _logger.Warn($"Błędna nazwa serwera. Serwer {_context.FtpName} nie istnieje w pliku konfiguracyjnym");
                return;
            }

            DeleteOldArchiwFiles(_dataArchiwDir);

            if (ftpServer.IsSendWare)
                PrepareWareData();

            if (ftpServer.IsSendContractor)
                PrepareContractorData();
        }

        private void MoveUnsentFile(string fileName)
        {
            if (File.Exists(Path.Combine(_dataDir, fileName)))
            {
                if (!Directory.Exists(_dataArchiwDir))
                    Directory.CreateDirectory(_dataArchiwDir);
                File.Move(Path.Combine(_dataDir, fileName), Path.Combine(_dataArchiwDir, Path.ChangeExtension(fileName, null) + "_" + _dateTimeNow + " - niewysłany.txt"));
            }
        }

        private void PrepareWareData()
        {
            MoveUnsentFile(_fileNameWare);
            FileStream fStream = null;
            StreamWriter sWriter = null;
            try
            {
                _logger.Info("Generating vares file");
                _windowLogger.Info("Generowanie pliku towarów...");
                if (!Directory.Exists(_dataDir)) Directory.CreateDirectory(_dataDir);

                fStream = new FileStream(Path.Combine(_dataDir, _fileNameWare), FileMode.OpenOrCreate);
                using (sWriter = new StreamWriter(fStream, Encoding.Default))
                {
                    try
                    {
                        _logger.Info("Processing wares data");
                        _windowLogger.Info("Przetwarzanie danych towarów...");
                        foreach (var p in _context.WareList)
                        {
                            string[] w = new string[16];
                            w[1] = p.Nazwa.Replace('|', ' ');
                            w[2] = p.Sww.Replace('|', ' ');
                            w[3] = p.Kod.Replace('|', ' ');
                            w[4] = p.JM.Replace('|', ' ');
                            w[5] = p.Opis4.Replace('|', ' ');
                            w[6] = p.Stan.ToString().Replace('|', ' ');
                            w[7] = p.CenaDet.ToString().Replace('|', ' ');
                            w[8] = p.CenaDet.ToString().Replace('|', ' ');
                            w[9] = p.CenaHurt.ToString().Replace('|', ' ');
                            w[10] = p.CenaDod.ToString().Replace('|', ' ');
                            w[11] = p.CenaNoc.ToString().Replace('|', ' ');
                            w[12] = p.IleWZgrzewce.ToString().Replace('|', ' ');
                            w[14] = p.CenaEw.ToString().Replace('|', ' ');
                            w[15] = "0";  // p.WysylacNaSklepInternetowy.ToString().Replace('|', ' ');

                            for (int i = 0; i < w.Length; i++)
                            {
                                if (String.IsNullOrEmpty(w[i]))
                                    w[i] = " ";
                            }
                            _windowLogger.Error("FileExportService    " + $"{ w[1]}|{w[2]}|{w[3]}|{w[4]}|{w[5]}|{w[6]}|{w[7]}|{w[8]}|{w[9]}|{w[10]}|{w[11]}|{w[12]}|{w[13]}|{w[14]}|{w[15]}");
                            sWriter.WriteLine($"{w[1]}|{w[2]}|{w[3]}|{w[4]}|{w[5]}|{w[6]}|{w[7]}|{w[8]}|{w[9]}|{w[10]}|{w[11]}|{w[12]}|{w[13]}|{w[14]}|{w[15]}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _windowLogger.Warn("Błąd podczas pracy programu. Szczegóły w log");
                        _logger.Warn(ex, "Error in processing wares data");
                    }
                }
            }
            catch (Exception ex)
            {
                _windowLogger.Warn("Błąd podczas pracy programu. Szczegóły w log");
                _logger.Warn(ex, "Error in generating ware file");
            }
            finally
            {
                if (fStream != null)
                    fStream.Dispose();
            }
        }

        private void PrepareContractorData()
        {
            MoveUnsentFile(_fileNameContractor);
            FileStream fStream = null;
            StreamWriter sWriter = null;
            try
            {
                _logger.Info("Generating contractors file");
                _windowLogger.Info("Generowanie pliku kontrahentów...");
                if (!Directory.Exists(_dataDir)) Directory.CreateDirectory(_dataDir);
                fStream = new FileStream(Path.Combine(_dataDir, _fileNameContractor), FileMode.OpenOrCreate);
                using (sWriter = new StreamWriter(fStream, Encoding.Default))
                {
                    try
                    {
                        _logger.Info("Processing contractors data");
                        _windowLogger.Info("Przetwarzanie danych kontrahentów...");
                        foreach (var p in _context.ContractorList)
                        {
                            string[] w = new string[14];
                            w[1] = p.Skrot.Replace('|', ' ');
                            w[2] = " ";
                            w[3] = " ";
                            w[4] = p.Miasto.Replace('|', ' ');
                            //if (String.IsNullOrEmpty(p.KrajNazwa))
                            //    w[5] = "POLSKA";
                            //else
                            //    w[5] = p.KrajNazwa.Replace('|', ' ');
                            w[5] = " ";
                            w[6] = p.Ulica.Replace('|', ' ');
                            if (!String.IsNullOrEmpty(p.NrDomu))
                            {
                                w[6] += " " + p.NrDomu;
                                if (!String.IsNullOrEmpty(p.NrLokalu))
                                    w[6] += "/" + p.NrLokalu;
                            }
                            w[7] = p.Kod.Replace('|', ' ');
                            w[8] = p.Telefon.Replace('|', ' ');
                            w[9] = p.Fax.Replace('|', ' ');
                            w[10] = p.Nazwa.Replace('|', ' ');
                            w[11] = p.NIP.Replace('|', ' ');
                            w[12] = " ";
                            w[13] = "0";

                            for (int i = 0; i < w.Length; i++)
                            {
                                if (String.IsNullOrEmpty(w[i]))
                                    w[i] = " ";
                            }

                            sWriter.WriteLine($"{w[1]}|{w[2]}|{w[3]}|{w[4]}|{w[5]}|{w[6]}|{w[7]}|{w[8]}|{w[9]}|{w[10]}|{w[11]}|{w[12]}|{w[13]}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _windowLogger.Warn("Błąd podczas pracy programu. Szczegóły w log");
                        _logger.Warn(ex, "Error in processing contractors data");
                    }
                }
            }
            catch (Exception ex)
            {
                _windowLogger.Warn("Błąd podczas pracy programu. Szczegóły w log");
                _logger.Warn(ex, "Error in generating ware file");
            }
            finally
            {
                if (fStream != null)
                    fStream.Dispose();
            }
        }

        private void DeleteOldArchiwFiles(string dirName)
        {
            string[] files = Directory.GetFiles(dirName);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastWriteTime < DateTime.Now.AddDays(-7))
                {
                    try
                    {
                        fi.Delete();
                        _logger.Info($"Delete old file in archiwum {fi.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn(ex, $"Delete old file in archiwum {fi.Name}");
                    }
                }
            }
        }
    }
}