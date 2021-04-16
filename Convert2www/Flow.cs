using Convert2www.Interfaces;
using Convert2www.Services;
using NLog;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Convert2www
{
    internal class Flow
    {
        private ILogger _windowLogger;
        private ILogger _logger;
        private ISqlService _sqlService;
        private IContext _context;
        private FileExportService _fileExportService;
        private FtpService _ftpService;

        public Flow(ISqlService sqlService, IContext context, FileExportService fileExportService, FtpService ftpService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _sqlService = sqlService;
            _context = context;
            _fileExportService = fileExportService;
            _ftpService = ftpService;
        }

        public void Run() => Task.Run(FlowTask);

        private Task FlowTask()
        {
            _windowLogger.Info($"Start aplikacji");
            _windowLogger.Info(_context.MainPath);
            if (!_sqlService.IsSqlConnectionActive)
                _windowLogger.Error("Brak dostępu do serwera SQL");

            _sqlService.WareReader();
            _sqlService.ConractorReader();

            _fileExportService.PrepareExportFile();

            _ftpService.Send();

            _windowLogger.Info($"Koniec aplikacji");
            Application.Exit();
            return Task.CompletedTask;
        }
    }
}