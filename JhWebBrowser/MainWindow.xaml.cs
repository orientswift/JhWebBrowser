using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JhWebBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        Rect rcnormal;//定义一个全局rect记录还原状态下窗口的位置和大小。
        Button max = new Button();//最大
        Button mid = new Button();//还原
        public MainWindow()
        {
            InitializeComponent();
            this.SourceInitialized += MainWindow_SourceInitialized;
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;//任务栏右键关闭退出系统
            MaskDelegate.delegateAlertMethod += AlterMaskMethod;                 
        }

        private bool? AlterMaskMethod(Window win)
        {
            win.Owner = this;
            bool? b = win.ShowDialog();
            return b;
        }
        /// <summary>
        /// 任务栏右键关闭退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        /// <summary>
        /// 获取所有button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Button btnClose = this.GetTemplateChild("btnClose") as Button;
            btnClose.Click += btnClose_Click;
            Button btnMin = this.GetTemplateChild("btnMin") as Button;
            btnMin.Click += btnMin_Click;
            Button btnMax = this.GetTemplateChild("btnMax") as Button;
            max = btnMax;
            max.Click += max_Click;
            Button btnMid = this.GetTemplateChild("btnMid") as Button;
            mid = btnMid;
            mid.Click += mid_Click;
            //Button btnSkin = this.GetTemplateChild("btnSkin") as Button;
            IsRegeditKeyExit();
            btnHome_Click(null,null);
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
        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mid_Click(object sender, RoutedEventArgs e)
        {
            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;
            max.Visibility = Visibility.Visible;
            mid.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 最大化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void max_Click(object sender, RoutedEventArgs e)
        {
            max.Visibility = Visibility.Collapsed;
            mid.Visibility = Visibility.Visible;
            rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rc.Width;
            this.Height = rc.Height;
        }
        /// <summary>
        /// 最小化到任务栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        /// <summary>
        /// 复制Title对拖动事件做准备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            TextBlock title = this.GetTemplateChild("TitleBar") as TextBlock;
            title.MouseDown += title_MouseDown;
        }
        /// <summary>
        /// 拖动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        Button btnSelectedLast;//公共button
        /// <summary>
        /// 主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            
            if (btnHome == btnSelectedLast) return;
            btnHome.Tag = "selected";
            if (btnSelectedLast != null)
                btnSelectedLast.Tag = "";
            btnSelectedLast = btnHome;
            wbInput.Source = "http://www.cnblogs.com/yanjinhua";
            Common.instance.Apply = 1;
        }
        /// <summary>
        /// webBrowser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            btnBrowser.Tag = "selected";
            if (btnBrowser == btnSelectedLast) return;
            btnBrowser.Tag = "selected";
            if (btnSelectedLast != null)
                btnSelectedLast.Tag = "";
            btnSelectedLast = btnBrowser;
            wbInput.Source = "http://cn.bing.com/";
        }
        /// <summary>
        /// 多媒体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMedia_Click(object sender, RoutedEventArgs e)
        {
            btnMedia.Tag = "selected";
            if (btnMedia == btnSelectedLast) return;
            btnMedia.Tag = "selected";
            if (btnSelectedLast != null)
                btnSelectedLast.Tag = "";
            btnSelectedLast = btnMedia;
            wbInput.Source = "http://www.panda.tv/lpl?psrc=pc_web-baidubox-lpl2016";
        }
        /// <summary>
        /// 新浪微博
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSina_Cilck(object sender, RoutedEventArgs e)
        {
            btnSina.Tag = "selected";
            if (btnSina == btnSelectedLast) return;
            btnSina.Tag = "selected";
            if (btnSelectedLast != null)
                btnSelectedLast.Tag = "";
            btnSelectedLast = btnSina;
            //wbInput.Source = "http://weibo.com/2u1mei";
            wbInput.Source = "http://weibo.com/";
            Common.instance.Apply = 2;
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMail_Click(object sender, RoutedEventArgs e)
        {
            btnMail.Tag = "selected";
            if (btnMail == btnSelectedLast) return;
            btnMail.Tag = "selected";
            if (btnSelectedLast != null)
                btnSelectedLast.Tag = "";
            btnSelectedLast = btnMail;
            wbInput.Source = "http://mail.163.com/";
            Common.instance.Apply = 3;
        }
        /// <summary>
        /// QQ音乐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMusic_Click(object sender, RoutedEventArgs e)
        {
            btnMusic.Tag = "selected";
            if (btnMusic == btnSelectedLast) return;
            btnMusic.Tag = "selected";
            if (btnSelectedLast != null)
                btnSelectedLast.Tag = "";
            btnSelectedLast = btnMusic;
            wbInput.Source = "http://i.y.qq.com/v8/fcg-bin/v8_cp.fcg?channel=first&format=html&page=index&tpl=v12";
        }

        private void btnSetUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void img_Click(object sender, RoutedEventArgs e)
        {
            UCSelf us = new UCSelf();
            bool? isOK = MaskDelegate.delegateAlertMethod(us);
        }
    }
}
