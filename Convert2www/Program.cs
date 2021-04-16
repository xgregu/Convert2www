using System.Windows.Forms;
using Unity;

namespace Convert2www
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Bootstrapper(args);
        }

        internal static void Run(IUnityContainer container)
        {
            Application.Run(container.Resolve<MainForm>());
        }
    }
}