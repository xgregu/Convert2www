using Convert2www.Interfaces;
using Convert2www.Model;
using Convert2www.Services;
using Convert2www.Utils;
using NLog;
using System;
using System.Reflection;
using System.Windows.Forms;
using Unity;
using Unity.Lifetime;

namespace Convert2www
{
    internal class Bootstrapper
    {
        private static readonly string _appName = Assembly.GetEntryAssembly()?.GetName().Name;
        private static readonly string _appVersion = Assembly.GetEntryAssembly()?.GetName().Version.ToString();

        private readonly IUnityContainer _container;
        private readonly ILogger _logger;

        public Bootstrapper(string[] args)
        {
            LogManager.Configuration = LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.ApplicationExit += OnApplicationClose;

            _logger = LogManager.GetCurrentClassLogger();
            _logger.Info($"Start application {_appName} v{_appVersion}");
            _container = new UnityContainer();

            if (args.Length == 0)
            {
                _logger.Warn("Arguments is null");
                Application.Exit();
            }

            var context = new Context(args[0]);
            _container.RegisterInstance(context);

            RegisterTypes();
            ResolveTypes();

            StartApp();
        }

        private void ResolveTypes() => _container.Resolve<IConfigInitializer>().Initialize(_container);

        private void RegisterTypes()
        {
            _logger.Debug("Register types");
            _container.RegisterType<IConfigInitializer, ConfigInitializer>(new ContainerControlledTransientManager());
            _container.RegisterType<ISqlService, SqlService>(new ContainerControlledTransientManager());
            _container.RegisterSingleton<IContext, Context>();
            _container.RegisterSingleton<FileExportService>();
            _container.RegisterSingleton<FtpService>();
            _container.RegisterSingleton<MainForm>();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal((Exception)e.ExceptionObject);
            Environment.Exit(1);
        }

        private void StartApp() => Program.Run(_container);

        private void OnApplicationClose(object sender, EventArgs e) => _logger.Info("Close application");
    }
}