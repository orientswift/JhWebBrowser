using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace JhWebBrowser
{
    /// <summary>
    /// UCSelf.xaml 的交互逻辑
    /// </summary>
    public partial class UCSelf : WinBaseforAlert
    {
        DispatcherTimer tm = new DispatcherTimer();//为定时关闭做的
        public UCSelf()
        {
            InitializeComponent();
        }

        #region  窗体从下到上然后关闭
        /// <summary>
        /// 窗体从下到上然后关闭
        /// </summary>
        public void CloseBegin()
        {

            TranslateTransform tt = new TranslateTransform();
            DoubleAnimation da = new DoubleAnimation();
            Duration duration = new Duration(TimeSpan.FromSeconds(2));
            this.RenderTransform = tt;
            tt.Y = 0;
            da.To = -600;
            da.Duration = duration;
            tt.BeginAnimation(TranslateTransform.YProperty, da);
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = TimeSpan.FromSeconds(1);
            tm.Start();
        }
        private void tm_Tick(object sender, EventArgs e)
        {
            this.Close();
        } 
        #endregion

        #region 窗体淡淡关闭
        /// <summary>
        /// 淡淡关闭
        /// </summary>
        //public void CloseBegin()
        //{
        //    DoubleAnimation animation = new DoubleAnimation()
        //    {
        //        From = 1.0,
        //        To = 0.0,
        //        Duration = new Duration(TimeSpan.FromSeconds(2))
        //    };
        //    this.BeginAnimation(Window.OpacityProperty, animation);
        //    animation.Completed += anim_Completed;
        //}
        //private void anim_Completed(object sender, EventArgs e)
        //{
        //    this.Close();
        //} 
        #endregion

        
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseBegin();
        }

        private void btnMan_Click(object sender, RoutedEventArgs e)
        {
            if(btnMan.IsChecked==true)
            {
                this.btnwoman.IsChecked = false;
            }
            else 
            {
                this.btnwoman.IsChecked = true;
            }
        }

        private void btnwoman_Click(object sender, RoutedEventArgs e)
        {
            if (btnwoman.IsChecked == true)
            {
                this.btnMan.IsChecked = false;
            }
            else
            {
                this.btnMan.IsChecked = true;
            }
        }
    }
}
