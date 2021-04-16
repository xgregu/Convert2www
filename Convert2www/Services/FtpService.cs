using Convert2www.Interfaces;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Convert2www.Services
{
    internal class FtpService
    {
        private readonly ILogger _windowLogger;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly string _dataDir;
        private readonly string _dataArchiwDir;
        private readonly string _dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");

        public FtpService(IConfig config, IContext context)
        {
            _context = context;
            _config = config;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _dataDir = Path.Combine(_context.MainPath, "DANE", _context.FtpName);
            _dataArchiwDir = Path.Combine(_dataDir, "___archiwum");
        }

        public void Send()
        {
            var ftpServer = _config.FtpServers.First(x => x.Name == _context.FtpName);

            if (ftpServer == null)
            {
                _windowLogger.Warn("Błąd podczas pracy programu. Szczegóły w log");
                _logger.Warn($"Błędna nazwa serwera. Serwer {_context.FtpName} nie istnieje w pliku konfiguracyjnym");
                return;
            }

            DirectoryInfo d = new DirectoryInfo(_dataDir);
            foreach (var file in d.GetFiles("*.txt"))
            {
                SendFileToFTP(file.Name, ftpServer);
            }
        }

        private void SendFileToFTP(string _file, Configs.FtpServer ftpServer)
        {
            try
            {
                _windowLogger.Info($"Wysyłka na serwer: ftp:/{ftpServer.Host}:{ftpServer.Port}/" + _file);
                FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create($"ftp://{ftpServer.Host}:{ftpServer.Port}/{ftpServer.Directory}/" + _file);
                ftpClient.Credentials = new NetworkCredential(ftpServer.Username, ftpServer.Password);
                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;
                ftpClient.Method = WebRequestMethods.Ftp.UploadFile;
                ftpClient.UsePassive = true;
                ftpClient.Timeout = 60 * 1000;

                FileInfo fi = new FileInfo(Path.Combine(_dataDir, _file));
                ftpClient.ContentLength = fi.Length;

                byte[] fileContents = File.ReadAllBytes(Path.Combine(_dataDir, _file));

                ftpClient.ContentLength = fileContents.Length;
                Stream requestStream = ftpClient.GetRequestStream();
                FtpWebResponse response = null;

                try
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                    response = (FtpWebResponse)ftpClient.GetResponse();
                    _windowLogger.Info("Wysyłka na serwer FTP zakończona");
                    File.Move(Path.Combine(_dataDir, _file), Path.Combine(_dataArchiwDir, Path.ChangeExtension(_file, null) + "_" + _dateTimeNow + " - wysłany.txt"));
                }
                catch (Exception ex)
                {
                    _windowLogger.Error("Problem z wysłanie pliku na serwer FTP");
                    _logger.Error(ex, "Error while sending to ftp");
                }
                finally
                {
                    string status = response.StatusDescription;
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                _windowLogger.Error("Brak dostępu do FTP");
                _logger.Error(ex, "Ftp not available");
            }
        }
    }
}