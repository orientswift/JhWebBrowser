using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using Microsoft.Win32;
using System.Security;
using System.Windows.Interop;
using System.Diagnostics;

namespace JhWebBrowser
{
    /// <summary>
    /// WebBrowserOverlayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowserOverlayWindow : Window
    {
        #region 定义IE版本的枚举
        /// <summary>
        /// 定义IE版本的枚举
        /// </summary>
        private enum IeVersion
        {
            标准ie11,//11000(0x2af8)× 
            强制ie10,//10001 (0x2711) Internet Explorer 10。网页以IE 10的标准模式展现，页面!DOCTYPE无效 
            标准ie10,//10000 (0x02710) Internet Explorer 10。在IE 10标准模式中按照网页上!DOCTYPE指令来显示网页。Internet Explorer 10 默认值。
            强制ie9,//9999 (0x270F) Windows Internet Explorer 9. 强制IE9显示，忽略!DOCTYPE指令 
            标准ie9,//9000 (0x2328) Internet Explorer 9. Internet Explorer 9默认值，在IE9标准模式中按照网页上!DOCTYPE指令来显示网页。
            强制ie8,//8888 (0x22B8) Internet Explorer 8，强制IE8标准模式显示，忽略!DOCTYPE指令 
            标准ie8,//8000 (0x1F40) Internet Explorer 8默认设置，在IE8标准模式中按照网页上!DOCTYPE指令展示网页
            标准ie7//7000 (0x1B58) 使用WebBrowser Control控件的应用程序所使用的默认值，在IE7标准模式中按照网页上!DOCTYPE指令来展示网页
        } 
        #endregion
        #region 设置WebBrowser的默认版本
        /// <summary>
        /// 设置WebBrowser的默认版本
        /// </summary>
        /// <param name="ver">IE版本</param>
        private void SetIE(IeVersion ver)
        {
            string productName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;//获取程序名称

            object version;
            switch (ver)
            {
                case IeVersion.标准ie7:
                    version = 0x1B58;
                    break;
                case IeVersion.标准ie8:
                    version = 0x1F40;
                    break;
                case IeVersion.强制ie8:
                    version = 0x22B8;
                    break;
                case IeVersion.标准ie9:
                    version = 0x2328;
                    break;
                case IeVersion.强制ie9:
                    version = 0x270F;
                    break;
                case IeVersion.标准ie10:
                    version = 0x02710;
                    break;
                case IeVersion.强制ie10:
                    version = 0x2711;
                    break;
                case IeVersion.标准ie11:
                    version = 0x2af8;
                    break;
                default:
                    version = 0x1F40;
                    break;
            }

            RegistryKey key = Registry.CurrentUser;
            RegistryKey software =
                key.CreateSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION\" + productName);
            if (software != null)
            {
                software.Close();
                software.Dispose();
            }
            RegistryKey wwui =
                key.OpenSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            //该项必须已存在
            if (wwui != null) 
                wwui.SetValue(productName, version, RegistryValueKind.DWord);
        } 
        #endregion
        #region 获取本机IE版本
        /// <summary>
        /// 获取本机IE版本
        /// </summary>
        private const string InternetExplorerRootKey = @"Software\Microsoft\Internet Explorer";

        public static int GetInternetExplorerMajorVersion()
        {
            int result;

            result = 0;

            try
            {
                RegistryKey key;

                key = Registry.LocalMachine.OpenSubKey(InternetExplorerRootKey);

                if (key != null)
                {
                    object value;

                    value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);

                    if (value != null)
                    {
                        string version;
                        int separator;

                        version = value.ToString();
                        separator = version.IndexOf('.');
                        if (separator != -1)
                        {
                            int.TryParse(version.Substring(0, separator), out result);
                        }
                    }
                }
            }
            catch (SecurityException)
            {
                // The user does not have the permissions required to read from the registry key.
            }
            catch (UnauthorizedAccessException)
            {
                // The user does not have the necessary registry rights.
            }
            return result;
        } 
        #endregion
        #region 制定webbrowser内核版本 
        public void DraftEdition()
        {
            int ieResult = GetInternetExplorerMajorVersion();
            if (ieResult == 11)
            {
                SetIE(IeVersion.标准ie11);
            }
            else if (ieResult == 10)
            {
                SetIE(IeVersion.强制ie10);
            }
            else if (ieResult == 9)
            {
                SetIE(IeVersion.强制ie9);
            }
            else
            {
                SetIE(IeVersion.强制ie8);
            }
        } 
        #endregion
        public WebBrowserOverlayWindow()
        {
            InitializeComponent();
            this.Loaded += WebBrowserOverlayWindow_Loaded;
            this.wfBrowser.CanGoBackChanged += wfBrowser_CanGoBackChanged;
            this.wfBrowser.CanGoForwardChanged += wfBrowser_CanGoForwardChanged;
        }
        /// <summary>
        /// 读取注册表看看是否修改webbrowser内核
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsRegeditKeyExit()
        {
            try
            {
                string[] subkeyNames;
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey software = hkml.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                subkeyNames = software.GetValueNames();
                //取得该项下所有键值的名称的序列，并传递给预定的数组中
                foreach (string keyName in subkeyNames)
                {
                    if (keyName == "JhWebBrowser.exe") //判断键值的名称
                    {
                        hkml.Close();
                        return true;
                    }
                }
                software.SetValue("JhWebBrowser.exe", 32768, RegistryValueKind.DWord);
                hkml.Close();
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        void WebBrowserOverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DraftEdition();
        }
        //判断及设定 前进按钮是否可用
        void wfBrowser_CanGoForwardChanged(object sender, EventArgs e)
        {
            this.btnGoForward.IsEnabled = wfBrowser.CanGoForward;
        }
        //判断及设定 后退按钮是否可用
        void wfBrowser_CanGoBackChanged(object sender, EventArgs e)
        {
            this.btnGoBack.IsEnabled = wfBrowser.CanGoBack;
        }
         public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register("TargetElement", typeof(FrameworkElement), typeof(WebBrowserOverlayWindow), new PropertyMetadata(TargetElementPropertyChanged));
        public FrameworkElement TargetElement
        {
            get
            {
                return GetValue(TargetElementProperty) as FrameworkElement;
            }
            set
            {
                SetValue(TargetElementProperty, value);
            }
        }


        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(WebBrowserOverlayWindow), new PropertyMetadata(SourcePropertyChanged));
        public string Source
        {
            get
            {
                return GetValue(SourceProperty) as string;
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }
        private static void SourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var webBrowserOverlayWindow = sender as WebBrowserOverlayWindow;

            if (webBrowserOverlayWindow != null)
            {
                webBrowserOverlayWindow.wfBrowser.Navigate(args.NewValue as string);
            }
        }

        private static void TargetElementPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var oldTargetElement = args.OldValue as FrameworkElement;
            var webBrowserOverlayWindow = sender as WebBrowserOverlayWindow;
            var mainWindow = Window.GetWindow(webBrowserOverlayWindow.TargetElement);

            if (webBrowserOverlayWindow != null && mainWindow != null)
            {
                webBrowserOverlayWindow.Owner = mainWindow;
                webBrowserOverlayWindow.Owner.LocationChanged += webBrowserOverlayWindow.PositionAndResize;
                webBrowserOverlayWindow.TargetElement.LayoutUpdated += webBrowserOverlayWindow.PositionAndResize;

                if (oldTargetElement != null)
                    oldTargetElement.LayoutUpdated -= webBrowserOverlayWindow.PositionAndResize;

                webBrowserOverlayWindow.PositionAndResize(sender, new EventArgs());

                if (webBrowserOverlayWindow.TargetElement.IsVisible && webBrowserOverlayWindow.Owner.IsVisible)
                {
                    webBrowserOverlayWindow.Show();
                }

                webBrowserOverlayWindow.TargetElement.IsVisibleChanged += (x, y) =>
                {
                    if (webBrowserOverlayWindow.TargetElement.IsVisible && webBrowserOverlayWindow.Owner.IsVisible)
                    {
                        webBrowserOverlayWindow.Show();
                    }
                    else
                    {
                        webBrowserOverlayWindow.Hide();
                    }
                };
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Owner.LocationChanged -= PositionAndResize;
            if (TargetElement != null)
            {
                TargetElement.LayoutUpdated -= PositionAndResize;
            }
        }

        private void PositionAndResize(object sender, EventArgs e)
        {
            if (TargetElement != null && TargetElement.IsVisible)
            {
                var point = TargetElement.PointToScreen(new System.Windows.Point());
                Left = point.X;
                Top = point.Y;

                Height = TargetElement.ActualHeight;
                Width = TargetElement.ActualWidth;
            }
        }
        /// <summary>
        /// 按下回车Enter
        /// / /导航在地址框中的URL时，回车键的同时按下ToolStripTextBox具有焦点。wpf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void tbInput_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        Navigate(tbInput.Text);
        //    }
        //}
        private void tbInput_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Navigate(tbInput.Text);
            }
        }
        /// <summary>
        /// 点击搜索按钮
        /// 导航在地址框中的URL时，单击“按钮”按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(tbInput.Text!=null)
            {
                Navigate(tbInput.Text);
            }
        } 
        /// <summary>
        /// 执行地址显示 导航到指定的URL是否有效。
        /// </summary>
        /// <param name="address"></param>
        private void Navigate(string address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }
            try
            {
                wfBrowser.Navigate(new Uri(address));
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }
        /// <summary>
        /// 清除RUL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            tbInput.Text = null;
        }
        /// <summary>
        /// 释放内存和显示url 更新的URL textboxaddress在导航。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wfBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            try
            {
                tbInput.Text = wfBrowser.Url.ToString();
                //LbTitle.Content = wfBrowser.Document.Title;
                IntPtr pHandle = GetCurrentProcess();
                SetProcessWorkingSetSize(pHandle, -1, -1); 
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        private void wfBrowser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //禁止打开本地的默认浏览器
            e.Cancel = true;
            wfBrowser.Navigate(wfBrowser.StatusText);
            ////创建新的TabItem
            //TabItem tab_item = new TabItem();
            //tabMain.Items.Add(tab_item);
            ////创建新的Grid
            //Grid grid = new Grid();
            //grid.Width = 160;
            //grid.Height = 23;

            ////创建新的Label
            //Label lable = new Label();
            //lable.HorizontalAlignment = HorizontalAlignment.Left;
            //lable.VerticalAlignment = VerticalAlignment.Center;
            //grid.Children.Add(lable);
            ////创建新的x关闭button
            //Button button = new Button();
            //button.Click += btnDelete_Click;
            //button.Style = FindResource("headerButtonStyle") as Style;
            //grid.Children.Add(button);

            //tab_item.Header = grid;
            ////创建winForm 的WebBroser
            //System.Windows.Forms.Integration.WindowsFormsHost host =
            //    new System.Windows.Forms.Integration.WindowsFormsHost();
            //System.Windows.Forms.WebBrowser webBroser = new System.Windows.Forms.WebBrowser();
            //webBroser.AllowWebBrowserDrop = false;
            //webBroser.WebBrowserShortcutsEnabled = false;
            //webBroser.IsWebBrowserContextMenuEnabled = false;
            //webBroser.Navigate(wfBrowser.StatusText);
            //lable.Content = "newTite";
            //tab_item.Content = webBroser;
        }
        /// <summary>
        /// 后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            this.wfBrowser.GoBack(); 
        }
        /// <summary>
        /// 前进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoForward_Click(object sender, RoutedEventArgs e)
        {
            this.wfBrowser.GoForward();
        }
        /// <summary>
        /// 快捷登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wfBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            //wfBrowser.Document.Body.Style = "overflow:hidden";
            #region 控制网站根据窗体大小加载窗体大小
            if (wfBrowser.ReadyState != WebBrowserReadyState.Complete) return;
            System.Drawing.Size szb = new System.Drawing.Size(wfBrowser.Document.Body.OffsetRectangle.Width,
                wfBrowser.Document.Body.OffsetRectangle.Height);
            System.Drawing.Size sz = (System.Drawing.Size)wfBrowser.Size;

            int xbili = (int)((float)sz.Width / (float)szb.Width * 100);//水平方向缩小比例
            int ybili = (int)((float)sz.Height / (float)szb.Height * 100);//垂直方向缩小比例
            wfBrowser.Document.Body.Style = "zoom:" + xbili.ToString() + "%";
            wfBrowser.Invalidate(); 
            #endregion
            #region 博客园登录
            if (Common.instance.Apply == 1)
            {
                string BlogKey = "sadf";//加密博客园密码
                string BlogDecrypt = Common.instance.DESCDecrypt(Common.instance.GetKey(), BlogKey);
                System.Windows.Forms.HtmlElement ClickBtn = null;
                if (e.Url.ToString().ToLower().IndexOf("/user/signin") > 0)
                {
                    System.Windows.Forms.HtmlDocument doc = wfBrowser.Document;
                    for (int i = 0; i < doc.All.Count; i++)
                    {
                        if (doc.All[i].TagName.ToUpper().Equals("INPUT"))
                        {
                            switch (doc.All[i].Id)
                            {
                                case "input1":
                                    doc.All[i].InnerText = "sdf"; // 用户名
                                    break;
                                case "input2":
                                    doc.All[i].InnerText = BlogDecrypt; // 密码
                                    break;
                                case "signin":
                                    ClickBtn = doc.All[i];
                                    break;
                            }
                        }
                    }
                    ClickBtn.InvokeMember("Click"); // 点击“登录”按钮
                }
            }
            #endregion
            #region 新浪微博登录
            else if (Common.instance.Apply == 2)
            {
                string SinaKey = "123123";//加密新浪微博密码
                string SinaDecrypt = Common.instance.DESCDecrypt(Common.instance.GetKey(), SinaKey);
                System.Windows.Forms.HtmlElement ClickBtn = null;
                if (e.Url.ToString().ToLower().IndexOf("/weibo.com") > 0)
                {
                    System.Windows.Forms.HtmlDocument doc = wfBrowser.Document;
                    for (int i = 0; i < doc.All.Count; i++)
                    {
                        if (doc.All[i].TagName.ToUpper().Equals("INPUT"))
                        {
                            switch (doc.All[i].TabIndex)
                            {
                                case 1:
                                    doc.All[i].InnerText = "sdf"; // 用户名
                                    break;
                                case 2:
                                    doc.All[i].InnerText = SinaDecrypt; // 密码
                                    break;
                                case 6:
                                    ClickBtn = doc.All[i];
                                    break;
                            }
                        }
                    }
                    ClickBtn.InvokeMember("Click"); // 点击“登录”按钮
                }
            }
            #endregion
            #region 网易邮箱登录
            else if (Common.instance.Apply == 3)
            {
                string MailKey = "123";//加密网易邮箱密码
                string MailDecrypt = Common.instance.DESCDecrypt(Common.instance.GetKey(), MailKey);
                System.Windows.Forms.HtmlElement ClickBtn = null;
                if (e.Url.ToString().ToLower().IndexOf("/mail.163.com") > 0)
                {
                    System.Windows.Forms.HtmlDocument doc = wfBrowser.Document;
                    for (int i = 0; i < doc.All.Count; i++)
                    {
                        if (doc.All[i].TagName.ToUpper().Equals("INPUT"))
                        {
                            switch (doc.All[i].TabIndex)
                            {
                                case 1:
                                    doc.All[i].InnerText = "sdf"; // 用户名
                                    break;
                                case 2:
                                    doc.All[i].InnerText = MailDecrypt; // 密码
                                    break;
                                case 8:
                                    ClickBtn = doc.All[i];
                                    break;
                            }
                        }
                    }
                    ClickBtn.InvokeMember("Click"); // 点击“登录”按钮
                }
            }
            #endregion
           
        }
        /// <summary>
        /// 打开收藏地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHouse_Click(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = true;
        }
        private IntPtr prsmwh;//外部EXE文件运行句柄
        private Process app;//外部exe文件对象
        //声明调用user32.dll中的函数
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow); 
        /// <summary>
        /// 点击更换地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllAddress_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            string btnName = btn.Name;
            if (btnName == "btnBing")
            {
                wfBrowser.Url = new Uri("http://www.bing.com");
            }
            else if (btnName == "btn163") 
            {
                wfBrowser.Url = new Uri("http://music.163.com/");
            }
            else if (btnName == "btnSina")
            {
                wfBrowser.Url = new Uri("http://www.weibo.com");
            }
            else if (btnName == "btnBoke")
            {
                wfBrowser.Url = new Uri("http://www.cnblogs.com/");
            }
            else if (btnName == "btnHao123")
            {
                wfBrowser.Url = new Uri("http://www.hao123.com");
            }
            else if(btnName=="btnWeiXin")
            {
                wfBrowser.Url = new Uri("https://wx.qq.com/");
                //获取当前窗口句柄
                //IntPtr handle = new WindowInteropHelper(this).Handle;
                ////app = System.Diagnostics.Process.Start("NOTEPAD.EXE");
                //app = System.Diagnostics.Process.Start("WeChat.exe");
                //prsmwh = app.MainWindowHandle;
                //while (prsmwh == IntPtr.Zero)
                //{
                //    prsmwh = app.MainWindowHandle;
                //}
                ////设置父窗口
                //SetParent(prsmwh, handle);
                //ShowWindowAsync(prsmwh, 3);//子窗口最大化 
            }
            else if(btnName=="btnYun")
            {
                wfBrowser.Url = new Uri("https://pan.baidu.com/");
            }
            else if(btnName=="btnMail")
            {
                wfBrowser.Url = new Uri("http://mail.163.com/");
            }
            else if (btnName == "btnGitHub")
            {
                wfBrowser.Url = new Uri("https://github.com/yanjinhuagood");
            }
            else if (btnName == "btnChart")
            {
                wfBrowser.Url = new Uri("http://www.iconfont.cn/");
            }
            else if (btnName == "btnYouku")
            {
                wfBrowser.Url = new Uri("http://www.youku.com/");
            }
            else if (btnName == "btnTmall")
            {
                wfBrowser.Url = new Uri("https://www.tmall.com/");
            }
            else if (btnName == "btnTaobao")
            {
                wfBrowser.Url = new Uri("https://www.taobao.com/");
            }
            else if (btnName == "btnJd")
            {
                wfBrowser.Url = new Uri("http://www.jd.com/");
            }
            else if (btnName == "btnKj")
            {
                wfBrowser.Url = new Uri("http://i.qq.com/");
            }
        }
    }
}
