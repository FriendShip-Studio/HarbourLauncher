using Gac;
using MahApps.Metro.Controls;
using SquareMinecraftLauncher.Core.OAuth;
using SquareMinecraftLauncher.Minecraft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HarbourLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {

        
        private Tools tools = new Tools();//
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

        #endregion 启动模式枚举

        #region 启动参数

        public string Minecraft_Token;
        public string uuid;
        public string name;
        public bool Online = false;

        #endregion 启动参数

        #region log and error

        private StreamWriter streamWriter1 = new(@"error.txt");
        private StreamWriter streamWriter = new(@"log.txt");

        #endregion log and error

        private SquareMinecraftLauncher.MinecraftDownload minecraftDownload = new();
        private Gac.DownLoadFile downLoadFile = new Gac.DownLoadFile();
        private ImageBrush redstone_lamp = new ImageBrush() { ImageSource = new BitmapImage(new Uri(@"Assets/redstone_lamp.png", UriKind.RelativeOrAbsolute)) };
        private ImageBrush redstone_lamp_on = new ImageBrush() { ImageSource = new BitmapImage(new Uri(@"Assets/redstone_lamp_on.png", UriKind.RelativeOrAbsolute)) };
        public MCVersionList[] mcVersionList = new MCVersionList[1].ToArray();
        private int tryCount = 0;
        GacDownloader gac1 = new GacDownloader(36);

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

        private VersionEnum versionEnum = new();

        #endregion 版本枚举

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

            Binding binding = new Binding();//创建Binding实例
            binding.Source = gac1;//指定数据源
            binding.Path = new PropertyPath("downloadPercent");//指定访问路径 
            binding.Mode = BindingMode.OneWay;
            downloadper.SetBinding(ProgressBar.ValueProperty, binding);

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

        /// <summary>
        /// 获取我的世界版本列表
        /// </summary>
        public async void MinecraftListGet()
        {
            try
            {
                #region 获取下载版本列表

                var v = await tools.GetMCVersionList();


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

                #endregion 筛选

                mcVersionDataGrid.ItemsSource = v;


                #endregion 获取下载版本列表
            }
            catch
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
                    streamWriter.WriteLine("");
                    streamWriter1.WriteLine("");

                    if (tools.GetMissingAsset(verCombo.Text).Count() != 0 || tools.GetMissingFile(verCombo.Text).Count() != 0)
                    {
                        StartGame.Title = "正在补全文件";
                        FileComplete();
                    }
                    
                    await Task.Run(() =>
                    {
                        while (gac1.GetEndDownload() != true || gac1.GetEndDownload() != true)
                        {
                            Thread.Sleep(1000);
                        }
                        Dispatcher.Invoke(() => {
                            if (tools.GetMissingFile(verCombo.Text).Count() != 0||tools.GetMissingAsset(verCombo.Text).Count()!=0)
                            {
                                MessageBox.Show("文件补全失败");
                            }
                            else
                            {
                                if (JavaCombo.Text != string.Empty && verCombo.Text != string.Empty)
                                    if (loginMode == LoginMode.Offline && playerName.Text != string.Empty)
                                    {
                                        StartGame.Title = "正在启动";
                                        OfflineLogin();
                                    }
                                    else if (loginMode == LoginMode.Microsoft)
                                    {
                                        StartGame.Title = "正在启动";
                                        MicrosoftL();

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

                           
                        });
                        
                    });

                    
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

        #region 游戏事件

        /// <summary>
        /// 日志事件
        /// </summary>
        /// <param name="Log"></param>
        private void Game_LogEvent(Game.Log Log)
        {
            streamWriter.WriteLine(Log.Message);
            //MessageBox.Show("日志");
            //MessageBox.Show(Log.Message);
        }

        /// <summary>
        /// 崩溃事件
        /// </summary>
        /// <param name="error"></param>
        private void Game_ErrorEvent(Game.Error error)
        {
            streamWriter1.WriteLine(error.Message);
            //MessageBox.Show("错误");
            //MessageBox.Show(error.Message);
            //MessageBox.Show(error.SeriousError);
        }

        #endregion 游戏事件

        #region 登录
        public async void MicrosoftL()
        {
            Game game = new();//声明对象
            await game.StartGame(verCombo.Text, JavaCombo.SelectedValue.ToString(), int.Parse(maxMem.Text.Trim()), name, uuid, Minecraft_Token, "", "");
            game.ErrorEvent += Game_ErrorEvent;
            game.LogEvent += Game_LogEvent;
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

        #endregion 登录

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
            var v2 = tools.GetMissingFile(verCombo.Text);
            var v3 = tools.GetMissingAsset(verCombo.Text);
            List<MCDownload> md = new List<MCDownload>();
            foreach (var v in v2)
            {
                md.Add(v);
            }
            foreach (var v in v3)
            {
                md.Add(v);
            }
            gac1.AddDownload(md.ToArray());
            gac1.StartDownload();


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
            tools.DownloadSourceInitialization(DownloadSource.bmclapiSource);
            var v = minecraftDownload.MCjarDownload(((MCVersionList)mcVersionDataGrid.SelectedItem).id);
            var v1 = minecraftDownload.MCjsonDownload(((MCVersionList)mcVersionDataGrid.SelectedItem).id);
            gac1.AddDownload(new MCDownload[] { v, v1 });
            gac1.StartDownload();
        }

        /// <summary>
        /// 多线程下载
        /// </summary>
        public class GacDownloader
        {
            public float downloadPercent { internal set; get; } = 100;
            private Thread[] Threads = new Thread[0];
            private MCDownload[] download = new MCDownload[0];
            private int EndDownload = 0;

            public GacDownloader(int thread, SquareMinecraftLauncher.Minecraft.MCDownload[] download)
            {
                Threads = new Thread[thread];
                this.download = download;
            }

            public GacDownloader(SquareMinecraftLauncher.Minecraft.MCDownload[] download)
            {
                Threads = new Thread[3];
                this.download = download;
            }
            public GacDownloader(int thread)
            {
                Threads = new Thread[thread];
            }
            public void AddDownload(MCDownload[] download)
            {
                this.download = download;
            }
            private int ADindex = 0;

            private MCDownload AssignedDownload()
            {
                if (ADindex == download.Length) return null;
                ADindex++;
                return download[ADindex - 1];
            }

            public void StartDownload()
            {
                for (int i = 0; i < Threads.Length; i++)
                {
                    Threads[i] = new Thread(DownloadProgress);
                    Threads[i].IsBackground = true;
                    Threads[i].Start();//启动线程
                }
                
            }

            private async void DownloadProgress()
            {
                List<FileDownloader> files = new List<FileDownloader>();
                for (int i = 0; i < 3; i++)
                {
                    MCDownload download = AssignedDownload();//分配下载任务
                    try
                    {
                        if (download != null)
                        {
                            
                            FileDownloader fileDownloader = new FileDownloader(download.Url, download.path.Replace(Path.GetFileName(download.path), ""), Path.GetFileName(download.path));//增加下载
                            //if (files.Count != 0)
                            //    downloadPercent = end / files.Count;
                            fileDownloader.download(null);
                            files.Add(fileDownloader);
                            
                        }
                    }
                    catch (Exception ex)//当出现下载失败时，忽略该文件
                    {
                        MessageBox.Show("下载失败"+ex.Message);
                    }
                }
                await Task.Factory.StartNew(() =>
                {
                    while (true)//循环检测当前线程files.Count个下载任务是否下载完毕
                    {
                        int end = 0;
                        for (int i = 0; i < files.Count; i++)
                        {
                            if (files[i].download(null) == files[i].getFileSize())
                            {
                                end++;
                                if (files.Count != 0)
                                    downloadPercent = end / files.Count * 100;
                                
                            }
                        }
                        Console.WriteLine(EndDownload);

                        if (end == files.Count)//完成则递归当前函数
                        {
                            EndDownload += files.Count;
                            if (files.Count != 0)
                                downloadPercent = end / files.Count *100;
                            DownloadProgress();//递归
                            return;
                        }
                        Thread.Sleep(1000);
                    }
                });
                if (files.Count == 0) return;
                
            }

            public bool GetEndDownload()
            {
                return EndDownload == download.Length ? true : false;
            }
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