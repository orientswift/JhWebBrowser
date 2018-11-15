using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace JhWebBrowser
{
    /// <summary>
    /// MyApplication.xaml 的交互逻辑
    /// </summary>
    public partial class MyApplication : Window
    {
        /// <summary>
        /// 定义NotifyIcon
        /// </summary>
        System.Windows.Forms.NotifyIcon notifyIcon1;
        /// <summary>
        /// //定义一个全局rect记录还原状态下窗口的位置和大小。
        /// </summary>
        Rect rcnormal;
        /// <summary>
        /// 1:为最小化 2：为最大化
        /// </summary>
        public int WindowsState = 1;


        /// <summary>
        /// 查找本地是否存在皮肤xml
        /// </summary>
        string path = Environment.CurrentDirectory + "\\JhSkin.dll";
        XmlDocument doc = new XmlDocument();
        /// <summary>
        /// 皮肤值
        /// </summary>
        public string skinXml { get; set; }
        public MyApplication()
        {
            InitializeComponent();
            icon();
            main_Selected(null, null);//调用music.163
            WindowsState = 1;//窗体启动为最小化
            new WindowResizer(this,
                new WindowBorder(BorderPosition.Top, top),
                new WindowBorder(BorderPosition.TopRight, topRight),
                new WindowBorder(BorderPosition.Right, right),
                new WindowBorder(BorderPosition.BottomRight, bottomRight),
                new WindowBorder(BorderPosition.Bottom, bottom),
                new WindowBorder(BorderPosition.BottomLeft, bottomLeft),
                new WindowBorder(BorderPosition.Left, left),
                new WindowBorder(BorderPosition.TopLeft, topLeft)
            );
            CreateSkinXml(skinXml);
            IsExistSkin();//皮肤是否存在
        }
        #region 创建托盘
        private void icon()
        {
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon1.BalloonTipText = "Hello, 我的小可爱"; //设置程序启动时显示的文本
            this.notifyIcon1.Text = @"昵称:爱的管家  
主人：小可爱
说明：Hello，我的小可爱 我是你的管家";//最小化到托盘时，鼠标点击时显示的文本
            this.notifyIcon1.Icon = new System.Drawing.Icon("../../Cutie.ico");//程序图标
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ShowBalloonTip(1000);
            notifyIcon1.ContextMenu = new System.Windows.Forms.ContextMenu();
            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            notifyIcon1.Click += notifyIcon1_Click; // 在这里处理鼠标右击托盘菜单的  
        }
        void notifyIcon1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs mouseArgs = e as System.Windows.Forms.MouseEventArgs;
            // 如果是鼠标不是右击，则直接返回  
            if (null == mouseArgs || mouseArgs.Button != System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }
            ContextMenu menu = (ContextMenu)grid_Container.FindResource("menu"); // 这句是查找资源（那里的菜单风格就可以自己写了）  
            menu.IsOpen = true;
        }
        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);// 自己写的一个激活窗口到最前的 
            this.Activate();
            //this.Visibility = System.Windows.Visibility.Visible;
            this.Show();
            //Dispatcher.BeginInvoke(new Action(delegate
            //{
            //    this.Activate();
            //}), System.Windows.Threading.DispatcherPriority.ContextIdle, null);
        }
        #endregion
        #region 任务栏右键关闭隐藏主窗体
        /// <summary>
        /// 任务栏右键关闭隐藏主窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void this__Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion

        #region 点击menuItem调转不同的网站
        private void myself_Selected(object sender, RoutedEventArgs e)
        {
            ImageSee imageSee = new ImageSee();
            //wbInput.Source = "http://www.weibo.com";
            this.gridMenu.Visibility = Visibility.Collapsed;
            this.gridMain.Children.Add(imageSee);
        }

        private void main_Selected(object sender, RoutedEventArgs e)
        {
            
            this.gridMenu.Visibility = Visibility.Visible; 
            wbInput.Source = "http://music.163.com/";
        }

        private void search_Selected(object sender, RoutedEventArgs e)
        {
            this.gridMenu.Visibility = Visibility.Visible; 
            wbInput.Source = "http://www.bing.com";
        }

        private void list_Selected(object sender, RoutedEventArgs e)
        {
            this.gridMenu.Visibility = Visibility.Visible; 
            wbInput.Source = "http://www.hao123.com";
        }

        private void cnblogs_Selected(object sender, RoutedEventArgs e)
        {
            this.gridMenu.Visibility = Visibility.Visible; 
            wbInput.Source = "http://www.cnblogs.com/yanjinhua/";
        }
        #endregion

        #region 拖动和双击Title
        /// <summary>
        /// 拖动和双击Title
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowsState == 2)
                {
                    mid_Click(null, null);
                }
                else
                {
                    max_Click(null, null);
                }
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion
        #region 最小化系统托盘
        /// <summary>
        /// 最小化系统托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion
        #region 退出系统
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (UCMessageBox.Show("关闭提示", "是否关闭系统？") == true)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion
        #region 最小化到任务栏
        /// <summary>
        /// 最小化到任务栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
        #region 最小化
        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mid_Click(object sender, RoutedEventArgs e)
        {
            Style styleMid = null;
            WindowsState = 1;
            if (styleMid == null)
                styleMid = FindResource("windowMidStyle") as Style;
            this.Style = styleMid;
            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;
            btnMax.Visibility = Visibility.Visible;
            btnMid.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region 最大化
        /// <summary>
        /// 最大化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void max_Click(object sender, RoutedEventArgs e)
        {
            Style style = null;
            WindowsState = 2;
            if (style == null)
                style = FindResource("windowMaxStyle") as Style;
            this.Style = style;
            btnMax.Visibility = Visibility.Collapsed;
            btnMid.Visibility = Visibility.Visible;
            rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rc.Width;
            this.Height = rc.Height;
        }
        #endregion

        #region 点击换肤
        Button btnSelectedLast;//公共button
        /// <summary>
        /// 点击换肤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllSkin_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string btnName = null;
            if (btn != null)
            {
                btnName = btn.Name;//将点击的button的name给btnName
                skinXml = btnName;//如果button为空讲值给皮肤值

                if (btn == btnSelectedLast) return;
                btn.Tag = "selected";
                if (btnSelectedLast != null)
                    btnSelectedLast.Tag = "";
                btnSelectedLast = btn;
            }
            if (skinXml == "" || skinXml == null)
            {
                skinXml = "Three";
            }
            if (btnName == "One" || skinXml == "One")
            {
                this.MainGrid.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Background/One.jpg"))
                };
                this.rectangleSkin.Fill = Brushes.Black;
                this.rectangleSkinTwo.Fill = Brushes.Black;
                this.rectangleSkinRight.Fill = Brushes.Black;
                this.rectangleSkinBottom.Fill = Brushes.Black;
                if (btnName == null)
                {
                    this.One.Tag = "selected";
                }
                else
                {
                    this.Two.Tag = "";
                    this.Three.Tag = "";
                    this.Four.Tag = "";
                    this.Five.Tag = "";
                    this.Six.Tag = "";
                }
            }
            else if (btnName == "Two" || skinXml == "Two")
            {
                this.MainGrid.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Background/Two.jpg"))
                };
                this.rectangleSkin.Fill = new SolidColorBrush(Color.FromRgb(41, 177, 238));
                this.rectangleSkinTwo.Fill = new SolidColorBrush(Color.FromRgb(41, 177, 238));
                this.rectangleSkinRight.Fill = new SolidColorBrush(Color.FromRgb(41, 177, 238));
                this.rectangleSkinBottom.Fill = new SolidColorBrush(Color.FromRgb(41, 177, 238));
                if (btnName == null)
                {
                    this.Two.Tag = "selected";
                }
                else
                {
                    this.One.Tag = "";
                    this.Three.Tag = "";
                    this.Four.Tag = "";
                    this.Five.Tag = "";
                    this.Six.Tag = "";
                }
            }
            else if (btnName == "Three" || skinXml == "Three")
            {
                this.MainGrid.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Background/Three.jpg"))
                };
                this.rectangleSkin.Fill = new SolidColorBrush(Color.FromRgb(33, 64, 126));
                this.rectangleSkinTwo.Fill = new SolidColorBrush(Color.FromRgb(33, 64, 126));
                this.rectangleSkinRight.Fill = new SolidColorBrush(Color.FromRgb(33, 64, 126));
                this.rectangleSkinBottom.Fill = new SolidColorBrush(Color.FromRgb(33, 64, 126));
                if (btnName == null)
                {
                    this.Three.Tag = "selected";
                }
                else
                {
                    this.One.Tag = "";
                    this.Two.Tag = "";
                    this.Four.Tag = "";
                    this.Five.Tag = "";
                    this.Six.Tag = "";
                }
            }
            else if (btnName == "Four" || skinXml == "Four")
            {
                this.MainGrid.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Background/Four.jpg"))
                };
                this.rectangleSkin.Fill = new SolidColorBrush(Color.FromRgb(49, 144, 237));
                this.rectangleSkinTwo.Fill = new SolidColorBrush(Color.FromRgb(49, 144, 237));
                this.rectangleSkinRight.Fill = new SolidColorBrush(Color.FromRgb(49, 144, 237));
                this.rectangleSkinBottom.Fill = new SolidColorBrush(Color.FromRgb(49, 144, 237));
                if (btnName == null)
                {
                    this.Four.Tag = "selected";
                }
                else
                {
                    this.One.Tag = "";
                    this.Two.Tag = "";
                    this.Three.Tag = "";
                    this.Five.Tag = "";
                    this.Six.Tag = "";
                }
            }
            else if (btnName == "Five" || skinXml == "Five")
            {
                this.MainGrid.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Background/Five.jpg"))
                };
                this.rectangleSkin.Fill = new SolidColorBrush(Color.FromRgb(15, 100, 165));
                this.rectangleSkinTwo.Fill = new SolidColorBrush(Color.FromRgb(15, 100, 165));
                this.rectangleSkinRight.Fill = new SolidColorBrush(Color.FromRgb(15, 100, 165));
                this.rectangleSkinBottom.Fill = new SolidColorBrush(Color.FromRgb(15, 100, 165));
                if (btnName == null)
                {
                    this.Five.Tag = "selected";
                }
                else
                {
                    this.One.Tag = "";
                    this.Two.Tag = "";
                    this.Three.Tag = "";
                    this.Four.Tag = "";
                    this.Six.Tag = "";
                }
            }
            else if (btnName == "Six" || skinXml == "Six")
            {
                this.MainGrid.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Background/Six.jpg"))
                };
                this.rectangleSkin.Fill = new SolidColorBrush(Color.FromRgb(142, 170, 203));
                this.rectangleSkinTwo.Fill = new SolidColorBrush(Color.FromRgb(142, 170, 203));
                this.rectangleSkinRight.Fill = new SolidColorBrush(Color.FromRgb(142, 170, 203));
                this.rectangleSkinBottom.Fill = new SolidColorBrush(Color.FromRgb(142, 170, 203));
                if (btnName == null)
                {
                    this.Six.Tag = "selected";
                }
                else
                {
                    this.One.Tag = "";
                    this.Two.Tag = "";
                    this.Three.Tag = "";
                    this.Four.Tag = "";
                    this.Five.Tag = "";
                }
            }
            btnName = skinXml;
            CreateSkinXml(btnName);
        }
        #endregion

        #region 向skinXml写入皮肤值
        /// <summary>
        /// 向skinXml写入皮肤值
        /// </summary>
        public void CreateSkinXml(string btnName)
        {
            if (!Directory.Exists(Environment.CurrentDirectory))//文件夹是否存在
            {
                Directory.CreateDirectory(Environment.CurrentDirectory);//不存在创建文件夹
            }
            try
            {
                //第一在xml指定皮肤为One
                if (!File.Exists(path))
                {
                    XmlElement Node = doc.CreateElement("Skin");
                    doc.AppendChild(Node);
                    XmlElement Node0 = doc.CreateElement("SkinNo");
                    Node0.InnerText = btnName;
                    doc.DocumentElement.AppendChild(Node0);
                    doc.Save(path);
                }
                //如果存在修改
                else
                {
                    doc.Load("JhSkin.dll");    //加载Xml文件
                    XmlElement root = doc.DocumentElement;
                    foreach (XmlNode node in root)
                    {
                        if (node.Name == "SkinNo")
                        {
                            if (btnName == null || btnName == "")
                            {
                                btnName = node.InnerText;
                            }
                            else
                            {
                                node.InnerText = btnName;
                            }
                        }
                    }
                    doc.Save(path);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region 皮肤是否存在
        /// <summary>
        /// 皮肤是否存在
        /// </summary>
        public void IsExistSkin()
        {
            doc.Load("JhSkin.dll");    //加载Xml文件 
            XmlElement root = doc.DocumentElement;   //获取根节点 
            foreach (XmlNode node in root)
            {
                if (node.Name == "SkinNo")
                {
                    skinXml = node.InnerText;
                }
                btnAllSkin_Click(null, null);
            }
        }
        #endregion
    }
}
