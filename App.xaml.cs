using System;
using System.Net;
using ProjBobcat.Class.Helper;
using System.Windows;
using ProjBobcat.DefaultComponent.Authenticator;

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

            MicrosoftAuthenticator.Configure(new ProjBobcat.Class.Model.MicrosoftAuth.MicrosoftAuthenticatorAPISettings
            {
                ClientId = "1aefa904-b887-4fbf-98fc-10185b7b8049",
                TenentId = "consumers",
                Scopes = new[] { "XboxLive.signin", "offline_access", "openid", "profile", "email" }
            });

    }

    }
}
