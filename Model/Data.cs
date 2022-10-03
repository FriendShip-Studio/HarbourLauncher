using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//定义数据类型
namespace HarbourLauncher_Reloaded.Model
{
    public class Data
    {
        //数据类型
        public class Java
        {
            public string path;
            public string name;
        }
        public class MicrosoftUser
        {
            /// <summary>
            /// 刷新令牌
            /// </summary>
            string refresh_token;
            /// <summary>
            /// 用户名
            /// </summary>
            string name;
            /// <summary>
            /// 皮肤Uri
            /// </summary>
            Uri skin;

        }

        /// <summary>
        /// Java列表
        /// </summary>
        public List<Java> javaList = new();
        //内存
        /// <summary>
        /// 是否为自动内存
        /// </summary>
        public bool isAutoMemory=false;
        /// <summary>
        /// 内存大小
        /// </summary>
        public int memory=1024;

        //登录
        /// <summary>
        /// 是否为微软登录
        /// </summary>
        public bool isMicrosoft=false;
        /// <summary>
        /// 离线登录用户名
        /// </summary>
        public List<string> offLineUsers = new() { "Steve","Alex" };
        public List<MicrosoftUser> microsoftUsers = new();



        public string indexBackGroundURI  =  "images/index.png";
        

    }
}
