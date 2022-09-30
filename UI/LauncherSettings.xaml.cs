using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;
using System.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using HarbourLauncher_Reloaded.GameBasis;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace HarbourLauncher_Reloaded.UI
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class LauncherSettings : Page
    {
        public Index index;


        public LauncherSettings()
        {
            index = new Index();
            InitializeComponent();
            LoadAutoMemStat();
            LoadWindowSize();
            memToLaunch.Text = index.maxMem.Text + " MB";
        }


        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            index=new Index();
            HamburgerMenuControl.Content = e.InvokedItem;

            if (e.IsItemOptions)
            {
                NavigationService.GoBack();
            }

            if (!e.IsItemOptions && HamburgerMenuControl.IsPaneOpen)
            {
                HamburgerMenuControl.SetCurrentValue(HamburgerMenu.IsPaneOpenProperty, false);
            }
            RefreshMemInfo();
            List<int> windowSize = new();
            try
            {
                windowSize.Add(int.Parse(WindowWidth.Text));
                windowSize.Add(int.Parse(WindowHeight.Text));
            }
            catch (System.FormatException)
            {
                windowSize.Add(800);
                windowSize.Add(600);
            }
            
            Core.WindowSizeRecord(windowSize);
            memToLaunch.Text = index.maxMem.Text + " MB";

        }

        private void WindowSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void IndexBG_Bamboo_Click(object sender, RoutedEventArgs e)
        {
        }

        private void IndexBG_Wxg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IsAutoMem_Toggled(object sender, RoutedEventArgs e)
        {
            RefreshMemInfo();
            SetAutoMem();
        }

        public void SetAutoMem()
        {
            MemoryInfo memoryInfo = SystemInfoHelper.GetWindowsMemoryStatus();
            if (IsAutoMem.IsOn)
            {
                if (memoryInfo.Free > 1024)
                {
                    Core.AutoMemRecord(true, (memoryInfo.Free * 0.5).ToString("0"));
                }
                else
                {
                    Core.AutoMemRecord(true, "1024");
                }
            }
            else
            {
                Core.AutoMemRecord(IsAutoMem.IsOn, "1024");
            }
            LoadAutoMemStat();
        }

        private void RefreshMemInfo()
        {
            MemoryInfo memoryInfo = SystemInfoHelper.GetWindowsMemoryStatus();
            int memPercent = (int)(memoryInfo.Percentage * 100);
            
            MemProg.Value = memPercent;
            UsedMem.Text = ((memoryInfo.Used)/ 1024).ToString("0.00");
            TotalMem.Text = ((memoryInfo.Total) / 1024).ToString("0.00");
        }

        private void MemInfoRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshMemInfo();
        }

        private void LoadAutoMemStat()
        {
            var rootPath = Environment.CurrentDirectory + "\\HL_config.json";

            string jsonText = File.ReadAllText(rootPath);

            Dictionary<string, dynamic>? javaDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonText);

        autoMemInit:
            try
            {
                IsAutoMem.IsOn = javaDict["autoMem"];
                memToLaunch.Text = javaDict["maxMem"] + " MB";
            }
            catch (KeyNotFoundException)
            {
                if (javaDict.ContainsKey("autoMem"))
                {
                    javaDict.Add("maxMem", index.maxMem.Text);
                }
                else 
                {
                    javaDict.Add("autoMem", false); 
                }
                
                goto autoMemInit;
            }
        }

        private void LoadWindowSize()
        {
            var rootPath = Environment.CurrentDirectory + "\\HL_config.json";

            string jsonText = File.ReadAllText(rootPath);

            Dictionary<string, dynamic>? javaDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonText);

        autoMemInit:
            try
            {
                WindowWidth.Text = javaDict["windowSize"][0].ToString();
                WindowHeight.Text = javaDict["windowSize"][1].ToString();
            }
            catch (KeyNotFoundException)
            {
                List<int> windowSize = new();
                windowSize.Add(800);
                windowSize.Add(600);
                javaDict["windowSize"] =windowSize;

                goto autoMemInit;
            }
        }

    }
}
