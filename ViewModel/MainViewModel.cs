using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using HarbourLauncher_Reloaded.Model;


namespace HarbourLauncher_Reloaded.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //定义数据与数据初始化
        public MainViewModel()
        {
            Data data = new Data();
            
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        //定义行为逻辑
    }
}
