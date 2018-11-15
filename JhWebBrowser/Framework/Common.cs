using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JhWebBrowser
{
   public class Common
   {
       public static readonly Common instance = new Common();
       
       /// <summary>
       /// 记录选择了某一个应用 一、博客园 二、新浪微博 三、网易邮箱
       /// </summary>
       public int Apply { get; set; }
   }
}
