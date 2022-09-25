using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using HarbourLauncher_Reloaded.GameBasis;
using System.Diagnostics;
using ProjBobcat.Class.Model;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model.LauncherProfile;
using ProjBobcat.DefaultComponent;
using ProjBobcat.DefaultComponent.Authenticator;
using ProjBobcat.DefaultComponent.ResourceInfoResolver;
using ProjBobcat.Event;
using ProjBobcat.Interface;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace HarbourLauncher_Reloaded
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {
        private List<VersionInfo> gamelist;

        private IAsyncEnumerable<string> javaFullList;

        public bool javaScanning=false;

        private List<string> javaList;

        public enum LoginMode
        {
            /// <summary>
            /// 离线
            /// </summary>
            Offline,
            /// <summary>
            /// 正版登录
            /// </summary>
            Online
        }
        public LoginMode loginMode = new();

        public MainWindow()
        {
            InitializeComponent();

            Core.CoreInit();

            RefreshVersionList();

            JavaPathLoaderAsync();
            playerNameInit();
            JavaCombo.ItemsSource = javaList;
            JavaCombo.SelectedItem = javaList.FirstOrDefault();
        }

        private async Task RefreshJavaList()
        {
            if (javaScanning)
            {
                MessageBox.Show("一个搜索任务正在进行中，请等待完成", "启动搜索失败");
                return;
            }

            javaScanning = true;
            javaList = new List<string>();
            javaFullList = SystemInfoHelper.FindJavaFull();

            await foreach (var java in javaFullList)
            {
                if (java != null)
                {
                    javaList.Add(java);
                }
            }

            JavaCombo.ItemsSource = javaList;

            MessageBox.Show("找到了 " + javaList.Count().ToString() + " 个Java运行环境");
            javaScanning=false;

            if (javaList.Count() > 0)
            {
                JavaCombo.SelectedItem = javaList.FirstOrDefault();
                Core.JavaPathRecord(javaList);
            }

        }

        private void RefreshVersionList()
        {
            gamelist = Core.core.VersionLocator.GetAllGames().ToList();
            verCombo.ItemsSource = gamelist;
            if (gamelist.Count() > 0)
            {
                verCombo.SelectedItem = gamelist.FirstOrDefault();
            }

        }

        public async void JavaPathLoaderAsync()
        {
            var rootPath = Environment.CurrentDirectory + "\\HL_config.json";

        configLoader:
            if (File.Exists(rootPath))
            {
                string jsonText = File.ReadAllText(rootPath);

                Dictionary<string, dynamic>? javaDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonText);

                javaList = javaDict["javaPath"].ToObject<List<string>>();
            }
            else
            {
                File.Create(rootPath).Close();
                File.WriteAllText(rootPath, "{\"javaPath\":[]}");
                MessageBox.Show("这可能是您第一次运行 HarbourLauncher\n因此我们将自动为您检测运行环境\n请稍候...", "程序初始化");
                await RefreshJavaList();

                goto configLoader;
            }


        }

        private void GameSettings_Click(object sender, RoutedEventArgs e)
        {
            GameSettingsFlyOuts.IsOpen = true;
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flipview = ((FlipView)sender);
            switch (flipview.SelectedIndex)
            {
                case 0:
                    flipview.BannerText = "欢迎来到 HarbourLauncher";
                    break;
                case 1:
                    flipview.BannerText = "启动器早期构建版本 EA 114";
                    break;
                case 2:
                    flipview.BannerText = "Friendship Studio 2022";
                    break;
            }
        }

        private void MaxMem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private async void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (JavaCombo.SelectedIndex == -1 || verCombo.SelectedIndex == -1)
            {
                MessageBox.Show("缺少关键参数，不能启动", "启动失败");
                return;
            }

            if (verCombo.SelectedItem is not VersionInfo versionInfo) return;

            Core.PlayerNameRecord(playerName.Text.Trim());

            var launchSettings = new LaunchSettings
            {
                FallBackGameArguments = new GameArguments
                {
                    GcType = GcType.G1Gc,
                    JavaExecutable = JavaCombo.SelectedItem.ToString(),
                    Resolution = new ResolutionModel
                    {
                        Height = 600,
                        Width = 800
                    },
                    MinMemory = 512,
                    MaxMemory = int.Parse(maxMem.Text.Trim()) 
                },
                Version = versionInfo.Id,
                GameName = versionInfo.Name,
                VersionInsulation = false,
                GameResourcePath = Core.core.RootPath,
                GamePath = Core.core.RootPath,
                VersionLocator = Core.core.VersionLocator,

                Authenticator = new OfflineAuthenticator //离线认证
                {
                    Username = playerName.Text.Trim(), //离线用户名
                    LauncherAccountParser = Core.core.VersionLocator.LauncherAccountParser
                }


            };

            Core.core.LaunchLogEventDelegate += Core_LaunchLogEventDelegate;
            Core.core.GameLogEventDelegate += Core_GameLogEventDelegate;
            Core.core.GameExitEventDelegate += Core_GameExitEventDelegate;

            var result = await Core.core.LaunchTaskAsync(launchSettings);



            LogRecord(result.Error?.Exception.ToString());
        }

        private void Core_GameExitEventDelegate(object sender, GameExitEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { LogRecord("Game exited."); });
        }

        private void Core_GameLogEventDelegate(object sender, GameLogEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { LogRecord($"[Game Log] - {e.Content}\n"); });
        }

        private void Core_LaunchLogEventDelegate(object sender, LaunchLogEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { LogRecord($"[Bobcat Log] - {e.Item}\n"); });
        }

        private void LogRecord(string info)
        {
            logRecord:
            try
            {
                File.WriteAllText(Environment.CurrentDirectory + "\\logs\\lastest.txt", info);
                Console.WriteLine("写入日志成功");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("未找到日志目录，已为你重新创建一个");
                Directory.CreateDirectory("logs");
                File.Create(Environment.CurrentDirectory + "\\logs\\lastest.txt").Close();
                goto logRecord;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("未找到日志文件，已为你重新创建一个");
                File.Create(Environment.CurrentDirectory + "\\logs\\lastest.txt").Close();
                goto logRecord;
            }

        }

        private async void JavaRefresh_Click(object sender, RoutedEventArgs e)
        {
            await RefreshJavaList();
        }

        private void IsAuth_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn)
                {
                    OnlineLoginSettings.Visibility = Visibility.Visible;
                    OfflineLoginSettings.Visibility = Visibility.Hidden;
                    loginMode = LoginMode.Online;
                }
                else
                {
                    OnlineLoginSettings.Visibility = Visibility.Hidden;
                    OfflineLoginSettings.Visibility = Visibility.Visible;
                    loginMode = LoginMode.Offline;
                }
            }
        }

        private void LauncherSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能开发中", "早期构建版本");
        }

        private void HarbourMarket_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能开发中", "早期构建版本");
        }

        private void MicrosoftLogin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能开发中\n请使用离线登录", "早期构建版本");
        }

        private void playerNameInit()
        {
            var rootPath = Environment.CurrentDirectory + "\\HL_config.json";

            string jsonText = File.ReadAllText(rootPath);

            Dictionary<string, dynamic>? javaDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonText);

            playerNameInit:
            try
            {
                playerName.Text = javaDict["playerName"];
            }
            catch (KeyNotFoundException)
            {
                javaDict.Add("playerName", "Steve");
                goto playerNameInit;
            }
            
        }
    }

}
