using System;
using System.Net;
using ProjBobcat.Class.Helper;
using System.Windows;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.Class.Model.MicrosoftAuth;

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

            MicrosoftAuthenticator.Configure(new MicrosoftAuthenticatorAPISettings
            {
                Client  = "1aefa904-b887-4fbf-98fc-10185b7b8049",   // Azure AD 应用程序ID
                TenentId  = "consumers",    // 请求用户标识符
                Scopes  = new[] {  "XboxLive.signin",  "offline_access",  "openid",  "profile",  "email"  } //请求授权服务的内容
            });
        }
    }
}
