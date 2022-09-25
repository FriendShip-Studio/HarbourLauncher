using System;
using System.Net;
using ProjBobcat.Class.Helper;
using System.Windows;

namespace HarbourLauncher_Reloaded
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(Object sender, StartupEventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 512;

            ServiceHelper.Init();
            HttpClientHelper.Init();
        }
    }
}
