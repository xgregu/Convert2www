using NLog;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Convert2www
{
    internal partial class MainForm : Form
    {
        private readonly Flow _flow;

        public MainForm(Flow flow)
        {
            _flow = flow;
            InitializeComponent();
            Load += OnLoad;
            Shown += OnShown;
        }

        private void OnShown(object sender, EventArgs e)
        {
            Refresh();
            _flow.Run();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            LogManager.Configuration = LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();

            Text += " - ver. " + Assembly.GetEntryAssembly()?.GetName().Version.ToString();
        }
    }
}