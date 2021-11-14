using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using SquareMinecraftLauncher.Minecraft;
using SquareMinecraftLauncher.Core;
using System.Text.RegularExpressions;
using System.Diagnostics;
using SquareMinecraftLauncher.Core.OAuth;

namespace HarbourLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {
        Tools tools = new Tools();
        public Dictionary<string, string> javaList = new();
        #region 启动模式枚举
        /// <summary>
        /// 启动模式枚举
        /// </summary>
        public enum LoginMode
        {
            /// <summary>
            /// 离线
            /// </summary>
            Offline,
            /// <summary>
            /// mojang登录
            /// </summary>
            Online,
            /// <summary>
            /// 微软登录
            /// </summary>
            Microsoft,
        }
        public LoginMode loginMode = new();
        #endregion
        #region 启动参数
        public string Minecraft_Token;
        public string uuid;
        public string name;
        public bool Online = false;
        #endregion
        SquareMinecraftLauncher.MinecraftDownload minecraftDownload = new();
        Gac.DownLoadFile downLoadFile = new Gac.DownLoadFile();
        ImageBrush redstone_lamp = new ImageBrush() { ImageSource = new BitmapImage(new Uri(@"Assets/redstone_lamp.png", UriKind.RelativeOrAbsolute)) };
        ImageBrush redstone_lamp_on = new ImageBrush() { ImageSource = new BitmapImage(new Uri(@"Assets/redstone_lamp_on.png", UriKind.RelativeOrAbsolute)) };
        public MCVersionList[] mcVersionList = new MCVersionList[1].ToArray();
        int tryCount = 0;
        #region 版本枚举
        public enum VersionEnum
        {
            /// <summary>
            /// 快照版
            /// </summary>
            alpha,
            /// <summary>
            /// 正式版
            /// </summary>
            release,
            /// <summary>
            /// 所有版本
            /// </summary>
            all
        }
        VersionEnum versionEnum = new();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            ServicePointManager.DefaultConnectionLimit = 512;
            Initialize();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            versionEnum = VersionEnum.release;
            tools.DownloadSourceInitialization(DownloadSource.bmclapiSource);
            MinecraftListGet();
            try
            {
                if (tools.GetJavaPath().Count() != 0)
                    JavaCombo.ItemsSource = tools.GetJavaPath();
                JavaCombo.SelectedItem = JavaCombo.Items[0];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "初始化错误");
            }

            try
            {
                if (tools.GetAllTheExistingVersion().Count() != 0)
                {
                    verCombo.ItemsSource = tools.GetAllTheExistingVersion();
                    IndexverCombo.ItemsSource = tools.GetAllTheExistingVersion();
                }
                if (verCombo.Items.Count != 0)
                {
                    verCombo.SelectedItem = verCombo.Items[0];
                    IndexverCombo.SelectedItem = verCombo.SelectedItem;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "初始化错误");
            }
        }
        /// <summary>
        /// 获取所有java
        /// </summary>
        /// <returns></returns>
        public JavaVersion[] GetAllJavaVersion()
        {
            //未完成
            return null;
        }
        public async void MinecraftListGet()
        {
            try
            {
                #region 获取下载版本列表
                await tools.GetMCVersionList();
                await tools.GetMCVersionList().ContinueWith(x =>
                {
                    mcVersionList = tools.GetMCVersionList().Result;
                    //MCVersionList[] mcVer = new MCVersionList[tools.GetMCVersionList().Result.Count()];
                    #region 筛选
                    //if (versionEnum == VersionEnum.release)
                    //{
                    //    foreach(var i in mcVersionList)
                    //    {
                    //        if(i.type == "正式版")
                    //        {

                    //            mcVer.Append(i);   
                    //        }
                    //    }

                    //}
                    //else if(versionEnum == VersionEnum.release)
                    //{
                    //    foreach(var i in mcVersionList)
                    //    {
                    //        if(i.type == "快照版")
                    //        {
                    //            mcVer.Append(i);
                    //        }
                    //    }

                    //}
                    //else if (versionEnum == VersionEnum.all)
                    //{
                    //    mcVer = mcVersionList;
                    //}
                    //mcVersionList = mcVer;
                    #endregion
                    Dispatcher.Invoke(() => { mcVersionDataGrid.ItemsSource = mcVersionList; });

                });
                #endregion
            }
            catch (Exception e)
            {
                tryCount++;
                if (tryCount < 5)
                {
                    MinecraftListGet();
                }
                else
                {
                    MessageBox.Show("获取版本列表错误");
                }

            }

        }

        private void Support(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/BiDuang/");
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.InvokedItem;

            if (!e.IsItemOptions && this.HamburgerMenuControl.IsPaneOpen)
            {
                // You can close the menu if an item was selected
                // this.HamburgerMenuControl.SetCurrentValue(HamburgerMenuControl.IsPaneOpenProperty, false);
            }
        }
        //启动
        private async void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (StartGame.Title == "启动游戏")
            {
                try
                {
                    StartGame.Title = "正在补全文件";
                    FileComplete();

                    if (JavaCombo.Text != string.Empty && verCombo.Text != string.Empty)
                        if (loginMode == LoginMode.Offline && playerName.Text != string.Empty)
                        {
                            StartGame.Title = "正在启动";
                            OfflineLogin();
                        }
                        else if (loginMode == LoginMode.Microsoft)
                        {
                            StartGame.Title = "正在启动";
                            Game game = new();//声明对象
                            await game.StartGame(verCombo.Text, JavaCombo.SelectedValue.ToString(), int.Parse(maxMem.Text.Trim()), name, uuid, Minecraft_Token, "", "");
                            game.ErrorEvent += Game_ErrorEvent;
                            game.LogEvent += Game_LogEvent;
                        }
                        else if (loginMode == LoginMode.Online)
                        {
                            StartGame.Title = "正在启动";
                            MojangLogin();
                        }
                        else
                        {
                            MessageBox.Show("请输入玩家名称", "离线登录");
                        }
                    else
                    {
                        MessageBox.Show("环境配置错误", "启动失败");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "启动失败");
                }
                finally
                {
                    StartGame.Title = "启动游戏";
                }
            }



        }
        /// <summary>
        /// 日志事件
        /// </summary>
        /// <param name="Log"></param>
        private void Game_LogEvent(Game.Log Log)
        {
            //MessageBox.Show("日志");
            //MessageBox.Show(Log.Message);
        }
        /// <summary>
        /// 崩溃事件
        /// </summary>
        /// <param name="error"></param>
        private void Game_ErrorEvent(Game.Error error)
        {
            //MessageBox.Show("错误");
            //MessageBox.Show(error.Message);
            //MessageBox.Show(error.SeriousError);
        }

        //微软
        public void MicrosoftLogin()
        {
            loginMode = LoginMode.Microsoft;
            bool auto = false;
            if (IsLocalAccount.IsOn)
            {
                auto = true;    //true是登录电脑设置里的微软账户，false是登录其他账户
            }
            MicrosoftLogin microsoftLogin = new();
            Xbox XboxLogin = new();
            try
            {
                Minecraft_Token = new MinecraftLogin().GetToken(XboxLogin.XSTSLogin(XboxLogin.GetToken(microsoftLogin.GetToken(microsoftLogin.Login(auto)).access_token)));
                MicrosoftLoginStat.Text = "微软登录成功";
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message, "微软登录错误");
                loginMode = LoginMode.Online;
                return;
            }

            MinecraftLogin minecraftlogin = new();
            MinecraftLoginToken Minecraft = minecraftlogin.GetMincraftuuid(Minecraft_Token);
            uuid = Minecraft.uuid;
            name = Minecraft.name;
        }
        //正版
        public async void MojangLogin()
        {
            Game game = new();//声明对象
            await game.StartGame(verCombo.Text, JavaCombo.SelectedValue.ToString(), int.Parse(maxMem.Text.Trim()), MojangAccount.Text.Trim(), MojangPassword.Password.Trim());
            game.ErrorEvent += Game_ErrorEvent;
            game.LogEvent += Game_LogEvent;
        }
        //离线
        public async void OfflineLogin()
        {
            Game game = new Game();//声明对象
            await game.StartGame(verCombo.Text, JavaCombo.SelectedValue.ToString(), int.Parse(maxMem.Text.Trim()), playerName.Text.Trim());
            game.ErrorEvent += Game_ErrorEvent;
            game.LogEvent += Game_LogEvent;
        }

        private void MaxMem_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void verCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IndexverCombo.SelectedItem = verCombo.SelectedItem;
        }

        private void IndexverCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            verCombo.SelectedItem = IndexverCombo.SelectedItem;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn)
                {
                    AuthLogin.Visibility = Visibility.Visible;
                    UnAuthLogin.Visibility = Visibility.Hidden;
                    loginMode = LoginMode.Online;
                }
                else
                {
                    AuthLogin.Visibility = Visibility.Hidden;
                    UnAuthLogin.Visibility = Visibility.Visible;
                    loginMode = LoginMode.Offline;
                }
            }
        }

        private void MicrosoftAuth_Click(object sender, RoutedEventArgs e)
        {
            MicrosoftLogin();
        }
        public void FileComplete()
        {
            Gac.DownLoadFile downLoadFile = new Gac.DownLoadFile();
            foreach (var i in tools.GetMissingFile(verCombo.Text))
            {
                downLoadFile.AddDown(i.Url, i.path);
            }
            downLoadFile.StartDown(0);
        }
        /// <summary>
        /// 游戏文件下载bmclapi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // changeTobmclapi.Background = redstone_lamp_on;
            tools.DownloadSourceInitialization(DownloadSource.bmclapiSource);
        }

        /// <summary>
        /// 下载按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var v = minecraftDownload.MCjarDownload(((MCVersionList)mcVersionDataGrid.SelectedItem).id);
            downLoadFile.AddDown(v.Url, v.path);
            var v1 = minecraftDownload.MCjsonDownload(((MCVersionList)mcVersionDataGrid.SelectedItem).id);
            downLoadFile.AddDown(v1.Url, v1.path);
            downLoadFile.StartDown(3);
            downLoadFile.doSendMsg += DownLoadFile_doSendMsg;


        }

        private void DownLoadFile_doSendMsg(Gac.DownMsg msg)
        {

        }

        /// <summary>
        /// 重新找版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (tools.GetAllTheExistingVersion().Count() != 0)
            {
                verCombo.ItemsSource = tools.GetAllTheExistingVersion();
                IndexverCombo.ItemsSource = tools.GetAllTheExistingVersion();
            }
        }
        private void MicrosoftSettings_Click(object sender, RoutedEventArgs e)
        {
            MicrosoftSettingsFlyout.IsOpen = true;
        }
    }

}

