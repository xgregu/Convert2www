using Convert2www.Configs;
using Convert2www.Interfaces;
using System.IO;
using Unity;

namespace Convert2www.Utils
{
    internal class ConfigInitializer : IConfigInitializer
    {
        private IContext _context;

        public ConfigInitializer(IContext context)
        {
            _context = context;
        }

        public void Initialize(IUnityContainer container)
        {
            IConfig config;
            var conf = Path.Combine(_context.MainPath, Constants.ConfigFile);
            if (!File.Exists(conf))
            {
                config = new Config();
                config.SaveSettings();
            }
            else
            {
                config = XmlHelpers.DeserializeXml<Config>(conf);
            }
            container.RegisterInstance(config);
        }
    }
}