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
using System.ComponentModel;
using HarbourLauncher_Reloaded.UI;

namespace HarbourLauncher_Reloaded
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            UI.Index index = new UI.Index();
            MainPage.Content = index;

        }

    }

}
